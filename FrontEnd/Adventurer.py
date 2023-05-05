from telebot.types import InlineKeyboardMarkup, InlineKeyboardButton, ReplyKeyboardMarkup
from Core import HelpersMethodes as Helpers
from Common.Menues import count_pages, assemble_markup, reset_pages, add_tick_to_element, remove_tick_from_element, index_converter
import requests
import json

from Common.Menues import go_back_to_main_menu
from Settings import Settings


class Adventurer:
    def __init__(self, bot, message, hasVisited=False):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id
        self.user_info = Helpers.get_user_info(self.current_user)
        self.hasVisited = hasVisited
        self.hasPremium = self.user_info["hasPremium"]
        self.isIdentityConfirmed = self.user_info["identityType"] != 0
        self.user_localization = self.user_info["userDataInfo"]["languageId"]

        #Indicates whether if user is managing his own adventures (1), subscribed adventures (2), a template (3), create from template (4)
        self.manageMode = 1

        self.previous_item = ''

        self.current_markup_elements = []
        self.markup_last_element = 0
        self.markup_page = 1
        self.markup_pages_count = 0

        self.question_index = 0

        self.current_callback_handler = None
        self.active_message = None
        self.secondary_message = None

        self.previous_section = None
        self.current_registration_step = None

        #Used for registering adventures
        self.data = {}
        self.isOffline = False
        self.doesExist = False
        self.editMode = False
        self.isCreatedFromTemplate = False
        #User's location
        self.country = self.user_info["userDataInfo"]["location"]["countryId"]
        self.city = self.user_info["userDataInfo"]["location"]["cityId"]

        #Used for template management
        self.current_template = 0
        self.isTemplate = False # Defines checkout's functionality

        #Used for adventure management
        self.current_adventure = 0
        self.current_attendee = 0
        self.current_attendee_data = None
        self.isNewAttendee = True
        self.current_attendees_statuses = {}

        self.countries = {}
        self.cities = {}

        self.register_checkout_message = "1.Change name\n2.Change country\n3.Change city\n4.Change media\n5.Change description\n6.Change experience description\n7.Change attendees description \n8.Describe unwanted people\n9.Describe reward\n10.Change Date\n11.Change Time\n12.Change duration\n13.{commOption}\n14.Change Auto Reply\n15. ⭐Change Keywords (tags)⭐\n16. Use group management\n17.{action}\n18. Abort"
        self.register_template_checkout_message = "1.Change name\n2.Change country\n3.Change city\n4.Change media\n5.Change description\n6.Change experience description\n7.Change attendees description \n8.Describe unwanted people\n9.Describe reward\n10.Change Date\n11.Change Time\n12.Change duration\n13.{commOption}\n14.Change Auto Reply\n15.{action}\n16. Abort"

        self.start_markup = InlineKeyboardMarkup() \
            .add(InlineKeyboardButton("My adventures", callback_data="1")) \
            .add(InlineKeyboardButton("Adventures I have joined", callback_data="2")) \
            .add(InlineKeyboardButton("Find Adventures", callback_data="3")) \
            .add(InlineKeyboardButton("My Templates", callback_data="6")) \
            .add(InlineKeyboardButton("Go Back", callback_data="-20"))

        self.pre_register_markup = InlineKeyboardMarkup() \
            .add(InlineKeyboardButton("Create manually", callback_data="160")) \
            .add(InlineKeyboardButton("Create from template", callback_data="161")) \
            .add(InlineKeyboardButton("Go Back", callback_data="-20"))

        self.search_markup = InlineKeyboardMarkup() \
            .add(InlineKeyboardButton("➕ Search ➕", callback_data="4")) \
            .add(InlineKeyboardButton("Join using code", callback_data="5")) \
            .add(InlineKeyboardButton("Go Back", callback_data="-20"))

        self.adventure_state_markup = InlineKeyboardMarkup() \
            .add(InlineKeyboardButton("Online", callback_data="4"), InlineKeyboardButton("❓", callback_data="304")) \
            .add(InlineKeyboardButton("Offline", callback_data="5"), InlineKeyboardButton("❓", callback_data="305")) \
            .add(InlineKeyboardButton("Go Back", callback_data="-20"))

        self.manage_template_markup = InlineKeyboardMarkup() \
            .add(InlineKeyboardButton("Change", callback_data="7")) \
            .add(InlineKeyboardButton("Delete", callback_data="8")) \
            .add(InlineKeyboardButton("Go Back", callback_data="-20"))

        self.manage_adventure_markup = InlineKeyboardMarkup() \
            .add(InlineKeyboardButton("Status", callback_data="-1"), InlineKeyboardButton("", callback_data="1")) \
            .add(InlineKeyboardButton("Attendees", callback_data="2")) \
            .add(InlineKeyboardButton("Change", callback_data="3")) \
            .add(InlineKeyboardButton("Delete", callback_data="4")) \
            .add(InlineKeyboardButton("Go Back", callback_data="-20"))

        self.manage_adventure_attendee_markup = InlineKeyboardMarkup() \
            .add(InlineKeyboardButton("Status", callback_data="-5"), InlineKeyboardButton("✅", callback_data="-5")) \
            .add(InlineKeyboardButton("Contact", callback_data="6")) \
            .add(InlineKeyboardButton("Remove", callback_data="7")) \
            .add(InlineKeyboardButton("Go Back", callback_data="-20"))

        self.set_attendee_status_markup = InlineKeyboardMarkup() \
            .add(InlineKeyboardButton("Accept", callback_data="10")) \
            .add(InlineKeyboardButton("Decline", callback_data="11")) \
            .add(InlineKeyboardButton("Go Back", callback_data="-20"))

        self.okMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("Ok")
        self.YNMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("Yes", "No")
        self.goBackMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("Go Back")
        self.skipMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("Skip")
        self.verificationMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("Now", "Later")
        self.locationOfflineMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("Use location from my profile", "Select manually")
        self.locationOnlineMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("Use location from my profile").add("Select manually", "No matter")
        self.registerCheckoutMarkup_E = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18")

        self.registerTemplateCheckoutMarkup_E = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14").add("15").add("16")

        self.my_adventuresMarkup = InlineKeyboardMarkup()
        self.my_adventures_attendeesMarkup = InlineKeyboardMarkup()
        self.my_templatesMarkup = InlineKeyboardMarkup()
        self.subscribed_adventuresMarkup = InlineKeyboardMarkup()

        self.starting_message = "It's time for adventures!\n The Adventures feature allows users to create and join various adventures with other users.\n Creating adventures is simple: the user selects the desired activity, date and location, and then invites other users to join.\n When other users confirm their participation, they can start chatting and plan the details of the adventure in chat.\n You can offer anything, but only what is legal in your country"
        self.verification_message = "For comfortable and safe use of this module, we have added a system of organizer verification.\nIt is designed for users to be sure that you are who you say you are and won't use this module with malicious intent\nVerifying your identity will give you more credibility. Users will be more motivated by your adventure.\n\n Would you like to do verification now or later?"
        self.location_message = "Where will the adventure be shown?"

        self.statusDict = {
            1: "❓", # New
            2: "❓", # Changed
            3: "✅", # Accepted
            4: "❌", # Declined
        }

        self.attendee_statusDict = {
            1: "❓", # New
            2: "✅" # Accepted
        }

        self.active_location_markup = None

        self.creation_limit = 2
        self.subscription_limit = 1

        #TODO: Extend limitations for premium user BUT ON API SIDE pls
        if self.hasPremium:
            self.creation_limit = 50
            self.subscription_limit = 50

        if not self.hasVisited:
            self.bot.send_message(self.current_user, self.starting_message)

        self.subscribe_callback_handler(self.start_callback_handler)
        self.start()

    def start(self):
        self.previous_section = self.destruct
        self.send_active_message("<b><i>Please, select an option</i></b>", markup=self.start_markup)

    def register_pre_register(self):
        self.previous_section = self.my_adventures_manager
        self.send_active_message("Please, select an option:", markup=self.pre_register_markup)

    def choose_template(self):
        self.manageMode = 4
        self.assemble_my_templates_markup(False)

        self.send_active_message("Please, select a template", markup=self.my_templatesMarkup)

    def register_from_template(self):
        self.editMode = False
        self.isTemplate = False
        self.data.clear()

        self.subscribe_callback_handler(self.registration_callback_handler)
        self.data = Helpers.get_template(self.current_template)
        self.data["isAwaiting"] = False

        self.register_checkout(self.message)

    def register_start(self):
        # if self.createdCount + 1 > self.creation_limit:
        #     self.send_secondary_message(f"You reached the limit. You can create up to {self.creation_limit} adventures. Please, wait until one of your adventures ends, or delete it manually.")
        # else:
        self.data.clear()
        self.editMode = False
        self.isTemplate = False

        self.subscribe_callback_handler(self.registration_callback_handler)
        self.data["userId"] = self.current_user
        self.register_state_step()

    def register_template_start(self):
        self.previous_section = self.start

        self.subscribe_callback_handler(self.registration_callback_handler)
        self.data.clear()
        self.data["userId"] = self.current_user
        self.editMode = False
        self.isTemplate = True
        self.register_state_step(ignoreVerification=True)

    def register_state_step(self, ignoreVerification=False):
        self.current_registration_step = self.register_state_step

        if not self.isIdentityConfirmed and not ignoreVerification:
            self.register_verification_step(self.message)
            return

        self.send_active_message("What kind of adventure will it be?\nPlease note, that this parameter will be impossible to change after it had been chosen", markup=self.adventure_state_markup)

    def register_verification_step(self, message, acceptMode=False):
        if not acceptMode:
            self.send_active_message(self.verification_message, markup=self.verificationMarkup)
            self.bot.register_next_step_handler(message, self.register_verification_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message)

            if message.text == "Now":
                Settings(self.bot, message, verificationOnly=True, activeMessage=self.active_message, returnMethod=self.register_state_step)
            elif message.text == "Later":
                self.register_state_step(True)
            else:
                self.send_secondary_message("Empty message")
                self.bot.register_next_step_handler(message, self.register_verification_step, acceptMode=acceptMode, chat_id=self.current_user)

    def register_online(self):
        self.isOffline = False
        self.data["isOffline"] = False
        self.delete_secondary_message()

        # self.delete_active_message()
        # self.delete_secondary_message()
        self.register_name_step(self.message)
        # else:
        #     self.send_secondary_message("<b>Registering online adventure does NOT require any identity confirmation. Good example of online adventure is: -------</b>")

    def register_offline(self):
        self.isOffline = True
        self.data["isOffline"] = True
        self.delete_secondary_message()

        # self.delete_active_message()
        # self.delete_secondary_message()
        self.register_name_step(self.message)

        # else:
        #     self.send_secondary_message("<b>Registering offline adventure requires identity confirmation. It is described in user agreement (NUMBER OF PARAGRAPH) Good example of offline adventure is: -------</b>")

    def register_name_step(self, message, acceptMode=False):
        self.current_registration_step = self.register_name_step
        self.delete_secondary_message()

        if not acceptMode:
            if self.isTemplate:
                self.send_active_message("Please, name your template")
            else:
                self.send_active_message("Please, name your adventure")
            self.bot.register_next_step_handler(message, self.register_name_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message)
            if not message.text:
                self.send_secondary_message("Empty message")
                self.bot.register_next_step_handler(message, self.register_name_step, acceptMode=acceptMode, chat_id=self.current_user)
                return

            self.data["name"] = message.text

            if self.editMode:
                if self.isTemplate:
                    self.register_template_checkout(message)
                    return

                self.register_checkout(message)
                return

            self.register_location_step(message)

    def register_location_step(self, message, acceptMode=False):
        self.current_registration_step = self.register_location_step
        self.delete_secondary_message()

        if not acceptMode:
            if self.isOffline:
                self.active_location_markup = self.locationOfflineMarkup
            else:
                self.active_location_markup = self.locationOnlineMarkup

            self.send_active_message(self.location_message, self.active_location_markup)
            self.bot.register_next_step_handler(message, self.register_location_step, acceptMode=True,
                                                chat_id=self.current_user)
        else:
            self.delete_message(message)
            if message.text == "Use location from my profile":
                self.data["countryId"] = self.country
                self.data["cityId"] = self.city
                self.register_media_step(message)
            elif message.text == "Select manually":
                self.register_country_step(message)
            elif message.text == "No matter" and not self.isOffline:
                self.register_media_step(message)
            else:
                self.send_secondary_message("No such option", markup=self.active_location_markup)
                self.bot.register_next_step_handler(message, self.register_location_step, acceptMode=acceptMode,
                                                    chat_id=self.current_user)

    def register_country_step(self, message=None, acceptMode=False):
        self.question_index = 4

        self.current_registration_step = self.register_country_step
        self.delete_secondary_message()

        if not acceptMode:
            self.load_countries()

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page,
                        self.markup_pages_count)
            count_pages(self.countries, self.current_markup_elements, self.markup_pages_count)
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

            self.previous_item = ''

            self.send_active_message("Please, select country", markup=markup)

            if self.editMode:
                self.previous_item = str(self.data["countryId"])
                add_tick_to_element(self.bot, self.current_user, self.active_message, self.current_markup_elements, self.markup_page, str(self.data["countryId"]))

            self.send_secondary_message("Choose one from above. Or simply type country name to chat", markup=self.okMarkup)
            self.bot.register_next_step_handler(message, self.register_country_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message)

            if not message.text:
                self.send_secondary_message("Country was not recognized, try finding it in our list above",
                                            markup=self.okMarkup)
                self.bot.register_next_step_handler(message, self.register_country_step, acceptMode=acceptMode,
                                                    chat_id=self.current_user)
                return False

            msg_text = message.text.lower().strip()
            if msg_text != "ok":
                country = self.country_convertor(msg_text)
                if country:
                    if self.previous_item:
                        remove_tick_from_element(self.bot, self.current_user, self.active_message,
                                                 self.current_markup_elements,
                                                 self.markup_page, self.previous_item)
                    self.previous_item = str(country)
                    add_tick_to_element(self.bot, self.current_user, self.active_message,
                                        self.current_markup_elements,
                                        self.markup_page, str(country))

                    self.data["countryId"] = country
                    self.send_secondary_message("Gotcha", markup=self.okMarkup)
                    self.bot.register_next_step_handler(message, self.register_country_step, acceptMode=acceptMode,
                                                        chat_id=self.current_user)
                    return True
                else:
                    self.send_secondary_message("Country was not recognized, try finding it in our list above", markup=self.okMarkup)
                    self.bot.register_next_step_handler(message, self.register_country_step, acceptMode=acceptMode,
                                                        chat_id=self.current_user)
                    return False
            else:
                if self.data["countryId"]:
                    self.register_city_step(message)
                    return
                self.send_secondary_message("You haven't chosen a country !", markup=self.okMarkup)
                self.bot.register_next_step_handler(message, self.register_country_step, acceptMode=acceptMode, chat_id=self.current_user)

    def register_city_step(self, message=None, acceptMode=False):
        self.current_registration_step = self.register_city_step
        self.delete_secondary_message()

        if not acceptMode:
            self.question_index = 5
            self.markup_page = 1

            self.load_cities(self.country)

            self.previous_item = ''

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page, self.markup_pages_count)
            count_pages(self.cities, self.current_markup_elements, self.markup_pages_count)
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

            self.send_active_message("Please, select city", markup=markup)

            if self.editMode:
                self.previous_item = str(self.data["cityId"])
                add_tick_to_element(self.bot, self.current_user, self.active_message, self.current_markup_elements, self.markup_page, str(self.data["cityId"]))

            self.send_secondary_message("Chose one from above, or simply type to chat", markup=self.okMarkup)
            self.bot.register_next_step_handler(message, self.register_city_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message)

            if not message.text:
                self.send_secondary_message("City was not recognized, try finding it in our list above",
                                            markup=self.okMarkup)
                self.bot.register_next_step_handler(message, self.register_city_step, acceptMode=acceptMode,
                                                    chat_id=self.current_user)
                return False

            msg_text = message.text.lower().strip()
            if msg_text != "ok":
                city = self.city_convertor(msg_text)
                if city:
                    if self.previous_item:
                        remove_tick_from_element(self.bot, self.current_user, self.active_message,
                                                 self.current_markup_elements,
                                                 self.markup_page, self.previous_item)
                    self.previous_item = str(city)
                    add_tick_to_element(self.bot, self.current_user, self.active_message,
                                        self.current_markup_elements,
                                        self.markup_page, str(city))
                    self.data["cityId"] = city
                    self.send_secondary_message("Gotcha", markup=self.okMarkup)
                    self.bot.register_next_step_handler(message, self.register_city_step, acceptMode=acceptMode, chat_id=self.current_user)
                    return True
                else:
                    self.send_secondary_message("City was not recognized, try finding it in our list above", markup=self.okMarkup)
                    self.bot.register_next_step_handler(message, self.register_city_step, acceptMode=acceptMode, chat_id=self.current_user)
                    return False

            if self.data["cityId"] or self.data["cityId"] == 0:
                self.previous_item = ''

                if self.editMode:
                    if self.isTemplate:
                        self.register_template_checkout(message)
                        return

                    self.register_checkout(message)
                    return

                self.register_media_step(message)

            else:
                self.send_secondary_message("You haven't chosen a city !", markup=self.okMarkup)
                self.bot.register_next_step_handler(message, self.register_city_step, acceptMode=acceptMode, chat_id=self.current_user)

    def register_media_step(self, message, acceptMode=False):
        self.current_registration_step = self.register_media_step
        self.delete_secondary_message()

        if not acceptMode:
            self.send_active_message("Send a photo or video (1 minute max), that describes your adventure")
            self.bot.register_next_step_handler(message, self.register_media_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message)
            if message.photo:
                self.data["media"] = message.photo[len(message.photo) - 1].file_id
                self.data["isMediaPhoto"] = True
            elif message.video:
                if message.video.duration > 80:
                    self.send_secondary_message("Video is to long")
                    self.bot.register_next_step_handler(message, self.register_media_step, acceptMode=acceptMode, chat_id=self.current_user)
                    return

                self.data["media"] = message.video.file_id
                self.data["isMediaPhoto"] = False
            else:
                self.send_secondary_message("Unsupported media type")
                self.bot.register_next_step_handler(message, self.register_media_step, acceptMode=acceptMode, chat_id=self.current_user)
                return

            if self.editMode:
                if self.isTemplate:
                    self.register_template_checkout(message)
                    return

                self.register_checkout(message)
                return

            self.register_description_step(message)

    def register_description_step(self, message=None, acceptMode=False):
        self.current_registration_step = self.register_description_step
        self.delete_secondary_message()

        if not acceptMode:
            self.send_active_message("What do you propose to do? \nDescribe the activity, the place, what does other users have to have")
            self.bot.register_next_step_handler(message, self.register_description_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message)
            if not message.text:
                self.send_secondary_message("Empty message")
                self.bot.register_next_step_handler(message, self.register_description_step, acceptMode=acceptMode, chat_id=self.current_user)
                return

            self.data["description"] = message.text

            if self.editMode:
                if self.isTemplate:
                    self.register_template_checkout(message)
                    return

                self.register_checkout(message)
                return

            self.register_experience_step(message)

    def register_experience_step(self, message, acceptMode=False):
        self.current_registration_step = self.register_experience_step
        self.delete_secondary_message()

        if not acceptMode:
            self.send_active_message("How are you connected to what you are proposing to do, and are you connected at all?\nHow long have you been doing this activity?\nHave you ever done it before?\nIs this your first time doing it?", self.skipMarkup)
            self.bot.register_next_step_handler(message, self.register_experience_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message)
            if not message.text:
                self.send_secondary_message("Empty message", self.skipMarkup)
                self.bot.register_next_step_handler(message, self.register_experience_step, acceptMode=acceptMode, chat_id=self.current_user)
                return

            elif not message.text or message.text == "Skip":
                self.data["experience"] = ""
                self.register_attendees_step(message)
                return

            self.data["experience"] = message.text

            if self.editMode:
                if self.isTemplate:
                    self.register_template_checkout(message)
                    return

                self.register_checkout(message)
                return

            self.register_attendees_step(message)

    def register_attendees_step(self, message, acceptMode=False):
        self.current_registration_step = self.register_attendees_step
        self.delete_secondary_message()

        if not acceptMode:
            self.send_active_message("Who would you like to see?\nDescribe the person you would like to see. Gender, age, hobbies.\nWhy should this person come?\nHow many people would you like to gather?", self.skipMarkup)
            self.bot.register_next_step_handler(message, self.register_attendees_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message)
            if not message.text:
                self.send_secondary_message("Empty message", self.skipMarkup)
                self.bot.register_next_step_handler(message, self.register_attendees_step, acceptMode=acceptMode, chat_id=self.current_user)
                return

            elif not message.text or message.text == "Skip":
                self.data["attendeesDescription"] = ""
                self.register_unwanted_attendees_step(message)
                return

            self.data["attendeesDescription"] = message.text

            if self.editMode:
                if self.isTemplate:
                    self.register_template_checkout(message)
                    return

                self.register_checkout(message)
                return

            self.register_unwanted_attendees_step(message)

    def register_unwanted_attendees_step(self, message, acceptMode=False):
        self.current_registration_step = self.register_unwanted_attendees_step
        self.delete_secondary_message()

        if not acceptMode:
            self.send_active_message("Who would you NOT wish to see?\nDescribe the person you would NOT wish to see.", self.skipMarkup)
            self.bot.register_next_step_handler(message, self.register_unwanted_attendees_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message)
            if not message.text:
                self.send_secondary_message("Empty message", self.skipMarkup)
                self.bot.register_next_step_handler(message, self.register_unwanted_attendees_step, acceptMode=acceptMode, chat_id=self.current_user)
                return

            elif not message.text or message.text == "Skip":
                self.data["unwantedAttendeesDescription"] = ""
                self.register_gratitude_step(message)
                return

            self.data["unwantedAttendeesDescription"] = message.text

            if self.editMode:
                if self.isTemplate:
                    self.register_template_checkout(message)
                    return

                self.register_checkout(message)
                return

            self.register_gratitude_step(message)

    def register_gratitude_step(self, message, acceptMode=False):
        self.current_registration_step = self.register_gratitude_step
        self.delete_secondary_message()

        if not acceptMode:
            self.send_active_message("What kind of thanks would you like to receive from attendees for organizing the adventure?\n*It could be a social media review, a donation, or just a simple 'thank you'", markup=self.skipMarkup)
            self.bot.register_next_step_handler(message, self.register_gratitude_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message)
            if not message.text or message.text == "Skip":
                self.data["gratitude"] = ""
                self.register_date_step(message)
                return

            self.data["gratitude"] = message.text

            if self.editMode:
                if self.isTemplate:
                    self.register_template_checkout(message)
                    return

                self.register_checkout(message)
                return

            self.register_date_step(message)

    def register_date_step(self, message, acceptMode=False):
        self.current_registration_step = self.register_date_step
        self.delete_secondary_message()

        if not acceptMode:
            self.send_active_message("State the exact date of the adventure.\n\nExample: 10.08.2024\n\n*Do not include time, it is gonna be the next step :)")
            self.bot.register_next_step_handler(message, self.register_date_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message)
            if not message.text:
                self.send_secondary_message("Empty message")
                self.bot.register_next_step_handler(message, self.register_date_step, acceptMode=acceptMode, chat_id=self.current_user)
                return

            self.data["date"] = message.text

            if self.editMode:
                if self.isTemplate:
                    self.register_template_checkout(message)
                    return

                self.register_checkout(message)
                return

            self.register_time_step(message)

    def register_time_step(self, message, acceptMode=False):
        self.current_registration_step = self.register_time_step
        self.delete_secondary_message()

        if not acceptMode:
            self.send_active_message("State the time of the adventure.\n\nExample:\nWe gather at 11:00 a.m.\nWe start at 11:20")
            self.bot.register_next_step_handler(message, self.register_time_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message)
            if not message.text:
                self.send_secondary_message("Empty message")
                self.bot.register_next_step_handler(message, self.register_time_step, acceptMode=acceptMode, chat_id=self.current_user)
                return

            self.data["time"] = message.text

            if self.editMode:
                if self.isTemplate:
                    self.register_template_checkout(message)
                    return

                self.register_checkout(message)
                return

            self.register_duration_step(message)

    def register_duration_step(self, message, acceptMode=False):
        self.current_registration_step = self.register_duration_step
        self.delete_secondary_message()

        if not acceptMode:
            self.send_active_message("How long is your adventure going to last?\n\nExample:\n'The event will last 4 hours'\nOR\n'The adventure will last from 2:00 p.m. to 7:00 p.m.'")
            self.bot.register_next_step_handler(message, self.register_duration_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message)
            if not message.text:
                self.send_secondary_message("Empty message")
                self.bot.register_next_step_handler(message, self.register_duration_step, acceptMode=acceptMode, chat_id=self.current_user)
                return

            self.data["duration"] = message.text

            if self.editMode:
                if self.isTemplate:
                    self.register_template_checkout(message)
                    return

                self.register_checkout(message)
                return

            if self.isOffline:
                self.register_address_step(message)
            else:
                self.register_application_step(message)

    def register_application_step(self, message, acceptMode=False):
        self.current_registration_step = self.register_application_step
        self.delete_secondary_message()

        if not acceptMode:
            self.send_active_message("What app will you use to communicate?")
            self.bot.register_next_step_handler(message, self.register_application_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message)
            if not message.text:
                self.send_secondary_message("Empty message")
                self.bot.register_next_step_handler(message, self.register_application_step, acceptMode=acceptMode, chat_id=self.current_user)
                return

            self.data["address"] = ""
            self.data["application"] = message.text

            if self.editMode:
                if self.isTemplate:
                    self.register_template_checkout(message)
                    return

                self.register_checkout(message)
                return

            self.register_group_step(message)

    def register_address_step(self, message, acceptMode=False):
        self.current_registration_step = self.register_address_step
        self.delete_secondary_message()

        if not acceptMode:
            self.send_active_message("State the address where you will meet")
            self.bot.register_next_step_handler(message, self.register_address_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message)
            if not message.text:
                self.send_secondary_message("Empty message")
                self.bot.register_next_step_handler(message, self.register_address_step, acceptMode=acceptMode, chat_id=self.current_user)
                return

            self.data["application"] = ""
            self.data["address"] = message.text

            if self.editMode:
                if self.isTemplate:
                    self.register_template_checkout(message)
                    return

                self.register_checkout(message)
                return

            if self.isTemplate:
                self.register_auto_reply_step(message)
                return

            self.register_group_step(message)

    def register_group_step(self, message, acceptMode=False):
        if not acceptMode:
            #TODO: Think how else can the bot be helpful in group chats
            self.send_active_message("Would you like to use group management ?\n\n <b>This feature allows the me to manage your adventure's chat. When you accept new attendees, I will invite them to your group, if you decide to remove them from your adventure, I will remove them from the group</b>", markup=self.YNMarkup)
            self.bot.register_next_step_handler(message, self.register_group_step, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "Yes":
                self.data["isAwaiting"] = True

            if self.editMode:
                self.register_checkout(message)
                return

            self.register_auto_reply_step(message)

    def register_auto_reply_step(self, message, acceptMode=False):
        self.current_registration_step = self.register_auto_reply_step
        self.delete_secondary_message()

        if not acceptMode:
            self.send_active_message("You can leave an auto reply for those who will like your adventure\nJust send a voice/text message or hit skip in case you don't want to add auto reply", self.skipMarkup)
            self.bot.register_next_step_handler(message, self.register_auto_reply_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message)
            if message.text == "Skip":
                self.data["autoReply"] = None
                self.data["isAutoReplyText"] = None
                self.register_branch_move_next(message)
            elif message.text:
                #TODO: check length (discuss), add to data
                self.data["autoReply"] = message.text
                self.data["isAutoReplyText"] = True

                self.register_branch_move_next(message)
            elif message.voice:
                #TODO: check length (discuss)
                self.data["autoReply"] = message.voice.file_id
                self.data["isAutoReplyText"] = False

                self.register_branch_move_next(message)
            else:
                self.send_secondary_message("Empty message", self.skipMarkup)
                self.bot.register_next_step_handler(message, self.register_auto_reply_step, acceptMode=acceptMode, chat_id=self.current_user)

    #Used to define, which way should registration go after auto reply step
    def register_branch_move_next(self, message):
        if self.editMode:
            if self.isTemplate:
                self.register_template_checkout(message)
                return

            self.register_checkout(message)
            return

        if self.hasPremium and not self.isTemplate:
            self.registration_tags_step(message)
            return

        if self.isTemplate:
            self.register_template_checkout(message)
            return

        self.register_checkout(message)

    #TODO: Implement, Check formatting
    def registration_tags_step(self, message, acceptMode=False):
        self.current_registration_step = self.registration_tags_step
        self.delete_secondary_message()

        if not acceptMode:
            self.send_active_message("Send us keywords (tags), associated with your adventure. It will help us to display it to those, more interested in it", markup=self.skipMarkup)
            self.bot.register_next_step_handler(message, self.registration_tags_step, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "Skip":
                self.register_checkout(message)
                return

    def register_checkout(self, message, acceptMode=False):
        if not acceptMode:
            addition1 = "Change Address"
            addition2 = "✨Create Adventure✨"

            if not self.isOffline:
                addition1 = "Change Application"

            if self.doesExist:
                addition2 = "✨Save Changes✨"

            msg = self.register_checkout_message.format(commOption=addition1, action=addition2)

            if self.data["isMediaPhoto"]:
                self.send_active_message_with_photo(self.assemble_adventure_checkout_message(), self.data["media"], markup=self.registerCheckoutMarkup_E)
            else:
                self.send_active_message_with_video(self.assemble_adventure_checkout_message(), self.data["media"], markup=self.registerCheckoutMarkup_E)

            # self.send_active_message(self.assemble_adventure_checkout_message(), markup=self.registerCheckoutMarkup)
            self.send_secondary_message(msg, markup=self.registerCheckoutMarkup_E)

            self.bot.register_next_step_handler(message, self.register_checkout, acceptMode=True, chat_id=self.current_user)
        else:
            self.editMode = True

            if message.text == "1":
                self.register_name_step(message)
            elif message.text == "2":
                self.register_country_step(message)
            elif message.text == "3":
                self.register_city_step(message)
            elif message.text == "4":
                self.register_media_step(message)
            elif message.text == "5":
                self.register_description_step(message)
            elif message.text == "6":
                self.register_experience_step(message)
            elif message.text == "7":
                self.register_attendees_step(message)
            elif message.text == "8":
                self.register_unwanted_attendees_step(message)
            elif message.text == "9":
                self.register_gratitude_step(message)
            elif message.text == "10":
                self.register_date_step(message)
            elif message.text == "11":
                self.register_time_step(message)
            elif message.text == "12":
                self.register_duration_step(message)
            elif message.text == "13":
                if not self.isOffline:
                    self.register_application_step(message)
                    return
                self.register_address_step(message)
            elif message.text == "14":
                self.register_auto_reply_step(message)
            elif message.text == "15":
                if self.hasPremium:
                    self.registration_tags_step(message)
                else:

                    self.send_simple_message("This functionality is available for premium users only", markup=self.registerCheckoutMarkup_E)
                    self.bot.register_next_step_handler(message, self.register_checkout, acceptMode=True, chat_id=self.current_user)
            elif message.text == "16":
                self.register_group_step(message)
            elif message.text == "17":
                if self.doesExist:
                    Helpers.change_adventure(self.data)
                    self.send_secondary_message("Changes were saved successfully.\nThey will come into force after approval by the administration")
                    self.subscribe_callback_handler(self.start_callback_handler)
                    self.previous_section()
                    return

                code = Helpers.register_adventure(self.data)

                #TODO: QR CODE
                self.send_simple_message(f"Done :). Here is your invitation code: <b>{code}</b>. Other users will be able to find your adventure with it")

                if not self.isCreatedFromTemplate:
                    self.registration_template_step(message)
                else:
                    self.subscribe_callback_handler(self.start_callback_handler)
                    self.previous_section()
            elif message.text == "18":
                self.registration_discard_changes(message)
            else:
                self.delete_message(message)
                self.bot.register_next_step_handler(message, self.register_checkout, acceptMode=acceptMode, chat_id=self.current_user)

    def register_template_checkout(self, message, acceptMode=False):
        if not acceptMode:
            addition1 = "Change Address"
            addition2 = "✨Create Template✨"

            if not self.isOffline:
                addition1 = "Change Application"

            if self.doesExist:
                addition2 = "✨Save Changes✨"

            msg = self.register_template_checkout_message.format(commOption=addition1, action=addition2)

            if self.data["isMediaPhoto"]:
                self.send_active_message_with_photo(self.assemble_adventure_checkout_message(), self.data["media"], markup=self.registerTemplateCheckoutMarkup_E)
            else:
                self.send_active_message_with_video(self.assemble_adventure_checkout_message(), self.data["media"], markup=self.registerTemplateCheckoutMarkup_E)

            # self.send_active_message(self.assemble_adventure_checkout_message(), markup=self.registerCheckoutMarkup)
            self.send_secondary_message(msg, markup=self.registerTemplateCheckoutMarkup_E)

            self.bot.register_next_step_handler(message, self.register_template_checkout, acceptMode=True, chat_id=self.current_user)
        else:
            self.editMode = True

            if message.text == "1":
                self.register_name_step(message)
            elif message.text == "2":
                self.register_country_step(message)
            elif message.text == "3":
                self.register_city_step(message)
            elif message.text == "4":
                self.register_media_step(message)
            elif message.text == "5":
                self.register_description_step(message)
            elif message.text == "6":
                self.register_experience_step(message)
            elif message.text == "7":
                self.register_attendees_step(message)
            elif message.text == "8":
                self.register_unwanted_attendees_step(message)
            elif message.text == "9":
                self.register_gratitude_step(message)
            elif message.text == "10":
                self.register_date_step(message)
            elif message.text == "11":
                self.register_time_step(message)
            elif message.text == "12":
                self.register_duration_step(message)
            elif message.text == "13":
                if not self.isOffline:
                    self.register_application_step(message)
                    return
                self.register_address_step(message)
            elif message.text == "14":
                self.register_auto_reply_step(message)
            elif message.text == "15":
                self.subscribe_callback_handler(self.start_callback_handler)

                if self.doesExist:
                    Helpers.save_template(self.data)
                    self.send_secondary_message("Changes were saved successfully")
                    self.previous_section()
                    return

                result = Helpers.save_template(self.data)

                if result:
                    self.send_simple_message(f"Done :)")
                else:
                    self.send_simple_message("Something went wrong! Please, contact the administration")

                self.previous_section()
            elif message.text == "16":
                self.registration_discard_changes(message)
            else:
                self.delete_message(message)
                self.bot.register_next_step_handler(message, self.register_checkout, acceptMode=acceptMode, chat_id=self.current_user)

    def registration_template_step(self, message, acceptMode=False):
        if not acceptMode:
            self.send_active_message("Would you like to save adventure as a template?\nBy doing that, you will be able to create the same adventure with a few clicks without going through all registration steps again", self.YNMarkup)
            self.bot.register_next_step_handler(message, self.registration_template_step, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "Yes":
                result = Helpers.save_template(self.data)
                if result:
                    self.send_secondary_message("Done :). You may access, change or use this template whenever you like in 'My Templates' section")
                else:
                    self.send_secondary_message("Something went wrong ! Please, contact the administration")
                self.subscribe_callback_handler(self.start_callback_handler)
                self.previous_section()
            elif message.text == "No":
                self.subscribe_callback_handler(self.start_callback_handler)
                self.previous_section()
            else:
                self.send_active_message("No such option", self.YNMarkup)
                self.bot.register_next_step_handler(message, self.registration_template_step, acceptMode=acceptMode, chat_id=self.current_user)

    def registration_discard_changes(self, message, acceptMode=False):
        if not acceptMode:
            self.send_secondary_message("Are you sure, you want to discard changes?", self.YNMarkup)
            self.bot.register_next_step_handler(message, self.registration_discard_changes, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "Yes":
                self.subscribe_callback_handler(self.start_callback_handler)

                if self.isTemplate:
                    self.previous_section(self.current_template)
                    return

                self.previous_section()
            else:
                if self.isTemplate:
                    self.register_template_checkout(message)
                else:
                    self.register_checkout(message)

    def registration_abandon(self, message, acceptMode=False):
        if not acceptMode:
            self.send_active_message("Are you sure, you want to leave? All changes will be lost", markup=self.YNMarkup)
            self.bot.register_next_step_handler(message, self.registration_abandon, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "Yes":
                self.subscribe_callback_handler(self.start_callback_handler)
                self.previous_section()
            else:
                self.current_registration_step(message)

    def my_adventures_manager(self):
        self.subscribe_callback_handler(self.start_callback_handler)

        self.previous_section = self.start
        self.assemble_my_adventures_markup()
        self.manageMode = 1
        self.send_active_message("<b><i>Here are adventures created by you</i></b>", markup=self.my_adventuresMarkup)

    def manage_adventure(self):
        self.previous_section = self.my_adventures_manager
        self.subscribe_callback_handler(self.manage_adventure_callback_handler)
        self.load_adventure_data(self.current_adventure)
        self.send_active_message("Please, choose an option", markup=self.manage_adventure_markup)

    #TODO: Add paging
    def manage_adventure_attendees(self):
        self.previous_section = self.manage_adventure
        self.assemble_my_adventure_attendees_markup()

        if len(self.my_adventures_attendeesMarkup.keyboard) == 1:
            self.send_secondary_message("No attendees yet!")
            self.previous_section()
            return

        self.send_active_message("Please, select an attendee", markup=self.my_adventures_attendeesMarkup)

    def manage_adventure_attendee(self):
        self.previous_section = self.manage_adventure_attendees

        #TODO: Use another method / Remake it to be more suitable -> THUS shown description will look better
        self.current_attendee_data = Helpers.get_user_info(self.current_attendee)

        #If is new attendee
        if self.current_attendees_statuses[self.current_attendee] == 1:
            self.display_current_attendee_data()
            self.send_secondary_message("<i><b>🔝You have new participation request🔝</b></i>", markup=self.set_attendee_status_markup)
            return

        self.display_current_attendee_data()
        self.send_secondary_message("Please, select an option", markup=self.manage_adventure_attendee_markup)

    def resolve_participation_request(self, status):
        Helpers.process_participation_request(self.current_adventure, self.current_attendee, status)

        self.delete_secondary_message()
        self.previous_section()

    def delete_adventure(self):
        pass

    def change_adventure(self):
        self.previous_section = self.manage_adventure

        self.isTemplate = False
        self.subscribe_callback_handler(self.registration_callback_handler)
        self.register_checkout(self.message)

    def subscribed_adventures_manager(self):
        self.previous_section = self.start

        self.manageMode = 2
        if len(self.subscribed_adventuresMarkup) > 1:
            self.send_active_message("<b><i>Here are all adventures, you are subscribed on</i></b>", markup=self.subscribed_adventuresMarkup)

        self.send_active_message("<b><i>You are not subscribed on any adventures!\nTry finding them :)</b></i>", markup=self.search_markup)

    def search_choice(self):
        self.previous_section = self.start
        self.send_active_message("<b><i>Please, chose an option</i></b>", markup=self.search_markup)

    def recurring_adventure_search(self):
        self.delete_active_message()
        self.delete_secondary_message()
        self.previous_section = self.search_choice
        #TODO: finish up

    def join_by_code(self, message, acceptMode=False):
        if not acceptMode:
            self.previous_section = self.search_choice

            self.send_active_message("Please, enter the code:", markup=self.goBackMarkup)
            self.bot.register_next_step_handler(message, self.join_by_code, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message)
            if message.text == "Go Back":
                self.previous_section()
                return

            response = Helpers.send_adventure_request_by_code(self.current_user, message.text)
            if response == 2:
                self.send_secondary_message("Invalid invitation code", markup=self.goBackMarkup)
                self.bot.register_next_step_handler(message, self.join_by_code, acceptMode=acceptMode, chat_id=self.current_user)
            elif response == 3:
                self.send_secondary_message("You are already participating in this adventure", markup=self.goBackMarkup)
                self.bot.register_next_step_handler(message, self.join_by_code, acceptMode=acceptMode, chat_id=self.current_user)
            elif response == 4:
                self.send_secondary_message("Unable to join the adventure. You are its owner", markup=self.goBackMarkup)
                self.bot.register_next_step_handler(message, self.join_by_code, acceptMode=acceptMode, chat_id=self.current_user)
            else:
                self.send_secondary_message("Done! your request had been sent to the adventure's owner :)")
                self.previous_section()

    def manage_templates(self):
        self.manageMode = 3
        self.previous_section = self.start
        self.assemble_my_templates_markup()
        self.send_active_message("<b><i>Please, select markup you want to manage</i></b>", markup=self.my_templatesMarkup)

    def manage_single_template(self, templateId):
        self.current_template = templateId
        self.previous_section = self.manage_templates
        self.send_active_message("<i><b>Please, select an option</b></i>", self.manage_template_markup)

    def change_current_template(self):
        self.isTemplate = True
        self.doesExist = True
        self.previous_section = self.manage_single_template
        self.data = Helpers.get_template(self.current_template)
        self.register_template_checkout(self.message)

    def delete_current_template(self, message, acceptMode=False):
        if not acceptMode:
            self.send_secondary_message("Are you sure, you want to delete template ?", markup=self.YNMarkup)
            self.bot.register_next_step_handler(message, self.delete_current_template, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "Yes":
                result = Helpers.delete_template(self.current_template)

                if result == 1:
                    self.send_secondary_message("Done !")
                elif result == 2:
                    self.send_secondary_message("Template does not exist !")
                else:
                    self.send_secondary_message("Something went wrong ! Please, contact administration")

                self.manage_templates()
            else:
                self.manage_single_template(self.current_template)

    def remove_attendee(self, message, acceptMode=False):
        if not acceptMode:
            self.send_secondary_message("Are you sure, you want to remove this attendee from this adventure ? It will not be possible for him to re-enter the adventure", markup=self.YNMarkup)
            self.bot.register_next_step_handler(message, self.remove_attendee, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "Yes":
                result = Helpers.delete_attendee(self.current_adventure, self.current_attendee)

                if result == 1:
                    if self.data["groupId"]:
                        self.bot.kick_chat_member(self.data["groupId"], self.current_attendee)

                        self.send_secondary_message("Done !")
                elif result == 2:
                    self.send_secondary_message("Attendee does not exist !")
                else:
                    self.send_secondary_message("Something went wrong ! Please, contact administration")

                self.manage_adventure_attendees()
            else:
                self.manage_adventure_attendee()

    def start_callback_handler(self, call):
        if call.data == "1":
            self.my_adventures_manager()
        elif call.data == "2":
            self.subscribed_adventures_manager()
        elif call.data == "3":
            self.search_choice()
        elif call.data == "4":
            self.recurring_adventure_search()
        elif call.data == "5":
            self.join_by_code(call.message)
        elif call.data == "6":
            self.manage_templates()
        elif call.data == "7":
            self.change_current_template()
        elif call.data == "8":
            self.delete_current_template(call.message)
        elif call.data == "150":
            self.register_pre_register()
        elif call.data == "160":
            self.isCreatedFromTemplate = False
            self.register_start()
        elif call.data == "161":
            self.isCreatedFromTemplate = True
            self.choose_template()
        elif call.data == "151":
            self.register_template_start()
        elif call.data == "-20":
            self.previous_section()
        #Adventure and Template selection
        else:
            if self.manageMode == 1:
                self.current_adventure = call.data
                self.manage_adventure()
            elif self.manageMode == 2:
                pass
            elif self.manageMode == 3:
                self.previous_section = self.manage_templates
                self.manage_single_template(call.data)
            elif self.manageMode == 4:
                self.current_template = call.data
                self.register_from_template()

    def registration_callback_handler(self, call):
        if call.data == "-1" or call.data == "-2":
            try:
                index = index_converter(call.data)
                if self.markup_page + index <= self.markup_pages_count or self.markup_page + index >= 1:
                    markup = assemble_markup(self.markup_page, self.current_markup_elements, index)
                    self.bot.edit_message_reply_markup(chat_id=call.message.chat.id, reply_markup=markup,
                                                       message_id=call.message.id)
                    self.markup_page += index
            except:
                pass

        # Registering adventures
        elif call.data == "4":
            self.register_online()
        elif call.data == "5":
            self.register_offline()
        elif call.data == "-20":
            self.registration_abandon(call.message)
        #TODO: add description handlers (304, 305)
        else:
            if self.question_index == 4:
                if int(call.data) in self.countries.keys():
                    self.data["countryId"] = int(call.data)
                    self.bot.answer_callback_query(call.id, "Gotcha")
                    if self.previous_item:
                        remove_tick_from_element(self.bot, self.current_user, call.message.id,
                                                 self.current_markup_elements,
                                                 self.markup_page, self.previous_item)
                    add_tick_to_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                        self.markup_page, call.data)
                    self.previous_item = call.data
                else:
                    self.bot.send_message(call.message.chat.id, "Incorrect country code")

            elif self.question_index == 5:
                if int(call.data) in self.cities.keys():
                    self.data["cityId"] = int(call.data)
                    self.bot.answer_callback_query(call.id, "Gotcha")
                    if self.previous_item:
                        remove_tick_from_element(self.bot, self.current_user, call.message.id,
                                                 self.current_markup_elements,
                                                 self.markup_page, self.previous_item)
                    add_tick_to_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                        self.markup_page, call.data)
                    self.previous_item = call.data
                else:
                    self.bot.answer_callback_query(call.id, "Incorrect city code")

    def manage_adventure_callback_handler(self, call):
        if call.data == "1":
            pass
        elif call.data == "2":
            self.manage_adventure_attendees()
        elif call.data == "3":
            self.load_adventure_data(self.current_adventure)
            self.register_checkout(call.message)
        elif call.data == "4":
            pass
        elif call.data == "6": # Get Attendee's username
            self.get_attendee_contact()
        elif call.data == "7": # Remove attendee
            self.remove_attendee(call.message)
        elif call.data == "10":
            self.resolve_participation_request(2)
        elif call.data == "11":
            self.resolve_participation_request(3)
        elif call.data == "-20":
            self.delete_secondary_message() #TODO: reconsider usefulness
            self.previous_section()
        else:
            self.current_attendee = call.data
            self.manage_adventure_attendee()

    def get_attendee_contact(self):
        username = self.current_attendee_data["userBaseInfo"]["userName"]

        if username:
            self.send_secondary_message("User does not have a username :(")

        self.send_secondary_message(f"{username}")

    def display_current_attendee_data(self):
        base = self.current_attendee_data["userBaseInfo"]

        if base["isMediaPhoto"]:
            self.send_active_message_with_photo(base["userDescription"], base["userMedia"])
        else:
            self.send_active_message_with_video(base["userDescription"], base["userMedia"])

    def send_active_message(self, text, markup=None):
        try:
            if self.active_message:
                self.bot.edit_message_text(text, self.current_user, self.active_message, reply_markup=markup)
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

    def send_simple_message(self, text, markup=None):
        self.bot.send_message(self.current_user, text, reply_markup=markup)

    def send_active_message_with_photo(self, text, photo, markup=None):
        self.delete_active_message()
        self.active_message = self.bot.send_photo(self.current_user, photo, text, reply_markup=markup).id

    def send_active_message_with_video(self, text, video, markup=None):
        self.delete_active_message()
        self.active_message = self.bot.send_video(self.current_user, video, text, reply_markup=markup).id

    def subscribe_callback_handler(self, handler):
        if self.current_callback_handler:
            self.bot.callback_query_handlers.remove(self.current_callback_handler)

        self.current_callback_handler = self.bot.register_callback_query_handler("", handler, user_id=self.current_user)

    def delete_active_message(self):
        if self.active_message:
            self.bot.delete_message(self.current_user, self.active_message)
            self.active_message = None

    def delete_secondary_message(self):
        if self.secondary_message:
            self.bot.delete_message(self.current_user, self.secondary_message)
            self.secondary_message = None

    def delete_message(self, message):
        self.bot.delete_message(self.current_user, message.id)

    def assemble_my_adventures_markup(self):
        adventures = json.loads(requests.get(f"https://localhost:44381/GetUserAdventures/{self.current_user}", verify=False).text)
        self.my_adventuresMarkup.clear()

        for adventure in adventures:
            status = self.statusDict[adventure["status"]]
            self.my_adventuresMarkup.add(InlineKeyboardButton(f"{status} {adventure['name']} {status}", callback_data=adventure["id"]))

        if len(adventures) + 1 <= self.creation_limit:
            self.my_adventuresMarkup.add(InlineKeyboardButton("➕ Add ➕", callback_data="150"))

        self.my_adventuresMarkup.add(InlineKeyboardButton("Go Back", callback_data="-20"))

    def assemble_my_adventure_attendees_markup(self):
        attendees = json.loads(requests.get(f"https://localhost:44381/adventure-attendees/{self.current_adventure}", verify=False).text)
        self.my_adventures_attendeesMarkup.clear()
        self.current_attendees_statuses.clear()

        for attendee in attendees:
            status = self.attendee_statusDict[attendee["status"]]
            self.current_attendees_statuses[str(attendee["userId"])] = attendee["status"]
            self.my_adventures_attendeesMarkup.add(InlineKeyboardButton(f"{status} {attendee['username']} {status}", callback_data=attendee["userId"]))

        self.my_adventures_attendeesMarkup.add(InlineKeyboardButton("Go Back", callback_data="-20"))

    def assemble_subscribed_adventures_markup(self):
        adventures = json.loads(requests.get(f"https://localhost:44381/GetUsersSubscribedAdventures/{self.current_user}", verify=False).text)

        self.subscribed_adventuresMarkup.clear()

        for adventure in adventures:
            self.subscribed_adventuresMarkup.add(InlineKeyboardButton(adventure["name"], callback_data=adventure["id"]))

        self.subscribed_adventuresMarkup.add(InlineKeyboardButton("Go Back", callback_data="-20"))

    def assemble_my_templates_markup(self, addNewButton=True):
        templates = Helpers.get_templates(self.current_user)

        self.my_templatesMarkup.clear()

        for template in templates:
            self.my_templatesMarkup.add(InlineKeyboardButton(template["name"], callback_data=template["id"]))

        #TODO: Create its own separate limit
        if addNewButton and len(templates) + 1 <= self.creation_limit:
            self.my_templatesMarkup.add(InlineKeyboardButton("➕ Add ➕", callback_data="151"))

        self.my_templatesMarkup.add(InlineKeyboardButton("Go Back", callback_data="-20"))

    def assemble_adventure_checkout_message(self):
        #Load dependencies if weren't loaded before
        self.load_countries()
        self.load_cities(self.data["countryId"])

        message = f"{self.data['name']}\n*{self.countries[self.data['countryId']]}, {self.cities[self.data['cityId']]}*\n{self.data['description']}\n{self.data['experience']}\n\n{self.data['attendeesDescription']}\n{self.data['unwantedAttendeesDescription']}\n{self.data['gratitude']}\n\n{self.data['date']}\n{self.data['time']}\n{self.data['duration']}\n{self.data['application']}{self.data['address']}"
        message = message.replace("\n\n\n", "\n")
        # message.replace("\n\n", "\n")

        return message

    def country_convertor(self, country):
        for c in self.countries:
            if country == self.countries[c]:
                return c
        return None

    def city_convertor(self, city):
        for c in self.cities:
            if city == self.cities[c]:
                return c
        return None

    def load_adventure_data(self, adventureId):
        self.data = json.loads(requests.get(f"https://localhost:44381/adventure/{adventureId}", verify=False).text)

        self.doesExist = True
        self.isOffline = self.data["isOffline"]
        self.country = self.data["countryId"]
        self.city = self.data["cityId"]

    def load_countries(self):
        if not self.countries:
            countries = json.loads(
                requests.get(f"https://localhost:44381/GetCountries/{self.user_localization}", verify=False).text)

            for country in countries:
                self.countries[country["id"]] = country["countryName"].lower().strip()

    def load_cities(self, country):
        if not self.cities:
            cities = json.loads(requests.get(f"https://localhost:44381/GetCities/{country}/{self.user_localization}",
                                             verify=False).text)

            # For edit purposes. If left as they are -> can result bugs
            self.cities.clear()

            for city in cities:
                self.cities[city["id"]] = city["cityName"].lower()

    def destruct(self):
        if self.current_callback_handler:
            self.bot.callback_query_handlers.remove(self.current_callback_handler)

        self.delete_active_message()
        go_back_to_main_menu(self.bot, self.current_user, self.message)
        del self