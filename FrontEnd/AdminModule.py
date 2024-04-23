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
        self.models_list = None

        self.calldata_mode: int = 0

        self.start_markup = InlineKeyboardMarkup([[InlineKeyboardButton('Feedbacks', callback_data='1'),
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

        self.feedbacks_menu_markup = InlineKeyboardMarkup([[InlineKeyboardButton('Recent', callback_data='10')],
                                                           [InlineKeyboardButton('All', callback_data='11')],
                                                           [InlineKeyboardButton('Go back', callback_data='a')]])

        self.goback_markup = InlineKeyboardMarkup([[InlineKeyboardButton('Go back', callback_data='a')]])

        self.start()

    def recent_reports_markup(self):
        markup = InlineKeyboardMarkup([[InlineKeyboardButton(f'Report #{button_data.id}', callback_data=f'{button_data.id}')]
                                       for button_data in self.models_list])
        markup.add(InlineKeyboardButton('Go back', callback_data='a'))
        return markup

    def recent_feedbacks_markup(self):
        markup = InlineKeyboardMarkup([[InlineKeyboardButton(f'Feedback #{button_data.id}', callback_data=f'{button_data.id}')]
                                       for button_data in self.models_list])
        markup.add(InlineKeyboardButton('Go back', callback_data='a'))
        return markup

    def all_feedbacks_markup(self):
        markup = InlineKeyboardMarkup([[InlineKeyboardButton(f'{button_data.username}', callback_data=f'{self.models_list.index(button_data)}')]
                                       for button_data in self.models_list])
        markup.add(InlineKeyboardButton('Go back', callback_data='a'))
        print(self.models_list)
        return markup

    def users_feedbacks_markup(self, models_list_index):
        markup = InlineKeyboardMarkup([[InlineKeyboardButton(f'Feedback #{button_data.id}', callback_data=f'{button_data.id}')]
                                       for button_data in self.models_list[models_list_index].feedbacks])
        markup.add(InlineKeyboardButton('Go back', callback_data='a'))
        return markup

    def start(self):
        self.send_active_message('Check the recent updates!', self.start_markup)
        self.calldata_mode = 0

    def feedbacks_menu(self):
        self.prev_func = self.start

        self.send_active_message('Recent feedbacks or all?', self.feedbacks_menu_markup)

    def recent_feedbacks(self):
        self.prev_func = self.feedbacks_menu

        self.calldata_mode = 1
        self.models_list = self.api_service.get_recent_feedbacks()

        self.send_active_message('Recent feedbacks:', self.recent_feedbacks_markup())

    def all_feedbacks(self):
        self.prev_func = self.feedbacks_menu

        self.calldata_mode = 2
        self.models_list = self.api_service.get_all_feedbacks()

        self.send_active_message('From which user?', self.all_feedbacks_markup())

    def users_feedbacks(self, models_list_index):
        self.prev_func = self.all_feedbacks

        self.calldata_mode = 20

        self.send_active_message('Which one?', self.users_feedbacks_markup(models_list_index))

    def show_feedback(self, feedback_id):
        for feedback_item in self.models_list:
            if feedback_item.id == feedback_id:
                self.send_active_message(f'Feedback #{feedback_item.id}\n\nFrom user: <code>{feedback_item.userId}</code>\n'
                                         f'Reason: {feedback_item.reason}\n\n'
                                         f'"{feedback_item.text}"', self.goback_markup)
                return

    def recent_reports_menu(self):
        self.prev_func = self.start

        self.calldata_mode = 3
        self.models_list = self.api_service.get_recent_reports()

        self.send_active_message('Recent reports:', self.recent_reports_markup())

    def show_report(self, report_id):
        self.prev_func = self.recent_reports_menu

        for report_item in self.models_list:
            if report_item.id == report_id:
                self.send_active_message(f'Report #{report_item.id}\n\nSender id: <code>{report_item.senderId}</code>\n'
                                         f'Reported id: <code>{report_item.userId}</code>\nReason: {report_item.reason}\n\n'
                                         f'"{report_item.text}"', self.goback_markup)
                return

    def callback_handler(self, call: CallbackQuery):
        if call.data == 'a':
            self.prev_menu()
        elif call.data == '1':
            self.feedbacks_menu()
        elif call.data == '10':
            self.recent_feedbacks()
        elif call.data == '11':
            self.all_feedbacks()
        elif self.calldata_mode == 1:
            self.show_feedback(int(call.data))
        elif self.calldata_mode == 2:
            self.users_feedbacks(int(call.data))
        elif self.calldata_mode == 20:
            self.show_feedback(int(call.data))
        elif self.calldata_mode == 3:
            self.show_report(int(call.data))
        elif call.data == '2':
            self.recent_reports_menu()
        else:
            self.send_error_message("This feature isn't ready")
