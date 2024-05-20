import telebot
from telebot.types import InlineKeyboardMarkup, InlineKeyboardButton, Message, CallbackQuery, ReplyKeyboardMarkup, KeyboardButton
from Common.Menues import go_back_to_main_menu
from Core.Api.Admin.AdminApi import AdminApi
from Models.Admin.Admin import ResolveVerificationRequest, ResolveAdvertisement

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
        self.next_handler = None

        self.recent_updates = self.api_service.get_recent_updates()
        self.models_list = None
        self.model = None
        self.verif_requests_list = None

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

        self.approve_decline_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).\
            add(KeyboardButton("Approve"), KeyboardButton('Decline'), KeyboardButton('Go back'))

        self.goback_markup = InlineKeyboardMarkup([[InlineKeyboardButton('Go back', callback_data='a')]])

        self.start()

    def recent_reports_markup(self):
        markup = InlineKeyboardMarkup([[InlineKeyboardButton(f'Report #{button_data.id}', callback_data=f'{button_data.id}')]
                                       for button_data in self.models_list])
        markup.add(InlineKeyboardButton('Go back', callback_data='a'))
        return markup

    def feedbacks_markup(self):
        markup = InlineKeyboardMarkup([[InlineKeyboardButton(f'Feedback #{button_data.id}', callback_data=f'{button_data.id}')]
                                       for button_data in self.models_list])
        markup.add(InlineKeyboardButton('Go back', callback_data='a'))
        return markup

    def all_feedbacks_markup(self):
        markup = InlineKeyboardMarkup([[InlineKeyboardButton(f'{button_data.username}', callback_data=f'{self.models_list.index(button_data)}')]
                                       for button_data in self.models_list])
        markup.add(InlineKeyboardButton('Go back', callback_data='a'))
        return markup

    def verification_request_markup(self):
        markup = InlineKeyboardMarkup([[InlineKeyboardButton(f'Request #{button_data.id}', callback_data=f'{button_data.id}')]
                                       for button_data in self.verif_requests_list])
        markup.add(InlineKeyboardButton('Go back', callback_data='a'))
        return markup

    def start(self):
        self.prev_func = None
        self.send_active_message('Check the recent updates!', self.start_markup)
        self.calldata_mode = 0

    def feedbacks_menu(self):
        self.prev_func = self.start

        self.send_active_message('Recent feedbacks or all?', self.feedbacks_menu_markup)

    def recent_feedbacks(self):
        self.prev_func = self.feedbacks_menu

        self.calldata_mode = 1
        self.models_list = self.api_service.get_recent_feedbacks()

        self.send_active_message('Recent feedbacks:', self.feedbacks_markup())

    def all_feedbacks(self):
        self.prev_func = self.feedbacks_menu

        self.calldata_mode = 2
        self.models_list = self.api_service.get_all_feedbacks()

        self.send_active_message('From which user?', self.all_feedbacks_markup())

    def users_feedbacks(self, models_list_index):
        self.prev_func = self.all_feedbacks

        self.calldata_mode = 20
        self.models_list = self.models_list[models_list_index].feedbacks

        self.send_active_message('Which one?', self.feedbacks_markup())

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

    def verification_requests_menu(self):
        self.prev_func = self.start

        self.calldata_mode = 4
        if not self.verif_requests_list:
            self.verif_requests_list = self.api_service.get_verification_requests()

        self.send_active_message('Recent requests:', self.verification_request_markup())

    def show_verif_request(self, request_id):
        for request in self.verif_requests_list:
            if request.id == request_id:
                if request.confirmationType == 'Full':

                    user_media_list = self.api_service.get_user_media(request.userId)
                    if len(user_media_list) > 1:
                        self.send_mediagroup_as_secondary_msg([media.media] for media in user_media_list)
                    else:
                        if user_media_list[0].mediaType == 'Photo':
                            self.send_secondary_message_with_photo(user_media_list[0].media)
                        else:
                            self.send_secondary_message_with_video(user_media_list[0].media)

                elif request.confirmationType == 'Partial':
                    self.send_secondary_message(f'{request.gesture}')

                if request.mediaType == 'Photo':
                    self.send_active_message_with_photo(f'Request #{request.id}\n\n'
                                                        f'User id: {request.userId}\n'
                                                        f'State: {request.state}\n', request.media,
                                                        markup=self.approve_decline_markup)
                elif request.mediaType == 'Video':
                    self.send_active_message_with_video(f'Request #{request.id}\n\n'
                                                        f'User id: {request.userId}\n'
                                                        f'State: {request.state}\n', request.media,
                                                        markup=self.approve_decline_markup)
                elif request.mediaType == 'VideoNote':
                    self.send_video_note_as_additional_msg(request.media)
                    self.send_active_message(f'Request #{request.id}\n\n'
                                             f'User id: {request.userId}\n'
                                             f'State: {request.state}\n',
                                             markup=self.approve_decline_markup)
                self.next_handler = self.bot.register_next_step_handler(None, self.resolve_request,
                                                                        chat_id=self.current_user, request=request)
                return

    def resolve_request(self, message: Message = None, request=None, isDeclined=False):
        self.delete_message(message.id)

        if not isDeclined:
            if message.text == 'Approve':
                self.api_service.post_verification_request(ResolveVerificationRequest(request.id, request.adminId, 'Approved'))
                self.verif_requests_list.remove(request)

                self.delete_secondary_message()
                self.delete_additional_message()

                self.verification_requests_menu()
            elif message.text == 'Decline':
                self.send_active_message(f'Tell the user why their request had been declined\n\n User`s language: '
                                         f'{self.api_service.get_user_language(request.userId)}', delete_msg=['s', 'a'])
                self.next_handler = self.bot.register_next_step_handler(message, self.resolve_request,
                                                                        chat_id=self.current_user, request=request,
                                                                        isDeclined=True)
            elif message.text == 'Go back':
                for request_item in self.verif_requests_list:
                    self.api_service.post_verification_request(ResolveVerificationRequest
                                                               (request_item.id, request_item.adminId, 'Aborted'))

                self.delete_secondary_message()
                self.delete_additional_message()

                self.start()
            else:
                self.send_error_message('Something went wrong...')
        elif isDeclined:
            self.api_service.post_verification_request(
                ResolveVerificationRequest(request.id, request.adminId, 'Declined', message.text))
            self.verif_requests_list.remove(request)

            self.delete_secondary_message()
            self.delete_additional_message()

            self.verification_requests_menu()

    def show_pending_ad(self):
        self.prev_func = self.start

        self.model = self.api_service.get_pending_advertisements()

        if self.model.mediaType == 'Photo':
            self.send_active_message_with_photo(f'Advertisement id: {self.model.id}\n\n'
                                                f'User id: <code>{self.model.sponsorId}</code>\n'
                                                f'Description: {self.model.text}\n'
                                                f'Tags: {self.model.tags}', self.model.media, self.approve_decline_markup)
        elif self.model.mediaType == 'Video':
            self.send_active_message_with_video(f'Advertisement id: {self.model.id}\n\n'
                                                f'User id: <code>{self.model.sponsorId}</code>\n'
                                                f'Description: {self.model.text}\n'
                                                f'Tags: {self.model.tags}', self.model.media, self.approve_decline_markup)
        elif self.model.mediaType == 'VideoNote':
            self.send_video_note_as_additional_msg(self.model.media)
            self.send_active_message(f'Advertisement id: {self.model.id}\n\n'
                                     f'User id: <code>{self.model.sponsorId}</code>\n'
                                     f'Description: {self.model.text}\n'
                                     f'Tags: {self.model.tags}', self.approve_decline_markup)
        else:
            self.send_error_message('Something went wrong...')

        self.next_handler = self.bot.register_next_step_handler(None, self.resolve_pending_ad,
                                                                chat_id=self.current_user)

    def resolve_pending_ad(self, message: Message = None):
        self.delete_message(message.id)

        if message.text == 'Approve':
            tags = 'No tags present yet'
            if len(self.model.tags) > 0:
                tags = self.model.tags
            self.send_active_message(f'Ad description:\n{self.model.text}\n\n'
                                     f'Target audience:\n{self.model.targetAudience}\n\n'
                                     f'Tags:\n{tags}', delete_msg=['a'])

            self.send_secondary_message('Please, correct tags according to the description\n\n'
                                        'Validation Rules: Tags have to be written in a single message. '
                                        'Separator - ",".\n\nNo special characters besides "," are allowed!')
            self.next_handler = self.bot.register_next_step_handler(message, self.accept_ad,
                                                                    chat_id=self.current_user)
        elif message.text == 'Decline':
            self.send_active_message(f'Tell the user why their ad had been declined\n\n User`s language: '
                                     f'{self.api_service.get_user_language(self.model.sponsorId)}', delete_msg=['s', 'a'])
            self.next_handler = self.bot.register_next_step_handler(message, self.decline_ad,
                                                                    chat_id=self.current_user)
        elif message.text == 'Go back':
            self.api_service.post_advertisement(ResolveAdvertisement
                                                (self.model.id, 'ToView', self.current_user))

            self.start()
        else:
            self.send_error_message('Something went wrong...')

    def accept_ad(self, message: Message = None):
        self.api_service.post_advertisement(ResolveAdvertisement
                                            (self.model.id, 'Approved', self.current_user, tags=message.text))
        self.delete_message(message.id)

        self.delete_active_message()
        self.delete_secondary_message()

        self.show_pending_ad()

    def decline_ad(self, message: Message = None):
        self.api_service.post_advertisement(ResolveAdvertisement
                                            (self.model.id, 'ToView', self.current_user, message.text))
        self.delete_message(message.id)

        self.delete_active_message()

        self.show_pending_ad()

    def callback_handler(self, call: CallbackQuery):
        if call.data == 'a':
            self.prev_menu()
        elif self.calldata_mode == 1:
            self.show_feedback(int(call.data))
        elif self.calldata_mode == 2:
            self.users_feedbacks(int(call.data))
        elif self.calldata_mode == 20:
            self.show_feedback(int(call.data))
        elif self.calldata_mode == 3:
            self.show_report(int(call.data))
        elif self.calldata_mode == 4:
            self.show_verif_request(int(call.data))
        elif call.data == '1':
            self.feedbacks_menu()
        elif call.data == '10':
            self.recent_feedbacks()
        elif call.data == '11':
            self.all_feedbacks()
        elif call.data == '2':
            self.recent_reports_menu()
        elif call.data == '3':
            self.verification_requests_menu()
        elif call.data == '4':
            self.show_pending_ad()
        else:
            self.send_error_message("This feature isn't ready")
