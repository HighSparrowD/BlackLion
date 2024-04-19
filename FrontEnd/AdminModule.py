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


        self.current_callback_handler = self.bot.register_callback_query_handler(message, self.callback_handler,
                                                                                 user_id=self.current_user)
        self.recent_updates = self.api_service.get_recent_updates()
        self.recent_reports_list = None

        self.reports_calldata: bool = False

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
                                                  [InlineKeyboardButton('Exit to main menu', callback_data='a')]])

        self.start()

    def recent_reports_markup(self):
        markup = InlineKeyboardMarkup([[InlineKeyboardButton(f'Report #{button_data.id}', callback_data=f'{button_data.id}')]
                                       for button_data in self.api_service.get_recent_reports()])
        markup.add(InlineKeyboardButton('Go back', callback_data='a'))
        return markup

    def start(self):
        self.send_active_message('Check the recent updates!', self.start_markup)
        self.reports_calldata = False

    def recent_reports(self):
        self.prev_func = self.start
        self.reports_calldata = True

        self.send_active_message('Recent reports:', self.recent_reports_markup())

    def callback_handler(self, call: CallbackQuery):
        if call.data == 'a':
            self.prev_menu()
        elif self.reports_calldata:
            pass
        elif call.data == '2':
            self.recent_reports()
        else:
            self.send_error_message("This feature isn't ready")
