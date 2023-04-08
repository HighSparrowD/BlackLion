import requests

from Common.Menues import go_back_to_main_menu
from Requester import *


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
        Helpers.switch_user_busy_status(self.current_user)
        self.active_user = None
        self.active_user_id = 0
        self.markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(self.btnYes, self.btnNo, self.btnLeave)
        self.YNmarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("Yes", "No")

        self.goBackmarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("Go Back")

        self.actions_markup = InlineKeyboardMarkup().add(InlineKeyboardButton("⚠ Report ⚠", callback_data=self.active_user_id)) \
            .add(InlineKeyboardButton("🔖 Help 🔖", callback_data="11"))

        self.reactToCallback = True

        self.basic_info = Helpers.get_user_basic_info(self.current_user)
        self.limitations = self.basic_info["limitations"]
        self.tagLimit = self.limitations["maxTagsPerSearch"]

        self.wasPersonalityTurnedOff = False

        self.start_message = "1. Normal search\n2. Search by tags\n3. Free search\n4.Exit"
        self.free_search_message = "Up for meeting someone today?"

        self.startMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("1", "2", "3", "4")

        self.report_reasons = {}
        self.people = []
        self.reasons_markup = None

        # self.eh = self.bot.register_message_handler(self.exit_handler, commands=["exit"], user_id=self.current_user)
        # self.helpHandler = self.bot.register_message_handler(self.help_handler, commands=["help"], user_id=self.current_user)
        self.ch = self.bot.register_callback_query_handler("", self.callback_handler, user_id=self.current_user)

        if not Helpers.check_user_have_chosen_free_search(self.current_user):
            self.free_search_switch(msg)
        else:
            self.start(msg)

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
                response = requests.get(f"https://localhost:44381/SetUserFreeSearchParam/{self.current_user}/{False}",
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
                if self.limitations["actualProfileViews"] < self.limitations["maxProfileViews"]:
                    self.normal_search_handler(message)
                else:
                    self.inform_about_limitations_with_message("Sorry, you have run out of profile searches for today.\nYou can still use Card Deck Mini or Card Deck Premium to replenish your views, buy premium and thus double your view count, or wait until tomorrow :)")
            elif message.text == "2":
                if self.limitations["actualTagViews"] < self.limitations["maxTagViews"]:
                    self.search_by_tags(message)
                else:
                    self.inform_about_limitations_with_message("Sorry, you have run out of tag searches for today.\nYou can still use Card Deck Mini or Card Deck Premium to replenish your views, buy premium and thus double your view count, or wait until tomorrow :)")
            elif message.text == "3":
                if self.limitations["actualProfileViews"] < self.limitations["maxProfileViews"]:
                    self.free_search()
                else:
                    self.inform_about_limitations_with_message("Sorry, you have run out of profile searches for today.\nYou can still use Card Deck Mini or Card Deck Premium to replenish your views, buy premium and thus double your view count, or wait until tomorrow :)")
            elif message.text == "4":
                self.destruct()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.startMarkup)
                self.bot.register_next_step_handler(message, self.start, acceptMode=acceptMode, chat_id=self.current_user)

    def inform_about_limitations_with_message(self, msg):
        self.bot.send_message(self.current_user, msg)
        self.start(msg)

    def normal_search_handler(self, message):
        self.people = Helpers.get_user_list(self.current_user)
        if self.people:
            if self.set_active_person():
                self.show_person(message)
        else:
            #TODO: Remove
            self.personalityOff_handler(message)

    def search_by_tags(self, message, acceptMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, f"Sent me up to {self.tagLimit} tags to conduct the search", reply_markup=self.goBackmarkup)
            self.bot.register_next_step_handler(message, self.search_by_tags, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "Go Back":
                self.start(message)
                return

            tags = message.text.split()
            #TODO: check tags formatting
            if 0 < len(tags) <= self.tagLimit:
                data = {
                    "userId": self.current_user,
                    "tags": tags
                }

                person = Helpers.get_user_list_by_tags(data)
                if not person:
                    self.bot.send_message(self.current_user, "No users matches your request yet. Try again with another tag list :)")
                    self.bot.register_next_step_handler(message, self.search_by_tags, acceptMode=True, chat_id=self.current_user)
                    return

                self.people.append(person)
                self.proceed()
            else:
                self.bot.send_message(self.current_user, f"Invalid tag count")
                self.bot.register_next_step_handler(message, self.search_by_tags, acceptMode=True, chat_id=self.current_user)

    def free_search(self):
        Helpers.get_free_user_list(self.current_user)
        self.proceed()

    #TODO: remove
    def personalityOff_handler(self, message, acceptMode=False):
        if not acceptMode:
            if Helpers.check_should_turnOf_personality(self.current_user) and not self.wasPersonalityTurnedOff:
                self.wasPersonalityTurnedOff = True
                self.bot.send_message(self.current_user,
                                      "We cant find anyone who matches your search parameters. Would you like to temporarily turn off PERSONALITY and continue search without it?",
                                      reply_markup=self.YNmarkup)
                self.bot.register_next_step_handler(message, self.personalityOff_handler, acceptMode=True, chat_id=self.current_user)
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

    def set_active_person(self, msg=None):
        if self.people:
            self.active_user = self.people[0]
            Helpers.register_user_encounter_familiator(self.current_user, self.active_user_id)
            self.active_user_id = self.active_user["userId"]
            self.people.pop(0)

            self.set_report_button_value()
            return True
        return False

    def show_person(self, message, acceptMode=False):
        if not acceptMode:
            user = self.active_user["userBaseInfo"]

            if self.active_user["comment"]:
                self.bot.send_message(self.current_user, self.active_user["comment"])

            if user["isMediaPhoto"]:
                self.bot.send_photo(self.current_user, user["userMedia"], user["userDescription"], reply_markup=self.markup)
            else:
                self.bot.send_video(self.current_user, video=user["userMedia"], caption=user["userDescription"], reply_markup=self.markup)

            self.bot.send_message(self.current_user, "Additional Actions:", reply_markup=self.actions_markup)
            self.bot.register_next_step_handler(message, self.show_person, acceptMode=True, chat_id=self.current_user)
        else:
            if not self.reactToCallback:
                self.bot.delete_message(self.current_user, message.id)
                self.bot.register_next_step_handler(message, self.show_person, acceptMode=acceptMode,chat_id=self.current_user)
                return

            if message.text == self.btnYes:
                if not self.basic_info["isBanned"]:
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
                        req_list = Helpers.get_user_requests(self.active_user_id)
                        Requester(self.bot, message, self.active_user_id, req_list)
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
            requests = Helpers.get_user_requests(self.current_user)
            if requests:
                Requester(self.bot, self.msg, self.current_user, requests, self.proceed)
                return
            self.proceed()

    def proceed(self, msg=None):
        if self.set_active_person():
            self.show_person(self.msg)
        else:
            self.bot.send_message(self.current_user, self.finish_message)
            #Go back to search options
            self.start(self.msg)

    def callback_handler(self, call):
        if call.data == "11":
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
            self.bot.send_message(self.current_user, self.finish_message)
            self.destruct()

    def set_report_button_value(self):
        self.actions_markup.keyboard[0][0].callback_data = self.active_user_id

    def destruct(self):
        self.bot.callback_query_handlers.remove(self.ch)
        go_back_to_main_menu(self.bot, self.current_user, self.msg)
        del self