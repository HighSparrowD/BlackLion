import random

from telebot.types import KeyboardButton, ReplyKeyboardMarkup
from Core import HelpersMethodes as Helpers


class Requester:
    def __init__(self, bot, message, receiver, request_list):
        self.bot = bot
        self.message = message
        self.receiver = receiver
        self.receiver_data = Helpers.get_user_base_info(receiver)
        self.current_request = None
        self.request_list = request_list

        Helpers.switch_user_busy_status(self.receiver)

        self.bYes = KeyboardButton("😏")
        self.bNo = KeyboardButton("👽")

        self.start_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)\
            .add(KeyboardButton("Show all"), KeyboardButton("Accept all"), KeyboardButton("Decline all"))
        self.markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(self.bYes, self.bNo)

        self.match_message = "You have got a match!\n"
        self.start_message = "Some people have liked you!"
        self.m = "Someone liked you\n"
        self.m1 = f"Hey! I have got a match for you. This person was notified about it, but he did not receive your username, thus he cannot write you first everything is in your hands, do not miss your chance!\n\n" #{data["userRealName"]}
        self.m2 = f"Hey! I have a match for you. Right now this person is deciding whether or not to write you Just wait for it!\n\n"

        self.bot.send_message(self.receiver, self.start_message, reply_markup=self.start_markup)
        self.bot.register_next_step_handler(message, self.start, chat_id=self.receiver)

        self.menu_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True) \
            .add(KeyboardButton("/search"),
                 KeyboardButton("/random"),
                 KeyboardButton("/feedback"),
                 KeyboardButton("/sponsoraccount"),
                 KeyboardButton("/random"),
                 KeyboardButton("/shop"))

        # if liked:
        #     self.d["userDescription"] = self.m + self.d["userDescription"]
        #     self.bot.send_photo(self.receiver, self.d["userPhoto"], self.d["userDescription"], reply_markup=self.markup)
        #     self.destruct(False)

    def message_handler(self, message, isStack=False):
        if message.text == self.bYes.text:
            self.accept_current_request(isStack)
            Helpers.delete_user_request(self.current_request['id'])

        elif message.text == self.bNo.text: #TODO: add message or smth???
            Helpers.delete_user_request(self.current_request['id'])
            if self.set_current_request():
                self.process_current_request()
            else:
                self.bot.send_message(self.receiver, "That is all for now ;-)")
                self.destruct(True)

    def start(self, message):
        if message.text == "Show all":
            if self.set_current_request():
                self.process_current_request()
            else:
                self.bot.send_message(self.receiver, "Self destruction immanent") #TODO: remove after debug
                self.destruct(True)
        elif message.text == "Decline all":
            Helpers.delete_user_requests(self.receiver)
            self.bot.send_message(self.receiver, "All requests have been declined !")
            self.destruct(True)
        elif message.text == "Accept all":
            for request in self.request_list:
                self.current_request = request
                self.message_handler(self.bYes, True)
            self.bot.send_message(self.receiver, "All requests have been accepted !")
            self.destruct(True)

    def set_current_request(self):
        if self.request_list:
            self.current_request = self.request_list[0]
            self.request_list.pop(0)
            Helpers.delete_user_request(self.current_request["id"])
            return True
        return False

    def process_current_request(self):
        sender = self.current_request["sender"]["userBaseInfo"]
        if bool(self.current_request["isLikedBack"]):
            if self.current_request["description"]:
                msg = f"{self.current_request['description']}\n\n{sender['userDescription']}"
            else:
                msg = self.m + sender["userDescription"]
        else:
            msg = self.m + sender["userDescription"]

        self.bot.send_photo(self.receiver, sender["userPhoto"], msg, reply_markup=self.markup)
        if bool(self.current_request["isLikedBack"]):
            if self.set_current_request():
                self.process_current_request()
            else:
                self.bot.send_message(self.receiver, "That is all for now ;-)")
                self.destruct(True)
            return False
        self.bot.register_next_step_handler(self.message, self.message_handler, chat_id=self.receiver)

    def accept_current_request(self, isStack=False):
        sender = self.current_request["sender"]["userBaseInfo"]
        i = random.Random().randint(0, 1)

        active_reply = Helpers.get_user_active_reply(sender["id"])
        if active_reply:
            if not active_reply["isEmpty"]:
                self.bot.send_message(self.receiver, "⬇ This user has a message for you ;-) ⬇")
                if active_reply["isText"]:
                    self.bot.send_message(self.receiver, f'"{active_reply["autoReply"]}"')
                else:
                    self.bot.send_voice(self.receiver, active_reply["autoReply"])

        if i == 0:  # TODO: Optimize!!!
            msg = f"{self.m1}\n@{self.receiver_data['userName']}"
            Helpers.register_user_request(self.receiver, sender["id"], True, msg)
            self.bot.send_message(self.receiver, self.m2)
        else:
            msg = f"{self.m1}\n@{sender['userName']}"
            self.bot.send_message(self.receiver, msg)
            self.bot.send_message(self.receiver, self.m1)
            Helpers.register_user_request(self.receiver, sender["id"], True, self.m2)

        if not Helpers.check_user_is_busy(sender["id"]):
            req_list = Helpers.get_user_requests(sender["id"])
            Requester(self.bot, self.message, sender["id"], req_list)

        if not isStack:
            if self.set_current_request():
                self.process_current_request()
            else:
                self.bot.send_message(self.receiver, "That is all for now ;-)")
                self.destruct(True)

    def destruct(self, shouldSend):
        Helpers.switch_user_busy_status(self.receiver)
        if shouldSend:
            self.bot.send_message(self.receiver, "What are we doing next? 😊", reply_markup=self.menu_markup)
        del self