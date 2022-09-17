import json

import requests
from telebot.types import ReplyKeyboardMarkup, KeyboardButton

from Common.Menues import go_back_to_main_menu
from Core import HelpersMethodes as Helpers


class Reporter:
    def __init__(self, bot, msg, language, reporters, hasVisited=True):
        self.bot = bot
        self.current_user = msg.from_user.id
        Helpers.switch_user_busy_status(self.current_user)
        self.reporters = reporters
        self.reporters.append(self)

        self.data = {}

        self.reasons = json.loads(requests.get(f"https://localhost:44381/GetFeedbackReasons/{language}", verify=False).text)
        self.reas = []

        self.markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)

        for reason in self.reasons:
            self.reas.append(reason["description"])
            self.markup.add(KeyboardButton(reason["description"]))

        self.start_text = "Hello, what are you willing to report? "
        self.middle_text = "Write us a message!"
        self.interrupt_text = "Give us your report when you ready"
        self.end_text = "Thank you for your input!"
        self.invalid_text = "No such option"

        self.bot.register_next_step_handler(msg, self.first_message, chat_id=self.current_user)
        self.bot.send_message(self.current_user, self.start_text, reply_markup=self.markup)

    def first_message(self, msg):
        if msg.text in self.reas:
            self.data["id"] = 0
            self.data["reasonId"] = self.reason_converter(msg.text)
            self.bot.send_message(self.current_user, self.middle_text)
            self.bot.register_next_step_handler(msg, self.second_message, chat_id=self.current_user)
        elif msg.text == "/exit":
            self.bot.send_message(self.current_user, self.interrupt_text)
            self.destruct()
            return False
        else:
            self.bot.send_message(self.current_user, self.invalid_text, reply_markup=self.markup)
            self.bot.register_next_step_handler(msg, self.first_message, chat_id=self.current_user)

    def second_message(self, msg):
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

    def reason_converter(self, reas):
        for reason in self.reasons:
            if reason["description"] == reas:
                return int(reason["id"])

    def destruct(self):
        go_back_to_main_menu(self.bot, self.current_user)
        self.reporters.remove(self)
        Helpers.switch_user_busy_status(self.current_user)
        del self
