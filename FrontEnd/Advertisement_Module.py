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

class AdvertisementModule:
    def __init__(self, bot, message, hasVisited=False):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id
        self.user_info = Helpers.get_user_info(self.current_user)
        self.hasVisited = hasVisited
        self.user_language = self.user_info["language"]

        self.return_method = None

        self.active_message = None
        self.secondary_message = None
        self.additional_message = None
        self.error_message = None

        self.current_callback_handler = self.bot.register_callback_query_handler(message, self.callback_handler, user_id=self.current_user)

        self.main_menu_markup = InlineKeyboardMarkup().add(InlineKeyboardButton('My ads', callback_data='1'))\
            .add(InlineKeyboardButton('Overall statistics', callback_data='2'))\
            .add(InlineKeyboardButton('Exit', callback_data='0'))

        self.start()

    def start(self):
        self.send_active_message('What you want to see?', markup=self.main_menu_markup)

    def send_active_message(self, text, markup):
        try:
            if self.active_message:
                self.bot.edit_message_text(text, self.current_user, self.active_message, reply_markup=markup)
                return
            self.active_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except:
            self.delete_active_message()
            self.send_active_message(text, markup)

    def send_error_message(self, text, markup=None):
        try:
            if self.error_message:
                self.bot.edit_message_text(text, self.current_user, self.secondary_message, reply_markup=markup)
                return
            self.error_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except:
            self.delete_error_message()
            self.send_error_message(text, markup)

    def delete_active_message(self):
        if self.active_message:
            self.bot.delete_message(self.current_user, self.active_message)
            self.active_message = None

    def delete_secondary_message(self):
        if self.secondary_message:
            self.bot.delete_message(self.current_user, self.secondary_message)
            self.secondary_message = None

    def delete_error_message(self):
        if self.error_message:
            self.bot.delete_message(self.current_user, self.error_message)
            self.error_message = None

    def delete_additional_message(self):
        if self.additional_message:
            self.bot.delete_message(self.current_user, self.additional_message)
            self.additional_message = None

    def cleanup(self):
        self.delete_active_message()
        self.delete_secondary_message()
        self.delete_error_message()
        self.delete_additional_message()

    def destruct(self):
        self.cleanup()
        if self.return_method:
            self.return_method()
        else:
            go_back_to_main_menu(self.bot, self.current_user, self.message)
        del self

    def callback_handler(self, call):
        # Exit
        if call.data == '0':
            self.destruct()
            self.bot.callback_query_handlers.remove(self.current_callback_handler)
        # My ads
        elif call.data == '1':
            self.send_error_message('This feature isn`t ready')
        # Overall statistics
        elif call.data == '2':
            self.send_error_message('This feature isn`t ready')
