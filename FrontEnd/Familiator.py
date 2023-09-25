import requests

from Common.Menues import go_back_to_main_menu
from Requester import *
import Core.HelpersMethodes as Helpers


class Familiator:
    def __init__(self, bot, msg, cr_user, hasVisited=True):
        self.btnYes = "👌"
        self.btnNo = "🙊"
        # self.btnReport = "⚠"
        self.btnLeave = "🔙"

        self.finish_message = "That is all for now, please wait until we find someone else for you"

        self.bot = bot
        self.msg = msg
        self.current_user = cr_user
        self.active_user = None
        self.active_user_id = 0
        self.markup = ReplyKeyboardMarkup(resize_keyboard=True).add(self.btnYes, self.btnNo, self.btnLeave)
        self.YNmarkup = InlineKeyboardMarkup().add(InlineKeyboardButton("Yes", callback_data="5"), InlineKeyboardButton("No", callback_data="6"))

        self.goBackmarkup = InlineKeyboardMarkup().add(InlineKeyboardButton("Go Back", callback_data="-20"))

        self.actions_markup = InlineKeyboardMarkup().add(InlineKeyboardButton("⚠ Report ⚠", callback_data=self.active_user_id)) \
            .add(InlineKeyboardButton("🔖 Help 🔖", callback_data="11"))

        self.reactToCallback = True

        self.user_data = Helpers.get_user_basic_info(self.current_user)
        self.limitations = self.user_data["limitations"]
        self.tagLimit = self.limitations["maxTagsPerSearch"]

        self.wasPersonalityTurnedOff = False

        self.free_search_message = "Up for meeting someone today?"

        self.startMarkup = InlineKeyboardMarkup().add(InlineKeyboardButton("Normal search", callback_data="1")) \
            .add(InlineKeyboardButton("Search by tags", callback_data="2")) \
            .add(InlineKeyboardButton("Free search", callback_data="3")) \
            .add(InlineKeyboardButton("Go Back", callback_data="4"))

        self.active_message = None
        self.secondary_message = None
        self.nextHandler = None

        self.report_reasons = {}
        self.people = []

        # self.eh = self.bot.register_message_handler(self.exit_handler, commands=["exit"], user_id=self.current_user)
        # self.helpHandler = self.bot.register_message_handler(self.help_handler, commands=["help"], user_id=self.current_user)
        self.ch = self.bot.register_callback_query_handler("", self.callback_handler, user_id=self.current_user)

        if self.user_data["isFree"] is None:
            self.free_search_switch()
        else:
            self.start(msg)

    def free_search_switch(self):
        self.send_active_message(self.free_search_message, markup=self.YNmarkup)

    def start(self, message=None):
        self.send_active_message("Please, select a search", markup=self.startMarkup)

    def inform_about_limitations_with_message(self, msg):
        self.send_secondary_message(msg)
        self.start()

    def normal_search_handler(self, message=None):
        response = Helpers.get_user_list(self.current_user)
        self.people = response["users"]
        if self.set_active_person():
            self.show_person(message)
        else:
            self.proceed(message)

    def search_by_tags(self, message=None, acceptMode=False):
        if not acceptMode:
            self.send_active_message(f"Send me up to {self.tagLimit} tags to conduct the search", markup=self.goBackmarkup)
            self.nextHandler = self.bot.register_next_step_handler(message, self.search_by_tags, acceptMode=True, chat_id=self.current_user)
        else:
            tags = message.text
            if 0 < len(tags) <= self.tagLimit:
                tags = Helpers.format_tags(tags)
                data = {
                    "userId": self.current_user,
                    "tags": tags
                }

                response = Helpers.get_user_list_by_tags(data)
                self.people = response["users"]
                if not self.people:
                    self.send_secondary_message("No users matches your request yet. Try again with another tag list :)")
                    self.bot.register_next_step_handler(message, self.search_by_tags, acceptMode=True, chat_id=self.current_user)
                    return

                self.proceed()
            else:
                self.send_secondary_message(f"Invalid tag count")
                self.nextHandler = self.bot.register_next_step_handler(message, self.search_by_tags, acceptMode=True, chat_id=self.current_user)

    def free_search(self):
        response = Helpers.get_free_user_list(self.current_user)
        self.people = response["users"]
        self.proceed()

    def set_active_person(self, msg=None):
        if self.people:
            self.active_user = self.people[0]
            self.active_user_id = self.active_user["userId"]
            Helpers.register_user_encounter_familiator(self.current_user, self.active_user_id)
            self.people.pop(0)

            self.set_report_button_value()
            return True
        return False

    def show_person(self, message, acceptMode=False):
        if not acceptMode:
            user = self.active_user["userData"]

            if user["mediaType"] == "Photo":
                self.bot.send_photo(self.current_user, user["userMedia"], user["userDescription"], reply_markup=self.markup)
            else:
                self.bot.send_video(self.current_user, video=user["userMedia"], caption=user["userDescription"], reply_markup=self.markup)

            self.bot.send_message(self.current_user, "Additional Actions:", reply_markup=self.actions_markup)

            if self.active_user["comment"]:
                self.bot.send_message(self.current_user, self.active_user["comment"])

            self.bot.register_next_step_handler(message, self.show_person, acceptMode=True, chat_id=self.current_user)
        else:
            if not self.reactToCallback:
                self.bot.delete_message(self.current_user, message.id)
                self.bot.register_next_step_handler(message, self.show_person, acceptMode=acceptMode,chat_id=self.current_user)
                return

            if message.text == self.btnYes:
                if not self.user_data["isBanned"]:
                    Helpers.register_user_request(self.current_user, self.active_user_id, False)

                active_reply = Helpers.get_user_active_reply(self.active_user_id)
                if active_reply:
                    if not active_reply["isEmpty"]:
                        if active_reply["isText"]:
                            self.bot.send_message(self.current_user, f"⬆This user has a message for you ;-)⬆\n\n{active_reply['autoReply']}")
                        else:
                            self.bot.send_voice(self.current_user, active_reply["autoReply"], "⬆ This user has a message for you ;-) ⬆")

                try:
                    if not Helpers.check_user_is_busy(self.active_user_id):
                        response = Helpers.get_user_requests(self.active_user_id)

                        request_list = response["requests"]

                        Requester(self.bot, message, self.active_user_id, request_list, response["notification"])
                except:
                    pass
            elif message.text == self.btnNo:
                sadMessage = Helpers.decline_user_request(self.current_user, self.active_user_id)

                if sadMessage:
                    self.bot.send_message(self.current_user, sadMessage)

            elif message.text == self.btnLeave:
                self.start(message)
                return
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.markup)
                self.bot.register_next_step_handler(message, self.show_person, acceptMode=acceptMode, chat_id=self.current_user)

            #Interception between Requester and Familiator
            response = Helpers.get_user_requests(self.current_user)
            request_list = response["requests"]

            if request_list:
                Requester(self.bot, self.msg, self.current_user, request_list, response["notification"], self.proceed)
                return
            self.proceed()

    def proceed(self, msg=None):
        if self.set_active_person():
            self.show_person(self.msg)
        else:
            self.send_secondary_message(self.finish_message)
            #Go back to search options
            self.start(self.msg)

    def callback_handler(self, call):
        self.bot.answer_callback_query(call.id, "")
        if call.data == "1":
            if self.limitations["actualProfileViews"] < self.limitations["maxProfileViews"]:
                self.normal_search_handler()
            else:
                self.inform_about_limitations_with_message(
                    "Sorry, you have run out of profile searches for today.\nYou can still use Card Deck Mini or Card Deck Premium to replenish your views, buy premium and thus double your view count, or wait until tomorrow :)")
        elif call.data == "2":
            if self.limitations["actualTagViews"] < self.limitations["maxTagViews"]:
                self.search_by_tags()
            else:
                self.inform_about_limitations_with_message(
                    "Sorry, you have run out of tag searches for today.\nYou can still use Card Deck Mini or Card Deck Premium to replenish your views, buy premium and thus double your view count, or wait until tomorrow :)")
        elif call.data == "3":
            if self.limitations["actualProfileViews"] < self.limitations["maxProfileViews"]:
                self.free_search()
            else:
                self.inform_about_limitations_with_message(
                    "Sorry, you have run out of profile searches for today.\nYou can still use Card Deck Mini or Card Deck Premium to replenish your views, buy premium and thus double your view count, or wait until tomorrow :)")
        elif call.data == "4":
            self.destruct()
        elif call.data == "5":
            response = requests.get(f"https://localhost:44381/SetUserFreeSearchParam/{self.current_user}/{True}",
                                    verify=False)

            if response.status_code == 200:
                self.send_secondary_message("Done. You can change this parameter at any time in settings")
            else:
                self.send_secondary_message("Something went wrong. Please try again later")

            self.start()
        elif call.data == "6":
            response = requests.get(f"https://localhost:44381/SetUserFreeSearchParam/{self.current_user}/{False}",
                                    verify=False)

            if response.status_code == 200:
                self.send_secondary_message("Done. You can change this parameter at any time in settings")
            else:
                self.send_secondary_message("Something went wrong. Please try again later")

            self.start()
        elif call.data == "-20":
            self.remove_next_step_handler_local()
            self.start()
        elif call.data == "11":
            self.help_handler(self.msg)
        else:
            if self.reactToCallback:
                ReportModule(self.bot, self.msg, call.data, self.proceed)

    def help_handler(self, message):
        self.reactToCallback = False
        Helper(self.bot, message, self.proceed_h)

    #Proceed method meant only for exiting Helper
    def proceed_h(self, message):
        self.reactToCallback = True
        pass
        # self.show_person(message)
        # if self.set_active_person(message):
        # else:
        #     self.start(message)

    def move_to_next_user(self, message):
        if self.set_active_person():
            self.show_person(message)
        else:
            self.send_secondary_message(self.finish_message)
            self.destruct()

    def set_report_button_value(self):
        self.actions_markup.keyboard[0][0].callback_data = self.active_user_id

    def send_active_message(self, text, markup=None):
        try:
            if self.active_message:
                self.bot.edit_message_text(text, self.current_user, self.active_message, reply_markup=markup)
                return

            self.active_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except Exception as ex:
            self.delete_active_message()
            self.send_active_message(text, markup)

    def send_secondary_message(self, text, markup=None):
        try:
            if self.secondary_message:
                self.bot.edit_message_text(text, self.current_user, self.secondary_message, reply_markup=markup)
                return

            self.secondary_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except:
            self.delete_secondary_message()
            self.send_secondary_message(text, markup)

    def delete_active_message(self):
        if self.active_message:
            self.bot.delete_message(self.current_user, self.active_message)
            self.active_message = None

    def delete_secondary_message(self):
        if self.secondary_message:
            self.bot.delete_message(self.current_user, self.secondary_message)
            self.secondary_message = None

    def remove_next_step_handler_local(self):
        if self.nextHandler:
            try:
                self.bot.remove_next_step_handler(self.current_user, self.nextHandler)
                self.nextHandler = None
            except:
                pass

    def destruct(self):
        self.bot.callback_query_handlers.remove(self.ch)

        self.delete_active_message()
        self.delete_secondary_message()

        go_back_to_main_menu(self.bot, self.current_user, self.msg)
        del self
