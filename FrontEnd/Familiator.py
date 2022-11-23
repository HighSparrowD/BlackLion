import requests
from telebot.types import *

from Common.Menues import go_back_to_main_menu
from ReportModule import ReportModule
from Requester import *


class Familiator:
    def __init__(self, bot, msg, cr_user, familiators, hasVisited=True):
        self.btnYes = "👌"
        self.btnNo = "🙊"
        self.btnReport = "Report"

        self.finish_message = "That is all for now, please wait until we find someone else for you"

        self.bot = bot
        self.msg = msg
        self.current_user = cr_user
        Helpers.switch_user_busy_status(self.current_user)
        self.familiators = familiators
        self.familiators.append(self)
        self.active_user = None
        self.active_user_id = 0
        self.markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(self.btnYes, self.btnNo, self.btnReport)
        self.YNmarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("Yes", "No")
        self.actions_markup = InlineKeyboardMarkup(row_width=5)
        self.actions_markup.add(InlineKeyboardButton("Report", callback_data=-1))

        self.tagLimit = Helpers.get_user_tag_limit(self.current_user)
        self.wasPersonalityTurnedOff = False

        self.start_message = "1. Normal search\n2. Search by tags\n3. Free search"
        self.free_search_message = "Up for meeting someone today?"

        self.startMarkup = ReplyKeyboardMarkup().add("1", "2", "3")

        self.report_reasons = {}
        self.people = None
        self.reasons_markup = None

        self.eh = self.bot.register_message_handler(self.exit_handler, commands=["exit"], user_id=self.current_user)

        if Helpers.check_user_have_chosen_free_search(self.current_user):
            self.free_search_switch(msg)


    def free_search_switch(self, message, acceptMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, self.free_search_message, reply_markup=self.YNmarkup)
            self.bot.register_next_step_handler(message, self.free_search_switch, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "Yes":
                response = requests.get(f"https://localhost:44381/SetUserFreeSearchParam/{self.current_user}/{True}", verify=False)

                if response.status_code == 200:
                    self.bot.send_message(self.current_user, "Done. You can change this parameter at any time in settings")
                else:
                    self.bot.send_message(self.current_user, "Something went wrong. Please try again later")

                self.start(message)
            elif message.text == "No":
                response = requests.get(f"https://localhost:44381/SetUserFreeSearchParam/{self.current_user}/{True}",
                                        verify=False)

                if response.status_code == 200:
                    self.bot.send_message(self.current_user,
                                          "Done. You can change this parameter at any time in settings")
                else:
                    self.bot.send_message(self.current_user, "Something went wrong. Please try again later")

                self.start(message)
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.YNmarkup)
                self.bot.register_next_step_handler(message, self.free_search_switch, acceptMode=True, chat_id=self.current_user)


    def start(self, message, acceptMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, self.start_message, reply_markup=self.startMarkup)
            self.bot.register_next_step_handler(message, self.start, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                self.normal_search_handler(message)
            elif message.text == "2":
                self.search_by_tags(message)
            elif message.text == "3":
                self.free_search()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.startMarkup)
                self.bot.register_next_step_handler(message, self.start, acceptMode=acceptMode, chat_id=self.current_user)

    def normal_search_handler(self, message):
        self.people = Helpers.get_user_list(self.current_user)
        if self.people:
            if self.set_active_person():
                self.show_person(message)
        else:
            self.personalityOff_handler(message)

    def search_by_tags(self, message, acceptMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, f"Sent me up to {self.tagLimit}")
            self.bot.register_next_step_handler(message, self.search_by_tags, acceptMode=True, chat_id=self.current_user)
        else:
            tags = len(message.text.split())
            #TODO: check tags formatting
            if 0 < tags <= self.tagLimit:
                data = {
                    "userId": self.current_user,
                    "tags": tags
                }

                self.people = Helpers.get_user_list_by_tags(data)
                self.proceed()
            else:
                self.bot.send_message(self.current_user, f"Invalid tag count")
                self.bot.register_next_step_handler(message, self.search_by_tags, acceptMode=True, chat_id=self.current_user)

    def free_search(self):
        Helpers.get_free_user_list(self.current_user)
        self.proceed()

    def personalityOff_handler(self, message, acceptMode=False):
        if not acceptMode:
            if Helpers.check_should_turnOf_personality(self.current_user) and not self.wasPersonalityTurnedOff:
                self.wasPersonalityTurnedOff = True
                self.bot.send_message(self.current_user,
                                      "We cant find anyone who matches your search parameters. Would you like to temporarily turn off PERSONALITY and continue search without it?",
                                      reply_markup=self.YNmarkup)
            else:
                self.proceed()
        else:
            if message.text == "Yes":
                Helpers.get_user_list_turnOffPersonality(self.current_user)
                if self.set_active_person():
                    self.show_person(message)
                else:
                    self.proceed()
            elif message.text == "No":
                self.proceed()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.YNmarkup)
                self.bot.register_next_step_handler(message, self.personalityOff_handler, acceptMode=acceptMode, chat_id=self.current_user)

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
            #Go back to search options
            self.start(self.msg)

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