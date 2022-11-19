from telebot.types import *

from Core import HelpersMethodes as Helpers
from Common.Menues import go_back_to_main_menu
from ReportModule import ReportModule
from Requester import *


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
        self.markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(self.btnYes, self.btnNo, self.btnReport)
        self.actions_markup = InlineKeyboardMarkup(row_width=5)
        self.actions_markup.add(InlineKeyboardButton("Report", callback_data=-1))

        self.report_reasons = {}
        self.reasons_markup = None

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

            active_reply = Helpers.get_user_active_reply(self.active_user_id)
            if active_reply:
                if not active_reply["isEmpty"]:
                    self.bot.send_message(self.current_user, "⬇ This user has a message for you ;-) ⬇")
                    if active_reply["isText"]:
                        self.bot.send_message(self.current_user, f'"{active_reply["autoReply"]}"')
                    else:
                        self.bot.send_voice(self.current_user, active_reply["autoReply"])

            if not Helpers.check_user_is_busy(self.active_user_id):
                req_list = Helpers.get_user_requests(self.active_user_id)
                Requester(self.bot, message, self.active_user_id, req_list)
        elif message.text == self.btnReport:
            ReportModule(self.bot, self.msg, self.current_user, self.proceed)

        requests = Helpers.get_user_requests(self.current_user)
        if requests:
            Requester(self.bot, self.msg, self.current_user, requests)
        self.proceed()

    def proceed(self):
        if self.set_active_person():
            self.show_person(self.msg)
        else:
            self.bot.send_message(self.current_user, self.finish_message)
            self.destruct()

    def exit_handler(self, message):
        self.destruct()

    def move_to_next_user(self, message):
        if self.set_active_person():
            self.show_person(message)
        else:
            self.bot.send_message(self.current_user, self.finish_message)
            self.destruct()

    def destruct(self):
        self.familiators.remove(self)
        self.bot.message_handlers.remove(self.eh)
        Helpers.switch_user_busy_status(self.current_user)
        go_back_to_main_menu(self.bot, self.current_user, self.msg)
        del self