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

        self.calldata_mode: int = 0

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

        self.goback_markup = InlineKeyboardMarkup([[InlineKeyboardButton('Go back', callback_data='a')]])

        self.start()

    def recent_reports_markup(self):
        markup = InlineKeyboardMarkup([[InlineKeyboardButton(f'Report #{button_data.id}', callback_data=f'{button_data.id}')]
                                       for button_data in self.recent_reports_list])
        markup.add(InlineKeyboardButton('Go back', callback_data='a'))
        return markup

    def start(self):
        self.send_active_message('Check the recent updates!', self.start_markup)
        self.calldata_mode = 0

    def recent_reports_menu(self):
        self.prev_func = self.start

        self.calldata_mode = 2
        self.recent_reports_list = self.api_service.get_recent_reports()

        self.send_active_message('Recent reports:', self.recent_reports_markup())

    def show_report(self, report_id):
        self.prev_func = self.recent_reports_menu

        for report_item in self.recent_reports_list:
            if report_item.id == report_id:
                self.send_active_message(f'Report #{report_item.id}\n\nSender id: <code>{report_item.senderId}</code>\n'
                                         f'Reported id: <code>{report_item.userId}</code>\nReason: {report_item.reason}\n\n'
                                         f'"{report_item.text}"', self.goback_markup)
                return

    def callback_handler(self, call: CallbackQuery):
        if call.data == 'a':
            self.prev_menu()
        elif self.calldata_mode == 2:
            self.show_report(int(call.data))
        elif call.data == '2':
            self.recent_reports_menu()
        else:
            self.send_error_message("This feature isn't ready")
