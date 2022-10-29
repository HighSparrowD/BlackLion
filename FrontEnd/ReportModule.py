import json

import requests
import telegram
from telebot.types import ReplyKeyboardMarkup, KeyboardButton

from Core import HelpersMethodes as Helpers


class ReportModule:
    #return_method represents a certain method, that will be called upon reporter destruction. Thus allowing user to proceed in his bot usage
    def __init__(self, bot, msg, active_user, return_method, dontAddToBlackList=True):
        self.bot = bot
        self.message = msg
        self.current_user = msg.from_user.id
        self.return_method = return_method
        Helpers.switch_user_busy_status(self.current_user)

        self.dontAddToBlackList = dontAddToBlackList

        self.reasons_markup = None
        self.report_reasons = {}

        self.user_language = Helpers.get_user_app_language(self.current_user)
        self.active_user = active_user

        self.report_data = {}

        self.reasons = json.loads(requests.get(f"https://localhost:44381/GetFeedbackReasons/{self.user_language}", verify=False).text)
        self.reas = []

        self.markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)
        self.checkoutMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("1", "2", "3", "4")
        self.YNmarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("yes", "no")

        for reason in self.reasons:
            self.reas.append(reason["description"])
            self.markup.add(KeyboardButton(reason["description"]))

        self.start_text = "Hello, what are you willing to report? "
        self.middle_text = "Write us a message!"
        self.interrupt_text = "Give us your report when you ready"
        self.end_text = "Thank you for your input! We will deal with it ASAP :)"
        self.black_list_message = "Would you like to add this user to your blacklist so that you wont encounter him again?"
        self.invalid_text = "No such option"
        self.checkout_message = "1. Change report reason\n2. Change report text\n3.Submit report\n4.Abort"

        self.report_step1(msg)

    def report_step1(self, message, acceptMode=False, editMode=False):
        if not acceptMode:
            self.load_report_reasons(self.user_language)
            self.reasons_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)

            for reason in self.report_reasons.keys():
                self.reasons_markup.add(self.report_reasons[reason])

            self.bot.send_message(self.current_user, self.start_text, reply_markup=self.reasons_markup)
            self.bot.register_next_step_handler(message, self.report_step1, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            if message.text in self.report_reasons.values():
                self.report_data = {"id": 0,
                                    "UserBaseInfoId": self.current_user,
                                    "UserBaseInfoId1": self.active_user,
                                    "ReasonId": self.reason_converter(message.text),
                                    "ReasonClassLocalisationId": 0,
                                    }

                if not editMode:
                    self.report_user(message)
                else:
                    self.report_user_final(message)

    def report_user(self, message, acceptMode=False, editMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, "Please, enter a brief description of your report")
            self.bot.register_next_step_handler(message, self.report_user, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            self.report_data["text"] = message.text
            self.report_user_final(message)

    def report_user_final(self, message, acceptMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, f"Would you like to change anything before submitting the report?\n{self.checkout_message}", reply_markup=self.checkoutMarkup)
            self.bot.register_next_step_handler(message, self.report_user_final, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                self.report_step1(message, editMode=True)
            elif message.text == "2":
                self.report_user(message, editMode=True)
            elif message.text == "3":
                self.bot.send_message(self.current_user, "Thank you for your report. We will process it as soon as possible\n\n<b>Personality Administration</b>", parse_mode=telegram.ParseMode.HTML)
                self.send_report()
                if not self.dontAddToBlackList:
                    self.add_user_to_blacklist_step(message)
                else:
                    self.destruct()
            elif message.text == "4":
                self.destruct()
            else:
                self.bot.send_message(self.current_user, "No such option")
                self.bot.register_next_step_handler(message, self.report_user_final, acceptMode=acceptMode, chat_id=self.current_user)

    def add_user_to_blacklist_step(self, message, acceptMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, "Would you like to add this user to your blacklist?\nThus you wont have problem with him ever again", reply_markup=self.YNmarkup)
            self.bot.register_next_step_handler(message, self.add_user_to_blacklist_step, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "yes":
                self.add_user_to_blacklist()
                self.destruct()
            elif message.text == "no":
                self.destruct()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.YNmarkup)
                self.bot.register_next_step_handler(message, self.add_user_to_blacklist_step, acceptMode=acceptMode, chat_id=self.current_user)

    def load_report_reasons(self, localisationId):
        data = json.loads(requests.get(f"https://localhost:44381/GetReportReasons/{localisationId}", verify=False).text)

        for reason in data:
            self.report_reasons[reason['id']] = reason['description']

    def reason_converter(self, reason):
        for reas in self.report_reasons:
            if reason == self.report_reasons[reas]:
                pass
                return reas


    def send_report(self):
        d = json.dumps(self.report_data)

        requests.post(f"https://localhost:44381/AddUserReport", d, headers={
            "Content-Type": "application/json"}, verify=False)

    def add_user_to_blacklist(self):
        return requests.get(f"https://localhost:44381/AddUserToBlackList/{self.current_user}/{self.active_user}", verify=False).text

    def destruct(self):
        self.bot.send_message(self.current_user, "Done :)")
        self.return_method()
        del self
        return False