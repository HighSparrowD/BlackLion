import json

import requests
from telebot.types import ReplyKeyboardMarkup, KeyboardButton

from Common.Menues import go_back_to_main_menu
from Core import HelpersMethodes as Helpers
from Helper import Helper


class FeedbackModule:
    def __init__(self, bot, msg, hasVisited=True):
        self.bot = bot
        self.message = msg
        self.current_user = msg.from_user.id
        self.active_section = None

        self.data = {}

        self.reasons = json.loads(requests.get(f"https://localhost:44381/feedback-reasons", verify=False).text)
        self.reas = []

        self.markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)
        self.exitMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("/exit")

        for reason in self.reasons:
            self.reas.append(reason["name"])
            self.markup.add(KeyboardButton(reason["name"]))

        self.markup.add(KeyboardButton("/exit"))

        self.start_text = "Hello, what are you willing to report? "
        self.middle_text = "Write us a message!"
        self.interrupt_text = "Give us your report when you ready"
        self.end_text = "Thank you for your input!"
        self.invalid_text = "No such option"

        self.helpHandler = self.bot.register_message_handler(self.help_handler, commands=["help"], user_id=self.current_user)
        self.first_message(msg)


    def first_message(self, msg, acceptMode=False):
        self.active_section = self.first_message
        if not acceptMode:
            self.bot.send_message(self.current_user, self.start_text, reply_markup=self.markup)
            self.bot.register_next_step_handler(msg, self.first_message, acceptMode=True, chat_id=self.current_user)
        else:
            if msg.text in self.reas:
                self.data["id"] = 0
                self.data["reasonId"] = self.reason_converter(msg.text)
                self.second_message(msg)
            elif msg.text == "/exit":
                self.bot.send_message(self.current_user, self.interrupt_text)
                self.destruct()
                return False
            else:
                self.bot.send_message(self.current_user, self.invalid_text, reply_markup=self.markup)
                self.bot.register_next_step_handler(msg, self.first_message, acceptMode=acceptMode, chat_id=self.current_user)

    def second_message(self, msg, acceptMode=False):
        self.active_section = self.second_message
        if not acceptMode:
            self.bot.send_message(self.current_user, self.middle_text, reply_markup=self.exitMarkup)
            self.bot.register_next_step_handler(msg, self.second_message, acceptMode=True, chat_id=self.current_user)
        else:
            if msg.text == "/exit":
                self.bot.send_message(self.current_user, self.interrupt_text)
                self.destruct()
                return False

            self.data["userBaseInfoId"] = msg.from_user.id
            self.data["text"] = msg.text

            d = json.dumps(self.data)
            requests.post("https://localhost:44381/AddFeedback", d, headers={
                "Content-Type": "application/json"}, verify=False)
            self.bot.send_message(self.current_user, self.end_text)
            self.destruct()

    def help_handler(self, message):
        Helper(self.bot, message, self.active_section)

    def reason_converter(self, reas):
        for reason in self.reasons:
            if reason["name"] == reas:
                return int(reason["id"])

    def destruct(self):
        self.bot.message_handlers.remove(self.helpHandler)
        go_back_to_main_menu(self.bot, self.current_user, self.message)
        del self
