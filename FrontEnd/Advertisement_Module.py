import telebot
from telebot.types import InlineKeyboardMarkup, InlineKeyboardButton, Message, CallbackQuery
from Common.GraphMaker import graph_one_x

from BaseModule import Personality_Bot
from Core.Api.Sponsor.SponsorApi import SponsorApi
from Models.Advertisement.Advertisement import AdvertisementNew, AdvertisementUpdate

class AdvertisementModule(Personality_Bot):
    def __init__(self, bot: telebot.TeleBot, message: Message, hasVisited=False):
        super().__init__(bot, message, hasVisited)

        self.sponsor_api = SponsorApi(self.current_user)

        #TODO: Delete module and navigate user to the main menu if False
        is_authenticated = self.sponsor_api.authenticate_sponsor()

        self.current_callback_handler = self.bot.register_callback_query_handler(message, self.callback_handler, user_id=self.current_user)

        self.ads_calldata: bool = False
        self.priority_calldata: bool = False
        self.shouldUpdate: bool = False

        self.turnedOnSticker = "✅"
        self.turnedOffSticker = "❌"

        # storage for ad model used in ad reg and ad settings
        self.ad_model: AdvertisementNew | AdvertisementUpdate | None = None
        self.existing_ads = None

        self.next_handler = None
        self.current_section = None
        self.editMode = False
        self.ad_reg_steps = []

        # advertisement settings buttons
        self.show_btn_indicator = InlineKeyboardButton(text=self.turnedOffSticker, callback_data='4')
        self.priority_btn_indicator = InlineKeyboardButton(text='', callback_data='6')

        self.main_menu_markup = InlineKeyboardMarkup().add(InlineKeyboardButton('My ads', callback_data='1'))\
            .add(InlineKeyboardButton('Economy statistics', callback_data='2'))\
            .add(InlineKeyboardButton('Engagement statistics', callback_data='8'))\
            .add(InlineKeyboardButton('Exit', callback_data='0'))

        self.my_ads_markup = InlineKeyboardMarkup()

        self.goback_markup = InlineKeyboardMarkup([[InlineKeyboardButton("Go Back", callback_data='0')]])

        self.priorities_list = self.sponsor_api.get_all_advertisement_priorities()

        for priority in self.priorities_list:  # temporary solution
            priority.name = priority.name.replace(' ', '')

        self.priority_markup = InlineKeyboardMarkup([[InlineKeyboardButton(self.priorities_list[0].name, callback_data=self.priorities_list[0].id)],
                                                     [InlineKeyboardButton(self.priorities_list[1].name, callback_data=self.priorities_list[1].id)],
                                                     [InlineKeyboardButton(self.priorities_list[2].name, callback_data=self.priorities_list[2].id)],
                                                     [InlineKeyboardButton(self.priorities_list[3].name, callback_data=self.priorities_list[3].id)],
                                                     [InlineKeyboardButton(self.priorities_list[4].name, callback_data=self.priorities_list[4].id)],
                                                     [InlineKeyboardButton("Go Back", callback_data='0')]])

        self.checkout_markup = InlineKeyboardMarkup([[InlineKeyboardButton('Name', callback_data='105')],
                                                     [InlineKeyboardButton('Text', callback_data='106')],
                                                     [InlineKeyboardButton('Media', callback_data='107')],
                                                     [InlineKeyboardButton('Target Audience', callback_data='108')],
                                                     [InlineKeyboardButton('Priority rate', callback_data='109')],
                                                     [InlineKeyboardButton('Cancel', callback_data='10'),
                                                      InlineKeyboardButton('Done', callback_data='100a')]])

        self.ad_settings_keyboard = InlineKeyboardMarkup([[InlineKeyboardButton(text='Statistics', callback_data='3')],
                                                          [InlineKeyboardButton(text='Show', callback_data='4'),
                                                           self.show_btn_indicator],
                                                          [InlineKeyboardButton(text='Edit', callback_data='5')],
                                                          [InlineKeyboardButton(text='Priority', callback_data='6'),
                                                           self.priority_btn_indicator],
                                                          [InlineKeyboardButton(text='Delete', callback_data='7')],
                                                          [InlineKeyboardButton(text='Go back', callback_data='0')]])

        self.ad_statistics_markup = InlineKeyboardMarkup([[InlineKeyboardButton(text='Monthly economy statistics', callback_data='30')],
                                                          [InlineKeyboardButton(text='Monthly engagement statistics', callback_data='31')],
                                                          [InlineKeyboardButton(text='Go back', callback_data='0')]])

        self.delete_markup = InlineKeyboardMarkup([[InlineKeyboardButton(text='Yes', callback_data='70')],
                                                   [InlineKeyboardButton(text='No', callback_data='0')]])

        self.to_show_ads_markup = InlineKeyboardMarkup([[InlineKeyboardButton('Return to my Ads', callback_data='10')]])

        self.start()

    def start(self):
        self.send_active_message('What you want to see?', markup=self.main_menu_markup)
        self.return_method = None
        self.ads_calldata = False

    def show_my_ads(self, shouldInsert=None):
        self.ads_calldata = True

        self.my_ads_markup.clear()
        self.existing_ads = self.sponsor_api.get_advertisement_list(self.current_user)

        # there is a hierarchy: call.data from previous btn is 1 so hear all call.data will start with 1
        self.my_ads_markup.add(InlineKeyboardButton("Add advertisement", callback_data="a"))
        for ad in self.existing_ads:
            self.my_ads_markup.add(InlineKeyboardButton(f"{ad.name}", callback_data=str(ad.id)))
        self.my_ads_markup.add(InlineKeyboardButton("Go back", callback_data="0"))

        self.send_active_message("Your advertisements:", self.my_ads_markup, ['e'])

        self.return_method = self.start

    def ad_settings(self, ad_id: int = None):
        if ad_id:
            self.ad_model = self.sponsor_api.get_advertisement_info(ad_id)
        self.show_btn_indicator.text = self.turnedOnSticker if self.ad_model.show else self.turnedOffSticker
        self.priority_btn_indicator.text = self.ad_model.priority
        self.send_active_message('Advertisement settings', markup=self.ad_settings_keyboard)

        self.ads_calldata = False
        self.return_method = self.show_my_ads

    def ad_delete(self, acceptMode=False):
        self.return_method = self.ad_settings
        if not acceptMode:
            self.send_active_message('Are you sure, you want to delete this advertisement?', self.delete_markup, ['e'])
        else:
            # Only if user want to delete the ad
            if self.sponsor_api.delete_advertisement(self.ad_model.id).status_code == 204:
                self.show_my_ads()
            else:
                self.prev_menu()
                self.send_error_message('Something went wrong\n\nCan not delete the ad')

    def economy_statistics(self):
        self.return_method = self.ad_settings

        statistics_list = self.sponsor_api.get_advertisement_economy_monthly_statistics(self.current_user, self.ad_model.id)

        params = [[], [], [], [], []]  # each of lists represents one of model params
        for model in statistics_list:
            params[0].append(model.payback)
            params[1].append(model.pricePerClick)
            params[2].append(model.totalPrice)
            params[3].append(model.income)
            params[4].append(model.created)

        graph = graph_one_x((params[0], 'Payback', 'r'),
                            (params[1], 'Price per click', 'g'),
                            (params[2], 'Total price', 'y'),
                            (params[3], 'Income', 'c'), x=params[4], xlabel='Days')

        self.send_active_message_with_photo('Economy statistics for your ad', graph, markup=self.goback_markup)

    def engagement_statistics(self):
        self.return_method = self.ad_settings

        statistics_list = self.sponsor_api.get_advertisement_engagement_monthly_statistics(self.current_user, self.ad_model.id)

        params = [[], [], [], [], []]  # each of lists represents one of model params
        for model in statistics_list:
            params[0].append(model.viewCount)
            params[1].append(model.averageStayInSeconds)
            params[2].append(model.clickCount)
            params[3].append(model.peoplePercentage)
            params[4].append(model.created)

        graph = graph_one_x((params[0], 'Views', 'r'),
                            (params[1], 'Average stay in sec', 'g'),
                            (params[2], 'Clicks', 'y'),
                            (params[3], 'People percentage', 'c'), x=params[4], xlabel='Days')

        self.send_active_message_with_photo('Engagement statistics for your ad', graph, markup=self.goback_markup)

    def overall_economy_statistics(self):
        self.return_method = self.start

        statistics_list = self.sponsor_api.get_overall_economy_monthly_statistics(self.current_user)

        params = [[], [], [], [], []]  # each of lists represents one of model params
        for model in statistics_list:
            params[0].append(model.payback)
            params[1].append(model.pricePerClick)
            params[2].append(model.totalPrice)
            params[3].append(model.income)
            params[4].append(model.created)

        graph = graph_one_x((params[0], 'Payback', 'r'),
                            (params[1], 'Price per click', 'g'),
                            (params[2], 'Total price', 'y'),
                            (params[3], 'Income', 'c'), x=params[4], xlabel='Days')

        self.send_active_message_with_photo('Your overall economy statistics', graph, markup=self.goback_markup)

    def overall_engagement_statistics(self):
        self.return_method = self.start

        statistics_list = self.sponsor_api.get_overall_engagement_monthly_statistics(self.current_user)

        params = [[], [], [], [], []]  # each of lists represents one of model params
        for model in statistics_list:
            params[0].append(model.viewCount)
            params[1].append(model.averageStayInSeconds)
            params[2].append(model.clickCount)
            params[3].append(model.peoplePercentage)
            params[4].append(model.created)

        graph = graph_one_x((params[0], 'Views', 'r'),
                            (params[1], 'Average stay in sec', 'g'),
                            (params[2], 'Clicks', 'y'),
                            (params[3], 'People percentage', 'c'), x=params[4], xlabel='Days')

        self.send_active_message_with_photo('Your overall engagement statistics', graph, markup=self.goback_markup)

    def name_step(self, message=None, acceptMode=False, shouldInsert=True):
        self.ads_calldata = False

        if not acceptMode:
            self.current_section = self.show_my_ads
            self.return_method = self.prev_reg_step

            self.configure_registration_step(self.name_step, shouldInsert)

            self.send_active_message('How want you to name your advertisement?', self.goback_markup, ['e'])
            self.next_handler = self.bot.register_next_step_handler(message, self.name_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message.id)

            if message.text:

                if len(message.text) > 20:
                    self.send_error_message("The name is too long")
                    self.next_handler = self.bot.register_next_step_handler(message, self.name_step, acceptMode=acceptMode, chat_id=self.current_user)
                    return

                self.ad_model.name = message.text

                if not self.editMode:
                    self.text_step(message)
                else:
                    self.checkout_step()

            else:
                self.send_error_message('Please enter the name')
                self.next_handler = self.bot.register_next_step_handler(message, self.name_step, acceptMode=acceptMode, chat_id=self.current_user)

    def text_step(self, message=None, acceptMode=False, shouldInsert=True):
        if not acceptMode:
            self.configure_registration_step(self.text_step, shouldInsert)

            self.send_active_message('Write a text for your advertisement', self.goback_markup, ['e'])
            self.next_handler = self.bot.register_next_step_handler(message, self.text_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message.id)

            if message.text:
                if len(message.text) > 1500:
                    self.send_error_message("The text is too long")
                    self.next_handler = self.bot.register_next_step_handler(message, self.text_step, acceptMode=acceptMode, chat_id=self.current_user)
                    return

                self.ad_model.text = message.text

                if not self.editMode:
                    self.media_step(message)
                else:
                    self.checkout_step()

            else:
                self.send_error_message('Please enter the text')
                self.next_handler = self.bot.register_next_step_handler(message, self.text_step, acceptMode=acceptMode, chat_id=self.current_user)

    def media_step(self, message=None, acceptMode=False, shouldInsert=True):
        if not acceptMode:
            self.configure_registration_step(self.media_step, shouldInsert)

            self.send_active_message('Send some media for your advertisement', self.goback_markup, ['e'])
            self.next_handler = self.bot.register_next_step_handler(message, self.media_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message.id)
            if message.photo:
                self.ad_model.media = message.photo[len(message.photo) - 1].file_id
                self.ad_model.mediaType = "Photo"

                if not self.editMode:
                    self.target_audience_step(message)
                else:
                    self.checkout_step()

            elif message.video:
                if message.video.duration > 15:
                    self.send_error_message('The video is too long')
                    self.next_handler = self.bot.register_next_step_handler(message, self.media_step,
                                                                            acceptMode=acceptMode,
                                                                            chat_id=self.current_user)
                    return

                self.ad_model.media = message.video.file_id
                self.ad_model.mediaType = "Video"

                if not self.editMode:
                    self.target_audience_step(message)
                else:
                    self.checkout_step()

            else:
                self.send_error_message('Please send the media')
                self.next_handler = self.bot.register_next_step_handler(message, self.media_step, acceptMode=acceptMode, chat_id=self.current_user)

    def target_audience_step(self, message=None, acceptMode=False, shouldInsert=True):
        if not acceptMode:
            self.configure_registration_step(self.name_step, shouldInsert)

            self.send_active_message('Describe target audience for this advertisement', self.goback_markup, ['e'])
            self.next_handler = self.bot.register_next_step_handler(message, self.target_audience_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message.id)

            if message.text:
                self.ad_model.targetAudience = message.text

                if len(message.text) > 150:
                    self.send_error_message("Your description is too long")
                    self.next_handler = self.bot.register_next_step_handler(message, self.target_audience_step, acceptMode=acceptMode, chat_id=self.current_user)
                    return

                if not self.editMode:
                    self.priority_step(message)
                else:
                    self.checkout_step()

            else:
                self.send_error_message('Please describe the audience so we know whom to show your ad')
                self.next_handler = self.bot.register_next_step_handler(message, self.target_audience_step, acceptMode=acceptMode, chat_id=self.current_user)

    def priority_step(self, message=None, acceptMode=False, shouldInsert=True, input=None):
        if not acceptMode:
            # i think we should to add some kind of link in this message so people can understand what priority is
            self.priority_calldata = True

            self.send_active_message('Choose a priority of this advertisement', self.priority_markup, ['e'])
        else:
            self.priority_calldata = False

            for priority_item in self.priorities_list:
                if priority_item.id == int(input):
                    self.ad_model.priority = priority_item.name
                    self.checkout_step()
                    return

            self.send_active_message('Something went wrong\n\nType anything to try again')
            self.next_handler = self.bot.register_next_step_handler(message, self.priority_step, chat_id=self.current_user)

    def checkout_step(self, acceptMode=False, shouldInsert=True):
        if not acceptMode:
            self.editMode = True
            if self.return_method != self.prev_reg_step:
                self.return_method = self.prev_reg_step

            self.configure_registration_step(self.checkout_step, shouldInsert)

            if self.ad_model.mediaType == "Photo":
                self.send_active_message_with_photo(f'{self.ad_model.name}\n{self.ad_model.text}\n'
                                                    f'{self.ad_model.targetAudience}\n'
                                                    f'Priority rate: {self.ad_model.priority}', self.ad_model.media)
            elif self.ad_model.mediaType == "Video":
                self.send_active_message_with_video(f'{self.ad_model.name}\n{self.ad_model.text}\n'
                                                    f'{self.ad_model.targetAudience}\n'
                                                    f'Priority rate: {self.ad_model.priority}', self.ad_model.media)
            else:
                self.send_active_message('Something went wrong')

            self.send_secondary_message('Want to change something?', self.checkout_markup)
        else:
            if hasattr(self.ad_model, 'id'):
                response = self.sponsor_api.update_advertisement(self.ad_model)
            else:
                response = self.sponsor_api.add_advertisement(self.ad_model)
            if response.status_code == 204:
                self.editMode = False
                self.ad_reg_steps = []
                self.delete_secondary_message()
                self.show_my_ads()
                return
            self.send_error_message("Something went wrong :-(", self.to_show_ads_markup)

    def configure_registration_step(self, step, shouldInsert):
        if shouldInsert:
            self.ad_reg_steps.insert(0, self.current_section)
        self.current_section = step

    def prev_reg_step(self):
        self.bot.remove_next_step_handler(self.current_user, self.next_handler)
        if self.editMode:
            self.checkout_step()
        else:
            previous_section = self.ad_reg_steps[0]
            self.ad_reg_steps.pop(0)
            previous_section(shouldInsert=False)

    def callback_handler(self, call: CallbackQuery):
        # Exit
        if call.data == '0':
            self.prev_menu()

        # Register new ad
        elif self.ads_calldata and call.data == 'a':
            self.ad_model = AdvertisementNew()
            self.ad_model.sponsorId = self.current_user

            self.name_step()

        # Ad settings
        elif self.ads_calldata and call.data != 'a':
            self.ad_settings(int(call.data))

        # Ad reg/update
        elif self.priority_calldata:
            self.priority_step(message=call.message, acceptMode=True, input=call.data)
        # Checkout call.data
        elif call.data == '105':
            self.name_step(shouldInsert=False)
            self.delete_secondary_message()
        elif call.data == '106':
            self.text_step(shouldInsert=False)
            self.delete_secondary_message()
        elif call.data == '107':
            self.media_step(shouldInsert=False)
            self.delete_secondary_message()
        elif call.data == '108':
            self.target_audience_step(shouldInsert=False)
            self.delete_secondary_message()
        elif call.data == '109':
            self.priority_step(shouldInsert=False)
            self.delete_secondary_message()
        elif call.data == '100a':
            self.checkout_step(acceptMode=True)

        elif call.data == '10' and self.ad_model.__class__ is AdvertisementUpdate:  # for easy return to settings menu
            self.delete_secondary_message()
            self.editMode = False
            self.ad_settings(self.ad_model.id)

        elif call.data == '10':
            self.delete_secondary_message()
            self.editMode = False
            self.show_my_ads()

        # My ads
        elif call.data == '1':
            self.show_my_ads()

        # Edit ad
        elif call.data == '5':
            self.checkout_step()
            self.ad_model = AdvertisementUpdate(self.ad_model.id, self.ad_model.sponsorId, self.ad_model.name,
                                                self.ad_model.text, self.ad_model.targetAudience,
                                                self.ad_model.media, self.ad_model.priority, self.ad_model.mediaType)

        # Ad statistics choice
        elif call.data == '3':
            self.return_method = self.show_my_ads

            self.send_active_message('What statistics of your ad you want to see?', markup=self.ad_statistics_markup)

        elif call.data == '30':
            self.economy_statistics()
        elif call.data == '31':
            self.engagement_statistics()

        # Ad deleting
        elif call.data == '7':
            self.ad_delete()
        elif call.data == '70':
            self.ad_delete(True)

        # Overall economics statistics
        elif call.data == '2':
            self.overall_economy_statistics()
        elif call.data == '8':
            self.overall_engagement_statistics()

        else:
            self.send_error_message('This feature isn`t ready')
