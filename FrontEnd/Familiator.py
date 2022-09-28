import telegram
from telebot.types import *

from Common.Menues import go_back_to_main_menu
from Core import HelpersMethodes as Helpers
from Requester import *
import requests
import json


class Familiator:
    def __init__(self, bot, msg, cr_user, familiators, requesters, hasVisited=True):
        self.btnYes = "👌"
        self.btnNo = "🙊"
        self.btnReport = "Report"

        self.finish_message = "That is all for now, please wait until we find someone else for you"

        self.bot = bot
        self.msg = msg
        self.current_user = cr_user
        Helpers.switch_user_busy_status(self.current_user)
        self.requesters = requesters
        self.familiators = familiators
        self.familiators.append(self)
        self.active_user = None
        self.active_user_id = 0
        self.markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(self.btnYes, self.btnNo)
        self.YN_markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(KeyboardButton("Yes"), KeyboardButton("No"))
        self.actions_markup = InlineKeyboardMarkup(row_width=5)
        self.actions_markup.add(InlineKeyboardButton("Report", callback_data=-1))

        self.report_reasons = {}
        self.reasons_markup = None

        self.ch = self.bot.register_callback_query_handler("", self.callback_handler, user_id=self.current_user)
        self.eh = self.bot.register_message_handler(self.exit_handler, commands=["exit"], user_id=self.current_user)


        self.people = Helpers.get_user_list(self.current_user)

        if self.people:
            if self.set_active_person():
                self.show_person(msg)
        else:
            self.bot.send_message(self.current_user, "Sorry, we cant find anyone who matches your search parameters. Please, try again later")
            self.destruct()


    def set_active_person(self):
        if self.people:
            self.active_user = self.people[0]
            Helpers.register_user_encounter_familiator(self.current_user, self.active_user_id)
            self.active_user_id = self.active_user["userId"]
            self.people.pop(0)
            return True
        return False

    def show_person(self, msg):
        user = self.active_user["userBaseInfo"]
        self.bot.send_photo(self.current_user, user["userPhoto"], user["userDescription"], reply_markup=self.markup)
        self.bot.send_message(self.current_user, "Additional Actions:", reply_markup=self.actions_markup)
        self.bot.register_next_step_handler(msg, self.message_handler, chat_id=self.current_user)

    def message_handler(self, message):
        if message.text == self.btnYes:
            Helpers.register_user_request(self.current_user, self.active_user_id, False)
            if not Helpers.check_user_is_busy(self.active_user_id):
                req_list = Helpers.get_user_requests(self.active_user_id)
                Requester(self.bot, message, self.active_user_id, req_list)
        if self.set_active_person():
            self.show_person(message)
        else:
            self.bot.send_message(self.current_user, self.finish_message)
            self.destruct()

    def exit_handler(self, message):
        self.destruct()

    def callback_handler(self, call):
        if int(call.data) == -1:
            self.report_step1(call.message)

    def report_step1(self, message):
        self.load_report_reasons(0) #TODO: When localized - pass in user appLanguage index
        self.reasons_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)
        for reason in self.report_reasons.keys():
            self.reasons_markup.add(self.report_reasons[reason])

        self.bot.send_message(self.current_user, "What do you want to report?", reply_markup=self.reasons_markup)
        self.bot.register_next_step_handler(message, self.report_user, chat_id=self.current_user)

    def report_user(self, message):
        if message.text not in self.report_reasons.values():
            self.bot.send_message("No such reason")
            self.bot.send_message(self.current_user, "What do you want to report?", reply_markup=self.reasons_markup)
            self.bot.register_next_step_handler(message, self.report_user, chat_id=self.current_user)
            return False

        report_data = {"id": 0,
                       "UserBaseInfoId": self.current_user,
                       "UserBaseInfoId1": self.active_user,
                       "ReasonId": self.reason_converter(message.text),
                       "ReasonClassLocalisationId": 0,
                       }

        self.bot.send_message(self.current_user, "Please, enter a brief description of your report")
        self.bot.register_next_step_handler(message, self.report_user_final, self.current_user, report_data, chat_id=self.current_user)

    def report_user_final(self, message, report_data):
        report_data["Text"] = message.text
        self.send_report(report_data)

        self.bot.send_message(self.current_user, "Thank you for your report. We will process it as soon as possible\n\n<b>Personality Administration</b>",
                              parse_mode=telegram.ParseMode.HTML)
        self.bot.send_message(self.current_user, "Would you like to add this user to your blacklist?\nThus you wont have problem with him ever again", reply_markup=self.YN_markup)
        self.bot.register_next_step_handler(message, self.add_user_to_blacklist_step, chat_id=self.current_user)

    def add_user_to_blacklist_step(self, message):
        if message.text == "Yes":
            self.add_user_to_black_list()
        else:
            self.move_to_next_user(message)

    def add_user_to_black_list(self):
        if int(requests.get(f"https://localhost:44381/AddUserToBlackList/{self.current_user}/{self.active_user}", verify=False).text) > 0:
            self.bot.send_message(self.current_user, "User had been added to your blacklist\nYou can undo that action in settings :-)")
        else:
            self.bot.send_message(self.current_user, "Something went wrong")

    def move_to_next_user(self, message):
        if self.set_active_person():
            self.show_person(message)
        else:
            self.bot.send_message(self.current_user, self.finish_message)
            self.destruct()

    def send_report(self, report):
        d = json.dumps(report)

        requests.post(f"https://localhost:44381/AddUserReport", d, headers={
            "Content-Type": "application/json"}, verify=False)

    def load_report_reasons(self, localisationId):
        data = json.loads(requests.get(f"https://localhost:44381/GetReportReasons/{localisationId}", verify=False).text)

        for reason in data:
            self.report_reasons[reason['id']] = reason['description']

    def reason_converter(self, reason):
        for reas in self.report_reasons:
            if reason == self.report_reasons[reas]:
                return reas

    def destruct(self):
        self.familiators.remove(self)
        self.bot.message_handlers.remove(self.eh)
        self.bot.callback_query_handlers.remove(self.ch)
        Helpers.switch_user_busy_status(self.current_user)
        if Helpers.check_user_has_requests(self.current_user):
            request_list = Helpers.get_user_requests(self.current_user)
            Requester(self.bot, self.msg, self.current_user, request_list)
            return False
        go_back_to_main_menu(self.bot, self.current_user, self.msg)
        del self