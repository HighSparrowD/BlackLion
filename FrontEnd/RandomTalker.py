import copy

import requests
from telebot.types import ReplyKeyboardMarkup, KeyboardButton

import Core.HelpersMethodes as Helpers

from Common.Menues import go_back_to_main_menu
from Requester import Requester


class RandomTalker:
    def __init__(self, bot, message, random_talkers, hasVisited=True):
        self.bot = bot
        self.message = message
        self.welcome_message = "We have found someone for you!"
        self.interrupt_message = "Come back soon"
        self.stop_message = "Conversation had been stopped. Finding a new person.\nHit /exit to exit this mode"
        self.exit_message = ""
        self.current_user = message.chat.id
        Helpers.switch_user_busy_status(self.current_user)
        #self.current_user_lang_prefs = Helpers.get_user_language_prefs(self.current_user) #TODO: Use another method from API, that retrieves an array of spoken languages and language preferences
        self.random_talkers = random_talkers
        self.user2 = 0
        self.user2_base = None
        self.isConversing = False
        self.ah = None
        self.rh = None

        self.random_talkers = random_talkers

        self.YNmarkup = ReplyKeyboardMarkup(resize_keyboard=True, row_width=3, one_time_keyboard=True).add(KeyboardButton("Yes")).add(KeyboardButton("No"))
        self.exit_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(KeyboardButton("/exit"))
        self.user_action_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(KeyboardButton("/stop"), KeyboardButton("/exit"))

        if not hasVisited:
            self.bot.send_message(self.current_user, "Hey! This is you first time here isn't it?\nFirst thing we have to do is to decide if your language preferences should be considered\n\nHow it works: If you press 'Yes' all companions you'll encounter are going to speak in a languages you have chosen during registration. If you decide to press 'No' - you will be able to communicate with absolutely random people, from random places.", reply_markup=self.YNmarkup)
            self.bot.register_next_step_handler(message, self.language_prefs_step, chat_id=self.current_user)
        else:
            self.enter()

    def enter(self):
        self.bot.send_message(self.current_user, "Waiting for another user to join", reply_markup=self.exit_markup)
        self.ah = self.bot.register_message_handler(self.awaiting_handler, commands=["exit"], user_id=self.current_user)
        self.random_talkers.append(self)

        if len(self.random_talkers) > 1:
            for rt in self.random_talkers:
                if Helpers.check_users_are_combinable(self.current_user, rt.current_user) and self.current_user != rt.current_user and not rt.isConversing:
                    rt.start(self)
                    self.start(rt)
                    break

    def language_prefs_step(self, message):
        if message.text == "Yes":
            self.set_rt_language_prefs(True)
            self.bot.send_message(self.current_user, "Gotcha")
            self.enter()
            return False
        elif message.text == "No":
            self.set_rt_language_prefs(False)
            self.bot.send_message(self.current_user, "Gotcha")
            self.enter()
            return False

        self.bot.send_message(self.current_user, "No such option ;-)", reply_markup=self.YNmarkup)
        self.bot.register_next_step_handler(message, self.language_prefs_step, chat_id=self.current_user)

    def start(self, user):
        self.user2_base = user
        self.user2 = user.current_user
        Helpers.register_user_encounter_rt(self.current_user, self.user2)
        self.bot.send_message(self.current_user, "We have found someone for you!\nYou can start chatting!", reply_markup=self.user_action_markup)
        self.rh = self.bot.register_message_handler(self.random_handler, user_id=self.current_user)
        self.isConversing = True

    def awaiting_handler(self, message):
        self.bot.send_message(self.current_user, self.interrupt_message)
        if self.user2_base:
            self.user2_base.stop()
        self.destruct()

    def random_handler(self, message):
        if message.text != "/stop":
            if message.from_user.id != self.current_user: #I have no fucking clue why it doesn't work THE FUCKING normal way, but fuck that, i don't care, just glad it works
                self.bot.send_message(self.current_user, message.text)
                return False
            self.bot.send_message(self.user2, message.text)

        else:
            if self.user2_base:
                self.user2_base.stop()
                self.stop()

    def stop(self):
        if self.user2_base:
            self.bot.send_message(self.current_user, self.stop_message, reply_markup=self.exit_markup)
            self.user2_base = None

        if self.rh in self.bot.message_handlers:
            self.bot.message_handlers.remove(self.rh)

        self.isConversing = False

    def set_rt_language_prefs(self, shouldBeConsidered):
        requests.get(f"https://localhost:44381/SetUserRtLanguagePrefs/{self.current_user}/{shouldBeConsidered}", verify=False)

    def destruct(self):
        Helpers.switch_user_busy_status(self.current_user)
        self.random_talkers.remove(self)
        if self.user2_base:
            if self.user2_base.user2_base:
                self.user2_base.user2_base = None
            self.user2_base = None
        if self.ah:
            self.bot.message_handlers.remove(self.ah)
        if self.rh in self.bot.message_handlers:
            self.bot.message_handlers.remove(self.rh)
        if Helpers.check_user_has_requests(self.current_user):
            request_list = Helpers.get_user_requests(self.current_user)
            Requester(self.bot, self.message, self.current_user, request_list)
            return False
        go_back_to_main_menu(self.bot, self.current_user)
        del self