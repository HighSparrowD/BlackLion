import telebot
from telebot.types import InlineKeyboardMarkup, InlineKeyboardButton, Message, CallbackQuery
from Common.Menues import go_back_to_main_menu
from Core.Api.Admin.AdminApi import AdminApi

from BaseModule import Personality_Bot


class AdminModule(Personality_Bot):
    def __init__(self, bot: telebot.TeleBot, message: Message, hasVisited=False):
        super().__init__(bot, message, hasVisited)

        self.api_service = AdminApi(self.current_user)

        if not self.api_service.authenticate_admin():
            self.send_error_message('You are not authorized to use advertisement module :(')
            self.destruct()
            return

        self.recent_updates = self.api_service.get_recent_updates()

        self.current_callback_handler = self.bot.register_callback_query_handler(message, self.callback_handler,
                                                                                 user_id=self.current_user)

        self.start_markup = InlineKeyboardMarkup([[InlineKeyboardButton('Recent feedbacks', callback_data='1'),
                                                   InlineKeyboardButton(f'{self.recent_updates.recentFeedbackCount}', callback_data='1')],
                                                  [InlineKeyboardButton('Recent reports', callback_data='2'),
                                                   InlineKeyboardButton(f'{self.recent_updates.recentReportCount}', callback_data='2')],
                                                  [InlineKeyboardButton('Verification requests', callback_data='3'),
                                                   InlineKeyboardButton(f'{self.recent_updates.verificationRequestCount}', callback_data='3')],
                                                  [InlineKeyboardButton('Pending advertisements', callback_data='4'),
                                                   InlineKeyboardButton(f'{self.recent_updates.pendingAdvertisementCount}', callback_data='4')],
                                                  [InlineKeyboardButton('Pending Adventures', callback_data='5'),
                                                   InlineKeyboardButton(f'{self.recent_updates.pendingAdventureCount}', callback_data='5')],
                                                  [InlineKeyboardButton('Exit to main menu', callback_data='0')]])

        self.start()

    def start(self):
        self.send_active_message('Check the recent updates!', self.start_markup)

    def callback_handler(self, call: CallbackQuery):
        if call.data == '0':
            self.prev_menu()