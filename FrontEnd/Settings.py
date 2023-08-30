import base64
import random

from Registration import *
from ReportModule import *
from Common.Menues import count_pages, assemble_markup, reset_pages, index_converter
from telebot.types import ReplyKeyboardMarkup, InlineKeyboardButton, InlineKeyboardMarkup
import Core.HelpersMethodes as Helpers

from Shop import Shop


class Settings:
    def __init__(self, bot: TeleBot, message: any, verificationOnly: bool = False, activeMessage=0, returnMethod=None):
        self.isInBlackList = False
        self.verificationOnly = verificationOnly
        #Indicates whether if callback query handler must be deleted. Set to true if it should
        self.notInMenu = False
        self.isDeciding = False
        self.previous_section = None
        self.active_section = None
        self.isEncounter = None
        self.current_query = 0
        self.current_managed_user = 0
        self.markup_last_element = 0
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id
        self.old_queries = []
        self.black_list = {}
        self.encounter_list = {}

        self.user_points = {}
        self.user_free_points = 0
        self.free_points_indicator = None
        self.o_indicator = None
        self.c_indicator = None
        self.e_indicator = None
        self.a_indicator = None
        self.n_indicator = None
        self.p_indicator = None
        self.oceanMarkup = None
        self.ocean_caps = {}
        self.ocean_updated_points = {}
        self.current_markup_elements = []
        self.markup_page = 1
        self.markup_pages_count = 0
        self.current_callback_handler = None
        self.current_user_data = Helpers.get_user_info(self.current_user)

        self.gestures = ["👍","👎","💪","✊","👊","🖐","✋","👋","👌","✌","🤘","🤟 ","🤙","🤞","🖕","🖖","☝","👆", "👇", "👉","👈"]
        self.gesture = None

        #TODO: check this parameter instead of calling API every time
        self.uses_ocean = self.current_user_data["settings"]["usesOcean"]
        self.has_Premium = self.current_user_data["hasPremium"]
        self.language_cons_status = self.current_user_data["settings"]["shouldConsiderLanguages"]
        self.free_status = self.current_user_data["settings"]["isFree"]
        self.comments_status = self.current_user_data["settings"]["shouldComment"]
        self.hints_status = self.current_user_data["settings"]["shouldSendHints"]
        self.real_photo_filter_status = self.current_user_data["settings"]["shouldFilterUsersWithoutRealPhoto"]
        self.increased_familiarity_status = self.current_user_data["settings"]["increasedFamiliarity"]
        self.user_language = self.current_user_data["data"]["language"]

        self.userBalance = Helpers.get_active_user_balance(self.current_user)
        self.requestStatus = requests.get(f"https://localhost:44381/CheckTickRequestStatus/{self.current_user}", verify=False).text

        self.secondChance_indicator = None
        self.valentine_indicator = None
        self.detector_indicator = None
        self.nullifier_indicator = None
        self.cardDeckMini_indicator = None
        self.cardDeckPlatinum_indicator = None

        self.usedEffectAmount = 0
        self.effect_index = 0
        self.secondChances = 0
        self.valentines = 0
        self.detectors = 0
        self.nullifiers = 0
        self.cardDecksMini = 0
        self.cardDecksPlatinum = 0
        self.effects_markup = None

        self.invitation_creds = None

        self.current_markup_elements = []
        self.markup_last_element = 0
        self.markup_page = 1
        self.markup_pages_count = 0

        self.achievements = {}
        self.achievements_data = {}

        self.active_message = activeMessage
        self.message_with_confirmation = 0
        self.active_secondary_message = 0
        self.active_error_message = 0

        self.return_method = returnMethod

        self.requestType = 0

        self.turnedOnSticker = "✅"
        self.turnedOffSticker = "❌"

        self.add_to_blacklist_text = "Add user to a blacklist"
        self.remove_from_blacklist_text = "Remove user from a blacklist"

        self.chooseOption_message = "Choose the option:"
        self.increased_familiarity_start_message = "When you invite someone, this person instantly receives like from you.\n"
        self.increased_familiarity_offline_message = "This functionality is currently Turned off. Would you like to turn it on?"
        self.increased_familiarity_online_message = "This functionality is currently Turned on. Would you like to turn it off?"
        self.effectsMessage = f"1. Manage my effects\n\n2.Go back"
        self.encounter_options_message = "1. Use 💥Second chance💥 to send like to a user once again. You have {}\n2. Report user\n3. {}\n4. Abort"
        self.currency_list_message = "1. USD\n2. EUR\n3. UAH\n4. RUB\n5. CZK\n6. PLN\n7. Go Back"

        # self.settingMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3", "4", "5", "6")
        self.settingMarkup = InlineKeyboardMarkup().add(InlineKeyboardButton("👤 My Profile 👤", callback_data="200")) \
            .add(InlineKeyboardButton("OCEAN+ Settings", callback_data="201")) \
            .add(InlineKeyboardButton("Filter Settings", callback_data="202")) \
            .add(InlineKeyboardButton("Inventory and Statistics", callback_data="203")) \
            .add(InlineKeyboardButton("Additional Actions", callback_data="204")) \
            .add(InlineKeyboardButton("Exit", callback_data="-20"))

        # self.settingMyProfileMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3", "4", "5", "6", "7")
        self.settingMyProfileMarkup = InlineKeyboardMarkup().add(InlineKeyboardButton("View the blacklist", callback_data="225")) \
            .add(InlineKeyboardButton("Manage recently encountered users", callback_data="226")) \
            .add(InlineKeyboardButton("Change profile properties", callback_data="227")) \
            .add(InlineKeyboardButton("⭐Set profile status⭐", callback_data="228")) \
            .add(InlineKeyboardButton("Set Auto reply message", callback_data="229")) \
            .add(InlineKeyboardButton("Change payment currency", callback_data="230")) \
            .add(InlineKeyboardButton("Go back", callback_data="-20")) \

        self.settingOceanSettingsMarkup_TurnedOn = InlineKeyboardMarkup().add(InlineKeyboardButton("Turn Off OCEAN+", callback_data="205")) \
            .add(InlineKeyboardButton("Manage OCEAN+ points", callback_data="206")) \
            .add(InlineKeyboardButton("Pass Tests", callback_data="207")) \
            .add(InlineKeyboardButton("Go back", callback_data="-20"))

        self.settingOceanSettingsMarkup_TurnedOff = InlineKeyboardMarkup()\
            .add(InlineKeyboardButton("Turn On OCEAN+", callback_data="205")) \
            .add(InlineKeyboardButton("Go back", callback_data="-20"))

        self.language_considerationIndicator = InlineKeyboardButton("", callback_data="210")
        self.free_statusIndicator = InlineKeyboardButton("", callback_data="211")
        self.hints_statusIndicator = InlineKeyboardButton("", callback_data="242")
        self.comments_statusIndicator = InlineKeyboardButton("", callback_data="243")
        self.real_photo_filterIndicator = InlineKeyboardButton("", callback_data="212")

        self.language_considerationIndicator.text = self.turnedOnSticker if self.language_cons_status else self.turnedOffSticker
        self.free_statusIndicator.text = self.turnedOnSticker if self.free_status else self.turnedOffSticker
        self.hints_statusIndicator.text = self.turnedOnSticker if self.hints_status else self.turnedOffSticker
        self.comments_statusIndicator.text = self.turnedOnSticker if self.comments_status else self.turnedOffSticker
        self.real_photo_filterIndicator.text = self.turnedOnSticker if self.real_photo_filter_status else self.turnedOffSticker

        self.settingFiltersSettingsMarkup = InlineKeyboardMarkup().add(InlineKeyboardButton("Consider languages in Random Conversations", callback_data="310"), self.language_considerationIndicator) \
            .add(InlineKeyboardButton("Free today", callback_data="311"), self.free_statusIndicator) \
            .add(InlineKeyboardButton("Send Hints", callback_data="342"), self.hints_statusIndicator) \
            .add(InlineKeyboardButton("Comment on user profiles", callback_data="343"), self.comments_statusIndicator) \
            .add(InlineKeyboardButton("⭐Filter by a real photo⭐", callback_data="312"), self.real_photo_filterIndicator) \
            .add(InlineKeyboardButton("Go back", callback_data="-20"))

        self.settingStatisticsMarkup = InlineKeyboardMarkup().add(InlineKeyboardButton("Achievements", callback_data="213")) \
            .add(InlineKeyboardButton("Manage effects", callback_data="214")) \
            .add(InlineKeyboardButton("💎Top-Up coin balance💎", callback_data="215")) \
            .add(InlineKeyboardButton("💎Top-Up OCEAN+ points balance💎", callback_data="216")) \
            .add(InlineKeyboardButton("💎Buy premium access💎", callback_data="217")) \
            .add(InlineKeyboardButton("Go back", callback_data="-20"))

        self.effects_manageMarkup = InlineKeyboardMarkup() \
            .add(InlineKeyboardButton("Active effects", callback_data="218")) \
            .add(InlineKeyboardButton("My effects", callback_data="219")) \
            .add(InlineKeyboardButton("Go Back", callback_data="-20"))

        self.increased_familiarityIndicator = InlineKeyboardButton("0", callback_data="221")
        self.increased_familiarityIndicator.text = self.turnedOnSticker if self.increased_familiarity_status else self.turnedOffSticker

        self.settingAdditionalActionsMarkup = InlineKeyboardMarkup() \
            .add(InlineKeyboardButton("Invitation Creds", callback_data="220")) \
            .add(InlineKeyboardButton("Increased Familiarity", callback_data="321"), self.increased_familiarityIndicator) \
            .add(InlineKeyboardButton("Delete profile", callback_data="250"))

        self.settingConfirmationRequestMarkup = InlineKeyboardMarkup()\
            .add(InlineKeyboardButton("Partial", callback_data="240"), InlineKeyboardButton("ℹ", callback_data="340"))\
            .add(InlineKeyboardButton("Full", callback_data="241"), InlineKeyboardButton("ℹ", callback_data="341")) \
            .add(InlineKeyboardButton("Go Back", callback_data="-20"))

        self.black_listButton = InlineKeyboardButton("", callback_data="102")
        self.encounterOptionMarkup = InlineKeyboardMarkup() \
            .add(InlineKeyboardButton("Use 💥Second chance💥", callback_data="100")) \
            .add(InlineKeyboardButton("Report user", callback_data="101")) \
            .add(self.black_listButton) \
            .add(InlineKeyboardButton("Go Back", callback_data="-20")) \

        #Api returns 1 if request was accepted. 3 - if it is being processed right now
        if self.requestStatus != "1" and self.requestStatus != "3":
            self.settingAdditionalActionsMarkup.add(InlineKeyboardButton("Confirm my identity", callback_data="222"))

        self.settingAdditionalActionsMarkup.add(InlineKeyboardButton("Go Back", callback_data="-20"))

        self.credsMarkup = InlineKeyboardMarkup() \
            .add(InlineKeyboardButton("Link", callback_data="223")) \
            .add(InlineKeyboardButton("QR code", callback_data="224")) \
            .add(InlineKeyboardButton("Go Back", callback_data="-20")) \

        self.activate_effectMarkup = InlineKeyboardMarkup().add(InlineKeyboardButton("💥Activate💥", callback_data="-10"))
        self.buy_effectMarkup = InlineKeyboardMarkup().add(InlineKeyboardButton("💥Buy💥", callback_data="-5"))

        self.YNMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("yes", "no")
        self.skipMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("Skip", "Go Back")
        self.abortMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("Go Back")
        self.currency_choiceMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3", "4", "5", "6", "7")

        self.effect_is_active_Warning = "<i><b>Warning! This effect is already active. Repeated usage will override the effect's timer !</b></i>"

        self.secondChanceDescription = "Second chance allows you to 'like' another user once again. It can be used in the 'Encounters' section"
        self.valentineDescription = "Doubles your OCEAN+ points for an hour"
        self.detectorDescription = "When matched, shows which OCEAN+ parameters were matched. Works for 1 hour"
        self.nullifierDescription = "Allows you to pass any test one more time, without waiting. It can be activated from the 'Test' section"
        self.cardDeckMiniDescription = "Instantly adds 20 profile views to your daily views"
        self.cardDeckPlatinumDescription = "Instantly adds 50 profile views to your daily views"

        self.ocean_description = "OCEAN+ system is a test-based system, which helps you in finding people or adventures that you need.\n\nEvery OCEAN+ test you pass provides us with better opportunity to give you search result that may suit you the most.\n\n"

        self.okMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("Ok")

        self.helpHandler = self.bot.register_message_handler(self.help_handler, commands=["help"], user_id=self.current_user, is_multihandler=True)

        self.nextHandler = None

        self.subscribe_callback_handler(self.menu_callback_handler)

        if self.verificationOnly:
            self.choose_confirmation_request(message)
            return

        self.setting_choice()

    def setting_choice(self, message=None):
        self.previous_section = self.destruct
        self.active_section = self.setting_choice

        self.send_active_message("<i><b>Please, select an option</b></i>", markup=self.settingMarkup)

    def my_profile_settings_choice(self, message):
        self.previous_section = self.setting_choice
        self.active_section = self.my_profile_settings_choice

        self.send_active_message("<i><b>You can type /help at any time to get more info about functionalities :)</b></i>", markup=self.settingMyProfileMarkup)

    def currency_change_manager(self, message):
        self.previous_section = self.my_profile_settings_choice
        self.active_section = self.currency_change_manager

        self.delete_active_message()
        self.delete_secondary_message()

        self.clear_callback_handler()
        CurrencySetter(self.bot, self.current_user, self.go_back_from_currency_change)

    def go_back_from_currency_change(self, message):
        self.subscribe_callback_handler(self.menu_callback_handler)
        self.previous_section(message)

    def auto_reply_manager(self, message, acceptMode=False):
        self.isDeciding = True
        self.previous_section = self.my_profile_settings_choice
        self.active_section = self.auto_reply_manager

        if not acceptMode:
            auto_reply = json.loads(requests.get(f"https://localhost:44381/GetActiveAutoReply/{self.current_user}", verify=False).text)
            if not auto_reply["isEmpty"]:
                if not auto_reply["isText"]:
                    self.send_secondary_message(voice=auto_reply['autoReply'], text=f"Send me a text message (up to 300 characters) or a ⭐Voice message (Up to 15 seconds)⭐ that will be sent to every user who likes your profile\n\nThis is your current auto reply:", markup=self.abortMarkup)
                else:
                    self.send_secondary_message(f"Send me a text message (up to 300 characters) or a ⭐Voice message (Up to 15 seconds)⭐ that will be sent to every user who likes your profile\n\nThis is your current auto reply:<i><b>{auto_reply['autoReply']}</b></i>", markup=self.abortMarkup)
                self.bot.register_next_step_handler(message, self.auto_reply_manager, acceptMode=True, chat_id=self.current_user)
                return

            self.send_secondary_message("Send me a text message (up to 300 characters) or a ⭐Voice message (Up to 15 seconds)⭐ that will be sent to every user who likes your profile", markup=self.abortMarkup)
            self.bot.register_next_step_handler(message, self.auto_reply_manager, acceptMode=True, chat_id=self.current_user)

        else:
            self.bot.delete_message(self.current_user, message.id)
            if message.text == "Go Back":
                self.proceed()
                return

            if message.voice:
                if self.has_Premium:
                    if message.voice.duration <= 15:
                        if bool(json.loads(json.loads(requests.get(f"https://localhost:44381/SetAutoReplyVoice/{self.current_user}/{message.voice.file_id}", verify=False).text))):
                            self.bot.send_message(self.current_user, "Set successfully !")
                            self.proceed()
                            return False
                        self.send_error_message("Something went wrong. Please, contact the administration")
                        self.proceed()
                    else:
                        self.send_error_message("Up to 15 seconds, my friend", markup=self.abortMarkup)
                        self.bot.register_next_step_handler(message, self.auto_reply_manager, acceptMode=acceptMode, chat_id=self.current_user)
                else:
                    self.send_error_message("Sorry, only users with premium access can perform that action", markup=self.abortMarkup)
                    self.bot.register_next_step_handler(message, self.auto_reply_manager, acceptMode=acceptMode, chat_id=self.current_user)
            elif message.text:
                if len(message.text) < 300:
                    if message.text == "abort":
                        self.proceed()
                        return False
                    if bool(json.loads(requests.get(f"https://localhost:44381/SetAutoReplyText/{self.current_user}/{message.text}", verify=False).text)):
                        self.bot.send_message(self.current_user, "Set successfully !")
                        self.proceed()
                        return False
                    self.send_error_message("Something went wrong. Please, contact the administration")
                    self.proceed()
                else:
                    self.send_error_message("Up to 300 characters please", markup=self.abortMarkup)
                    self.bot.register_next_step_handler(message, self.auto_reply_manager, acceptMode=acceptMode, chat_id=self.current_user)
            else:
                self.send_error_message("Only text and voice messages are accepted", markup=self.abortMarkup)
                self.bot.register_next_step_handler(message, self.auto_reply_manager, acceptMode=acceptMode, chat_id=self.current_user)

    def ocean_settings_choice(self, message, acceptMode=False):
        self.previous_section = self.setting_choice
        self.active_section = self.ocean_settings_choice

        activeMarkup = None

        if self.uses_ocean:
            activeMarkup = self.settingOceanSettingsMarkup_TurnedOn
        else:
            activeMarkup = self.settingOceanSettingsMarkup_TurnedOff

        self.send_active_message("Please select an option", activeMarkup)

    def filters_settings_choice(self, message=None):
        self.previous_section = self.setting_choice
        self.active_section = self.filters_settings_choice

        self.send_active_message("Click on a filter's name to see it's description", markup=self.settingFiltersSettingsMarkup)

    def language_consideration_manager(self, message, showDescription=False):
        self.active_section = self.language_consideration_manager

        if showDescription:
            description = "When this filter is turned on - all people you'll encounter in /random section are going to speak in a languages you have chosen during registration. If it is turned off - you will be able to encounter absolutely random people, from random places, speaking languages, that can vary from yours.\n"

            self.send_secondary_message(description)
        else:
            response = requests.get(f"https://localhost:44381/SwitchUserRTLanguageConsideration/{self.current_user}", verify=False)
            self.language_cons_status = not self.language_cons_status
            if response.status_code == 200:
                self.switch_toggle_filter(self.language_considerationIndicator)
                self.edit_active_message_markup(self.settingFiltersSettingsMarkup)
            else:
                self.send_secondary_message("Something went wrong. Please, contact the administration")

    def free_status_manager(self, showDescription=False):
        self.active_section = self.free_status_manager

        if showDescription:
            description = "This filter allows you to access so called 'free search'. Using it you can find people, who are up for a meeting today :)"

            self.send_secondary_message(description)
        else:
            response = requests.get(f"https://localhost:44381/SwitchUserFreeSearchParam/{self.current_user}", verify=False)
            self.free_status = not self.free_status
            if response.status_code == 200:
                self.switch_toggle_filter(self.free_statusIndicator)
                self.edit_active_message_markup(self.settingFiltersSettingsMarkup)
            else:
                self.send_secondary_message("Something went wrong. Please, contact the administration")

    def comment_status_manager(self, showDescription=False):
        self.active_section = self.free_status_manager

        if showDescription:
            description = "Personality bot can make its own comments of profiles you encounter in any kind of search. Try it out!"

            self.send_secondary_message(description)
        else:
            response = Helpers.switch_comment_status(self.current_user)
            self.comments_status = not self.comments_status
            if response.status_code == 200:
                self.switch_toggle_filter(self.comments_statusIndicator)
                self.edit_active_message_markup(self.settingFiltersSettingsMarkup)
            else:
                self.send_secondary_message("Something went wrong. Please, contact the administration")

    def hints_status_manager(self, showDescription=False):
        self.active_section = self.free_status_manager

        if showDescription:
            description = "Hints are informative or funny phrases. They are sent when you travel between bot's modules"

            self.send_secondary_message(description)
        else:
            response = Helpers.switch_hint_status(self.current_user)
            self.hints_status = not self.hints_status
            if response.status_code == 200:
                self.switch_toggle_filter(self.hints_statusIndicator)
                self.edit_active_message_markup(self.settingFiltersSettingsMarkup)
            else:
                self.send_secondary_message("Something went wrong. Please, contact the administration")

    def real_photo_filter_manager(self, message, showDescription=False):
        self.active_section = self.real_photo_filter_manager

        self.active_section = self.free_status_manager

        if self.has_Premium:
            if showDescription:
                description = "When this filter is turned on, you will encounter only people with a verified profile"

                self.send_secondary_message(description)
            else:
                response = requests.get(f"https://localhost:44381/SwitchUserFilteringByPhoto/{self.current_user}", verify=False)
                self.free_status = not self.language_cons_status
                if response.status_code == 200:
                    self.switch_toggle_filter(self.real_photo_filterIndicator)
                    self.edit_active_message_markup(self.settingFiltersSettingsMarkup)
                else:
                    self.send_secondary_message("Something went wrong. Please, contact the administration")
        else:
            self.send_secondary_message("Sorry, only users with premium can activate that filter")

    def delete_profile(self, message, acceptMode=False):
        if not acceptMode:
            self.previous_section = self.additional_actions_settings_choice
            self.send_secondary_message("Are you sure, you want to delete your profile ?", self.YNMarkup)
            self.bot.register_next_step_handler(message, self.delete_profile, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "yes":
                self.delete_profile_checkout(message)
            else:
                self.proceed()

    def delete_profile_checkout(self, message, acceptMode=False):
        if not acceptMode:
            self.send_secondary_message("We want to improve and make your service suitable and comfortable for everyone.\nPlease, tell us why have you decided to delete your profile", self.skipMarkup)
            self.bot.register_next_step_handler(message, self.delete_profile_checkout, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "Go Back":
                self.proceed(message)
            else:
                Helpers.delete_user_profile(self.current_user, message.text)
                self.bot.send_message(self.current_user, "Done! Your profile had been deleted successfully.\nYou may restore all your data during 30 days by hitting /registration in the main menu")
                self.destruct()

    def my_statistics_settings_choice(self, message):
        self.previous_section = self.setting_choice
        self.active_section = self.my_statistics_settings_choice

        self.send_active_message(self.construct_user_inventory_message(self.userBalance), markup=self.settingStatisticsMarkup)

    #TODO: Test and finish
    def achievement_manager(self, message=None):
        self.achievements.clear()
        self.achievements_data.clear()
        self.notInMenu = True

        self.previous_section = self.my_statistics_settings_choice
        achievements = Helpers.get_all_user_achievements(self.current_user)

        for achievement in achievements:
            name = achievement["achievement"]["name"]

            #Add a tick if user has acquired an achievement
            if achievement["isAcquired"]:
                name = "✅" + name

            self.achievements[achievement["id"]] = name
            self.achievements_data[achievements["id"]] = achievement

        reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page, self.markup_pages_count)
        count_pages(self.achievements, self.current_markup_elements, self.markup_pages_count)
        markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)
        self.markup_page = 1

        # self.current_callback_handler = self.bot.register_callback_query_handler("", self.achievement_callback_handler)
        self.subscribe_callback_handler(self.achievement_callback_handler)
        self.bot.send_message(self.current_user, "Your achievement list", reply_markup=markup)
        self.bot.send_message(self.current_user, "Already acquired achievements are marked with '✅'\nSelect an achievement to view more info")

    def effects_manager(self, message):
        self.previous_section = self.my_statistics_settings_choice
        self.active_section = self.effects_manager

        self.send_active_message("Please, select an option", markup=self.effects_manageMarkup)

    def show_active_users_effects(self, message=None):
        effects = json.loads(requests.get(f"https://localhost:44381/GetUserActiveEffects/{self.current_user}", verify=False).text)
        self.send_secondary_message(self.construct_active_effects_message(effects))

    def show_users_effects(self, message):
        self.notInMenu = True
        self.previous_section = self.effects_manager

        self.construct_effects_markup(self.userBalance)
        self.send_active_message("Here is your effects inventory:", markup=self.effects_markup)
        self.send_secondary_message("Select an effect to read it description")
        self.subscribe_callback_handler(self.effects_callback_handler)

    def additional_actions_settings_choice(self, message, acceptMode=False):
        self.previous_section = self.setting_choice
        self.active_section = self.additional_actions_settings_choice

        self.send_active_message(f"Please, choose an option", markup=self.settingAdditionalActionsMarkup)

    def increased_familiarity_switch(self, showDescription=False):
        if showDescription:
            self.send_secondary_message(self.increased_familiarity_start_message)
        else:
            response = requests.get(f"https://localhost:44381/SwitchIncreasedFamiliarity/{self.current_user}",
                                    verify=False)
            self.increased_familiarity_status = not self.increased_familiarity_status
            if response.status_code == 200:
                self.switch_toggle_filter(self.increased_familiarityIndicator)
                self.edit_active_message_markup(self.settingAdditionalActionsMarkup)
            else:
                self.send_secondary_message("Something went wrong. Please, contact the administration")

    def ocean_points(self, message):
        self.previous_section = self.ocean_settings_choice
        self.active_section = self.ocean_points
        self.notInMenu = True

        self.user_free_points = int(json.loads(requests.get(f"https://localhost:44381/ocean-points-amount/{self.current_user}", verify=False).text))
        self.user_points = json.loads(requests.get(f"https://localhost:44381/ocean-points/{self.current_user}", verify=False).text)
        self.ocean_caps = json.loads(requests.get(f"https://localhost:44381/ocean-caps/{self.current_user}", verify=False).text)

        self.free_points_indicator = InlineKeyboardButton(self.user_free_points, callback_data="0")
        self.subscribe_callback_handler(self.ocean_callback_handler)

        if not self.user_points:
            self.send_error_message(self.current_user, "Something went wrong")
            self.proceed()

        self.o_indicator = InlineKeyboardButton(f"{self.user_points['openness']}", callback_data="0")
        self.c_indicator = InlineKeyboardButton(self.user_points["conscientiousness"], callback_data="0")
        self.e_indicator = InlineKeyboardButton(self.user_points["extroversion"], callback_data="0")
        self.a_indicator = InlineKeyboardButton(self.user_points["agreeableness"], callback_data="0")
        self.n_indicator = InlineKeyboardButton(self.user_points["neuroticism"], callback_data="0")
        self.p_indicator = InlineKeyboardButton(self.user_points["nature"], callback_data="0")

        self.oceanMarkup = InlineKeyboardMarkup() \
            .add(InlineKeyboardButton("Available points:", callback_data="0"), self.free_points_indicator) \
            .add(InlineKeyboardButton("Openness", callback_data="0"))\
            .add(InlineKeyboardButton("-", callback_data="-1"), self.o_indicator, InlineKeyboardButton("+", callback_data="1")) \
            .add(InlineKeyboardButton("Conscientiousness", callback_data="0")) \
            .add(InlineKeyboardButton("-", callback_data="-2"), self.c_indicator, InlineKeyboardButton("+", callback_data="2")) \
            .add(InlineKeyboardButton("Extroversion", callback_data="0")) \
            .add(InlineKeyboardButton("-", callback_data="-3"), self.e_indicator, InlineKeyboardButton("+", callback_data="3")) \
            .add(InlineKeyboardButton("Agreeableness", callback_data="0")) \
            .add(InlineKeyboardButton("-", callback_data="-4"), self.a_indicator, InlineKeyboardButton("+", callback_data="4")) \
            .add(InlineKeyboardButton("Neuroticism", callback_data="0")) \
            .add(InlineKeyboardButton("-", callback_data="-5"), self.n_indicator, InlineKeyboardButton("+", callback_data="5")) \
            .add(InlineKeyboardButton("Nature", callback_data="0")) \
            .add(InlineKeyboardButton("-", callback_data="-6"), self.p_indicator, InlineKeyboardButton("+", callback_data="6")) \
            .add(InlineKeyboardButton("✨SAVE✨", callback_data="7"))

        self.send_active_message("Points table:", markup=self.oceanMarkup)

    def ocean_points_save(self):
        self.ocean_updated_points["balance"] = self.user_free_points
        self.ocean_updated_points["userId"] = self.current_user

        d = json.dumps(self.ocean_updated_points)
        response = requests.post(f"https://localhost:44381/update-ocean-points", d,
                                 headers={"Content-Type": "application/json"},
                                 verify=False)

        if response.status_code == 200:
            self.send_secondary_message("Changes saved successfully :)")
            self.proceed()
        else:
            self.send_secondary_message("Something went wrong. Please, contact the administration")
            self.proceed()

    def credentials_management(self, message=None):
        self.previous_section = self.additional_actions_settings_choice
        self.active_section = self.credentials_management

        #Dispose of description, possibly shown in previous section
        self.delete_secondary_message()

        self.send_active_message("Please, choose an option", markup=self.credsMarkup)

    def show_creds_link(self, message=None):
        if self.invitation_creds is None:
            self.invitation_creds = json.loads(requests.get(f"https://localhost:44381/GenerateInvitationCredentials/{self.current_user}", verify=False).text)

        link = self.invitation_creds["link"]

        if link:
            self.send_secondary_message(f"<b><i>Here you go:</i></b>\n\n{link}")
        else:
            self.send_secondary_message("Something went wrong. Please, contact the administration")

    def show_creds_qrcode(self, message=None):
        if self.invitation_creds is None:
            self.invitation_creds = json.loads(requests.get(f"https://localhost:44381/GenerateInvitationCredentials/{self.current_user}", verify=False).text)

        qrcode = self.invitation_creds["qrCode"]

        if qrcode:
            self.send_error_message("", photo=base64.b64decode(qrcode))
        else:
            self.send_secondary_message("Something went wrong. Please, contact the administration")

    def encounter_list_management(self, message, acceptMode=False):
        self.previous_section = self.my_profile_settings_choice
        self.active_section = self.encounter_list_management
        self.notInMenu = True

        if not acceptMode:
            self.encounter_list = {}
            self.markup_page = 1

            encounters = json.loads(requests.get(f"https://localhost:44381/profile-encounters/{self.current_user}", verify=False).text)

            if encounters:
                for encounter in encounters:
                    self.encounter_list[encounter["encounteredUserId"]] = encounter["encounteredUser"]["userRealName"]

                # self.current_callback_handler = self.bot.register_callback_query_handler("", self.encounters_callback_handler,user_id=self.current_user)
                self.subscribe_callback_handler(self.encounters_callback_handler)

                reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page,
                            self.markup_pages_count)
                self.markup_pages_count = count_pages(self.encounter_list, self.current_markup_elements, self.markup_pages_count, additionalButton=True, buttonText="Go Back", buttonData=-20)
                markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

                self.send_active_message("Here are all your recent encounters\nSelect a user to view more options", markup=markup)
            else:
                self.send_error_message("No encounters so far :)")
        else:
            if message.text == "1":
                if self.userBalance['secondChances'] > 0:
                    requests.get(f"https://localhost:44381/ActivateToggleEffect/{self.current_user}/{5}/{self.current_managed_user}", verify=False)
                    self.send_error_message("Done :)")
                    self.userBalance['secondChances'] = int(self.userBalance['secondChances'] - 1)
                else:
                    self.send_error_message("You dont have that effect :(")

            elif message.text == "2":
                ReportModule(self.bot, message, self.current_managed_user, self.proceed_with_encounters, dontAddToBlackList=True)
            elif message.text == "3":
                if not self.isInBlackList:
                    self.add_to_black_list(message)
                else:
                    self.send_secondary_message("Are you sure, you want to delete that user from your black list?", markup=self.YNMarkup)
                    self.bot.register_next_step_handler(message, self.black_list_management, acceptMode=True, isEncounter=True, chat_id=self.current_user)
            elif message.text == "4":
                self.proceed()

    def add_to_black_list(self, message, acceptMode=False):
        self.active_section = self.add_to_black_list
        if not acceptMode:
            self.send_secondary_message("Are you sure, you want to add that user to your black list?", markup=self.YNMarkup)
            self.bot.register_next_step_handler(message, self.add_to_black_list, acceptMode=True, chat_id=self.current_user)
        else:
            self.bot.delete_message(self.current_user, message.id)

            if message.text == "yes":
                if bool(json.loads(requests.get(
                        f"https://localhost:44381/AddUserToBlackList/{self.current_user}/{self.current_managed_user}",
                        verify=False).text)):
                    self.send_secondary_message("User have been successfully added to you black list")
                    self.proceed_with_encounters()
                else:
                    self.send_secondary_message("User was not recognised. His account had probably been already deleted :)")
                    self.proceed_with_encounters()
            elif message.text == "no":
                self.proceed_with_encounters()
            elif message.text == "abort":
                self.proceed()
            else:
                self.send_error_message("No such option", markup=self.abortMarkup)
                self.bot.register_next_step_handler(message, self.add_to_black_list, acceptMode=acceptMode, chat_id=self.current_user)

    def black_list_management(self, message, acceptMode=False, isEncounter=False):
        self.previous_section = self.my_profile_settings_choice
        self.isEncounter = isEncounter
        self.active_section = self.black_list_management
        self.notInMenu = True

        if not acceptMode:
            self.black_list = {}
            self.markup_page = 1

            users = json.loads(requests.get(f"https://localhost:44381/GetBlackList/{self.current_user}", verify=False).text)
            if users:
                for user in users:
                    self.black_list[user["bannedUser"]["id"]] = user["bannedUser"]["userRealName"].lower().strip()

                # self.current_callback_handler = self.bot.register_callback_query_handler("", self.black_list_callback_handler, user_id=self.current_user)
                self.subscribe_callback_handler(self.black_list_callback_handler)

                reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page,
                            self.markup_pages_count)
                self.markup_pages_count = count_pages(self.black_list, self.current_markup_elements, self.markup_pages_count, additionalButton=True, buttonText="Go Back", buttonData=-20)
                markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

                self.send_active_message("Here are all users you have in your black list\nSelect a user to remove him from the black list", markup=markup)
            else:
                self.send_error_message("There are no users in your blacklist :)")
        else:
            self.bot.delete_message(self.current_user, message.id)

            if message.text == "yes":
                if bool(json.loads(requests.delete(f"https://localhost:44381/RemoveUserFromBlackList/{self.current_user}/{self.current_managed_user}", verify=False).text)):
                    self.bot.send_message(self.current_user, "User have been successfully removed from you black list")
                    if not isEncounter:
                        self.black_list_management(message)
                        self.isEncounter = None
                    else:
                        self.proceed_with_encounters()
                        self.isEncounter = None
                else:
                    self.bot.send_message(self.current_user, "User was not recognised. His account had probably been already deleted :)")
                    if not isEncounter:
                        self.black_list_management(message)
                        self.isEncounter = None
                    else:
                        self.proceed_with_encounters()
                        self.isEncounter = None
            elif message.text == "no":
                if not isEncounter:
                    self.black_list_management(message)
                    self.isEncounter = None
                else:
                    self.proceed_with_encounters()
                    self.isEncounter = None
            elif message.text == "abort":
                self.proceed()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.abortMarkup)
                self.bot.register_next_step_handler(message, self.black_list_management, acceptMode=acceptMode, isEncounter=isEncounter, chat_id=self.current_user)

    def ocean_switch(self, message, acceptMode=False):
        self.previous_section = self.ocean_settings_choice
        self.active_section = self.ocean_switch

        if not acceptMode:
            status = ""
            switchMessage = ""

            if self.uses_ocean:
                status = "Online"
                switchMessage = "Would you like to turn it off?"
            else:
                status = "Offline"
                switchMessage = "Would you like to turn it on?"

            self.send_secondary_message(f"{self.ocean_description}OCEAN+ is currently {status}\n{switchMessage}", markup=self.YNMarkup)
            self.bot.register_next_step_handler(message, self.ocean_switch, acceptMode=True, chat_id=self.current_user)

        else:
            self.bot.delete_message(self.current_user, message.id)
            if message.text == "yes":
                # try:
                Helpers.switch_ocean_status(self.current_user)
                self.uses_ocean = not self.uses_ocean
                self.send_secondary_message("Done :)")
                # except:
                #     self.send_error_message("Something went wrong. Please, contact the administration")

                self.proceed()
            elif message.text == "no":
                self.proceed()
            else:
                self.send_error_message("No such option", markup=self.YNMarkup)
                self.bot.register_next_step_handler(message, self.ocean_switch, acceptMode=acceptMode, chat_id=self.current_user)

    def set_profile_status(self, message, acceptMode=False):
        self.previous_section = self.my_profile_settings_choice
        self.active_section = self.set_profile_status

        if not acceptMode:
            if self.has_Premium:
                self.send_secondary_message(f"Please, send me your new status (up to 50 characters)", markup=self.abortMarkup)
                self.bot.register_next_step_handler(message, self.set_profile_status, acceptMode=True, chat_id=self.current_user)
            else:
                self.send_secondary_message(f"This action is available only for users with premium", markup=self.YNMarkup)
        else:
            self.bot.delete_message(self.current_user, message.id)

            if 50 > len(message.text) > 0:
                if message.text == "Go Back":
                    self.proceed()
                    return
                elif Helpers.update_user_status(self.current_user, message.text):
                    self.send_error_message(f"Done :)")
                else:
                    self.send_error_message(f"Something went wrong. Please contact the administration")
                self.proceed()
            else:
                self.send_error_message(f"Incorrect status length", markup=self.abortMarkup)
                self.bot.register_next_step_handler(message, self.set_profile_status, acceptMode=acceptMode, chat_id=self.current_user)

    def black_list_callback_handler(self, call):
        if call.message.id not in self.old_queries:
            self.current_query = call.message.id

            if call.data == "-1" or call.data == "-2":
                index = index_converter(call.data)
                if self.markup_page + index <= self.markup_pages_count or self.markup_page + index >= 1:
                    markup = assemble_markup(self.markup_page, self.current_markup_elements, index)
                    self.bot.edit_message_reply_markup(chat_id=call.message.chat.id, reply_markup=markup,
                                                       message_id=call.message.id)
                    self.markup_page += index

            elif call.data == "-20":
                self.proceed()

            else:
                try:
                    self.current_managed_user = int(call.data)
                    self.send_secondary_message("Are you sure, you want to delete that user from your black list?", markup=self.YNMarkup)
                    self.bot.register_next_step_handler(call.message, self.black_list_management, acceptMode=True, chat_id=self.current_user)
                except:
                    self.send_secondary_message("Something went wrong, please contact the administration")
                    # self.proceed()

    def choose_confirmation_request(self, message):
        self.previous_section = self.additional_actions_settings_choice

        if self.verificationOnly:
            self.previous_section = self.destruct

        self.active_section = self.choose_confirmation_request
        self.subscribe_callback_handler(self.menu_callback_handler)

        self.send_active_message("Please choose type of identity confirmation", markup=self.settingConfirmationRequestMarkup)

    def send_confirmation_request(self, message, acceptMode=False):
        # self.previous_section = self.choose_confirmation_request
        # self.active_section = self.send_confirmation_request

        if not acceptMode:
            self.gesture = random.choice(self.gestures)
            self.gesture += " " + random.choice(self.gestures)

            action = f"sending us a Video or 'Circle' (15 seconds max), in which you are showing those two gestures one after another: {self.gesture} .\n!Your face has to be visible!\n\n<b><i>Warning! Your profile media has to contain your face</i></b>"
            if self.requestType == 2:
                action = "Sending us your passport photo\n\n<b><i>Warning! Your profile media has to contain your face</i></b>"

            if not self.requestStatus:
                self.send_secondary_message(f"You can confirm your identity by {action}")

            else:
                self.send_secondary_message(f"You can update current request by {action}")

            self.nextHandler = self.bot.register_next_step_handler(message, self.send_confirmation_request, acceptMode=True, chat_id=self.current_user)
        else:
            try:
                self.bot.delete_message(self.current_user, message.id)
            except:
                pass

            data = {
                "userId": self.current_user,
                "type": self.requestType,
                "gesture": self.gesture
            }

            if self.requestType == 1:
                if message.video:
                    if message.video.duration > 15:
                        self.send_secondary_message("Video is too long")
                        self.bot.register_next_step_handler(message, self.send_confirmation_request, acceptMode=acceptMode, chat_id=self.current_user)
                        return

                    data["video"] = message.video[len(message.video) - 1].file_id
                    d = json.dumps(data)
                    if bool(json.loads(requests.post(f"https://localhost:44381/SendTickRequest", d, headers={"Content-Type": "application/json"}, verify=False).text)):
                        self.send_message_with_confirmation("Done! Please wait until administration resolves your request.\nIf you have any questions, please contact @Admin")
                        return

                elif message.video_note:
                    if message.video_note.duration > 15:
                        self.send_error_message(self.current_user, "Video is too long")
                        self.bot.register_next_step_handler(message, self.send_confirmation_request, acceptMode=acceptMode, chat_id=self.current_user)
                        return

                    data["circle"] = message.video_note.file_id
                    d = json.dumps(data)
                    if bool(json.loads(requests.post(f"https://localhost:44381/SendTickRequest", d, headers={"Content-Type": "application/json"}, verify=False).text)):
                        self.send_message_with_confirmation("Done! Please wait until administration resolves your request.\nIf you have any questions, please contact @Admin")
                        return

            elif self.requestType == 2:
                if message.photo:
                    data["photo"] = message.photo[len(message.photo) - 1].file_id
                    d = json.dumps(data)

                    if bool(json.loads(requests.post(f"https://localhost:44381/SendTickRequest", d, headers={"Content-Type": "application/json"}, verify=False).text)):
                        self.send_message_with_confirmation("Done! Please wait until administration resolves your request.\nIf you have any questions, please contact @Admin")
                        return

            self.send_error_message("This type of data cannot be accepted as your identity confirmation")
            self.bot.register_next_step_handler(message, self.send_confirmation_request, acceptMode=acceptMode, chat_id=self.current_user)

    def encounters_callback_handler(self, call):
        if call.message.id not in self.old_queries:
            self.current_query = call.message.id

            if call.data == "-1" or call.data == "-2":
                index = index_converter(call.data)
                if self.markup_page + index <= self.markup_pages_count or self.markup_page + index >= 1:
                    markup = assemble_markup(self.markup_page, self.current_markup_elements, index)
                    self.bot.edit_message_reply_markup(chat_id=call.message.chat.id, reply_markup=markup,
                                                       message_id=call.message.id)
                    self.markup_page += index

            elif call.data == "-20":
                self.delete_active_message()
                self.delete_error_message()
                self.delete_secondary_message()
                self.proceed()

            elif call.data == "100":
                if self.userBalance['secondChances'] > 0:
                    requests.get(f"https://localhost:44381/ActivateToggleEffect/{self.current_user}/{5}/{self.current_managed_user}", verify=False)
                    self.send_error_message("Done :)")
                    self.userBalance['secondChances'] = int(self.userBalance['secondChances'] - 1)
                else:
                    self.send_error_message("You dont have that effect :(")

            elif call.data == "101":
                ReportModule(self.bot, self.message, self.current_managed_user, self.proceed_with_encounters, dontAddToBlackList=True)
            elif call.data == "102":
                if not self.isInBlackList:
                    self.add_to_black_list(self.message)
                else:
                    self.send_secondary_message("Are you sure, you want to delete that user from your black list?", markup=self.YNMarkup)
                    self.bot.register_next_step_handler(self.message, self.black_list_management, acceptMode=True, isEncounter=True, chat_id=self.current_user)
            else:
                try:
                    self.current_managed_user = int(call.data)
                    self.proceed_with_encounters()
                except:
                    #TODO: probably delete encounter from db afterwards (If it is not deleted automatically)
                    self.send_error_message("Unable to load user data. His account may already be banned or deleted")

    def effects_callback_handler(self, call):
        self.bot.answer_callback_query(call.id, "")
        if call.message.id not in self.old_queries:

            if call.data == "5":
                self.effect_index = call.data
                if self.secondChances > 0:
                    self.send_secondary_message(self.secondChanceDescription)
                else:
                    self.send_secondary_message(self.secondChanceDescription, markup=self.buy_effectMarkup)
            elif call.data == "6":
                self.effect_index = call.data
                if self.uses_ocean:
                    if self.valentines > 0:
                        self.usedEffectAmount = self.valentines
                        #Warn user if effect is already active
                        if bool(json.loads(requests.get(f"https://localhost:44381/CheckEffectIsActive/{self.current_user}/{call.data}", verify=False).text)):
                            self.send_secondary_message(f"<i>The Valentine:</i> {self.valentineDescription}\n\n{self.effect_is_active_Warning}", markup=self.activate_effectMarkup)
                        else:
                            self.send_secondary_message(self.valentineDescription, markup=self.activate_effectMarkup)
                    else:
                        self.send_secondary_message(self.valentineDescription, markup=self.buy_effectMarkup)
                else:
                    self.send_secondary_message("You have to turn on OCEAN+ to use this effect")
            elif call.data == "7":
                self.effect_index = call.data
                if self.uses_ocean:
                    if self.detectors > 0:
                        self.usedEffectAmount = self.detectors
                        if bool(json.loads(requests.get(f"https://localhost:44381/CheckEffectIsActive/{self.current_user}/{call.data}", verify=False).text)):
                            self.send_secondary_message(f"<i>The Detector</i>{self.detectorDescription}\n\n{self.effect_is_active_Warning}", markup=self.activate_effectMarkup)
                        else:
                            self.send_secondary_message(self.detectorDescription, markup=self.activate_effectMarkup)
                    else:
                        self.send_secondary_message(self.detectorDescription, markup=self.buy_effectMarkup)
                else:
                    self.send_secondary_message("You have to turn on OCEAN+ to use this effect")
            elif call.data == "8":
                self.effect_index = call.data
                if self.nullifiers > 0:
                    self.send_secondary_message(self.nullifierDescription)
                else:
                    self.send_secondary_message(self.nullifierDescription, markup=self.buy_effectMarkup)
            elif call.data == "9":
                self.effect_index = call.data
                if self.cardDecksMini > 0:
                    self.usedEffectAmount = self.cardDecksMini
                    self.send_secondary_message(self.cardDeckMiniDescription, markup=self.activate_effectMarkup)
                else:
                    self.send_secondary_message(self.cardDeckMiniDescription, markup=self.buy_effectMarkup)
            elif call.data == "10":
                self.effect_index = call.data
                if self.cardDecksPlatinum > 0:
                    self.usedEffectAmount = self.cardDecksPlatinum
                    self.send_secondary_message(self.cardDeckPlatinumDescription, markup=self.activate_effectMarkup)
                else:
                    self.send_secondary_message(self.cardDeckPlatinumDescription, markup=self.buy_effectMarkup)

            elif call.data == "-10":
                self.use_effect_manager(self.effect_index)

            elif call.data == "-5":
                self.buy_effect_manager()

            elif call.data == "-20":
                self.proceed()

    def use_effect_manager(self, effectId):
        text = "Activated!"
        if effectId == "6" or effectId == "7":
            if effectId == "6":
                response = requests.get(f"https://localhost:44381/ActivateDurableEffect/{self.current_user}/{effectId}", verify=False).text

                if response:
                    self.valentines -= 1
                    self.valentine_indicator.text = self.valentines
                    self.send_secondary_message(f"<i>The Valentine:</i> {self.valentineDescription}\n\n{self.effect_is_active_Warning}", markup=self.activate_effectMarkup)
                else:
                    text = "You did not spend any OCEAN+ points yet"

            elif effectId == "7":
                self.detectors -= 1
                self.detector_indicator.text = self.detectors
                self.send_secondary_message(f"<i>The Detector</i>{self.detectorDescription}\n\n{self.effect_is_active_Warning}", markup=self.activate_effectMarkup)

        else:
            response = requests.get(f"https://localhost:44381/ActivateToggleEffect/{self.current_user}/{effectId}", verify=False).text
            if effectId == "9":
                self.cardDecksMini -= 1
                self.cardDeckMini_indicator.text = self.cardDecksMini
            elif effectId == "10":
                self.cardDecksPlatinum -= 1
                self.cardDeckPlatinum_indicator.text = self.cardDecksPlatinum

        self.update_effects_markup()

        if self.usedEffectAmount == 0:
            self.edit_secondary_message_markup(self.buy_effectMarkup)
        self.send_error_message(text)

    def buy_effect_manager(self):
        self.delete_secondary_message()
        self.delete_error_message()
        hasVisited = Helpers.check_user_has_visited_section(self.current_user, 10)
        Shop(self.bot, self.message, hasVisited, startingTransaction=2, returnMethod=self.proceed, active_message=self.active_message)

    def menu_callback_handler(self, call):
        self.bot.answer_callback_query(call.id, "")
        if not self.isDeciding:
            if call.data == "200":
                self.my_profile_settings_choice(call.message)
            elif call.data == "201":
                self.ocean_settings_choice(call.message)
            elif call.data == "202":
                self.filters_settings_choice()
            elif call.data == "203":
                self.my_statistics_settings_choice(call.message)
            elif call.data == "204":
                self.additional_actions_settings_choice(call.message)
            elif call.data == "205":
                self.ocean_switch(call.message)
            elif call.data == "206":
                self.ocean_points(call.message)
            elif call.data == "207":
                self.previous_section = self.ocean_settings_choice
                self.notInMenu = True
                self.clear_callback_handler()
                self.delete_secondary_message()
                self.delete_error_message()
                TestModule(self.bot, self.message, returnMethod=self.proceed, active_message=self.active_message)
            elif call.data == "208":
                pass
            elif call.data == "209":
                self.auto_reply_manager(call.message)
            elif call.data == "210":
                self.language_consideration_manager(call.message)
            elif call.data == "310":
                self.language_consideration_manager(call.message, True)
            elif call.data == "211":
                self.free_status_manager()
            elif call.data == "311":
                self.free_status_manager(True)
            elif call.data == "212":
                self.real_photo_filter_manager(call.message)
            elif call.data == "312":
                self.real_photo_filter_manager(call.message, True)
            elif call.data == "213":
                self.achievement_manager()
            elif call.data == "214":
                self.effects_manager(call.message)
            elif call.data == "215":
                self.notInMenu = True
                self.previous_section = self.my_statistics_settings_choice
                hasVisited = Helpers.check_user_has_visited_section(self.current_user, 10)
                Shop(self.bot, self.message, hasVisited, startingTransaction=1, returnMethod=self.proceed, active_message=self.active_message)
            elif call.data == "216":
                self.notInMenu = True
                self.previous_section = self.my_statistics_settings_choice
                hasVisited = Helpers.check_user_has_visited_section(self.current_user, 10)
                Shop(self.bot, self.message, hasVisited, startingTransaction=4, returnMethod=self.proceed, active_message=self.active_message)
            elif call.data == "217":
                self.notInMenu = True
                self.previous_section = self.my_statistics_settings_choice
                hasVisited = Helpers.check_user_has_visited_section(self.current_user, 10)
                Shop(self.bot, self.message, hasVisited, startingTransaction=3, returnMethod=self.proceed, active_message=self.active_message)
            elif call.data == "218":
                self.show_active_users_effects()
            elif call.data == "219":
                self.show_users_effects(call.message)
            elif call.data == "220":
                self.credentials_management()
            elif call.data == "221":
                self.increased_familiarity_switch()
            elif call.data == "321":
                self.increased_familiarity_switch(True)
            elif call.data == "222":
                self.choose_confirmation_request(call.message)
            elif call.data == "223":
                self.show_creds_link()
            elif call.data == "224":
                self.show_creds_qrcode()
            elif call.data == "225":
                self.black_list_management(call.message)
            elif call.data == "226":
                self.encounter_list_management(call.message)
            elif call.data == "227":
                #TODO: Probably change it to work the same way as the other sections of Settings
                self.notInMenu = True
                self.delete_active_message()
                self.clear_callback_handler()
                self.previous_section = self.my_profile_settings_choice
                Registrator(self.bot, self.message, True, self.proceed, self.user_language)
            elif call.data == "228":
                self.set_profile_status(call.message)
            elif call.data == "229":
                self.auto_reply_manager(call.message)
            elif call.data == "230":
                self.currency_change_manager(call.message)
            elif call.data == "240":
                if self.requestType != 1:
                    self.delete_secondary_message()
                    self.requestType = 1

                    self.remove_next_step_handler_local()

                    self.send_confirmation_request(call.message)
            elif call.data == "241":
                if self.requestType != 2:
                    self.delete_secondary_message()
                    self.requestType = 2

                    self.remove_next_step_handler_local()

                    self.send_confirmation_request(call.message)
            elif call.data == "242":
                self.hints_status_manager()
            elif call.data == "250":
                self.delete_profile(call.message)
            elif call.data == "342":
                self.hints_status_manager(True)
            elif call.data == "243":
                self.comment_status_manager()
            elif call.data == "343":
                self.comment_status_manager(True)
            elif call.data == "340":
                self.requestType = 0
                self.send_secondary_message("Partial identity confirmation:\n✅ Face confirmation\n⛔ Age confirmation\n⛔ Location confirmation")
            elif call.data == "341":
                self.requestType = 0
                self.send_secondary_message("Full identity confirmation:\n✅ Face confirmation\n✅ Age confirmation\n✅ Location confirmation")
            #TODO: Continue. Code below must be the last statement before 'else'
            elif call.data == "-20":
                self.proceed()

    def ocean_callback_handler(self, call):
        self.bot.answer_callback_query(call.id, "")
        if call.message.id not in self.old_queries:

            if call.data == "1":
                if self.ocean_caps["canO"]:
                    if self.user_free_points > 0:
                        self.user_free_points -= 1
                        self.user_points["openness"] += 1
                        self.free_points_indicator.text = self.user_free_points
                        self.ocean_updated_points["openness"] = self.user_points["openness"]

                        self.o_indicator.text = f"{self.user_points['openness']}"
                        self.update_ocean_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any OCEAN+ points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "-1":
                if self.ocean_caps["canO"]:
                    if self.user_points["openness"] > 0:
                        self.user_free_points += 1
                        self.user_points["openness"] -= 1
                        self.free_points_indicator.text = self.user_free_points
                        self.ocean_updated_points["openness"] = self.user_points["openness"]

                        self.o_indicator.text = f"{self.user_points['openness']}"
                        self.update_ocean_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any OCEAN+ points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "2":
                if self.ocean_caps["canC"]:
                    if self.user_free_points > 0:
                        self.user_free_points -= 1
                        self.user_points["conscientiousness"] += 1
                        self.free_points_indicator.text = self.user_free_points
                        self.ocean_updated_points["conscientiousness"] = self.user_points["conscientiousness"]

                        self.c_indicator.text = self.user_points["conscientiousness"]
                        self.update_ocean_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any OCEAN+ points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "-2":
                if self.ocean_caps["canC"]:
                    if self.user_points["conscientiousness"] > 0:
                        self.user_free_points += 1
                        self.user_points["conscientiousness"] -= 1
                        self.free_points_indicator.text = self.user_free_points
                        self.ocean_updated_points["conscientiousness"] = self.user_points["conscientiousness"]

                        self.c_indicator.text = self.user_points["conscientiousness"]
                        self.update_ocean_markup()
                        return False
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "3":
                if self.ocean_caps["canE"]:
                    if self.user_free_points > 0:
                        self.user_free_points -= 1
                        self.user_points["extroversion"] += 1
                        self.free_points_indicator.text = self.user_free_points
                        self.ocean_updated_points["extroversion"] = self.user_points["extroversion"]

                        self.e_indicator.text = self.user_points["extroversion"]
                        self.update_ocean_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any OCEAN+ points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "-3":
                if self.ocean_caps["canE"]:
                    if self.user_points["extroversion"] > 0:
                        self.user_free_points += 1
                        self.user_points["extroversion"] -= 1
                        self.free_points_indicator.text = self.user_free_points
                        self.ocean_updated_points["extroversion"] = self.user_points["extroversion"]

                        self.e_indicator.text = self.user_points["extroversion"]
                        self.update_ocean_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any OCEAN+ points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "4":
                if self.ocean_caps["canA"]:
                    if self.user_free_points > 0:
                        self.user_free_points -= 1
                        self.user_points["agreeableness"] += 1
                        self.free_points_indicator.text = self.user_free_points
                        self.ocean_updated_points["agreeableness"] = self.user_points["agreeableness"]

                        self.a_indicator.text = self.user_points["agreeableness"]
                        self.update_ocean_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any OCEAN+ points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "-4":
                if self.ocean_caps["canA"]:
                    if self.user_points["agreeableness"] > 0:
                        self.user_free_points += 1
                        self.user_points["agreeableness"] -= 1
                        self.free_points_indicator.text = self.user_free_points
                        self.ocean_updated_points["agreeableness"] = self.user_points["agreeableness"]

                        self.a_indicator.text = self.user_points["agreeableness"]
                        self.update_ocean_markup()
                        return False
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "5":
                if self.ocean_caps["canN"]:
                    if self.user_free_points > 0:
                        self.user_free_points -= 1
                        self.user_points["neuroticism"] += 1
                        self.free_points_indicator.text = self.user_free_points
                        self.ocean_updated_points["neuroticism"] = self.user_points["neuroticism"]

                        self.n_indicator.text = self.user_points["neuroticism"]
                        self.update_ocean_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any OCEAN+ points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "-5":
                if self.ocean_caps["canN"]:
                    if self.user_points["neuroticism"] > 0:
                        self.user_free_points += 1
                        self.user_points["neuroticism"] -= 1
                        self.free_points_indicator.text = self.user_free_points
                        self.ocean_updated_points["neuroticism"] = self.user_points["neuroticism"]

                        self.n_indicator.text = self.user_points["neuroticism"]
                        self.update_ocean_markup()
                        return False
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "6":
                if self.ocean_caps["canP"]:
                    if self.user_free_points > 0:
                        self.user_free_points -= 1
                        self.user_points["nature"] += 1
                        self.free_points_indicator.text = self.user_free_points
                        self.ocean_updated_points["nature"] = self.user_points["nature"]

                        self.p_indicator.text = self.user_points["nature"]
                        self.update_ocean_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any OCEAN+ points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "-6":
                if self.ocean_caps["canP"]:
                    if self.user_points["nature"]:
                        self.user_free_points += 1
                        self.user_points["nature"] -= 1
                        self.free_points_indicator.text = self.user_free_points
                        self.ocean_updated_points["nature"] = self.user_points["nature"]

                        self.p_indicator.text = self.user_points["nature"]
                        self.update_ocean_markup()
                        return False
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "7":
                self.ocean_points_save()

    def achievement_callback_handler(self, call):
        if call.message.id not in self.old_queries:
            self.current_query = call.message.id

            if call.data == "-1" or call.data == "-2":
                index = index_converter(call.data)
                if self.markup_page + index <= self.markup_pages_count or self.markup_page + index >= 1:
                    markup = assemble_markup(self.markup_page, self.current_markup_elements, index)
                    self.bot.edit_message_reply_markup(chat_id=call.message.chat.id, reply_markup=markup,
                                                       message_id=call.message.id)
                    self.markup_page += index

            else:
                try:
                    if self.active_message:
                        self.bot.edit_message_text(self.construct_achievement_message(call.data), self.current_user, self.active_message,
                                                   reply_markup=self.abortMarkup)
                        return

                    self.active_message = self.bot.send_message(self.current_user, self.construct_achievement_message(call.data), reply_markup=self.abortMarkup).id
                    self.bot.send_message(self.current_user, "Type 'Go Back' to return to the previous section")
                except:
                    self.bot.send_message(self.current_user, "Unable to load achievement data, please, contact the administration")
                    self.proceed()

    def update_ocean_markup(self):
        try:
            self.bot.edit_message_reply_markup(chat_id=self.current_user, reply_markup=self.oceanMarkup, message_id=self.active_message)
        except:
            pass

    def update_effects_markup(self):
        try:
            self.bot.edit_message_reply_markup(chat_id=self.current_user, reply_markup=self.effects_markup, message_id=self.active_message)
        except:
            pass

    def subscribe_callback_handler(self, handler):
        self.clear_callback_handler()

        self.current_callback_handler = self.bot.register_callback_query_handler(self.message, handler, user_id=self.current_user)

    def clear_callback_handler(self):
        if self.current_callback_handler:
            self.bot.callback_query_handlers.remove(self.current_callback_handler)
            self.current_callback_handler = None

    def proceed(self, msg=None, **kwargs):

        self.requestType = 0
        self.isDeciding = False

        self.remove_next_step_handler_local()

        #Return menu handler if it was replaced by another one
        if self.notInMenu:
            self.notInMenu = False
            self.subscribe_callback_handler(self.menu_callback_handler)
        self.delete_secondary_message()

        if kwargs.get("backFromShop"):
            self.userBalance = json.loads(requests.get(f"https://localhost:44381/user-balance/{self.current_user}", verify=False).text)

        self.previous_section(self.message)

    def proceed_with_encounters(self):
        self.isInBlackList = Helpers.check_user_in_a_blacklist(self.current_user, self.current_managed_user)

        if self.isInBlackList:
            self.black_listButton.text = self.remove_from_blacklist_text
        else:
            self.black_listButton.text = self.add_to_blacklist_text

        user = Helpers.get_user_info(self.current_managed_user)

        self.delete_active_message()
        self.send_active_message_with_photo(f"{user['userBaseInfo']['userDescription']}\n\n<b><i>Please, choose an option from a list below</i></b>", self.encounterOptionMarkup, user["userBaseInfo"]["userMedia"])
        # self.bot.send_photo(self.current_user, user["userBaseInfo"]["userMedia"], user["userBaseInfo"]["userDescription"], reply_markup=self.encounterOptionMarkup)
        self.bot.register_next_step_handler(self.message, self.encounter_list_management, acceptMode=True, chat_id=self.current_user)

    def construct_achievement_message(self, achievementId) -> str:
        achievement = self.achievements_data[achievementId]
        name = achievement["achievement"]["name"]

        if achievement["isAcquired"]:
            name = "✅" + name + "✅"

        return f"{name}\n\n{achievement['achievement']['description']}\nProgress: {achievement['progress']} / {['achievement']['condition']}\n\nReward:{achievement['value']} Coins"

    def construct_effects_markup(self, balance):
        self.secondChances = balance["secondChances"]
        self.valentines = balance["valentines"]
        self.detectors = balance["detectors"]
        self.nullifiers = balance["nullifiers"]
        self.cardDecksMini = balance["cardDecksMini"]
        self.cardDecksPlatinum = balance["cardDecksPlatinum"]

        self.secondChance_indicator = InlineKeyboardButton(self.secondChances, callback_data="0")
        self.valentine_indicator = InlineKeyboardButton(self.valentines, callback_data="0")
        self.detector_indicator = InlineKeyboardButton(self.detectors, callback_data="0")
        self.nullifier_indicator = InlineKeyboardButton(self.nullifiers, callback_data="0")
        self.cardDeckMini_indicator = InlineKeyboardButton(self.cardDecksMini, callback_data="0")
        self.cardDeckPlatinum_indicator = InlineKeyboardButton(self.cardDecksPlatinum, callback_data="0")

        self.effects_markup = InlineKeyboardMarkup() \
            .add(InlineKeyboardButton("Second Chance", callback_data="5"), self.secondChance_indicator) \
            .add(InlineKeyboardButton("Valentine", callback_data="6"), self.valentine_indicator) \
            .add(InlineKeyboardButton("Detector", callback_data="7"), self.detector_indicator) \
            .add(InlineKeyboardButton("Nullifier", callback_data="8"), self.nullifier_indicator) \
            .add(InlineKeyboardButton("Card Deck Mini", callback_data="9"), self.cardDeckMini_indicator) \
            .add(InlineKeyboardButton("Card Deck Platinum", callback_data="10"), self.cardDeckPlatinum_indicator) \
            .add(InlineKeyboardButton("Go Back", callback_data="-20")) \


    def help_handler(self, message):
        self.bot.delete_message(self.current_user, message.id)
        Helper(self.bot, message, self.active_section, activeMessageId=self.active_message, secondaryMessageId=self.active_secondary_message, isEncounter=self.isEncounter)

    def construct_active_effects_message(self, effects):
        msg = ""

        if effects:
            for effect in effects:
                msg += f"{effect['name']}\nExpires at: {effect['expirationTime']}\n\n"

                return msg

        return "No active effects"

    def construct_user_inventory_message(self, balance):
        msg = ""

        if balance:
            return f"💎Coins: {balance['points']}\n💎Ocean Points: {balance['oceanPoints']}\n💥Second Chances{balance['secondChances']}\n💥Valentines: {balance['valentines']}\n💥Detectors: {balance['detectors']}\n💥Nullifiers: {balance['nullifiers']}\n💥Card Decks Mini: {balance['cardDecksMini']}\n💥Card Decks Platinum: {balance['cardDecksPlatinum']}"

        return "Something went wrong"

    def switch_toggle_filter(self, indicator):
        if indicator.text == self.turnedOnSticker:
            indicator.text = self.turnedOffSticker
        else:
            indicator.text = self.turnedOnSticker

    def send_active_message(self, text, markup=None):
        try:
            if self.active_message:
                self.bot.edit_message_text(text, self.current_user, self.active_message, reply_markup=markup)
                return
            self.active_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except:
            self.delete_active_message()
            self.send_active_message(text, markup)

    def send_active_message_with_photo(self, text, markup, photo):
        if self.active_message:
            self.bot.edit_message_media(media=photo, chat_id=self.current_user, message_id=self.active_message, reply_markup=markup)
            return

        self.active_message = self.bot.send_photo(self.current_user, photo=photo, caption=text, reply_markup=markup).id

    def send_active_message_with_video(self, text, markup, video):
        if self.active_message:
            self.bot.edit_message_media(media=video, chat_id=self.current_user, message_id=self.active_message, reply_markup=markup)
            return

        self.active_message = self.bot.send_video(self.current_user, video=video, caption=text, reply_markup=markup).id

    def send_message_with_confirmation(self, text):
        msgId = self.bot.send_message(self.current_user, text, reply_markup=self.okMarkup).id
        self.bot.register_next_step_handler(self.message, self.delete_message_with_confirmation, messageToDelete=msgId, chat_id=self.current_user)

    def delete_message_with_confirmation(self, message, messageToDelete):
        try:
            self.bot.delete_message(self.current_user, messageToDelete)
            self.bot.delete_message(self.current_user, message.id)
        except:
            pass
        self.proceed()

    def edit_active_message_markup(self, markup):
        self.bot.edit_message_reply_markup(self.current_user, self.active_message, reply_markup=markup)

    def edit_secondary_message_markup(self, markup):
        self.bot.edit_message_reply_markup(self.current_user, self.active_secondary_message, reply_markup=markup)

    def send_secondary_message(self, text, markup=None, voice=None):
        try:
            if self.active_secondary_message:
                self.bot.edit_message_text(text, self.current_user, self.active_secondary_message, reply_markup=markup)
                return

            if not voice:
                self.active_secondary_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
                return
        except:
            self.bot.delete_message(self.current_user, self.active_secondary_message)
            self.active_secondary_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
            return

        self.active_secondary_message = self.bot.send_voice(self.current_user, voice=voice, caption=text, reply_markup=markup).id

    def send_error_message(self, text, markup=None, photo=None):
        if self.active_error_message and not photo:
            try:
                self.bot.edit_message_text(text, self.current_user, self.active_error_message, reply_markup=markup)
            except:
                try:
                    self.bot.delete_message(self.current_user, self.active_error_message)
                except:
                    pass

                self.active_error_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
            return

        if not photo:
            self.active_error_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
            return

        if not self.active_error_message:
            self.active_error_message = self.bot.send_photo(self.current_user, photo=photo, reply_markup=markup).id

    def delete_active_message(self):
        if self.active_message:
            self.bot.delete_message(self.current_user, self.active_message)
            self.active_message = None

    def delete_secondary_message(self):
        if self.active_secondary_message:
            self.bot.delete_message(self.current_user, self.active_secondary_message)
            self.active_secondary_message = None

        self.delete_error_message()

    def delete_error_message(self):
        if self.active_error_message:
            self.bot.delete_message(self.current_user, self.active_error_message)
            self.active_error_message = None

    def remove_next_step_handler_local(self):
        if self.nextHandler:
            try:
                self.bot.remove_next_step_handler(self.current_user, self.nextHandler)
                self.nextHandler = None
            except:
                pass

    def destruct(self, msg=None):
        self.clear_callback_handler()
        self.bot.message_handlers.remove(self.helpHandler)

        self.delete_active_message()

        if self.verificationOnly:
            self.return_method(True, True)
            return

        go_back_to_main_menu(self.bot, self.current_user, self.message)
        del self


class CurrencySetter:
    def __init__(self, bot: TeleBot, user_id: any, return_method: any):
        self.bot = bot
        self.return_method = return_method
        self.current_user = user_id

        self.active_message = None

        self.currency_markup = InlineKeyboardMarkup()
            # .add(InlineKeyboardButton("USD", callback_data="12"))\
            # .add(InlineKeyboardButton("EUR", callback_data="13"))\
            # .add(InlineKeyboardButton("UAH", callback_data="14"))\
            # .add(InlineKeyboardButton("RUB", callback_data="15"))\
            # .add(InlineKeyboardButton("CZK", callback_data="16"))\
            # .add(InlineKeyboardButton("PLN", callback_data="17"))\
            # .add(InlineKeyboardButton("Go Back", callback_data="-20"))

        currencies = Helpers.get_payment_currencies()

        for currency in currencies:
            self.currency_markup.add(InlineKeyboardButton(currency["name"], callback_data=f"{currency['id']}"))

        self.currency_markup.add(InlineKeyboardButton("Go Back", callback_data="-20"))

        # I am putting user_id here, just to shut the IDE up.
        # Normally message has to be put there, but, it does not matter, I suppose, (I hope...)
        self.ch = self.bot.register_callback_query_handler(user_id, self.callback_query_handler, user_id=self.current_user)

        self.start()

    def start(self):
        self.send_active_message("Choose the currency you feel most comfortable using to make payments", markup=self.currency_markup)

    def send_active_message(self, text, markup):
        try:
            if self.active_message is not None:
                self.bot.edit_message_text(text, self.current_user, self.active_message, reply_markup=markup)
                return
            self.active_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except:
            self.delete_active_message()
            self.send_active_message(text, markup)

    def delete_active_message(self):
        if self.active_message is not None:
            self.bot.delete_message(self.current_user, self.active_message)
            self.active_message = None

    def callback_query_handler(self, call):
        if call.data == "-20":
            self.destruct()
            return

        Helpers.set_user_currency(self.current_user, call.data)
        self.destruct()

    def destruct(self):
        self.bot.callback_query_handlers.remove(self.ch)
        self.delete_active_message()

        self.return_method(None)
