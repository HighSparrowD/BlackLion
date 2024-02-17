from telebot.types import InlineKeyboardMarkup, InlineKeyboardButton, ReplyKeyboardMarkup
from Core import HelpersMethodes as Helpers
from Common.Menues import count_pages, assemble_markup, reset_pages, add_tick_to_element, remove_tick_from_element, index_converter
import requests
import json

from Models import Advertisement as models
from Common.Menues import go_back_to_main_menu
from Helper import Helper
from ReportModule import ReportModule
from Settings import Settings
from Enums.AttendeeStatus import AttendeeStatus


class Personality_Bot:
    def __init__(self, bot, message, hasVisited=False):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id
        self.user_info = Helpers.get_user_info(self.current_user)
        self.hasVisited = hasVisited
        self.user_language = self.user_info.language

        self.return_method = None

        self.active_message = None
        self.secondary_message = None
        self.additional_message = None
        self.error_message = None

        self.current_callback_handler = None

        self.del_funcs = {"s": self.delete_secondary_message,
                          "e": self.delete_error_message,
                          "a": self.delete_additional_message}

        self.turnedOnSticker = "✅"
        self.turnedOffSticker = "❌"

    def send_active_message(self, text, markup=None, delete_msg: list[str] = None):
        """
        :param text: text of your message
        :param markup: markup to your message
        :param delete_msg: can be a list with "s", "e" or "a" for secondary, error or additional message correspondingly
        :return: sends active message and deletes messages which types are in the delete_msg param
        """
        try:
            if delete_msg is not None:
                for msg in delete_msg:
                    self.del_funcs[msg]()
            if self.active_message:
                self.bot.edit_message_text(text=text, chat_id=self.current_user, message_id=self.active_message, reply_markup=markup)
                return
            self.active_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except:
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

    def send_additional_message(self, text, markup=None):
        try:
            if self.additional_message:
                self.bot.edit_message_text(text, self.current_user, self.additional_message, reply_markup=markup)
                return

            self.additional_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except:
            self.delete_additional_message()
            self.send_additional_message(text, markup)

    def send_error_message(self, text, markup=None):
        try:
            if self.error_message:
                self.bot.edit_message_text(text, self.current_user, self.error_message, reply_markup=markup)
                return
            self.error_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except:
            self.delete_error_message()
            self.send_error_message(text, markup)

    def send_additional_actions_message(self, text, markup=None):
        self.delete_additional_message()
        self.additional_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id

    def send_simple_message(self, text, markup=None):
        self.bot.send_message(self.current_user, text, reply_markup=markup)

    def send_active_message_with_photo(self, text, photo, markup=None):
        if self.active_message:
            self.delete_active_message()
            self.active_message = self.bot.send_photo(self.current_user, photo, text, reply_markup=markup).id
        else:
            self.active_message = self.bot.send_photo(self.current_user, photo, text, reply_markup=markup).id

    def send_active_message_with_video(self, text, video, markup=None):
        if self.active_message:
            self.delete_active_message()
            self.active_message = self.bot.send_video(self.current_user, video, text, reply_markup=markup).id
        else:
            self.active_message = self.bot.send_video(self.current_user, video, text, reply_markup=markup).id

    def delete_active_message(self):
        if self.active_message:
            self.bot.delete_message(self.current_user, self.active_message)
            self.active_message = None

    def delete_secondary_message(self):
        if self.secondary_message:
            self.bot.delete_message(self.current_user, self.secondary_message)
            self.secondary_message = None

    def delete_additional_message(self):
        if self.additional_message:
            self.bot.delete_message(self.current_user, self.additional_message)
            self.additional_message = None

    def delete_error_message(self):
        if self.error_message:
            self.bot.delete_message(self.current_user, self.error_message)
            self.error_message = None

    def delete_message(self, message_id):
        self.bot.delete_message(self.current_user, message_id)

    def cleanup(self):
        self.delete_active_message()
        self.delete_secondary_message()
        self.delete_error_message()
        self.delete_additional_message()

    def prev_menu(self):  # returns you to previous menu (no matter is the menu in ad_module or in main)
        self.cleanup()
        if self.return_method:
            self.return_method()
        else:
            go_back_to_main_menu(self.bot, self.current_user, self.message)
            self.bot.callback_query_handlers.remove(self.current_callback_handler)
        del self
