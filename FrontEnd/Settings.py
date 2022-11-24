import base64

import requests

from Registration import *
from ReportModule import *
from telebot.types import ReplyKeyboardMarkup, KeyboardButton, InlineKeyboardButton, InlineKeyboardMarkup


class Settings:
    def __init__(self, bot, message):
        self.isInBlackList = False
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
        self.p_indicator = None
        self.e_indicator = None
        self.r_indicator = None
        self.s_indicator = None
        self.o_indicator = None
        self.n_indicator = None
        self.a_indicator = None
        self.l_indicator = None
        self.i_indicator = None
        self.t_indicator = None
        self.y_indicator = None
        self.personalityMarkup = None
        self.active_personality_message = 0
        self.personality_caps = {}
        self.personality_updated_points = {}
        self.current_markup_elements = []
        self.markup_page = 1
        self.markup_pages_count = 0
        self.current_callback_handler = None
        self.shouldRestrictTickRequest = False
        Helpers.switch_user_busy_status(self.current_user)
        self.requestStatus = requests.get(f"https://localhost:44381/CheckTickRequestStatus/{self.current_user}", verify=False).text

        self.add_to_blacklist_text = "Add user to a blacklist"
        self.remove_from_blacklist_text = "Remove user from a blacklist"

        #TODO: Add Free search prefs switch (settingsFiltersSettings)
        self.chooseOption_message = "Choose the option:"
        self.setting_message = "1. My Profile\n2. Personality Settings\n3. Filter Settings\n4. My Statistics\n5. Additional Actions\n\n6. Exit"
        self.settingMyProfile_message = f"{self.chooseOption_message}\n1. View the blacklist\n2. Manage recently encountered users\n3. Change profile properties\n4. ⭐Set profile status⭐\n\n5. Go back"
        self.settingPersonalitySettings_message = f"{self.chooseOption_message}\n1. Turn On / Turn Off PERSONALITY\n2. Manage PERSONALITY points\n3. View my tests\n\n4. Go back"
        self.settingFiltersSettings_message = f"{self.chooseOption_message}\n1. Turn On / Turn Off language consideration (Random Conversation)\n2.Change 'Free' status\n3. ⭐Turn on / Turn off filtration by a real photo⭐\n\n4. Go back"
        self.settingStatistics_message = f"{self.chooseOption_message}\n1. View Achievements\n2. 💎Top-Up coin balance💎\n3. 💎Top-Up Personality points balance💎\n4. 💎Buy premium access💎\n\n5. Go back"
        self.settingAdditionalActions_message = f"{self.chooseOption_message}\n1. Get invitation credentials\n"
        self.encounter_options_message = f"1. Use 💥Second chance💥 to send like to a user once again. You have SECOND_CHANCE_COUNT\n2. Report user\n3. Abort\n4." #TODO: replace caps message to a real "second chance" effect amount
        self.settingMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3", "4", "5", "6", "7", "8")
        self.settingMyProfileMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3", "4", "5")
        self.settingPersonalitySettingsMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3", "4")
        self.settingFiltersSettingsMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3")
        self.settingStatisticsMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3", "4", "5")
        self.YNMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("yes", "no")
        self.abortMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("abort")
        self.doneMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("Done")
        self.encounterOptionMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3", "4")
        self.credsMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3")

        #Api return 1 if request was accepted
        if self.requestStatus:
            self.settingAdditionalActions_message += "2. Confirm my identity\n\n3. Go back"
            self.settingAdditionalActionsMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3")
        #TODO: think if confirmation request text should be changed if request exists. That is cool, but not efficient !
        # elif not self.requestStatus != "1":
        #     self.settingAdditionalActions_message += "2. Confirm my identity\n\n3. Go back"
        #     self.settingAdditionalActionsMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3")
        else:
            self.shouldRestrictTickRequest = True
            self.settingAdditionalActions_message += "\n2. Go back"
            self.settingAdditionalActionsMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2")
        self.setting_choice(message)

    def setting_choice(self, message, acceptMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, self.setting_message, reply_markup=self.settingMarkup)
            self.bot.register_next_step_handler(message, self.setting_choice, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                self.my_profile_settings_choice(message)
            elif message.text == "2":
                self.personality_settings_choice(message)
            elif message.text == "3":
                self.filters_settings_choice(message)
            elif message.text == "4":
                self.my_statistics_settings_choice(message)
            elif message.text == "5":
                self.additional_actions_settings_choice(message)
            elif message.text == "6":
                self.destruct()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.settingMarkup)
                self.bot.register_next_step_handler(message, self.setting_choice, acceptMode=acceptMode, chat_id=self.current_user)

    def my_profile_settings_choice(self, message, acceptMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, f"{self.settingMyProfile_message}", reply_markup=self.settingMyProfileMarkup)
            self.bot.register_next_step_handler(message, self.my_profile_settings_choice, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                self.black_list_management(message)
            elif message.text == "2":
                self.encounter_list_management(message)
            elif message.text == "3":
                Registrator(self.bot, self.message, Helpers.check_user_has_visited_section(self.current_user, 1), self.proceed)
            elif message.text == "4":
                self.set_profile_status(message)
            elif message.text == "5":
                self.proceed()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.settingMyProfileMarkup)
                self.bot.register_next_step_handler(message, self.my_profile_settings_choice, acceptMode=acceptMode, chat_id=self.current_user)

    def personality_settings_choice(self, message, acceptMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, f"{self.settingPersonalitySettings_message}", reply_markup=self.settingPersonalitySettingsMarkup)
            self.bot.register_next_step_handler(message, self.personality_settings_choice, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                self.personality_switch(message)
            elif message.text == "2":
                if Helpers.check_user_uses_personality(self.current_user):
                    self.personality_points(message)
                else:
                    self.bot.send_message(self.current_user, "You have to turn on PERSONALITY to do this")
                    self.bot.send_message(self.current_user, f"{self.settingPersonalitySettings_message}", reply_markup=self.settingPersonalitySettingsMarkup)
                    self.bot.register_next_step_handler(message, self.personality_settings_choice, acceptMode=True, chat_id=self.current_user)
            elif message.text == "3":
                TestModule(self.bot, self.message, isActivatedFromShop=False, returnMethod=self.proceed)
            elif message.text == "4":
                self.proceed()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.settingPersonalitySettingsMarkup)
                self.bot.register_next_step_handler(message, self.personality_settings_choice, acceptMode=acceptMode, chat_id=self.current_user)


    def filters_settings_choice(self, message, acceptMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, f"{self.settingFiltersSettings_message}", reply_markup=self.settingFiltersSettingsMarkup)
            self.bot.register_next_step_handler(message, self.filters_settings_choice, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                # TODO: implement
                self.bot.send_message(self.current_user, "Not implemented yet!")
                self.proceed()
            elif message.text == "2":
                # TODO: implement
                self.bot.send_message(self.current_user, "Not implemented yet!")
                self.proceed()
            elif message.text == "3":
                # TODO: implement
                self.bot.send_message(self.current_user, "Not implemented yet!")
                self.proceed()
            elif message.text == "4":
                self.proceed()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.settingFiltersSettingsMarkup)
                self.bot.register_next_step_handler(message, self.filters_settings_choice, acceptMode=acceptMode, chat_id=self.current_user)


    def my_statistics_settings_choice(self, message, acceptMode=False):
        if not acceptMode:
            #TODO: Send a message, containing user coins, PP, effects and so on
            self.bot.send_message(self.current_user, f"{self.settingStatistics_message}", reply_markup=self.settingStatisticsMarkup)
            self.bot.register_next_step_handler(message, self.my_statistics_settings_choice, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                # TODO: implement
                self.bot.send_message(self.current_user, "Not implemented yet!")
                self.proceed()
            elif message.text == "2":
                # TODO: implement
                self.bot.send_message(self.current_user, "Not implemented yet!")
                self.proceed()
            elif message.text == "3":
                # TODO: implement
                self.bot.send_message(self.current_user, "Not implemented yet!")
                self.proceed()
            elif message.text == "4":
                # TODO: implement
                self.bot.send_message(self.current_user, "Not implemented yet!")
                self.proceed()
            elif message.text == "5":
                self.proceed()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.settingStatisticsMarkup)
                self.bot.register_next_step_handler(message, self.my_statistics_settings_choice, acceptMode=acceptMode, chat_id=self.current_user)


    def additional_actions_settings_choice(self, message, acceptMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, f"{self.settingAdditionalActions_message}", reply_markup=self.settingAdditionalActionsMarkup)
            self.bot.register_next_step_handler(message, self.additional_actions_settings_choice, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                self.credentials_management(message)
            elif message.text == "2":
                self.send_confirmation_request(message)
            elif message.text == "3":
                self.proceed()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.settingAdditionalActionsMarkup)
                self.bot.register_next_step_handler(message, self.additional_actions_settings_choice, acceptMode=acceptMode, chat_id=self.current_user)


    def personality_points(self, message, acceptMode=False):
        if not acceptMode:
            self.user_free_points = int(json.loads(requests.get(f"https://localhost:44381/GetUserPersonalityPointsAmount/{self.current_user}", verify=False).text))
            self.user_points = json.loads(requests.get(f"https://localhost:44381/GetUserPersonalityPoints/{self.current_user}", verify=False).text)
            self.personality_caps = json.loads(requests.get(f"https://localhost:44381/GetUserPersonalityCaps/{self.current_user}", verify=False).text)

            self.free_points_indicator = InlineKeyboardButton(self.user_free_points, callback_data="0")
            self.current_callback_handler = self.bot.register_callback_query_handler("", self.personality_callback_handler, user_id=self.current_user)

            if not self.user_points:
                self.bot.send_message(self.current_user, "Something went wrong")
                self.proceed()

            self.p_indicator = InlineKeyboardButton(f"         {self.user_points['personality']}         ", callback_data="0")
            self.e_indicator = InlineKeyboardButton(self.user_points["emotionalIntellect"], callback_data="0")
            self.r_indicator = InlineKeyboardButton(self.user_points["reliability"], callback_data="0")
            self.s_indicator = InlineKeyboardButton(self.user_points["compassion"], callback_data="0")
            self.o_indicator = InlineKeyboardButton(self.user_points["openMindedness"], callback_data="0")
            self.n_indicator = InlineKeyboardButton(self.user_points["agreeableness"], callback_data="0")
            self.a_indicator = InlineKeyboardButton(self.user_points["selfAwareness"], callback_data="0")
            self.l_indicator = InlineKeyboardButton(self.user_points["levelOfSense"], callback_data="0")
            self.i_indicator = InlineKeyboardButton(self.user_points["intellect"], callback_data="0")
            self.t_indicator = InlineKeyboardButton(self.user_points["nature"], callback_data="0")
            self.y_indicator = InlineKeyboardButton(self.user_points["creativity"], callback_data="0")

            self.personalityMarkup = InlineKeyboardMarkup() \
                .add(InlineKeyboardButton("Available points:", callback_data="0"), self.free_points_indicator) \
                .add(InlineKeyboardButton("Personality", callback_data="0"))\
                .add(InlineKeyboardButton("-", callback_data="-1"), self.p_indicator, InlineKeyboardButton("+", callback_data="1")) \
                .add(InlineKeyboardButton("Emotional intellect", callback_data="0")) \
                .add(InlineKeyboardButton("-", callback_data="-2"), self.e_indicator, InlineKeyboardButton("+", callback_data="2")) \
                .add(InlineKeyboardButton("Reliability", callback_data="0")) \
                .add(InlineKeyboardButton("-", callback_data="-3"), self.r_indicator, InlineKeyboardButton("+", callback_data="3")) \
                .add(InlineKeyboardButton("Compassion", callback_data="0")) \
                .add(InlineKeyboardButton("-", callback_data="-4"), self.s_indicator, InlineKeyboardButton("+", callback_data="4")) \
                .add(InlineKeyboardButton("Open-Mindedness", callback_data="0")) \
                .add(InlineKeyboardButton("-", callback_data="-5"), self.o_indicator, InlineKeyboardButton("+", callback_data="5")) \
                .add(InlineKeyboardButton("Agreeableness", callback_data="0")) \
                .add(InlineKeyboardButton("-", callback_data="-6"), self.n_indicator, InlineKeyboardButton("+", callback_data="6")) \
                .add(InlineKeyboardButton("Self-Awareness", callback_data="0")) \
                .add(InlineKeyboardButton("-", callback_data="-7"), self.a_indicator, InlineKeyboardButton("+", callback_data="7")) \
                .add(InlineKeyboardButton("Levels Of Sense", callback_data="0")) \
                .add(InlineKeyboardButton("-", callback_data="-8"), self.l_indicator, InlineKeyboardButton("+", callback_data="8")) \
                .add(InlineKeyboardButton("Intellect", callback_data="0")) \
                .add(InlineKeyboardButton("-", callback_data="-9"), self.i_indicator, InlineKeyboardButton("+", callback_data="9")) \
                .add(InlineKeyboardButton("Nature", callback_data="0")) \
                .add(InlineKeyboardButton("-", callback_data="-10"), self.t_indicator, InlineKeyboardButton("+", callback_data="10")) \
                .add(InlineKeyboardButton("Creativity", callback_data="0")) \
                .add(InlineKeyboardButton("-", callback_data="-11"), self.y_indicator, InlineKeyboardButton("+", callback_data="11"))

            self.bot.send_message(self.current_user, "✨", reply_markup=self.doneMarkup)
            self.active_personality_message = self.bot.send_message(self.current_user, "Points table:", reply_markup=self.personalityMarkup).id
            self.bot.register_next_step_handler(message, self.personality_points_save, chat_id=self.current_user)

    def personality_points_save(self, message):
        #Check if message was not sent by the bot
        if message.from_user.id == self.current_user:
            if message.text == "Done":
                self.personality_updated_points["balance"] = self.user_free_points
                self.personality_updated_points["userId"] = self.current_user

                d = json.dumps(self.personality_updated_points)
                response = requests.post(f"https://localhost:44381/UpdateUserPersonalityPoints", d,
                                                headers={"Content-Type": "application/json"},
                                                verify=False)
                if response.status_code == 200:
                    self.bot.send_message(self.current_user, "Changes are saved successfully :)")
                    self.proceed()
                else:
                    self.bot.send_message(self.current_user, "Something went wrong. Please, contact the administration")
                    self.proceed()
            else:
                self.bot.send_message(self.current_user, "You can leave and apply changes by typing 'Done'", reply_markup=self.doneMarkup)

    def credentials_management(self, message, acceptMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, "1. Get an Invitation link\n2. Get an Invitation QR code\n3. Abort", reply_markup=self.credsMarkup)
            self.bot.register_next_step_handler(message, self.credentials_management, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                link = requests.get(f"https://localhost:44381/GetInvitationLink/{self.current_user}", verify=False).text
                if link:
                    self.bot.send_message(self.current_user, f"Here you go:\n\n{link}")
                    self.credentials_management(message)
                else:
                    self.bot.send_message(self.current_user, "Something went wrong. Please, contact the administration")
                    self.credentials_management(message)
            elif message.text == "2":
                qrcode = requests.get(f"https://localhost:44381/GetQRCode/{self.current_user}", verify=False).text
                if qrcode:
                    self.bot.send_message(self.current_user, f"Here you go:")
                    self.bot.send_photo(self.current_user, base64.b64decode(qrcode))
                    self.credentials_management(message)
                else:
                    self.bot.send_message(self.current_user, "Something went wrong. Please, contact the administration")
                    self.credentials_management(message)
            elif message.text == "3":
                self.proceed()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.credsMarkup)
                self.bot.register_next_step_handler(message, self.credentials_management, acceptMode=acceptMode, chat_id=self.current_user)

    def encounter_list_management(self, message, acceptMode=False):
        if not acceptMode:
            self.encounter_list = {}
            self.markup_page = 1

            encounters = json.loads(requests.get(f"https://localhost:44381/GetUserProfileEncounters/{self.current_user}", verify=False).text)

            if encounters:
                for encounter in encounters:
                    self.encounter_list[encounter["encounteredUserId"]] = encounter["encounteredUser"]["userRealName"]

                self.current_callback_handler = self.bot.register_callback_query_handler("", self.encounters_callback_handler,
                                                                       user_id=self.current_user)

                reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page,
                            self.markup_pages_count)
                self.markup_pages_count = count_pages(self.encounter_list, self.current_markup_elements, self.markup_pages_count)
                markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

                self.bot.send_message(self.current_user, "There are all your recent encounters", reply_markup=markup)
                self.bot.send_message(self.current_user, "Select a user to view more options", reply_markup=self.abortMarkup)
            else:
                self.bot.send_message(self.current_user, "No encounters so far :)")
                self.proceed()
        else:
            if message.text == "1":
                #TODO: Implement when is ready
                self.bot.send_message(self.current_user, "Functionality had not been implemented yet")
                self.proceed_with_encounters()
            elif message.text == "2":
                ReportModule(self.bot, message, self.current_managed_user, self.proceed_with_encounters, dontAddToBlackList=True)
            elif message.text == "3":
                self.proceed()
            elif message.text == "4":
                if not self.isInBlackList:
                    self.add_to_black_list(message)
                else:
                    self.bot.send_message(self.current_user, "Are you sure, you want to delete that user from your black list?", reply_markup=self.YNMarkup)
                    self.bot.register_next_step_handler(message, self.black_list_management, acceptMode=True, isEncounter=True, chat_id=self.current_user)

    def add_to_black_list(self, message, acceptMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, "Are you sure, you want to add that user to your black list?", reply_markup=self.YNMarkup)
            self.bot.register_next_step_handler(message, self.add_to_black_list, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "yes":
                if bool(json.loads(requests.get(
                        f"https://localhost:44381/AddUserToBlackList/{self.current_user}/{self.current_managed_user}",
                        verify=False).text)):
                    self.bot.send_message(self.current_user, "User have been successfully added to you black list")
                    self.proceed_with_encounters()
                else:
                    self.bot.send_message(self.current_user, "User was not recognised. His account had probably been already deleted :)")
                    self.proceed_with_encounters()
            elif message.text == "no":
                self.proceed_with_encounters()
            elif message.text == "abort":
                self.proceed()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.abortMarkup)
                self.bot.register_next_step_handler(message, self.add_to_black_list, acceptMode=acceptMode,
                                                    chat_id=self.current_user)

    def black_list_management(self, message, acceptMode=False, isEncounter=False):
        if not acceptMode:
            self.black_list = {}
            self.markup_page = 1

            users = json.loads(requests.get(f"https://localhost:44381/GetBlackList/{self.current_user}", verify=False).text)
            if users:
                for user in users:
                    self.black_list[user["bannedUser"]["id"]] = user["bannedUser"]["userRealName"].lower().strip()

                self.current_callback_handler = self.bot.register_callback_query_handler("", self.black_list_callback_handler,
                                                                       user_id=self.current_user)

                reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page,
                            self.markup_pages_count)
                self.markup_pages_count = count_pages(self.black_list, self.current_markup_elements, self.markup_pages_count)
                markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

                self.bot.send_message(self.current_user, "There are all users you have in your black list", reply_markup=markup)
                self.bot.send_message(self.current_user, "Select a user to remove him from the black list", reply_markup=self.abortMarkup)
            else:
                self.bot.send_message(self.current_user, "There are no users in your blacklist :)")
                self.proceed()
        else:
            if message.text == "yes":
                if bool(json.loads(requests.delete(f"https://localhost:44381/RemoveUserFromBlackList/{self.current_user}/{self.current_managed_user}", verify=False).text)):
                    self.bot.send_message(self.current_user, "User have been successfully removed from you black list")
                    if not isEncounter:
                        self.black_list_management(message)
                    else:
                        self.proceed_with_encounters()
                else:
                    self.bot.send_message(self.current_user, "User was not recognised. His account had probably been already deleted :)")
                    if not isEncounter:
                        self.black_list_management(message)
                    else:
                        self.proceed_with_encounters()
            elif message.text == "no":
                if not isEncounter:
                    self.black_list_management(message)
                else:
                    self.proceed_with_encounters()
            elif message.text == "abort":
                self.proceed()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.abortMarkup)
                self.bot.register_next_step_handler(message, self.black_list_management, acceptMode=acceptMode, chat_id=self.current_user)

    def personality_switch(self, message, acceptMode=False):
        if not acceptMode:
            doesUse = Helpers.check_user_uses_personality(self.current_user)
            status = ""
            switchMessage = ""

            if doesUse:
                status = "Online"
                switchMessage = "Would you like to turn it off?"
            else:
                status = "Offline"
                switchMessage = "Would you like to turn it on?"

            self.bot.send_message(self.current_user, f"PERSONALITY is currently {status}\n{switchMessage}", reply_markup=self.YNMarkup)
            self.bot.register_next_step_handler(message, self.personality_switch, acceptMode=True, chat_id=self.current_user)

        else:
            if message.text == "yes":
                try:
                    Helpers.switch_personality_status(self.current_user)
                    self.bot.send_message(self.current_user, "Done :)")
                except:
                    self.bot.send_message(self.current_user, "Something went wrong. Please, contact the administration")

                self.proceed()
            elif message.text == "no":
                self.proceed()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.YNMarkup)
                self.bot.register_next_step_handler(message, self.personality_switch, acceptMode=acceptMode, chat_id=self.current_user)

    def set_profile_status(self, message, acceptMode=False):
        if not acceptMode:
            if Helpers.check_user_has_premium(self.current_user):
                self.bot.send_message(self.current_user, f"Please, send me your new status (up to 50 characters)")
                self.bot.register_next_step_handler(message, self.set_profile_status, acceptMode=True, chat_id=self.current_user)
            else:
                self.bot.send_message(self.current_user, f"This action is available only for users with premium", reply_markup=self.YNMarkup)
                self.proceed()
        else:
            if 50 > len(message.text) > 0:
                if Helpers.update_user_status(self.current_user, message.text):
                    self.bot.send_message(self.current_user, f"Done :)")
                else:
                    self.bot.send_message(self.current_user, f"Something went wrong. Please contact the administration")
                self.proceed()
            else:
                self.bot.send_message(self.current_user, f"Incorrect status length")
                self.bot.register_next_step_handler(message, self.set_profile_status, acceptMode=acceptMode, chat_id=self.current_user)


    def black_list_callback_handler(self, call):
        if call.message.id not in self.old_queries:
            self.current_query = call.message.id

            if call.data == "-1" or call.data == "-2":
                index = self.index_converter(call.data)
                if self.markup_page + index <= self.markup_pages_count or self.markup_page + index >= 1:
                    markup = assemble_markup(self.markup_page, self.current_markup_elements, index)
                    self.bot.edit_message_reply_markup(chat_id=call.message.chat.id, reply_markup=markup,
                                                       message_id=call.message.id)
                    self.markup_page += index

            elif "/" in call.data:  # TODO: Make it work another way... maybe
                self.bot.answer_callback_query(call.id, call.data)

            else:
                try:
                    self.current_managed_user = int(call.data)
                    self.bot.send_message(self.current_user, "Are you sure, you want to delete that user from your black list?", reply_markup=self.YNMarkup)
                    self.bot.register_next_step_handler(call.message, self.black_list_management, acceptMode=True,
                                                        chat_id=self.current_user)
                except:
                    self.bot.send_message(self.current_user, "Something went wrong, please contact the administration")
                    self.proceed()

    def switch_user_filtering(self, message, acceptMode=False):
        if not acceptMode:
            status_string = requests.get(f"https://localhost:44381/GetUserFilteringByPhotoStatus/{self.current_user}", verify=False)
            self.bot.send_message(self.current_user, status_string, reply_markup=self.YNMarkup)
            self.bot.register_next_step_handler(message, self.switch_user_filtering, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "Yes":
                requests.get(f"https://localhost:44381/GetUserFilteringByPhotoStatus/{self.current_user}", verify=False)
            elif message.text == "No":
                pass
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.YNMarkup)
                self.bot.register_next_step_handler(message, self.switch_user_filtering, acceptMode=True, chat_id=self.current_user)
                return False
        self.bot.send_message(self.current_user, "Done :)")
        self.proceed()

    def send_confirmation_request(self, message, acceptMode=False):
        if not acceptMode:

            if not self.requestStatus:
                self.bot.send_message(self.current_user, "You can confirm your identity by sending us:\nVideo or 'Circle' (15 seconds max) in which you are saying the code frase 'LALALA'.\n!Your face have to be visible!", reply_markup=self.abortMarkup)
                self.bot.register_next_step_handler(message, self.send_confirmation_request, acceptMode=True, chat_id=self.current_user)
            else:
                self.bot.send_message(self.current_user, f"Status is: {self.requestStatus}")
                self.bot.send_message(self.current_user, "You can update current request, you have sent by sending us:\nVideo or 'Circle' (15 seconds max) in which you are saying the code frase 'LALALA'.\n!Your face have to be visible!", reply_markup=self.abortMarkup)
                self.bot.register_next_step_handler(message, self.send_confirmation_request, acceptMode=True, chat_id=self.current_user)

        else:
            if message.text == "abort":
                self.proceed()

            data = {
                "userId": self.current_user
            }
            if message.video:
                if message.video.duration > 15:
                    self.bot.send_message(self.current_user, "To long video")
                    self.bot.register_next_step_handler(message, self.send_confirmation_request, acceptMode=acceptMode, chat_id=self.current_user)

                data["video"] = message.video[len(message.video) - 1].file_id
                d = json.dumps(data)
                if bool(json.loads(requests.post(f"https://localhost:44381/SendTickRequest", d, headers={"Content-Type": "application/json"}, verify=False).text)):
                    self.bot.send_message(self.current_user, "Done :)")
                    self.proceed()
            elif message.video_note:
                if message.video_note.duration > 15:
                    self.bot.send_message(self.current_user, "To long video")
                    self.bot.register_next_step_handler(message, self.send_confirmation_request, acceptMode=acceptMode, chat_id=self.current_user)

                data["circle"] = message.video_note.file_id
                d = json.dumps(data)
                if bool(json.loads(requests.post(f"https://localhost:44381/SendTickRequest", d,
                                                 headers={"Content-Type": "application/json"}, verify=False).text)):
                    self.bot.send_message(self.current_user, "Done :)")
                    self.proceed()
            else:
                self.bot.send_message(self.current_user, "This type of data cannot be accepted as your identity confirmation", reply_markup=self.abortMarkup)
                self.bot.register_next_step_handler(message, self.send_confirmation_request, acceptMode=acceptMode, chat_id=self.current_user)


    def encounters_callback_handler(self, call):
        if call.message.id not in self.old_queries:
            self.current_query = call.message.id

            if call.data == "-1" or call.data == "-2":
                index = self.index_converter(call.data)
                if self.markup_page + index <= self.markup_pages_count or self.markup_page + index >= 1:
                    markup = assemble_markup(self.markup_page, self.current_markup_elements, index)
                    self.bot.edit_message_reply_markup(chat_id=call.message.chat.id, reply_markup=markup,
                                                       message_id=call.message.id)
                    self.markup_page += index

            elif "/" in call.data:  # TODO: Make it work another way... maybe
                self.bot.answer_callback_query(call.id, call.data)

            else:
                try:
                    self.current_managed_user = int(call.data)
                    self.proceed_with_encounters()
                except:
                    self.bot.send_message(self.current_user, "Unable to load user data. His account may already be banned or deleted")
                    self.proceed()

    def personality_callback_handler(self, call):
        self.bot.answer_callback_query(call.id, "")
        if call.message.id not in self.old_queries:

            if call.data == "1":
                if self.personality_caps["canP"]:
                    if self.user_free_points > 0:
                        self.user_free_points -= 1
                        self.user_points["personality"] += 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["personality"] = self.user_points["personality"]

                        self.p_indicator.text = f"         {self.user_points['personality']}         "
                        self.update_personality_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any personality points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "-1":
                if self.personality_caps["canP"]:
                    if self.user_points["personality"] > 0:
                        self.user_free_points += 1
                        self.user_points["personality"] -= 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["personality"] = self.user_points["personality"]

                        self.p_indicator.text = f"         {self.user_points['personality']}         "
                        self.update_personality_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any personality points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "2":
                if self.personality_caps["canE"]:
                    if self.user_free_points > 0:
                        self.user_free_points -= 1
                        self.user_points["emotionalIntellect"] += 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["emotionalIntellect"] = self.user_points["emotionalIntellect"]

                        self.e_indicator.text = self.user_points["emotionalIntellect"]
                        self.update_personality_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any personality points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "-2":
                if self.personality_caps["canE"]:
                    if self.user_points["emotionalIntellect"] > 0:
                        self.user_free_points += 1
                        self.user_points["emotionalIntellect"] -= 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["emotionalIntellect"] = self.user_points["emotionalIntellect"]

                        self.e_indicator.text = self.user_points["emotionalIntellect"]
                        self.update_personality_markup()
                        return False
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "3":
                if self.personality_caps["canR"]:
                    if self.user_free_points > 0:
                        self.user_free_points -= 1
                        self.user_points["reliability"] += 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["reliability"] = self.user_points["reliability"]

                        self.r_indicator.text = self.user_points["reliability"]
                        self.update_personality_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any personality points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "-3":
                if self.personality_caps["canR"]:
                    if self.user_points["reliability"] > 0:
                        self.user_free_points += 1
                        self.user_points["reliability"] -= 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["reliability"] = self.user_points["reliability"]

                        self.r_indicator.text = self.user_points["reliability"]
                        self.update_personality_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any personality points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "4":
                if self.personality_caps["canS"]:
                    if self.user_free_points > 0:
                        self.user_free_points -= 1
                        self.user_points["compassion"] += 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["compassion"] = self.user_points["compassion"]

                        self.s_indicator.text = self.user_points["compassion"]
                        self.update_personality_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any personality points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "-4":
                if self.personality_caps["canS"]:
                    if self.user_points["compassion"] > 0:
                        self.user_free_points += 1
                        self.user_points["compassion"] -= 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["compassion"] = self.user_points["compassion"]

                        self.s_indicator.text = self.user_points["compassion"]
                        self.update_personality_markup()
                        return False
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "5":
                if self.personality_caps["canO"]:
                    if self.user_free_points > 0:
                        self.user_free_points -= 1
                        self.user_points["openMindedness"] += 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["openMindedness"] = self.user_points["openMindedness"]

                        self.o_indicator.text = self.user_points["openMindedness"]
                        self.update_personality_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any personality points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "-5":
                if self.personality_caps["canO"]:
                    if self.user_points["openMindedness"] > 0:
                        self.user_free_points += 1
                        self.user_points["openMindedness"] -= 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["openMindedness"] = self.user_points["openMindedness"]

                        self.o_indicator.text = self.user_points["openMindedness"]
                        self.update_personality_markup()
                        return False
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "6":
                if self.personality_caps["canN"]:
                    if self.user_free_points > 0:
                        self.user_free_points -= 1
                        self.user_points["agreeableness"] += 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["agreeableness"] = self.user_points["agreeableness"]

                        self.n_indicator.text = self.user_points["agreeableness"]
                        self.update_personality_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any personality points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "-6":
                if self.personality_caps["canN"]:
                    if self.user_points["agreeableness"]:
                        self.user_free_points += 1
                        self.user_points["agreeableness"] -= 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["agreeableness"] = self.user_points["agreeableness"]

                        self.n_indicator.text = self.user_points["agreeableness"]
                        self.update_personality_markup()
                        return False
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "7":
                if self.personality_caps["canA"]:
                    if self.user_free_points > 0:
                        self.user_free_points -= 1
                        self.user_points["selfAwareness"] += 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["selfAwareness"] = self.user_points["selfAwareness"]

                        self.a_indicator.text = self.user_points["selfAwareness"]
                        self.update_personality_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any personality points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "-7":
                if self.personality_caps["canA"]:
                    if self.user_points["selfAwareness"] > 0:
                        self.user_free_points += 1
                        self.user_points["selfAwareness"] -= 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["selfAwareness"] = self.user_points["selfAwareness"]

                        self.a_indicator.text = self.user_points["selfAwareness"]
                        self.update_personality_markup()
                        return False
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "8":
                if self.personality_caps["canL"]:
                    if self.user_free_points > 0:
                        self.user_free_points -= 1
                        self.user_points["levelOfSense"] += 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["levelOfSense"] = self.user_points["levelOfSense"]

                        self.l_indicator.text = self.user_points["levelOfSense"]
                        self.update_personality_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any personality points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "-8":
                if self.personality_caps["canL"]:
                    if self.user_points["levelOfSense"] > 0:
                        self.user_free_points += 1
                        self.user_points["levelOfSense"] -= 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["levelOfSense"] = self.user_points["levelOfSense"]

                        self.l_indicator.text = self.user_points["levelOfSense"]
                        self.update_personality_markup()
                        return False
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "9":
                if self.personality_caps["canI"]:
                    if self.user_free_points > 0:
                        self.user_free_points -= 1
                        self.user_points["intellect"] += 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["intellect"] = self.user_points["intellect"]

                        self.i_indicator.text = self.user_points["intellect"]
                        self.update_personality_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any personality points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "-9":
                if self.personality_caps["canI"]:
                    if self.user_points["intellect"] > 0:
                        self.user_free_points += 1
                        self.user_points["intellect"] -= 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["intellect"] = self.user_points["intellect"]

                        self.i_indicator.text = self.user_points["intellect"]
                        self.update_personality_markup()
                        return False
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "10":
                if self.personality_caps["canT"]:
                    if self.user_free_points > 0:
                        self.user_free_points -= 1
                        self.user_points["nature"] += 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["nature"] = self.user_points["nature"]

                        self.t_indicator.text = self.user_points["nature"]
                        self.update_personality_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any personality points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "-10":
                if self.personality_caps["canT"]:
                    if self.user_points["nature"] > 0:
                        self.user_free_points += 1
                        self.user_points["nature"] -= 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["nature"] = self.user_points["nature"]

                        self.t_indicator.text = self.user_points["nature"]
                        self.update_personality_markup()
                        return False
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "11":
                if self.personality_caps["canY"]:
                    if self.user_free_points > 0:
                        self.user_free_points -= 1
                        self.user_points["creativity"] += 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["creativity"] = self.user_points["creativity"]

                        self.y_indicator.text = self.user_points["creativity"]
                        self.update_personality_markup()
                        return False
                    self.bot.send_message(self.current_user, "You dont have any personality points left")
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")
            elif call.data == "-11":
                if self.personality_caps["canY"]:
                    if self.user_points["creativity"] > 0:
                        self.user_free_points += 1
                        self.user_points["creativity"] -= 1
                        self.free_points_indicator.text = self.user_free_points
                        self.personality_updated_points["creativity"] = self.user_points["creativity"]

                        self.y_indicator.text = self.user_points["creativity"]
                        self.update_personality_markup()
                        return False
                    return False
                self.bot.send_message(self.current_user, "Pass at least one test related to that parameter first :)")

    def update_personality_markup(self):
        self.bot.edit_message_reply_markup(chat_id=self.current_user, reply_markup=self.personalityMarkup, message_id=self.active_personality_message)

    def proceed(self):
        if self.current_callback_handler:
            self.bot.callback_query_handlers.remove(self.current_callback_handler)
            self.current_callback_handler = None
        self.setting_choice(self.message)

    def proceed_with_encounters(self):
        self.isInBlackList = Helpers.check_user_in_a_blacklist(self.current_user, self.current_managed_user)
        Helpers.get_user_info(self.current_managed_user)

        if self.isInBlackList:
            m = self.remove_from_blacklist_text
        else:
            m = self.add_to_blacklist_text

        user = Helpers.get_user_info(self.current_managed_user)

        self.bot.send_photo(self.current_user, user["userBaseInfo"]["userPhoto"], user["userBaseInfo"]["userDescription"],
                            reply_markup=self.encounterOptionMarkup)
        self.bot.send_message(self.current_user, f"{self.encounter_options_message}{m}", reply_markup=self.encounterOptionMarkup)
        self.bot.register_next_step_handler(self.message, self.encounter_list_management, acceptMode=True,
                                            chat_id=self.current_user)

    @staticmethod
    def index_converter(index):
        if index == "-1":
            return -1
        return 1

    def destruct(self):
        if self.current_callback_handler:
            self.bot.callback_query_handlers.remove(self.current_callback_handler)
        Helpers.switch_user_busy_status(self.current_user)
        go_back_to_main_menu(self.bot, self.current_user, self.message)
        del self