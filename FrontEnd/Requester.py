import random

import telegram
from telebot.types import KeyboardButton, ReplyKeyboardMarkup
from Core import HelpersMethodes as Helpers
from ReportModule import ReportModule


class Requester:
    def __init__(self, bot, message, receiver, request_list, returnMethod=None):
        self.bot = bot
        self.message = message
        self.current_user = receiver
        self.current_user_data = Helpers.get_user_base_info(receiver)
        self.current_request = None
        self.request_list = request_list
        self.returnMethod = returnMethod

        self.current_managed_user = None
        self.current_managed_user_id = None

        Helpers.switch_user_busy_status(self.current_user)

        self.bYes = "😏"
        self.bNo = "👽"
        self.btnReport = "⚠"

        self.start_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)\
            .add(KeyboardButton("Show all"), KeyboardButton("Accept all"), KeyboardButton("Decline all"))
        self.markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(self.bYes, self.bNo)

        self.match_message = "You have got a match!\n"
        self.start_message = "Some people have liked you!"
        self.m = "<b>Someone liked you\n</b>"
        self.m1 = f"Hey! I have got a match for you. This person was notified about it, but he did not receive your username, thus he cannot write you first everything is in your hands, do not miss your chance!\n\n" #{data["userRealName"]}
        self.m2 = f"Hey! I have a match for you. Right now this person is deciding whether or not to write you Just wait for it!\n\n"

        self.menu_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True) \
            .add(KeyboardButton("/search"),
                 KeyboardButton("/random"),
                 KeyboardButton("/feedback"),
                 KeyboardButton("/settings"))

        self.start(message)

    def start(self, message, acceptMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, self.start_message, reply_markup=self.start_markup)
            self.bot.register_next_step_handler(message, self.start, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "Show all":
                self.process_request(message)
            elif message.text == "Accept all":
                self.accept_all_requests(message)
            elif message.text == "Decline all":
                self.decline_all_requests()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.start_markup)
                self.bot.register_next_step_handler(message, self.start, acceptMode=True, chat_id=self.current_user)

    def process_request(self, message, acceptMode=False):
        if not acceptMode:
            if self.set_current_request():

                self.get_request_sender_data()

                if self.current_request["isLikedBack"]:
                    self.bot.send_photo(self.current_user, self.current_managed_user["userPhoto"], f"<b>{self.current_request['description']}</b>\n\n{self.current_managed_user['userDescription']}", reply_markup=self.markup, parse_mode=telegram.ParseMode.HTML)
                    self.process_request(message)
                    return

                self.bot.send_photo(self.current_user, self.current_managed_user["userPhoto"], f"<b>Someone have liked you</b>\n\n{self.current_managed_user['userDescription']}", reply_markup=self.markup, parse_mode=telegram.ParseMode.HTML)
                self.bot.register_next_step_handler(message, self.process_request, acceptMode=True, chat_id=self.current_user)
            else:
                self.destruct()
        else:
            if message.text == self.bYes:
                self.accept_request(message)
            elif message.text == self.bNo:
                self.decline_request(message)
            elif message.text == self.btnReport:
                ReportModule(self.bot, message, self.current_managed_user['id'], self.process_request)
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.markup)
                self.bot.register_next_step_handler(message, self.process_request, acceptMode=acceptMode, chat_id=self.current_user)

    def accept_request(self, message):
        msg = Helpers.register_user_request(self.current_user, self.current_managed_user["id"], True)
        self.bot.send_message(self.current_user, msg)
        self.process_request(message)

    def accept_all_requests(self, message):
        for request in self.request_list:
            self.current_request = request
            self.get_request_sender_data()
            self.accept_request(message)
        self.destruct()


    def decline_all_requests(self):
        Helpers.delete_user_requests(self.current_user)

    def decline_request(self, message):
        Helpers.delete_user_request(self.current_request["id"])
        self.process_request(message)

    def set_current_request(self):
        if len(self.request_list) > 0:
            self.current_request = self.request_list[0]
            self.request_list.pop(0)
            return True
        return False

    def get_request_sender_data(self):
        user = Helpers.get_request_sender(self.current_request['id'])
        self.current_managed_user_id = user["userId"]
        self.current_managed_user = user["userBaseInfo"]

    def destruct(self):

        self.bot.send_message(self.current_user, "That is all for now :)")
        Helpers.switch_user_busy_status(self.current_user)

        if self.returnMethod:
            self.returnMethod(self.message)
            return

        self.bot.send_message(self.current_user, "What are we doing next? 😊", reply_markup=self.menu_markup)
        return