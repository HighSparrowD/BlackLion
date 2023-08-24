import copy

from telebot import TeleBot
from telebot.types import KeyboardButton, ReplyKeyboardMarkup, InlineKeyboardMarkup, InlineKeyboardButton
from Core import HelpersMethodes as Helpers
from Common.Menues import count_pages, assemble_markup, reset_pages, add_tick_to_element, add_tick_to_elements, remove_tick_from_element, remove_tick_from_elements, index_converter
import requests
import json
from Core.Resources import Resources
from Common.Menues import go_back_to_main_menu
from TestModule import TestModule


class Registrator:
    def __init__(self, bot: TeleBot, message: any, hasVisited: bool = False, return_method: any = None,
                 localizationIndex: int = None, promoCode: str = None):
        self.bot = bot
        self.message = message
        self.return_method = return_method
        self.previous_item = '' #Is used to remove a tick from single-type items (country, city, etc..)
        self.current_user = message.from_user.id
        self.hasVisited = hasVisited
        self.localization = Resources.get_registrator_localization(localizationIndex)
        self.okMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(KeyboardButton(self.localization['OkButton']))
        self.YNmarkup = InlineKeyboardMarkup().add(InlineKeyboardButton(self.localization['YesButton'], callback_data="1"),
                                                   InlineKeyboardButton(self.localization['NoButton'], callback_data="2"))
        self.goBackInlineMarkup = InlineKeyboardMarkup().add(InlineKeyboardButton("🔙 Go Back", callback_data="-10"))

        self.chCode = self.bot.register_callback_query_handler(message, self.callback_handler, user_id=self.current_user)
        self.next_handler = None

        self.premium_limit = Helpers.get_user_language_limit(self.current_user)

        self.promo = promoCode

        self.question_index = 0 # Represents current question index

        self.question_count = 0
        self.max_questions_count = 16

        self.tags = ""
        self.maxTagCount = Helpers.get_user_limitations(self.current_user)["maxTagsPerSearch"]

        self.current_markup_elements = []
        self.markup_last_element = 0
        self.markup_page = 1
        self.markup_pages_count = 0

        self.app_languages_markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True)
        self.gender_markup = InlineKeyboardMarkup()
        self.reason_markup = InlineKeyboardMarkup()
        self.communication_pref_markup = InlineKeyboardMarkup()
        self.age_pref_markup = InlineKeyboardMarkup()
        self.skip_markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(self.localization['SkipMessage'])
        self.go_backMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(self.localization['GoBackButton'])
        self.photo_Markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("Use photo from profile")

        self.app_language = localizationIndex

        self.data = {}
        self.current_user_data = None
        self.country = None
        self.city = None

        self.current_step_id = self.spoken_language_step

        self.languages = {}

        self.genders = {}
        self.countries = {}
        self.cities = {}

        self.reasons = {}
        self.communication_pref = {}
        self.app_langs = {}

        self.chosen_langs = []
        self.pref_langs = []
        self.pref_countries = []

        self.reply_voice = ""
        self.reply_text = ""

        self.active_message = None
        self.secondary_message = None
        self.error_message = None
        self.additional_message = None

        self.registration_steps = []
        self.previous_section = None
        self.current_section = None

        self.editMode = False

        if not hasVisited:
            self.get_localisations()
            self.current_section = self.destruct
            self.spoken_language_step(message)
        else:
            self.current_user_data = Helpers.get_user_info(self.current_user)
            if self.current_user_data:
                data = self.current_user_data["data"]

                self.pref_countries = data["locationPreferences"]
                self.pref_langs = data["languagePreferences"]
                self.chosen_langs = data["userLanguages"]
                self.app_language = data["language"]

                if "location" in data:
                    self.country = self.current_user_data["location"]["countryId"]
                    self.city = data["location"]["cityId"]

                if "tags" in self.data.keys():
                    self.tags = ' '.join(self.current_user_data["tags"])

                self.data["appLanguage"] = self.app_language
                self.data["userName"] = data["userName"]
                self.data["id"] = self.current_user
                self.data["userRealName"] = data["userRealName"]
                self.data["userDescription"] = data["userRawDescription"]
                self.data["userMedia"] = data["userMedia"]
                self.data["reasonId"] = data["reason"]
                self.data["userAge"] = data["userAge"]
                self.data["userLanguages"] = self.chosen_langs
                self.data["userCountryCode"] = self.country
                self.data["userCityCode"] = self.city
                self.data["userGender"] = data["userGender"]
                self.data["userLanguagePreferences"] = self.pref_langs
                self.data["userLocationPreferences"] = self.pref_countries
                self.data["agePrefs"] = data["agePrefs"]
                self.data["communicationPrefs"] = data["communicationPrefs"]
                self.data["userGenderPrefs"] = data["userGenderPrefs"]
                self.data["tags"] = self.tags
                self.data["isMediaPhoto"] = data["isMediaPhoto"]

                self.get_localisations()

                cities = json.loads(
                    requests.get(f"https://localhost:44381/GetCities/{self.country}/{self.app_language}",
                                 verify=False).text)

                # For edit purposes. If left as they are -> can result bugs
                self.cities.clear()

                if self.country:
                    for city in cities:
                        self.cities[city["id"]] = city["cityName"].lower()

                self.registration_steps.insert(0, self.destruct)
                self.current_section = self.checkout_step

                self.checkout_step(message)
            else:
                self.spoken_language_step(message)

    # Case without 'editMode=True' is redundant
    def app_language_step(self, message=None, acceptMode=False, shouldInsert=True):
        if not acceptMode:
            self.question_index = 1

            self.configure_registration_step(self.app_language_step, shouldInsert)

            for lang in json.loads(requests.get("https://localhost:44381/app-languages", verify=False).text):
                self.app_langs[lang["id"]] = lang["languageNameShort"]

            self.app_languages_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)
            start_message = "START MESSAGE" #self.localisation[0]["loc"][0]["elementText"]
            b = []

            for lang in self.app_langs.values():
                b.append(KeyboardButton(lang))

            self.app_languages_markup.add(b[0], b[1], b[2])
            self.send_active_message(start_message, markup=self.app_languages_markup)
            self.send_additional_actions_message()
            self.next_handler = self.bot.register_next_step_handler(message, self.app_language_step, acceptMode=True,
                                                                    chat_id=self.current_user)

        else:
            self.delete_message(message.id)

            self.app_language = self.app_language_converter(message.text)
            if self.app_language is not None:

                self.checkout_step(message)
                self.data["userAppLanguageId"] = self.app_language

            else:
                self.send_error_message(self.localization['LanguageNotRecognizedErrorMessage'], markup=self.app_languages_markup)
                self.next_handler = self.bot.register_next_step_handler(message, self.app_language_step, acceptMode=acceptMode,
                                                                        shouldInsert=False,
                                                                        chat_id=self.current_user)

    def spoken_language_step(self, message=None, acceptMode=False, shouldInsert=True):
        if not acceptMode:
            self.question_index = 2
            self.markup_page = 1

            question_counter = self.move_forward()

            self.current_section = self.spoken_language_step

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page, self.markup_pages_count)
            count_pages(self.languages, self.current_markup_elements, self.markup_pages_count)
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)
            self.markup_page = 1

            self.send_active_message(question_counter + self.localization['SateLanguagesMessage'], markup=markup)

            #Add ticks if in edit mode
            if self.editMode:
                for l in self.chosen_langs:
                    add_tick_to_element(self.bot, self.current_user, self.active_message, self.current_markup_elements, self.markup_page, str(l))

            self.send_secondary_message(self.localization['ChooseLanguageMessage'], markup=self.okMarkup)
            self.next_handler = self.bot.register_next_step_handler(message, self.spoken_language_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message.id)

            if not message.text:
                self.send_error_message(self.localization['LanguageNotRecognizedErrorMessage'])
                self.next_handler = self.bot.register_next_step_handler(message, self.spoken_language_step, acceptMode=acceptMode,
                                                                        shouldInsert=False,
                                                                        chat_id=self.current_user)
                return

            message_text = message.text.lower().strip()
            if message_text != self.localization['OkButton']:
                lang = self.spoken_languages_convertor(message_text)
                if lang:  # TODO: Get string, separate by "," and process it
                    if lang not in self.chosen_langs:
                        if len(self.chosen_langs) + 1 > self.premium_limit:
                            self.send_error_message(self.localization['LanguagesPremiumErrorMessage'].format(self.premium_limit), markup=self.okMarkup)
                        else:
                            self.chosen_langs.append(lang)
                            add_tick_to_element(self.bot, self.current_user, self.active_message,
                                                self.current_markup_elements,
                                                self.markup_page, str(lang))
                            self.send_error_message(self.localization['AddedMessage'], markup=self.okMarkup)
                    else:
                        self.chosen_langs.remove(lang)
                        remove_tick_from_element(self.bot, self.current_user, self.active_message,
                                                 self.current_markup_elements,
                                                 self.markup_page, str(lang))
                        self.send_error_message(self.localization['RemovedMessage'], markup=self.okMarkup)
                    self.next_handler = self.bot.register_next_step_handler(message, self.spoken_language_step, acceptMode=acceptMode, chat_id=self.current_user)
                    return
                else:
                    self.send_error_message(self.localization['LanguageNotRecognizedErrorMessage'])
                    self.next_handler = self.bot.register_next_step_handler(message, self.spoken_language_step, acceptMode=acceptMode,
                                                                            chat_id=self.current_user)
                    return

            if self.chosen_langs:
                self.data["userLanguages"] = self.chosen_langs

                if not self.editMode:
                    self.data["id"] = self.current_user
                    self.data["userName"] = message.from_user.username
                    self.data["userAppLanguageId"] = self.app_language
                    self.data["promo"] = self.promo

                    self.gender_step()
                else:
                    self.checkout_step(message)

            else:
                self.send_error_message(self.localization['NoLanguagesMessage'], markup=self.okMarkup)
                self.next_handler = self.bot.register_next_step_handler(message, self.spoken_language_step, acceptMode=acceptMode,
                                                                        chat_id=self.current_user)

    def gender_step(self, shouldInsert=True):
        self.question_index = 3

        question_counter = self.move_forward()

        self.configure_registration_step(self.gender_step, shouldInsert)

        self.gender_markup.clear()

        genders = copy.copy(self.genders)
        genders.pop(4) # Remove "Does not matter"

        for g in genders.keys():
            self.gender_markup.add(InlineKeyboardButton(self.genders[g], callback_data=f"{g}"))

        self.gender_markup.add(InlineKeyboardButton("🔙 Go Back", callback_data="-10"))

        self.send_active_message(question_counter + self.localization['GenderQuestionMessage'], markup=self.gender_markup)

    def reason_step(self, shouldInsert=True):
        self.question_index = 9

        question_counter = self.move_forward()
        # self.send_encourage_message("Wow. You already here...")

        self.configure_registration_step(self.reason_step, shouldInsert)

        self.reason_markup.clear()

        for reason in self.reasons.keys():
            self.reason_markup.add(InlineKeyboardButton(self.reasons[reason], callback_data=f"{reason}"))

        self.reason_markup.add(InlineKeyboardButton("🔙 Go Back", callback_data="-10"))

        self.send_active_message(question_counter + self.localization['SearchQuestionMessage'], markup=self.reason_markup)

    def communication_preferences_step(self, shouldInsert=True):
        self.question_index = 10

        question_counter = self.move_forward()

        self.configure_registration_step(self.communication_preferences_step, shouldInsert)

        self.communication_pref_markup.clear()

        for pref in self.communication_pref:
            self.communication_pref_markup.add(InlineKeyboardButton(self.communication_pref[pref], callback_data=f"{pref}"))

        self.communication_pref_markup.add(InlineKeyboardButton("🔙 Go Back", callback_data="-10"))

        self.send_active_message(question_counter + self.localization['CommunicationQuestionMessage'], markup=self.communication_pref_markup)

    def location_step(self, message=None, acceptMode=False, shouldInsert=True):
        if not acceptMode:
            self.question_index = 4
            self.markup_page = 1

            question_counter = self.move_forward()

            self.configure_registration_step(self.location_step, shouldInsert)
            # self.send_encourage_message("You have done good job answering our questions. Keep it up!")

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page,
                        self.markup_pages_count)
            count_pages(self.countries, self.current_markup_elements, self.markup_pages_count, additionalButton=True, buttonText="🔙 Go Back", buttonData="-10")
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

            self.previous_item = ''

            self.send_active_message(question_counter + self.localization['CountryQuestionMessage'], markup=markup)

            if self.editMode:
                self.previous_item = str(self.country)
                add_tick_to_element(self.bot, self.current_user, self.active_message, self.current_markup_elements, self.markup_page, str(self.country))

            # Allow skipping location part if user prefers to communicate online
            if self.data["communicationPrefs"] == 1:
                m = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(self.localization['NoMatterButton'], row_width=2).add(self.localization['OkButton'], row_width=1)
                self.send_secondary_message(self.localization['ChooseOrSkipMessage'], markup=m)
            else:
                self.send_secondary_message(self.localization['ChooseCountryMessage'], markup=self.okMarkup)

            self.next_handler = self.bot.register_next_step_handler(message, self.location_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message.id)

            if not message.text:
                self.send_error_message(self.localization['CountyNotRecognizedErrorMessage'])
                self.next_handler = self.bot.register_next_step_handler(message, self.location_step, acceptMode=acceptMode,
                                                                        chat_id=self.current_user)
                return

            message_text = message.text.lower().strip()
            if message_text != self.localization['OkButton']:
                country = self.country_convertor(message_text)
                if country:
                    if self.previous_item:
                        remove_tick_from_element(self.bot, self.current_user, self.active_message,
                                                 self.current_markup_elements,
                                                 self.markup_page, self.previous_item)
                    self.previous_item = str(country)
                    add_tick_to_element(self.bot, self.current_user, self.active_message,
                                        self.current_markup_elements,
                                        self.markup_page, str(country))
                    self.country = country
                    self.send_error_message(self.localization['GotchaMessage'], markup=self.okMarkup)
                    self.next_handler = self.bot.register_next_step_handler(message, self.location_step, acceptMode=acceptMode, chat_id=self.current_user)
                    return
                elif message_text == self.localization['NoMatterButton'] and self.data["communicationPrefs"] == 1:
                    if self.data["communicationPrefs"] == 1:

                        #In case user had chosen a country before pressing 'Does not matter' button
                        self.country = None
                        self.city = None
                        self.pref_countries.clear()
                        self.data["userLocationPreferences"] = None
                        self.data["userCityCode"] = None
                        self.data["userCountryCode"] = None

                        self.max_questions_count = 15

                        if not self.editMode:
                            self.bot.send_message(self.current_user, self.localization['RestrictLocationMessage'])
                            self.name_step(message)
                            return False
                        else:
                            self.checkout_step(message)
                            self.bot.send_message(self.current_user, self.localization['LocationResetMessage'])
                            return False
                else:
                    self.send_error_message(self.localization['CountyNotRecognizedErrorMessage'])
                    self.suggest_countries(message_text)
                    self.next_handler = self.bot.register_next_step_handler(message, self.location_step, acceptMode=acceptMode, chat_id=self.current_user)
                    return False

            if self.country or self.country == 0:
                self.data["userCountryCode"] = self.country
                self.previous_item = ''

                if self.editMode:
                    self.city = None

                self.city_step(message)
            else:
                self.send_error_message(self.localization['CountryErrorMessage'], markup=self.okMarkup)
                self.next_handler = self.bot.register_next_step_handler(message, self.location_step, acceptMode=acceptMode, chat_id=self.current_user)

    def city_step(self, message=None, acceptMode=False, shouldInsert=True):
        if not self.country:
            self.checkout_step(message)
            self.send_error_message(self.localization['CityErrorMessage1'])
            return False

        if not acceptMode:
            self.question_index = 5
            self.markup_page = 1

            question_counter = self.move_forward()

            self.configure_registration_step(self.city_step, shouldInsert)

            cities = json.loads(
                requests.get(f"https://localhost:44381/GetCities/{self.country}/{self.app_language}", verify=False).text)

            #For edit purposes. If left as they are -> can result bugs
            self.cities.clear()

            for city in cities:
                self.cities[city["id"]] = city["cityName"].lower()

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page, self.markup_pages_count)
            count_pages(self.cities, self.current_markup_elements, self.markup_pages_count, additionalButton=True, buttonText="🔙 Go Back", buttonData="-10")
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

            self.send_active_message(question_counter + self.localization['ChooseCityMessage'], markup=markup)

            if self.editMode:
                self.previous_item = str(self.city)
                add_tick_to_element(self.bot, self.current_user, self.active_message,
                                    self.current_markup_elements, self.markup_page, str(self.city))

            self.send_secondary_message(self.localization['ChoosMessage'], markup=self.okMarkup)
            self.next_handler = self.bot.register_next_step_handler(message, self.city_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message.id)

            if not message.text:
                self.send_error_message(self.localization['CityNotRecognizedErrorMessage'])
                self.next_handler = self.bot.register_next_step_handler(message, self.spoken_language_step, acceptMode=acceptMode,
                                                    chat_id=self.current_user)
                return False

            message_text = message.text.lower().strip()
            if message_text != self.localization['OkButton']:
                city = self.city_convertor(message_text)
                if city:
                    if self.previous_item:
                        remove_tick_from_element(self.bot, self.current_user, self.active_message,
                                                 self.current_markup_elements,
                                                 self.markup_page, self.previous_item)
                    self.previous_item = str(city)
                    add_tick_to_element(self.bot, self.current_user, self.active_message,
                                        self.current_markup_elements,
                                        self.markup_page, str(city))
                    self.city = city
                    self.send_error_message(self.localization['GotchaMessage'], markup=self.okMarkup)
                    self.next_handler = self.bot.register_next_step_handler(message, self.city_step, acceptMode=acceptMode, chat_id=self.current_user)
                    return True
                else:
                    self.send_error_message(self.localization['CityNotRecognizedErrorMessage'])
                    self.suggest_cities(message_text)
                    self.next_handler = self.bot.register_next_step_handler(message, self.city_step, acceptMode=acceptMode, chat_id=self.current_user)
                    return False

            if self.city or self.city == 0:
                self.previous_item = ''
                self.data["userCityCode"] = self.city

                if not self.editMode:
                    self.name_step(message)
                else:
                    self.checkout_step(message)

                # self.data["userCity"] = self.cities[self.country][self.city]
            else:
                self.send_error_message(self.localization['CityErrorMessage2'], markup=self.okMarkup)
                self.next_handler = self.bot.register_next_step_handler(message, self.city_step, acceptMode=acceptMode, chat_id=self.current_user)

    def name_step(self, message=None, acceptMode=False, shouldInsert=True):
        if not acceptMode:
            self.question_index = 6

            question_counter = self.move_forward()

            self.configure_registration_step(self.name_step, shouldInsert)

            self.send_active_message(question_counter + self.localization['NameQuestionMessage'])
            self.send_additional_actions_message()
            self.next_handler = self.bot.register_next_step_handler(message, self.name_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message.id)

            if message.text:
                self.data["userRealName"] = message.text

                if len(message.text) > 50:
                    self.send_error_message(self.localization['LongNameErrorMessage'])
                    self.next_handler = self.bot.register_next_step_handler(message, self.name_step, acceptMode=acceptMode, chat_id=self.current_user)
                    return

                if not self.editMode:
                    self.age_step(message)
                else:
                    self.checkout_step(message)

            else:
                self.send_error_message(self.localization['EmptyNameErrorMessage'])
                self.next_handler = self.bot.register_next_step_handler(message, self.name_step, acceptMode=acceptMode, chat_id=self.current_user)

    def age_step(self, message=None, acceptMode=False, shouldInsert=True):
        if not acceptMode:
            self.question_index = 7

            question_counter = self.move_forward()

            self.configure_registration_step(self.age_step, shouldInsert)

            self.send_additional_actions_message()
            self.send_active_message(question_counter + self.localization['AgeQuestionMessage'])
            self.next_handler = self.bot.register_next_step_handler(message, self.age_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message.id)

            try:
                age = int(message.text)
                if age > 100:
                    self.send_error_message(self.localization['HighAgeErrorMessage'])
                    self.next_handler = self.bot.register_next_step_handler(message, self.age_step, acceptMode=acceptMode, chat_id=self.current_user)
                    return
                elif age < 14:
                    self.send_error_message(self.localization['UnderagedErrorMessage'])
                    self.next_handler = self.bot.register_next_step_handler(message, self.age_step, acceptMode=acceptMode, chat_id=self.current_user)
                    return

                self.data["userAge"] = age

                if not self.editMode:
                    self.description_step(message)
                else:
                    self.checkout_step(message)

            except:
                self.send_error_message(self.localization['DigitalAgeErrorMessage'])
                self.next_handler = self.bot.register_next_step_handler(message, self.age_step, acceptMode=acceptMode, chat_id=self.current_user)

    def description_step(self, message=None, acceptMode=False, shouldInsert=True):
        if not acceptMode:
            self.question_index = 8

            question_counter = self.move_forward()

            self.configure_registration_step(self.description_step, shouldInsert)

            self.send_additional_actions_message()
            self.send_active_message(question_counter + self.localization['DescritpionQuestionMessage'], markup=self.skip_markup)
            self.next_handler = self.bot.register_next_step_handler(message, self.description_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message.id)

            if message.text == self.localization['SkipMessage']:
                self.data["userDescription"] = ""
                self.send_error_message(self.localization['DescriptionAdviseMessage'])

                if not self.editMode:
                    self.photo_step(message)
                else:
                    self.checkout_step(message)

            elif message.text:
                if len(message.text) > 1000:
                    self.send_error_message(self.localization['LargeDescriptionErrorMessage'])
                    self.next_handler = self.bot.register_next_step_handler(message, self.description_step, acceptMode=acceptMode, chat_id=self.current_user)

                self.data["userDescription"] = message.text

                if not self.editMode:
                    self.photo_step(message)
                else:
                    self.checkout_step(message)
            else:
                self.send_error_message(self.localization['EmptyDescriptionErrorMessage'])
                self.next_handler = self.bot.register_next_step_handler(message, self.description_step, acceptMode=acceptMode, chat_id=self.current_user)

    def photo_step(self, message=None, acceptMode=False, shouldInsert=True):
        if not acceptMode:

            question_counter = self.move_forward()

            self.configure_registration_step(self.photo_step, shouldInsert)

            self.send_active_message(question_counter + self.localization['MediaQuestionMessage'], markup=self.photo_Markup)

            #Warn user about the consequences of changing media
            if self.hasVisited:
                self.bot.send_message(self.current_user, self.localization['ChangeMediaWarningMessage'], reply_markup=self.go_backMarkup)

            self.send_additional_actions_message()
            self.next_handler = self.bot.register_next_step_handler(message, self.photo_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message.id)

            if message.photo:
                self.data["userMedia"] = message.photo[len(message.photo) - 1].file_id  # TODO: troubleshoot photos
                self.data["isMediaPhoto"] = True

                if self.editMode:
                    self.checkout_step(message)
                else:
                    self.gender_preferences_step(message)

            elif message.video:
                if message.video.duration > 15:
                    self.send_error_message(self.localization['LongVideoErrorMessage'])
                    self.next_handler = self.bot.register_next_step_handler(message, self.photo_step, acceptMode=acceptMode, chat_id=self.current_user)
                    return

                self.data["userMedia"] = message.video.file_id
                self.data["isMediaPhoto"] = False

                if self.editMode:
                    self.checkout_step(message)
                else:
                    self.gender_preferences_step(message)

            elif message.text == "Use photo from profile":
                photos = self.bot.get_user_profile_photos(self.current_user, limit=1)

                if photos is None or len(photos.photos) == 0:
                    self.send_error_message("No photos found. Please, check your profile permissions in telegram settings and try again", markup=self.photo_Markup)
                    self.next_handler = self.bot.register_next_step_handler(message, self.photo_step, acceptMode=acceptMode, chat_id=self.current_user)
                    return

                self.data["userMedia"] = photos.photos[0][len(photos.photos[0]) -1].file_id
                self.data["isMediaPhoto"] = True

                if self.editMode:
                    self.checkout_step(message)
                else:
                    self.gender_preferences_step(message)

            elif message.text == self.localization['GoBackButton'] and self.hasVisited:
                self.checkout_step(message)
            else:
                self.send_error_message(self.localization['IncorrectMediaErrorMessage'], markup=self.photo_Markup)
                self.next_handler = self.bot.register_next_step_handler(message, self.photo_step, acceptMode=acceptMode, chat_id=self.current_user)

    def gender_preferences_step(self, shouldInsert=True):
        self.question_index = 11

        question_counter = self.move_forward()

        self.configure_registration_step(self.gender_preferences_step, shouldInsert)

        self.gender_markup.clear()

        for g in self.genders.keys():
            self.gender_markup.add(InlineKeyboardButton(self.genders[g], callback_data=f"{g}"))

        self.gender_markup.add(InlineKeyboardButton("🔙 Go Back", callback_data="-10"))

        self.send_active_message(question_counter + self.localization['GenderQuestionMessage'], markup=self.gender_markup)

    def language_preferences_step(self, message=None, acceptMode=False, shouldInsert=True):
        if not acceptMode:
            self.question_index = 12
            self.markup_page = 1

            question_counter = self.move_forward()

            self.configure_registration_step(self.language_preferences_step, shouldInsert)

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page,
                        self.markup_pages_count)
            count_pages(self.languages, self.current_markup_elements, self.markup_pages_count, True, additionalButton=True, buttonText="🔙 Go Back", buttonData="-10")
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

            self.send_active_message(question_counter + self.localization['LanguagesQuestionMessage'], markup=markup)

            if self.editMode:
                for l in self.pref_langs:
                    add_tick_to_element(self.bot, self.current_user, self.active_message,
                                        self.current_markup_elements, self.markup_page, str(l))

            self.send_secondary_message(self.localization['ChoosMessage'], markup=self.okMarkup)
            self.next_handler = self.bot.register_next_step_handler(message, self.language_preferences_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message.id)

            if not message.text:
                self.send_error_message(self.localization['LanguageNotRecognizedErrorMessage'])
                self.next_handler = self.bot.register_next_step_handler(message, self.spoken_language_step, acceptMode=acceptMode,
                                                    chat_id=self.current_user)
                return False

            message_text = message.text.lower().strip()
            if message_text != self.localization['OkButton']:
                lang = self.spoken_languages_convertor(message_text)
                if lang:  # TODO: Get string, separate by , and process it
                    if lang not in self.pref_langs:
                        if len(self.pref_langs) + 1 > self.premium_limit:
                            self.send_error_message(self.localization['LanguagesPremiumErrorMessage'].format(self.premium_limit), markup=self.okMarkup)
                        else:
                            self.pref_langs.append(lang)
                            add_tick_to_element(self.bot, self.current_user, self.active_message,
                                                self.current_markup_elements,
                                                self.markup_page, str(lang))
                            self.send_error_message(self.localization['AddedMessage'], markup=self.okMarkup)
                    else:
                        self.pref_langs.remove(lang)
                        remove_tick_from_element(self.bot, self.current_user, self.active_message,
                                                 self.current_markup_elements,
                                                 self.markup_page, str(lang))
                        self.send_error_message(self.localization['RemovedMessage'], markup=self.okMarkup)
                    self.next_handler = self.bot.register_next_step_handler(message, self.language_preferences_step, acceptMode=acceptMode,
                                                        chat_id=self.current_user)
                    return True
                else:
                    self.send_error_message(self.localization['LanguageNotRecognizedErrorMessage'])
                    self.suggest_languages(message_text)
                    self.next_handler = self.bot.register_next_step_handler(message, self.language_preferences_step, acceptMode=acceptMode,
                                                        chat_id=self.current_user)
                    return False

            if self.pref_langs:
                self.data["userLanguagePreferences"] = self.pref_langs

                if not self.editMode:
                    if not self.country:
                        self.age_pref_step(message)
                    else:
                        self.location_preferences_step(message)
                else:
                    self.checkout_step(message)

            else:
                self.send_error_message(self.localization['NoLanguagesMessage'])
                self.next_handler = self.bot.register_next_step_handler(message, self.language_preferences_step, acceptMode=acceptMode, chat_id=self.current_user)

    def location_preferences_step(self, message=None, acceptMode=False, shouldInsert=True):
        if not self.country:
            self.checkout_step(message)
            self.send_error_message(self.localization['LocationPrefsErrorMessage'])
            return False

        if not acceptMode:
            self.question_index = 14
            self.markup_page = 1

            question_counter = self.move_forward()

            self.configure_registration_step(self.location_preferences_step, shouldInsert)

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page,
                        self.markup_pages_count)
            count_pages(self.countries, self.current_markup_elements, self.markup_pages_count, True, additionalButton=True, buttonText="🔙 Go Back", buttonData="-10")
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

            self.send_active_message(question_counter + self.localization['LocationPrefsQuestionMessage'], markup=markup)

            if self.editMode:
                for c in self.pref_countries:
                    add_tick_to_element(self.bot, self.current_user, self.active_message,
                                        self.current_markup_elements, self.markup_page, str(c))

            self.send_secondary_message(self.localization['ChoosMessage'], markup=self.okMarkup)
            self.next_handler = self.bot.register_next_step_handler(message, self.location_preferences_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message.id)

            if not message.text:
                self.send_error_message(self.localization['CountyNotRecognizedErrorMessage'])
                self.next_handler = self.bot.register_next_step_handler(message, self.spoken_language_step, acceptMode=acceptMode,
                                                    chat_id=self.current_user)
                return False

            message_text = message.text.lower().strip()
            if message_text != self.localization['OkButton']:
                country = self.country_convertor(message_text)
                if country:  # TODO: Get string, separate by , and process it
                    if country not in self.pref_countries:
                        if len(self.pref_countries) + 1 > self.premium_limit:
                            self.send_error_message(self.localization['CountriesPremiumErrorMessage'].format(self.premium_limit), markup=self.okMarkup)
                        else:
                            self.pref_countries.append(country)
                            add_tick_to_element(self.bot, self.current_user, self.active_message,
                                                self.current_markup_elements,
                                                self.markup_page, str(country))
                            self.send_error_message(self.localization['AddedMessage'], markup=self.okMarkup)
                    else:
                        self.pref_countries.remove(country)
                        remove_tick_from_element(self.bot, self.current_user, self.active_message,
                                                 self.current_markup_elements,
                                                 self.markup_page, str(country))
                        self.send_error_message(self.localization['RemovedMessage'], markup=self.okMarkup)
                    self.next_handler = self.bot.register_next_step_handler(message, self.location_preferences_step, acceptMode=acceptMode, chat_id=self.current_user)
                    return True
                else:
                    self.send_secondary_message(self.localization['ChoosMessage'])
                    self.suggest_countries(message_text)
                    self.next_handler = self.bot.register_next_step_handler(message, self.location_preferences_step, acceptMode=acceptMode, chat_id=self.current_user)
                    return False

            if self.pref_countries:
                self.data["userLocationPreferences"] = self.pref_countries

                if not self.editMode:
                    self.age_pref_step(message)
                else:
                    self.checkout_step(message)

            else:
                self.send_error_message(self.localization['CountryErrorMessage'])
                self.next_handler = self.bot.register_next_step_handler(message, self.location_step, acceptMode=acceptMode, chat_id=self.current_user)

    def age_pref_step(self, message=None, acceptMode=False, shouldInsert=True):
        if not acceptMode:
            question_counter = self.move_forward()

            self.configure_registration_step(self.age_pref_step, shouldInsert)

            age_range = self.generate_personal_age_range()

            self.send_additional_actions_message()
            self.send_active_message(question_counter + self.localization['AgePrefsQuestionMessage'].format(age_range))
            self.next_handler = self.bot.register_next_step_handler(message, self.age_pref_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message.id)

            try:
                if "-" in message.text:
                    nums = message.text.replace(" ", "").split("-")
                    age_pref = list(range(int(nums[0]), int(nums[1])))
                    self.data["agePrefs"] = age_pref

                else:
                    age_pref = list(range(int(message.text) - 3, int(message.text) + 5))
                    self.data["agePrefs"] = age_pref

                if not self.editMode:
                    self.tags_step(message)
                else:
                    self.checkout_step(message)

            except:
                self.send_error_message(self.localization['AgePrefsErrorMessage'], markup=self.age_pref_markup)
                self.next_handler = self.bot.register_next_step_handler(message, self.age_pref_step, acceptMode=acceptMode, chat_id=self.current_user)

    def tags_step(self, message=None, acceptMode=False, shouldInsert=True):
        if not acceptMode:

            question_counter = self.move_forward()

            self.configure_registration_step(self.tags_step, shouldInsert)

            if not self.hasVisited:
                self.send_active_message(question_counter + self.localization['TagsDescritpionMessage'], markup=self.skip_markup)
            else:
                self.send_active_message(question_counter + self.localization['TagsQuestionMessage'], markup=self.skip_markup)

            self.send_additional_actions_message()
            self.next_handler = self.bot.register_next_step_handler(message, self.tags_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message.id)

            if message.text == self.localization['SkipMessage']:
                self.send_error_message(self.localization['GotchaMessage'])
                if not self.editMode:
                    self.auto_reply_step(message)
                else:
                    self.checkout_step(message)
            else:
                if message.text:
                    try:
                        if len(message.text.split()) <= self.maxTagCount:
                            self.tags = message.text.lower().strip()

                            if not self.editMode:
                                self.auto_reply_step(message)
                            else:
                                self.checkout_step(message)
                        else:
                            self.send_error_message(self.localization['ToManyTagsErrorMessage'].format(self.maxTagCount), markup=self.skip_markup)
                            self.next_handler = self.bot.register_next_step_handler(message, self.tags_step, acceptMode=acceptMode, chat_id=self.current_user)
                    except:
                        self.send_error_message(self.localization['TagsErrorMessage'], markup=self.skip_markup)
                        self.next_handler = self.bot.register_next_step_handler(message, self.tags_step, acceptMode=acceptMode, chat_id=self.current_user)

                else:
                    self.send_error_message(self.localization['EmptyErrorMessage'], markup=self.skip_markup)
                    self.next_handler = self.bot.register_next_step_handler(message, self.tags_step, acceptMode=acceptMode, chat_id=self.current_user)

    def auto_reply_step(self, message=None, acceptMode=False, shouldInsert=True):
        if not acceptMode:
            description_message = "---Functionality description---\n"
            question_message = self.localization['AutoreplyDescriptionMessage']

            question_counter = self.move_forward()

            self.configure_registration_step(self.auto_reply_step, shouldInsert)

            if not self.hasVisited:
                question_message = description_message + question_message

            self.send_additional_actions_message()
            self.send_active_message(question_counter + question_message, markup=self.skip_markup)
            self.next_handler = self.bot.register_next_step_handler(message, self.auto_reply_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message.id)

            if message.text == self.localization['SkipMessage']:
                self.send_active_message(self.localization['GotchaMessage'])
                if not self.editMode:
                    self.change_something_step(message)
                    return
                self.checkout_step(message)

            elif message.voice:
                if Helpers.check_user_has_premium(self.current_user):
                    if message.voice.duration <= 15:
                        self.reply_voice = message.voice.file_id
                        if not self.editMode:
                            self.change_something_step(message)
                            return
                        self.checkout_step(message)
                    else:
                        self.send_error_message(self.localization['LongVideoErrorMessage'], markup=self.skip_markup)
                        self.next_handler = self.bot.register_next_step_handler(message, self.auto_reply_step, acceptMode=acceptMode, chat_id=self.current_user)
                else:
                    self.send_error_message(self.localization['PremiumErrorMessage'], markup=self.skip_markup)
                    self.next_handler = self.bot.register_next_step_handler(message, self.auto_reply_step, acceptMode=acceptMode, chat_id=self.current_user)

            elif message.text:
                if len(message.text) < 300:
                    self.reply_text = message.text
                    if not self.editMode:
                        self.change_something_step(message)
                        return
                    self.checkout_step(message)
                else:
                    self.send_error_message(self.localization['ToLongErrorMessage'], markup=self.skip_markup)
                    self.next_handler = self.bot.register_next_step_handler(message, self.auto_reply_step, acceptMode=acceptMode, chat_id=self.current_user)
            else:
                self.send_error_message(self.localization['EmptyErrorMessage'], markup=self.skip_markup)
                self.next_handler = self.bot.register_next_step_handler(message, self.auto_reply_step, acceptMode=acceptMode, chat_id=self.current_user)

    def change_something_step(self, message):
        self.question_index = 13
        if self.data["isMediaPhoto"]:
            self.send_active_message_with_photo(self.localization['CheckoutMessage'].format(self.profile_constructor()), self.data["userMedia"])
        else:
            self.send_active_message_with_video(self.localization['CheckoutMessage'].format(self.profile_constructor()), video=self.data["userMedia"])
        self.send_secondary_message(self.localization['CheckoutQuestionMessage'], markup=self.YNmarkup)

    def checkout_step(self, message=None, acceptMode=False, shouldInsert=True):
        change_markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18")
        final_message = self.localization['FinishRegistrationButton']

        if self.hasVisited:
            final_message = self.localization['SaveChangesButton']

        change_text = self.localization['CheckoutMenuMessage'].format(final_message)

        #Add discard changes button if user exists
        if self.hasVisited:
            change_text += self.localization['DiscardChangesButton']

        self.editMode = True
        if not acceptMode:
            self.configure_registration_step(self.checkout_step, shouldInsert)

            if self.data["isMediaPhoto"]:
                self.send_active_message_with_photo(self.localization['CheckoutMessage'].format(self.profile_constructor()), self.data["userMedia"])
            else:
                self.send_active_message_with_video(self.localization['CheckoutMessage'].format(self.profile_constructor()), self.data["userMedia"])
            self.send_secondary_message(change_text, markup=change_markup)
            self.send_additional_actions_message()
            self.next_handler = self.bot.register_next_step_handler(message, self.checkout_step, acceptMode=True, chat_id=self.current_user)
        else:
            self.delete_message(message.id)

            if message.text == "1":
                self.data["wasChanged"] = True
                self.app_language_step(message, shouldInsert=False)
            elif message.text == "2":
                self.data["wasChanged"] = True
                self.name_step(message, shouldInsert=False)
            elif message.text == "3":
                self.data["wasChanged"] = True
                self.description_step(message, shouldInsert=False)
            elif message.text == "4":
                self.data["wasChanged"] = True
                self.spoken_language_step(message, shouldInsert=False)
            elif message.text == "5":
                self.data["wasChanged"] = True
                self.location_step(message, shouldInsert=False)
            elif message.text == "6":
                self.data["wasChanged"] = True
                self.city_step(message, shouldInsert=False)
            elif message.text == "7":
                self.data["wasChanged"] = True
                self.reason_step(shouldInsert=False)
            elif message.text == "8":
                self.data["wasChanged"] = True
                self.age_step(message, shouldInsert=False)
            elif message.text == "9":
                self.data["wasChanged"] = True
                self.gender_step(shouldInsert=False)
            elif message.text == "10":
                self.data["wasChanged"] = True
                self.photo_step(message, shouldInsert=False)
            elif message.text == "11":
                self.data["wasChanged"] = True
                self.language_preferences_step(message, shouldInsert=False)
            elif message.text == "12":
                self.data["wasChanged"] = True
                self.location_preferences_step(message, shouldInsert=False)
            elif message.text == "13":
                self.data["wasChanged"] = True
                self.age_pref_step(message, shouldInsert=False)
            elif message.text == "14":
                self.data["wasChanged"] = True
                self.gender_preferences_step(shouldInsert=False)
            elif message.text == "15":
                self.data["wasChanged"] = True
                self.communication_preferences_step(shouldInsert=False)
            elif message.text == "16":
                self.data["wasChanged"] = True
                self.tags_step(message, shouldInsert=False)
            elif message.text == "17":
                self.data["wasChanged"] = True
                self.auto_reply_step(message, shouldInsert=False)
            elif message.text == "18":
                self.tests_step(message, shouldInsert=False)
            elif self.hasVisited and message.text == "19":
                self.destruct()
            else:
                self.send_error_message(self.localization['NoSuchOptionMessage'], markup=change_markup)
                self.next_handler = self.bot.register_next_step_handler(message, self.checkout_step, acceptMode=acceptMode, chat_id=self.current_user)

    def tests_step(self, message, shouldInsert=True):
        d = json.dumps(self.data)
        response = None

        self.question_index = 15

        if self.editMode:
            response = requests.post("https://localhost:44381/UpdateUserProfile", d, headers={
                "Content-Type": "application/json"}, verify=False)
        else:
            # del self.data["wasChanged"]
            response = requests.post("https://localhost:44381/RegisterUser", d, headers={
                "Content-Type": "application/json"}, verify=False)

        if self.tags:
            updateTagsModel = json.dumps({
                "userId": self.data["id"],
                "rawTags": self.tags
            })

            requests.post("https://localhost:44381/UpdateTags", updateTagsModel, d, headers={
                "Content-Type": "application/json"}, verify=False)

        if self.reply_voice:
            requests.get(f"https://localhost:44381/SetAutoReplyVoice/{self.current_user}/{self.reply_voice}", verify=False)
        elif self.reply_text:
            requests.get(f"https://localhost:44381/SetAutoReplyText/{self.current_user}/{self.reply_text}", verify=False)

        if self.hasVisited:
            if response.text == "1":
                self.bot.send_message(self.current_user, self.localization['ChangesSavedMessage'])
                self.destruct()
                return False
            self.send_error_message(self.localization['ChangesErrorMessage'])
            self.destruct()
            return False

        self.send_secondary_message(self.localization['OceanDescriptionMessage'], markup=self.YNmarkup)
        self.next_handler = self.bot.register_next_step_handler(message, self.tests_step, acceptMode=True, chat_id=self.current_user)


    def profile_constructor(self):

        cityName = "---"
        countryName = "---"

        if self.country:
            cityName = self.cities[self.data['userCityCode']]
            countryName = self.countries[self.data['userCountryCode']]

        if self.tags:
            return f"{self.data['userRealName']}, {countryName}, {cityName}\n{self.data['userAge']}\n{self.data['userDescription']}\n\n{self.tags}"

        return f"{self.data['userRealName']}, {countryName}, {cityName}\n{self.data['userAge']}\n{self.data['userDescription']}"

    def send_encourage_message(self, text):
        #If User is registering his profile
        if not self.hasVisited:
            self.bot.send_message(self.current_user, f"<i><b>{text}</b></i>")

    def spoken_languages_convertor(self, lang):
        for l in self.languages:
            if lang == self.languages[l]:
                return l
        return None

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

    def app_language_converter(self, lang):
        for l in self.app_langs:
            if lang == self.app_langs[l]:
                return l
        return None

    def send_active_message(self, text, markup=None):
        try:
            if self.active_message:
                self.bot.edit_message_text(text, self.current_user, self.active_message, reply_markup=markup)
                return

            self.active_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except Exception as ex:
            self.delete_active_message()
            self.send_active_message(text, markup)

    def send_active_message_with_photo(self, text, photo, markup=None):
        self.delete_active_message()
        self.active_message = self.bot.send_photo(self.current_user, photo, text, reply_markup=markup).id

    def send_active_message_with_video(self, text, video, markup=None):
        self.delete_active_message()
        self.active_message = self.bot.send_video(self.current_user, video, text, reply_markup=markup).id

    def send_secondary_message(self, text, markup=None):
        try:
            if self.secondary_message:
                self.bot.edit_message_text(text, self.current_user, self.secondary_message, reply_markup=markup)
                return

            self.secondary_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except:
            self.delete_secondary_message()
            self.send_secondary_message(text, markup)

    def send_error_message(self, text, markup=None):
        try:
            if self.error_message:
                self.bot.edit_message_text(text, self.current_user, self.secondary_message, reply_markup=markup)
                return

            self.error_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except:
            self.delete_error_message()
            self.send_secondary_message(text, markup)

    def send_additional_actions_message(self):
        self.delete_additional_message()
        self.additional_message = self.bot.send_message(self.current_user, "Additional actions",
                                                        reply_markup=self.goBackInlineMarkup).id

    def delete_additional_message(self):
        try:
            if self.additional_message:
                self.bot.delete_message(self.current_user, self.additional_message)
                self.additional_message = None
        except:
            pass

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

    def go_back_to_previous_registration_step(self):
        self.remove_registration_handler()

        self.delete_additional_message()

        self.question_count -= 2

        self.previous_section = self.registration_steps[0]
        self.registration_steps.pop(0)
        self.previous_section(shouldInsert=False)

    def configure_registration_step(self, step, shouldInsert):
        if shouldInsert:
            self.registration_steps.insert(0, self.current_section)
        self.current_section = step

    def remove_registration_handler(self):
        try:
            self.bot.remove_next_step_handler(self.current_user, self.next_handler)
        except:
            pass

    def callback_handler(self, call):

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

        elif call.data == "-10":
            self.go_back_to_previous_registration_step()

        elif self.question_index == 2:
            if int(call.data) not in self.chosen_langs:
                #Notify user if limit had been exceeded
                if len(self.chosen_langs) + 1 > self.premium_limit:
                    self.bot.send_message(self.current_user, self.localization['LanguagesPremiumErrorMessage'].format(self.premium_limit))
                    return False
                else:
                    # self.bot.send_message(chatId, call.data)
                    self.chosen_langs.append(int(call.data))
                    self.bot.answer_callback_query(call.id, self.localization['AddedMessage'])
                    add_tick_to_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                        self.markup_page, call.data)

            else:
                self.chosen_langs.remove(int(call.data))
                self.bot.answer_callback_query(call.id, self.localization['RemovedMessage'])
                remove_tick_from_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                         self.markup_page, call.data)

        elif self.question_index == 3:
            gender = call.data
            self.data["userGender"] = gender

            if not self.editMode:
                self.reason_step()
            else:
                self.checkout_step(call.message)

        elif self.question_index == 4:
            if int(call.data) in self.countries.keys():
                self.country = int(call.data)
                self.bot.answer_callback_query(call.id, self.localization['GotchaMessage'])
                if self.previous_item:
                    remove_tick_from_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                             self.markup_page, self.previous_item)
                add_tick_to_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                    self.markup_page, call.data)
                self.previous_item = call.data
            else:
                self.bot.send_message(call.message.chat.id, self.localization['CountyNotRecognizedErrorMessage'])

        elif self.question_index == 5:
            if int(call.data) in self.cities.keys():
                self.city = int(call.data)
                self.bot.answer_callback_query(call.id, self.localization['GotchaMessage'])
                if self.previous_item:
                    remove_tick_from_element(self.bot, self.current_user, call.message.id,
                                             self.current_markup_elements,
                                             self.markup_page, self.previous_item)
                add_tick_to_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                    self.markup_page, call.data)
                self.previous_item = call.data
            else:
                self.bot.answer_callback_query(call.id, self.localization['CountyNotRecognizedErrorMessage'])

        elif self.question_index == 9:
            self.data["reasonId"] = call.data

            if not self.editMode:
                self.communication_preferences_step()
            else:
                self.checkout_step(call.message)

        elif self.question_index == 10:
            self.data["communicationPrefs"] = call.data

            # TODO: Find out what the fuck is that ?
            if not self.editMode:
                self.location_step(call.message)
            else:
                #If prefs are not 'Online' and there is no country chosen
                if call.data != "1" and not self.data["userCountryCode"]:
                    self.location_step(call.message)
                else:
                    self.checkout_step(call.message)

        elif self.question_index == 11:
            gender = call.data
            self.data["userGenderPrefs"] = gender

            if not self.editMode:
                self.language_preferences_step(call.message)
            else:
                self.checkout_step(call.message)

        elif self.question_index == 12:
            if int(call.data) == -5:

                remove_tick_from_elements(self.bot, self.current_user, call.message.id, self.current_markup_elements, self.markup_page, self.pref_langs)
                add_tick_to_elements(self.bot, self.current_user, self.active_message, self.current_markup_elements, self.markup_page, self.chosen_langs)
                for l in self.chosen_langs:
                    if l not in self.pref_langs:
                        add_tick_to_element(self.bot, self.current_user, self.active_message, self.current_markup_elements, self.markup_page, str(l))
                        self.pref_langs.append(l)
                self.bot.answer_callback_query(call.id, self.localization['AddedMessage'])
                self.send_error_message(self.localization['SameAsMineMessage'],
                                      markup=self.okMarkup)

            elif int(call.data) not in self.pref_langs:
                if len(self.pref_langs) + 1 > self.premium_limit:
                    self.bot.send_message(self.current_user, self.localization['LanguagesPremiumErrorMessage'].format(self.premium_limit))
                    return False
                else:
                    self.pref_langs.append(int(call.data))
                    self.bot.answer_callback_query(call.id, self.localization['AddedMessage'])
                    add_tick_to_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                        self.markup_page, call.data)
            else:
                self.pref_langs.remove(int(call.data))
                self.bot.answer_callback_query(call.id, self.localization['RemovedMessage'])
                remove_tick_from_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                         self.markup_page, call.data)

        elif self.question_index == 13:
            if call.data == "1":
                self.checkout_step(call.message)
                return

            self.tests_step(call.message)

        elif self.question_index == 14:
            if int(call.data) == -5:
                if self.country not in self.pref_countries:
                    remove_tick_from_elements(self.bot, self.current_user, call.message.id, self.current_markup_elements, self.markup_page, self.pref_countries)
                    self.pref_countries.append(self.country)
                    self.send_error_message(self.localization['SameAsMineMessage'], markup=self.okMarkup)
                    self.bot.answer_callback_query(call.id, self.localization['AddedMessage'])

                    add_tick_to_element(self.bot, self.current_user, self.active_message, self.current_markup_elements, self.markup_page, str(self.country))

            elif int(call.data) not in self.pref_countries:
                if len(self.pref_countries) + 1 > self.premium_limit:
                    self.bot.send_message(self.current_user, self.localization['LanguagesPremiumErrorMessage'].format(self.premium_limit))
                    return False
                else:
                    self.pref_countries.append(int(call.data))
                    self.bot.answer_callback_query(call.id, self.localization['AddedMessage'])
                    add_tick_to_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                        self.markup_page, call.data)
            else:
                self.pref_countries.remove(int(call.data))
                self.bot.answer_callback_query(call.id, self.localization['RemovedMessage'])
                remove_tick_from_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                         self.markup_page, call.data)

        elif self.question_index == 15:
            if call.data == "1":
                self.bot.send_message(self.current_user, self.localization['HelperDescriptionMessage'])
                TestModule(self.bot, self.message, isActivatedFromShop=False)

            self.destruct()

    def move_forward(self) -> str:
        self.delete_secondary_message()
        self.delete_error_message()

        if self.editMode:
            return ""

        self.question_count += 1
        return f"<b>{self.question_count} / {self.max_questions_count}</b>\n\n"

    def delete_message(self, message_id):
        self.bot.delete_message(self.current_user, message_id)

    def generate_personal_age_range(self):
        min_value = self.data["userAge"] - 3
        max_value = self.data["userAge"] + 5

        #Age below 12 is forbidden, thus, no need for that :)
        # if min_value:
        #     min_value = 0
        #
        # if max_value > 100:
        #     max_value = 100

        return f"{min_value} - {max_value}"

    #TODO: Send Accept-Language header !
    def get_localisations(self):
        for language in json.loads(
                requests.get(f"https://localhost:44381/GetLanguages/{self.app_language}", verify=False).text):
            self.languages[language["id"]] = language["languageName"].lower().strip()

        for gender in json.loads(
                requests.get(f"https://localhost:44381/genders", verify=False).text):
            self.genders[gender["id"]] = gender["name"].strip()

        for country in json.loads(
                requests.get(f"https://localhost:44381/GetCountries/{self.app_language}", verify=False).text):
            self.countries[country["id"]] = country["countryName"].lower().strip()

        for reason in json.loads(
                requests.get(f"https://localhost:44381/usage-reasons", verify=False).text):
            self.reasons[reason["id"]] = reason["name"].strip()

        for pref in json.loads(requests.get(f"https://localhost:44381/communication-preferences",
                                            verify=False).text):
            self.communication_pref[pref["id"]] = pref["name"].strip()

    def suggest_languages(self, language):
        languages = Helpers.suggest_languages(language)

        if languages:
            self.bot.send_message(self.current_user, self.localization['ClarifyingQuestionMessage'].format(', '.join(languages)))

    def suggest_countries(self, country):
        countries = Helpers.suggest_countries(country)

        if countries:
            self.bot.send_message(self.current_user, self.localization['ClarifyingQuestionMessage'].format(', '.join(countries)))

    def suggest_cities(self, city):
        cities = Helpers.suggest_cities(city)

        if cities:
            self.bot.send_message(self.current_user, self.localization['ClarifyingQuestionMessage'].format(', '.join(cities)))

    def destruct(self, shouldInsert=False):
        self.delete_active_message()
        self.delete_secondary_message()
        self.delete_error_message()
        self.delete_additional_message()

        self.bot.callback_query_handlers.remove(self.chCode)
        if self.hasVisited:
            Helpers.switch_user_busy_status(self.current_user, 12)
        if self.return_method:
            self.return_method()
        else:
            go_back_to_main_menu(self.bot, self.current_user, self.message)
        del self