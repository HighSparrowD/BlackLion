from telebot.types import KeyboardButton, ReplyKeyboardMarkup, InlineKeyboardButton, InlineKeyboardMarkup
from Core import HelpersMethodes as Helpers
from Helper import Helper
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
        self.current_request_id = None
        self.comment = None

        self.active_section = None

        self.active_user_id = "0"

        self.bYes = "😏"
        self.bNo = "👽"

        self.reactToCallback = True

        self.start_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)\
            .add(KeyboardButton("Show all"), KeyboardButton("Accept all"), KeyboardButton("Decline all"))
        self.markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(self.bYes, self.bNo)

        self.actions_markup = InlineKeyboardMarkup().add(InlineKeyboardButton("⚠ Report ⚠", callback_data=self.active_user_id)) \
            .add(InlineKeyboardButton("🔖 Help 🔖", callback_data="11"))

        self.start_message = "Some people have liked you!"

        if len(self.request_list) > 1:
            self.start_message = self.request_list[0]["description"]
            self.request_list.pop(0)

        self.menu_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True) \
            .add(KeyboardButton("/search"),
                 KeyboardButton("/random"),
                 KeyboardButton("/feedback"),
                 KeyboardButton("/settings"))

        self.ch = self.bot.register_callback_query_handler("", self.callback_handler, user_id=self.current_user)

        self.start(message)

    def start(self, message, acceptMode=False):
        self.active_section = self.start

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
        self.active_section = self.process_request
        if not acceptMode:
            if self.set_current_request():

                self.get_request_sender_data()

                if self.current_request["isLikedBack"]:
                    # self.bot.send_photo(self.current_user, self.current_managed_user["userMedia"], f"<b>{self.current_request['description']}</b>\n\n{self.current_managed_user['userDescription']}", reply_markup=self.markup, parse_mode=telegram.ParseMode.HTML)
                    self.send_user_media(True)
                    Helpers.delete_user_request(self.current_request_id)
                    self.request_list.pop(0)
                    self.process_request(message)
                    # self.bot.register_next_step_handler(message, self.process_request, acceptMode=True, chat_id=self.current_user)
                    return

                self.send_user_media(False)

                # self.bot.send_photo(self.current_user, self.current_managed_user["userMedia"], f"<b>Someone had liked you</b>\n\n{self.current_managed_user['userDescription']}", reply_markup=self.markup)
                self.bot.register_next_step_handler(message, self.process_request, acceptMode=True, chat_id=self.current_user)
            else:
                self.destruct()
        else:
            if not self.reactToCallback:
                self.bot.delete_message(self.current_user, message.id)
                self.bot.register_next_step_handler(message, self.process_request, acceptMode=acceptMode, chat_id=self.current_user)
                return

            if message.text == self.bYes:
                self.accept_request(message)
            elif message.text == self.bNo:
                self.decline_request(message)
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.markup)
                self.bot.register_next_step_handler(message, self.process_request, acceptMode=acceptMode, chat_id=self.current_user)

    def accept_request(self, message):
        msg = Helpers.register_user_request(self.current_user, self.current_managed_user["id"], True)
        self.bot.send_message(self.current_user, msg)
        self.request_list.pop(0)
        self.process_request(message)

    def accept_all_requests(self, message):
        for request in self.request_list:
            self.current_request = request
            self.get_request_sender_data()
            self.accept_request(message)
        self.request_list.clear()
        self.destruct()

    def decline_all_requests(self):
        Helpers.delete_user_requests(self.current_user)
        self.request_list.clear()
        self.destruct()

    def decline_request(self, message):
        sadMessage = Helpers.decline_user_request(self.current_user, self.current_managed_user_id)

        if sadMessage:
            self.bot.send_message(self.current_user, sadMessage)

        Helpers.delete_user_request(self.current_request_id)
        self.request_list.pop(0)
        self.process_request(message)

    def set_current_request(self):
        if len(self.request_list) > 0:
            self.current_request = self.request_list[0]
            self.active_user_id = self.current_request["senderId"]
            self.current_request_id = self.current_request["id"]
            return True
        return False

    def send_user_media(self, isLikedBack):
        bonus = "<b>Someone had liked you</b>\n\n"

        if isLikedBack:
            bonus = f"<b>{self.current_request['description']}</b>"

        if self.current_managed_user["mediaType"] == "Photo":
            self.bot.send_photo(self.current_user, self.current_managed_user["userMedia"],
                                f"{bonus}{self.current_managed_user['userDescription']}",
                                reply_markup=self.markup)
        else:
            self.bot.send_video(self.current_user, video=self.current_managed_user["userMedia"],
                                caption=f"{bonus}{self.current_managed_user['userDescription']}",
                                reply_markup=self.markup)

        self.bot.send_message(self.current_user, "Additional Actions:", reply_markup=self.actions_markup)

        if self.comment:
            self.bot.send_message(self.current_user, self.comment)

    def get_request_sender_data(self):
        response = Helpers.get_request_sender(self.active_user_id)
        self.current_managed_user_id = response["userId"]
        self.current_managed_user = response["userDataInfo"]
        self.comment = response["comment"]

    def help_handler(self, message):
        self.reactToCallback = False
        Helper(self.bot, message, self.proceed_h)

    #Proceed method meant only for exiting Helper
    def proceed_h(self, message):
        self.reactToCallback = True

    def callback_handler(self, call):
        if call.data == "11":
            self.help_handler(self.message)
        else:
            if self.reactToCallback:
                ReportModule(self.bot, self.message, call.data, self.process_request)

    def destruct(self):
        self.bot.send_message(self.current_user, "That is all for now :)")

        self.bot.callback_query_handlers.remove(self.ch)

        Helpers.switch_user_busy_status(self.current_user, 12)

        if self.returnMethod:
            self.returnMethod(self.message)
            return

        self.bot.send_message(self.current_user, "What are we doing next? 😊", reply_markup=self.menu_markup)
        return