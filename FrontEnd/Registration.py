import copy

import telegram
from telebot.types import KeyboardButton, ReplyKeyboardMarkup
from Core import HelpersMethodes as Helpers
from Common.Menues import count_pages, assemble_markup, reset_pages, add_tick_to_element, add_tick_to_elements, remove_tick_from_element, remove_tick_from_elements, index_converter
import requests
import json
from Core.Localization import Localization
from Common.Menues import go_back_to_main_menu
from TestModule import TestModule


class Registrator:
    def __init__(self, bot=None, msg=None, hasVisited=False, return_method=None, localizationIndex=None, promoCode=None):
        self.bot = bot
        self.msg = msg
        self.return_method = return_method
        self.previous_item = '' #Is used to remove a tick from single-type items (country, city, etc..)
        self.current_inline_message_id = 0 #Represents current message with inline markup
        self.current_user = msg.from_user.id
        self.hasVisited = hasVisited
        self.localization = Localization.get_registrator_localization(localizationIndex)
        self.okMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(KeyboardButton(self.localization['OkButton']))
        self.YNmarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(self.localization['YesButton'], self.localization['NoButton'])
        self.chCode = self.bot.register_callback_query_handler("", self.callback_handler, user_id=self.current_user)

        self.premium_limit = Helpers.get_user_language_limit(self.current_user)

        self.promo = promoCode

        self.question_index = 0 #Represents current question index

        self.current_query = 0
        self.old_queries = []

        self.tags = ""
        self.maxTagCount = Helpers.get_user_limitations(self.current_user)["maxTagsPerSearch"]

        self.current_markup_elements = []
        self.markup_last_element = 0
        self.markup_page = 1
        self.markup_pages_count = 0

        self.app_languages_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)
        self.gender_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)
        self.reason_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)
        self.communication_pref_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)
        self.age_pref_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)
        self.skip_markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(self.localization['SkipMessage'])
        self.go_backMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(self.localization['GoBackButton'])

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

        if not hasVisited:
            self.get_localisations()
            self.spoken_language_step(msg)
        else:
            self.current_user_data = Helpers.get_user_info(self.current_user)
            if self.current_user_data:
                data = self.current_user_data["data"]
                settings = self.current_user_data["settings"]

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

                self.checkout_step(msg)
            else:
                self.spoken_language_step(msg)

    def app_language_step(self, msg, acceptMode=False, editMode=False):
        if not acceptMode:
            self.question_index = 1

            for lang in json.loads(requests.get("https://localhost:44381/app-languages", verify=False).text):
                self.app_langs[lang["id"]] = lang["languageNameShort"]

            self.app_languages_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)
            start_message = "START MESSAGE" #self.localisation[0]["loc"][0]["elementText"]
            b = []

            for lang in self.app_langs.values():
                b.append(KeyboardButton(lang))

            self.app_languages_markup.add(b[0], b[1], b[2])
            self.bot.send_message(msg.chat.id, start_message, reply_markup=self.app_languages_markup)
            self.bot.register_next_step_handler(msg, self.app_language_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)

        else:
            self.app_language = self.app_language_converter(msg.text)
            if self.app_language is not None:

                self.checkout_step(msg)
                self.data["userAppLanguageId"] = self.app_language

            else:
                self.bot.send_message(self.current_user, self.localization['LanguageNotRecognizedErrorMessage'], reply_markup=self.app_languages_markup)
                self.bot.register_next_step_handler(msg, self.app_language_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

    def spoken_language_step(self, msg, acceptMode=False, editMode=False):
        if not acceptMode:
            self.question_index = 2
            self.markup_page = 1

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page, self.markup_pages_count)
            count_pages(self.languages, self.current_markup_elements, self.markup_pages_count)
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)
            self.markup_page = 1

            self.current_inline_message_id = self.bot.send_message(self.msg.chat.id, self.localization['SateLanguagesMessage'], reply_markup=markup).json['message_id']

            #Add ticks if in edit mode
            if editMode:
                for l in self.chosen_langs:
                    add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id, self.current_markup_elements, self.markup_page, str(l))

            self.bot.send_message(self.msg.chat.id, self.localization['ChooseLanguageMessage'], reply_markup=self.okMarkup)
            self.bot.register_next_step_handler(msg, self.spoken_language_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            if not msg.text:
                self.bot.send_message(self.current_user,
                                      self.localization['LanguageNotRecognizedErrorMessage'])
                self.bot.register_next_step_handler(msg, self.spoken_language_step, acceptMode=acceptMode,
                                                    editMode=editMode, chat_id=self.current_user)
                return False

            msg_text = msg.text.lower().strip()
            if msg_text != self.localization['OkButton']:
                lang = self.spoken_languages_convertor(msg_text)
                if lang:  # TODO: Get string, separate by , and process it
                    if lang not in self.chosen_langs:
                        if len(self.chosen_langs) + 1 > self.premium_limit:
                            self.bot.send_message(self.current_user, self.localization['LanguagesPremiumErrorMessage'].format(self.premium_limit), reply_markup=self.okMarkup)
                        else:
                            self.chosen_langs.append(lang)
                            add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id,
                                                self.current_markup_elements,
                                                self.markup_page, str(lang))
                            self.bot.send_message(self.current_user, self.localization['AddedMessage'], reply_markup=self.okMarkup)
                    else:
                        self.chosen_langs.remove(lang)
                        remove_tick_from_element(self.bot, self.current_user, self.current_inline_message_id,
                                                 self.current_markup_elements,
                                                 self.markup_page, str(lang))
                        self.bot.send_message(self.current_user, self.localization['RemovedMessage'], reply_markup=self.okMarkup)
                    self.bot.register_next_step_handler(msg, self.spoken_language_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)
                    return True
                else:
                    self.bot.send_message(self.current_user, self.localization['LanguageNotRecognizedErrorMessage'])
                    self.bot.register_next_step_handler(msg, self.spoken_language_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)
                    return False

            if self.chosen_langs:
                self.old_queries.append(self.current_query)
                self.data["userLanguages"] = self.chosen_langs

                if not editMode:
                    self.data["id"] = self.current_user
                    self.data["userName"] = msg.from_user.username
                    self.data["userAppLanguageId"] = self.app_language
                    self.data["promo"] = self.promo

                    self.gender_step(msg)
                else:
                    self.checkout_step(msg)

            else:
                self.bot.send_message(self.current_user, self.localization['NoLanguagesMessage'],
                                      reply_markup=self.okMarkup)
                self.bot.register_next_step_handler(msg, self.spoken_language_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

    def gender_step(self, msg, acceptMode=False, editMode=False):
        if not acceptMode:
            self.question_index = 3

            self.gender_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)

            genders = copy.copy(self.genders)
            genders.pop(4) # Remove "Does not matter"

            for g in genders.keys():
                self.gender_markup.row().add(KeyboardButton(self.genders[g]))

            self.bot.send_message(self.msg.chat.id, self.localization['GenderQuestionMessage'], reply_markup=self.gender_markup)
            self.bot.register_next_step_handler(msg, self.gender_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            gender = self.gender_converter(msg.text)
            if gender or gender == 0:
                self.data["userGender"] = self.gender_converter(msg.text)

                if not editMode:
                    self.reason_step(msg)
                else:
                    self.checkout_step(msg)

            else:
                self.bot.send_message(self.current_user, self.localization['ChooseExistingMessage'],
                                      reply_markup=self.gender_markup)
                self.bot.register_next_step_handler(msg, self.gender_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

    def reason_step(self, msg, acceptMode=False, editMode=False):
        if not acceptMode:
            self.question_index = 9

            # self.send_encourage_message("Wow. You already here...")

            for reason in self.reasons.values():
                self.reason_markup.add(KeyboardButton(reason))
            self.bot.send_message(msg.chat.id, self.localization['SearchQuestionMessage'], reply_markup=self.reason_markup)
            self.bot.register_next_step_handler(msg, self.reason_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            reason = self.reason_convertor(msg.text)
            if reason or reason == 0:
                self.data["reasonId"] = reason

                if not editMode:
                    self.communication_preferences_step(msg)
                else:
                    self.checkout_step(msg)

            else:
                self.bot.send_message(msg.chat.id, self.localization['InvalidReasonErrorMessage'],
                                      reply_markup=self.reason_markup)
                self.bot.register_next_step_handler(msg, self.reason_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

    def communication_preferences_step(self, msg, acceptMode=False, editMode=False):
        if not acceptMode:
            self.question_index = 13


            self.communication_pref_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)

            for pref in self.communication_pref:
                self.communication_pref_markup.add(self.communication_pref[pref])

            self.bot.send_message(msg.chat.id, self.localization['CommunicationQuestionMessage'], reply_markup=self.communication_pref_markup)

            self.bot.register_next_step_handler(msg, self.communication_preferences_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            comm_pref = self.communication_pref_convertor(msg.text)
            if comm_pref or comm_pref == 0:
                self.old_queries.append(self.current_query)
                self.data["communicationPrefs"] = comm_pref

                if not editMode:
                    self.location_step(msg)
                else:
                    #If prefs are not 'Online' and there is no country chosen
                    if comm_pref != 1 and not self.data["userCountryCode"]:
                        self.location_step(msg, editMode=editMode)
                    else:
                        self.checkout_step(msg)

            else:
                self.bot.send_message(self.current_user, self.localization['NoSuchOptionMessage'],
                                      reply_markup=self.communication_pref_markup)
                self.bot.register_next_step_handler(msg, self.communication_preferences_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

    def location_step(self, msg, acceptMode=False, editMode=False):
        if not acceptMode:
            self.question_index = 4
            self.markup_page = 1

            # self.send_encourage_message("You have done good job answering our questions. Keep it up!")

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page,
                        self.markup_pages_count)
            count_pages(self.countries, self.current_markup_elements, self.markup_pages_count)
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

            self.previous_item = ''

            self.current_inline_message_id = \
                self.bot.send_message(self.msg.chat.id, self.localization['CountryQuestionMessage'], reply_markup=markup).id#.json[
            #'message_id']

            if editMode:
                self.previous_item = str(self.country)
                add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id, self.current_markup_elements, self.markup_page, str(self.country))


            #Allow skipping location part if user prefers to communicate online
            if self.data["communicationPrefs"] == 1:
                m = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(self.localization['NoMatterButton'], row_width=2).add(self.localization['OkButton'], row_width=1)
                self.bot.send_message(self.current_user, self.localization['ChooseOrSkipMessage'], reply_markup=m)
            else:
                self.bot.send_message(self.current_user, self.localization['ChooseCountryMessage'], reply_markup=self.okMarkup)

            self.bot.register_next_step_handler(msg, self.location_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            if not msg.text:
                self.bot.send_message(self.current_user,
                                      self.localization['CountyNotRecognizedErrorMessage'])
                self.bot.register_next_step_handler(msg, self.location_step, acceptMode=acceptMode,
                                                    editMode=editMode, chat_id=self.current_user)
                return False

            msg_text = msg.text.lower().strip()
            if msg_text != self.localization['OkButton']:
                country = self.country_convertor(msg_text)
                if country:
                    if self.previous_item:
                        remove_tick_from_element(self.bot, self.current_user, self.current_inline_message_id,
                                                 self.current_markup_elements,
                                                 self.markup_page, self.previous_item)
                    self.previous_item = str(country)
                    add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id,
                                        self.current_markup_elements,
                                        self.markup_page, str(country))
                    self.country = country
                    self.bot.send_message(self.current_user, self.localization['GotchaMessage'], reply_markup=self.okMarkup)
                    self.bot.register_next_step_handler(msg, self.location_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)
                    return True
                elif msg_text == self.localization['NoMatterButton'] and self.data["communicationPrefs"] == 1:
                    if self.data["communicationPrefs"] == 1:

                        #In case user had chosen a country before pressing 'Does not matter' button
                        self.country = None
                        self.city = None
                        self.pref_countries.clear()
                        self.data["userLocationPreferences"] = None
                        self.data["userCityCode"] = None
                        self.data["userCountryCode"] = None

                        if not editMode:
                            self.bot.send_message(self.current_user,
                                                  self.localization['RestrictLocationMessage'])
                            self.name_step(msg)
                            return False
                        else:
                            self.checkout_step(msg)
                            self.bot.send_message(self.current_user,
                                                  self.localization['LocationResetMessage'])
                            return False
                else:
                    self.bot.send_message(self.current_user, self.localization['CountyNotRecognizedErrorMessage'])
                    self.suggest_countries(msg_text)
                    self.bot.register_next_step_handler(msg, self.location_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)
                    return False

            if self.country or self.country == 0:
                self.old_queries.append(self.current_query)
                self.data["userCountryCode"] = self.country
                self.previous_item = ''

                if editMode:
                    self.city = None

                self.city_step(msg, editMode=editMode)
            else:
                self.bot.send_message(self.current_user, self.localization['CountryErrorMessage'], reply_markup=self.okMarkup)
                self.bot.register_next_step_handler(msg, self.location_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

    def city_step(self, msg, acceptMode=False, editMode=False):
        if not self.country:
            self.checkout_step(msg)
            self.bot.send_message(self.current_user,
                                  self.localization['CityErrorMessage1'])
            return False

        if not acceptMode:
            self.question_index = 5
            self.markup_page = 1

            cities = json.loads(
                requests.get(f"https://localhost:44381/GetCities/{self.country}/{self.app_language}", verify=False).text)

            #For edit purposes. If left as they are -> can result bugs
            self.cities.clear()

            for city in cities:
                self.cities[city["id"]] = city["cityName"].lower()

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page, self.markup_pages_count)
            count_pages(self.cities, self.current_markup_elements, self.markup_pages_count)
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

            self.current_inline_message_id = self.bot.send_message(self.msg.chat.id, self.localization['ChooseCityMessage'], reply_markup=markup).json['message_id']

            if editMode:
                self.previous_item = str(self.city)
                add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id,
                                    self.current_markup_elements, self.markup_page, str(self.city))

            self.bot.send_message(self.msg.chat.id, self.localization['ChoosMessage'], reply_markup=self.okMarkup)
            self.bot.register_next_step_handler(msg, self.city_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            if not msg.text:
                self.bot.send_message(self.current_user,
                                      self.localization['CityNotRecognizedErrorMessage'])
                self.bot.register_next_step_handler(msg, self.spoken_language_step, acceptMode=acceptMode,
                                                    editMode=editMode, chat_id=self.current_user)
                return False

            msg_text = msg.text.lower().strip()
            if msg_text != self.localization['OkButton']:
                city = self.city_convertor(msg_text)
                if city:
                    if self.previous_item:
                        remove_tick_from_element(self.bot, self.current_user, self.current_inline_message_id,
                                                 self.current_markup_elements,
                                                 self.markup_page, self.previous_item)
                    self.previous_item = str(city)
                    add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id,
                                        self.current_markup_elements,
                                        self.markup_page, str(city))
                    self.city = city
                    self.bot.send_message(self.current_user, self.localization['GotchaMessage'], reply_markup=self.okMarkup)
                    self.bot.register_next_step_handler(msg, self.city_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)
                    return True
                else:
                    self.bot.send_message(self.current_user, self.localization['CityNotRecognizedErrorMessage'])
                    self.suggest_cities(msg_text)
                    self.bot.register_next_step_handler(msg, self.city_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)
                    return False

            if self.city or self.city == 0:
                self.old_queries.append(self.current_query)
                self.previous_item = ''
                self.data["userCityCode"] = self.city

                if not editMode:
                    self.name_step(msg)
                else:
                    self.checkout_step(msg)

                # self.data["userCity"] = self.cities[self.country][self.city]
            else:
                self.bot.send_message(self.current_user, self.localization['CityErrorMessage2'], reply_markup=self.okMarkup)
                self.bot.register_next_step_handler(msg, self.city_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

    def name_step(self, msg, acceptMode=False, editMode=False):
        if not acceptMode:
            self.question_index = 6

            self.bot.send_message(msg.chat.id, self.localization['NameQuestionMessage'])
            self.bot.register_next_step_handler(msg, self.name_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            if msg.text:
                self.data["userRealName"] = msg.text

                if len(msg.text) > 50:
                    self.bot.send_message(self.current_user, self.localization['LongNameErrorMessage'])
                    self.bot.register_next_step_handler(msg, self.name_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)
                    return

                if not editMode:
                    self.age_step(msg)
                else:
                    self.checkout_step(msg)

            else:
                self.bot.send_message(self.current_user, self.localization['EmptyNameErrorMessage'])
                self.bot.register_next_step_handler(msg, self.name_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)


    def age_step(self, msg, acceptMode=False, editMode=False):
        if not acceptMode:
            self.question_index = 7

            self.bot.send_message(msg.chat.id, self.localization['AgeQuestionMessage'])
            self.bot.register_next_step_handler(msg, self.age_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            try:
                age = int(msg.text)
                if age > 100:
                    self.bot.send_message(msg.chat.id, self.localization['HighAgeErrorMessage'])
                    self.bot.register_next_step_handler(msg, self.age_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)
                    return
                elif age < 14:
                    self.bot.send_message(msg.chat.id, self.localization['UnderagedErrorMessage'])
                    self.bot.register_next_step_handler(msg, self.age_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)
                    return

                self.data["userAge"] = age

                if not editMode:
                    self.description_step(msg)
                else:
                    self.checkout_step(msg)

            except:
                self.bot.send_message(msg.chat.id, self.localization['DigitalAgeErrorMessage'])
                self.bot.register_next_step_handler(msg, self.age_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

    def description_step(self, msg, acceptMode=False, editMode=False):
        if not acceptMode:
            self.question_index = 8

            self.bot.send_message(msg.chat.id, self.localization['DescritpionQuestionMessage'], reply_markup=self.skip_markup)
            self.bot.register_next_step_handler(msg, self.description_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            if msg.text == self.localization['SkipMessage']:
                self.data["userDescription"] = ""
                self.bot.send_message(self.current_user, self.localization['DescriptionAdviseMessage'])

                if not editMode:
                    self.photo_step(msg)
                else:
                    self.checkout_step(msg)

            elif msg.text:
                if len(msg.text) > 1000:
                    self.bot.send_message(self.current_user, self.localization['LargeDescriptionErrorMessage'])
                    self.bot.register_next_step_handler(msg, self.description_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

                self.data["userDescription"] = msg.text

                if not editMode:
                    self.photo_step(msg)
                else:
                    self.checkout_step(msg)
            else:
                self.bot.send_message(msg.chat.id,
                                      self.localization['EmptyDescriptionErrorMessage'])
                self.bot.register_next_step_handler(msg, self.description_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

    def photo_step(self, msg, acceptMode=False, editMode=False):
        if not acceptMode:
            self.question_index = 10

            self.bot.send_message(msg.chat.id, self.localization['MediaQuestionMessage'])

            #Warn user about the consequences of changing media
            if self.hasVisited:
                self.bot.send_message(self.current_user, self.localization['ChangeMediaWarningMessage'], reply_markup=self.go_backMarkup)

            self.bot.register_next_step_handler(msg, self.photo_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            if msg.photo:
                self.data["userMedia"] = msg.photo[len(msg.photo) - 1].file_id  # TODO: troubleshoot photos
                self.data["isMediaPhoto"] = True
                self.gender_preferences_step(msg, editMode=editMode)
            elif msg.video:
                if msg.video.duration > 15:
                    self.bot.send_message(self.current_user, self.localization['LongVideoErrorMessage'])
                    self.bot.register_next_step_handler(msg, self.photo_step, acceptMode=acceptMode, editMode=editMode,chat_id=self.current_user)
                    return

                self.data["userMedia"] = msg.video.file_id
                self.data["isMediaPhoto"] = False
                self.gender_preferences_step(msg, editMode=editMode)
            elif msg.text == self.localization('GoBackButton') and self.hasVisited:
                self.checkout_step(msg)
            else:
                self.bot.send_message(self.current_user, self.localization['IncorrectMediaErrorMessage'])
                self.bot.register_next_step_handler(msg, self.photo_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

    def gender_preferences_step(self, msg, acceptMode=False, editMode=False):
        if not acceptMode:
            self.question_index = 11

            self.gender_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)

            for g in self.genders.keys():
                self.gender_markup.row().add(KeyboardButton(self.genders[g]))

            self.bot.send_message(self.msg.chat.id, self.localization['GenderQuestionMessage'], reply_markup=self.gender_markup)
            self.bot.register_next_step_handler(msg, self.gender_preferences_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            gender = self.gender_converter(msg.text)
            if gender or gender == 0:
                self.data["userGenderPrefs"] = gender

                if not editMode:
                    self.language_preferences_step(msg)
                else:
                    self.checkout_step(msg)

            else:
                self.bot.send_message(self.current_user, self.localization['NoSuchOptionMessage'],
                                      reply_markup=self.gender_markup)
                self.bot.register_next_step_handler(msg, self.gender_preferences_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

    def language_preferences_step(self, msg, acceptMode=False, editMode=False):
        if not acceptMode:
            self.question_index = 12
            self.markup_page = 1

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page,
                        self.markup_pages_count)
            count_pages(self.languages, self.current_markup_elements, self.markup_pages_count, True)
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

            self.current_inline_message_id = \
                self.bot.send_message(msg.chat.id, self.localization['LanguagesQuestionMessage'],
                                      reply_markup=markup, parse_mode=telegram.ParseMode.HTML).id

            if editMode:
                for l in self.pref_langs:
                    add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id,
                                        self.current_markup_elements, self.markup_page, str(l))

            self.bot.send_message(self.msg.chat.id, self.localization['ChoosMessage'],
                                  reply_markup=self.okMarkup)
            self.bot.register_next_step_handler(msg, self.language_preferences_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            if not msg.text:
                self.bot.send_message(self.current_user,
                                      self.localization['LanguageNotRecognizedErrorMessage'])
                self.bot.register_next_step_handler(msg, self.spoken_language_step, acceptMode=acceptMode,
                                                    editMode=editMode, chat_id=self.current_user)
                return False

            msg_text = msg.text.lower().strip()
            if msg_text != self.localization['OkButton']:
                lang = self.spoken_languages_convertor(msg_text)
                if lang:  # TODO: Get string, separate by , and process it
                    if lang not in self.pref_langs:
                        if len(self.pref_langs) + 1 > self.premium_limit:
                            self.bot.send_message(self.current_user, self.localization['LanguagesPremiumErrorMessage'].format(self.premium_limit), reply_markup=self.okMarkup)
                        else:
                            self.pref_langs.append(lang)
                            add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id,
                                                self.current_markup_elements,
                                                self.markup_page, str(lang))
                            self.bot.send_message(self.current_user, self.localization['AddedMessage'], reply_markup=self.okMarkup)
                    else:
                        self.pref_langs.remove(lang)
                        remove_tick_from_element(self.bot, self.current_user, self.current_inline_message_id,
                                                 self.current_markup_elements,
                                                 self.markup_page, str(lang))
                        self.bot.send_message(self.current_user, self.localization['RemovedMessage'], reply_markup=self.okMarkup)
                    self.bot.register_next_step_handler(msg, self.language_preferences_step, acceptMode=acceptMode, editMode=editMode,
                                                        chat_id=self.current_user)
                    return True
                else:
                    self.bot.send_message(self.current_user, self.localization['LanguageNotRecognizedErrorMessage'])
                    self.suggest_languages(msg_text)
                    self.bot.register_next_step_handler(msg, self.language_preferences_step, acceptMode=acceptMode, editMode=editMode,
                                                        chat_id=self.current_user)
                    return False

            if self.pref_langs:
                self.old_queries.append(self.current_query)
                self.data["userLanguagePreferences"] = self.pref_langs

                if not editMode:
                    if not self.country:
                        self.age_pref_step(msg)
                    else:
                        self.location_preferences_step(msg)
                else:
                    self.checkout_step(msg)

            else:
                self.bot.send_message(self.current_user, self.localization['NoLanguagesMessage'])
                self.bot.register_next_step_handler(msg, self.language_preferences_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

    def location_preferences_step(self, msg, acceptMode=False, editMode=False):
        if not self.country:
            self.checkout_step(msg)
            self.bot.send_message(self.current_user,
                                  self.localization['LocationPrefsErrorMessage'])
            return False

        if not acceptMode:
            self.question_index = 14
            self.markup_page = 1

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page,
                        self.markup_pages_count)
            count_pages(self.countries, self.current_markup_elements, self.markup_pages_count, True)
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

            self.current_inline_message_id = self.bot.send_message(self.msg.chat.id,
                                                                   self.localization['LocationPrefsQuestionMessage'],
                                                                   reply_markup=markup).id

            if editMode:
                for c in self.pref_countries:
                    add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id,
                                        self.current_markup_elements, self.markup_page, str(c))

            self.bot.send_message(self.msg.chat.id, self.localization['ChoosMessage'],
                                  reply_markup=self.okMarkup)
            self.bot.register_next_step_handler(msg, self.location_preferences_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            if not msg.text:
                self.bot.send_message(self.current_user,
                                      self.localization['CountyNotRecognizedErrorMessage'])
                self.bot.register_next_step_handler(msg, self.spoken_language_step, acceptMode=acceptMode,
                                                    editMode=editMode, chat_id=self.current_user)
                return False

            msg_text = msg.text.lower().strip()
            if msg_text != self.localization['OkButton']:
                country = self.country_convertor(msg_text)
                if country:  # TODO: Get string, separate by , and process it
                    if country not in self.pref_countries:
                        if len(self.pref_countries) + 1 > self.premium_limit:
                            self.bot.send_message(self.current_user, self.localization['CountriesPremiumErrorMessage'].format(self.premium_limit), reply_markup=self.okMarkup)
                        else:
                            self.pref_countries.append(country)
                            add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id,
                                                self.current_markup_elements,
                                                self.markup_page, str(country))
                            self.bot.send_message(self.current_user, self.localization['AddedMessage'], reply_markup=self.okMarkup)
                    else:
                        self.pref_countries.remove(country)
                        remove_tick_from_element(self.bot, self.current_user, self.current_inline_message_id,
                                                 self.current_markup_elements,
                                                 self.markup_page, str(country))
                        self.bot.send_message(self.current_user, self.localization['RemovedMessage'], reply_markup=self.okMarkup)
                    self.bot.register_next_step_handler(msg, self.location_preferences_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)
                    return True
                else:
                    self.bot.send_message(self.current_user, self.localization['ChoosMessage'])
                    self.suggest_countries(msg_text)
                    self.bot.register_next_step_handler(msg, self.location_preferences_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)
                    return False

            if self.pref_countries:
                self.old_queries.append(self.current_query)
                self.data["userLocationPreferences"] = self.pref_countries

                if not editMode:
                    self.age_pref_step(msg)
                else:
                    self.checkout_step(msg)

            else:
                self.bot.send_message(self.current_user, self.localization['CountryErrorMessage'])
                self.bot.register_next_step_handler(msg, self.location_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)


    def age_pref_step(self, msg, acceptMode=False, editMode=False):
        if not acceptMode:
            self.question_index = 15
            age_range = self.generate_personal_age_range()
            self.bot.send_message(msg.chat.id, self.localization['AgePrefsQuestionMessage'].format(age_range))
            self.bot.register_next_step_handler(msg, self.age_pref_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            try:
                if "-" in msg.text:
                    nums = msg.text.replace(" ", "").split("-")
                    age_pref = list(range(int(nums[0]), int(nums[1])))
                    self.data["agePrefs"] = age_pref

                else:
                    age_pref = list(range(int(msg.text) - 3, int(msg.text) + 5))
                    self.data["agePrefs"] = age_pref

                if not editMode:
                    self.tags_step(msg)
                else:
                    self.checkout_step(msg)

            except:
                self.bot.send_message(self.current_user, self.localization['AgePrefsErrorMessage'], reply_markup=self.age_pref_markup)
                self.bot.register_next_step_handler(msg, self.age_pref_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

    def tags_step(self, msg, acceptMode=False, editMode=False):
        if not acceptMode:

            if not self.hasVisited:
                self.bot.send_message(self.current_user, self.localization['TagsDescritpionMessage'], reply_markup=self.skip_markup)
            else:
                self.bot.send_message(self.current_user, self.localization['TagsQuestionMessage'], reply_markup=self.skip_markup)

            self.bot.register_next_step_handler(msg, self.tags_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            if msg.text == self.localization['SkipMessage']:
                self.bot.send_message(self.current_user, self.localization['GotchaMessage'])
                if not editMode:
                    self.auto_reply_step(msg)
                else:
                    self.checkout_step(msg)
            else:
                if msg.text:
                    try:
                        if len(msg.text.split()) <= self.maxTagCount:
                            self.tags = msg.text.lower().strip()

                            if not editMode:
                                self.auto_reply_step(msg)
                            else:
                                self.checkout_step(msg)
                        else:
                            self.bot.send_message(self.current_user, self.localization['ToManyTagsErrorMessage'].format(self.maxTagCount), reply_markup=self.skip_markup)
                            self.bot.register_next_step_handler(msg, self.tags_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)
                    except:
                        self.bot.send_message(self.current_user, self.localization['TagsErrorMessage'], reply_markup=self.skip_markup)
                        self.bot.register_next_step_handler(msg, self.tags_step, acceptMode=acceptMode,
                                                            editMode=editMode, chat_id=self.current_user)

                else:
                    self.bot.send_message(self.current_user, self.localization['EmptyErrorMessage'], reply_markup=self.skip_markup)
                    self.bot.register_next_step_handler(msg, self.tags_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

    def auto_reply_step(self, message, acceptMode=False, editMode=False):
        if not acceptMode:
            description_message = "---Functionality description---\n"
            question_message = self.localization['AutoreplyDescriptionMessage']

            if not self.hasVisited:
                question_message = description_message + question_message

            self.bot.send_message(self.current_user, question_message, reply_markup=self.skip_markup)
            self.bot.register_next_step_handler(message, self.auto_reply_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            if message.text == self.localization['SkipMessage']:
                self.bot.send_message(self.current_user, self.localization['GotchaMessage'])
                if not editMode:
                    self.change_something_step(message)
                    return
                self.checkout_step(message)

            elif message.voice:
                if Helpers.check_user_has_premium(self.current_user):
                    if message.voice.duration <= 15:
                        self.reply_voice = message.voice.file_id
                        if not editMode:
                            self.change_something_step(message)
                            return
                        self.checkout_step(message)
                    else:
                        self.bot.send_message(self.current_user, self.localization['LongVideoErrorMessage'], reply_markup=self.skip_markup)
                        self.bot.register_next_step_handler(message, self.auto_reply_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)
                else:
                    self.bot.send_message(self.current_user, self.localization['PremiumErrorMessage'], reply_markup=self.skip_markup)
                    self.bot.register_next_step_handler(message, self.auto_reply_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

            elif message.text:
                if len(message.text) < 300:
                    self.reply_text = message.text
                    if not editMode:
                        self.change_something_step(message)
                        return
                    self.checkout_step(message)
                else:
                    self.bot.send_message(self.current_user, self.localization['ToLongErrorMessage'], reply_markup=self.skip_markup)
                    self.bot.register_next_step_handler(message, self.auto_reply_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)
            else:
                self.bot.send_message(self.current_user, self.localization['EmptyErrorMessage'], reply_markup=self.skip_markup)
                self.bot.register_next_step_handler(message, self.auto_reply_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

    def change_something_step(self, message, acceptMode=False):
        if not acceptMode:
            if self.data["isMediaPhoto"]:
                self.bot.send_photo(self.current_user, self.data["userMedia"], self.localization['CheckoutMessage'].format(self.profile_constructor()))
            else:
                self.bot.send_video(self.current_user, video=self.data["userMedia"], caption=self.localization['CheckoutMessage'].format(self.profile_constructor()))
            self.bot.send_message(self.current_user, self.localization['CheckoutQuestionMessage'], reply_markup=self.YNmarkup)
            self.bot.register_next_step_handler(message, self.change_something_step, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == self.localization['YesButton']:
                self.checkout_step(message, firstTime=True)
            elif message.text == self.localization['NoButton']:
                self.tests_step(message)
            else:
                self.bot.send_message(self.current_user, self.localization['NoSuchOptionMessage'], reply_markup=self.YNmarkup)
                self.bot.register_next_step_handler(message, self.change_something_step, acceptMode=acceptMode, chat_id=self.current_user)

    def checkout_step(self, msg, acceptMode=False, firstTime=False):
        change_markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15", "16", "17", "18")
        final_msg = self.localization['FinishRegistrationButton']

        if self.hasVisited:
            final_msg = self.localization['SaveChangesButton']

        change_text = self.localization['CheckoutMenuMessage'].format(final_msg)

        #Add discard changes button if user exists
        if self.hasVisited:
            change_text += self.localization['DiscardChangesButton']

        if not acceptMode:
            if not firstTime:
                if self.data["isMediaPhoto"]:
                    self.bot.send_photo(self.current_user, self.data["userMedia"], self.localization['CheckoutMessage'].format(self.profile_constructor()))
                else:
                    self.bot.send_video(self.current_user, video=self.data["userMedia"], caption=self.localization['CheckoutMessage'].format(self.profile_constructor()))
            self.bot.send_message(self.current_user, change_text, reply_markup=change_markup)
            self.bot.register_next_step_handler(msg, self.checkout_step, acceptMode=True, chat_id=self.current_user)
        else:
            if msg.text == "1":
                self.data["wasChanged"] = True
                self.app_language_step(msg, editMode=True)
            elif msg.text == "2":
                self.data["wasChanged"] = True
                self.name_step(msg, editMode=True)
            elif msg.text == "3":
                self.data["wasChanged"] = True
                self.description_step(msg, editMode=True)
            elif msg.text == "4":
                self.data["wasChanged"] = True
                self.spoken_language_step(msg, editMode=True)
            elif msg.text == "5":
                self.data["wasChanged"] = True
                self.location_step(msg, editMode=True)
            elif msg.text == "6":
                self.data["wasChanged"] = True
                self.city_step(msg, editMode=True)
            elif msg.text == "7":
                self.data["wasChanged"] = True
                self.reason_step(msg, editMode=True)
            elif msg.text == "8":
                self.data["wasChanged"] = True
                self.age_step(msg, editMode=True)
            elif msg.text == "9":
                self.data["wasChanged"] = True
                self.gender_step(msg, editMode=True)
            elif msg.text == "10":
                self.data["wasChanged"] = True
                self.photo_step(msg, editMode=True)
            elif msg.text == "11":
                self.data["wasChanged"] = True
                self.language_preferences_step(msg, editMode=True)
            elif msg.text == "12":
                self.data["wasChanged"] = True
                self.location_preferences_step(msg, editMode=True)
            elif msg.text == "13":
                self.data["wasChanged"] = True
                self.age_pref_step(msg, editMode=True)
            elif msg.text == "14":
                self.data["wasChanged"] = True
                self.gender_preferences_step(msg, editMode=True)
            elif msg.text == "15":
                self.data["wasChanged"] = True
                self.communication_preferences_step(msg, editMode=True)
            elif msg.text == "16":
                self.data["wasChanged"] = True
                self.tags_step(msg, editMode=True)
            elif msg.text == "17":
                self.data["wasChanged"] = True
                self.auto_reply_step(msg, editMode=True)
            elif msg.text == "18":
                self.tests_step(msg, editMode=self.hasVisited)
            elif self.hasVisited and msg.text == "19":
                self.destruct()
            else:
                self.bot.send_message(self.current_user, self.localization['NoSuchOptionMessage'], reply_markup=change_markup)
                self.bot.register_next_step_handler(msg, self.checkout_step, acceptMode=acceptMode, chat_id=self.current_user)

    def tests_step(self, msg, acceptMode=False, editMode=False):
        if not acceptMode:
            d = json.dumps(self.data)
            response = None

            if editMode:
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
                self.bot.send_message(self.current_user, self.localization['ChangesErrorMessage'])
                self.destruct()
                return False

            self.bot.send_message(self.current_user, self.localization['OceanDescriptionMessage'], reply_markup=self.YNmarkup)
            self.bot.register_next_step_handler(msg, self.tests_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)

        else:
            if msg.text == self.localization('YesButton'):
                self.bot.send_message(self.current_user, self.localization['HelperDescriptionMessage'])
                TestModule(self.bot, self.msg, isActivatedFromShop=False)
                self.destruct()
            elif msg.text == self.localization('NoButton'):
                self.destruct()

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

    def gender_converter(self, gend):
        for g in self.genders:
            if gend == self.genders[g]:
                return g
        return None

    def communication_pref_convertor(self, comm):
        for c in self.communication_pref:
            if comm == self.communication_pref[c]:
                return c

    def reason_convertor(self, res):
        for g in self.reasons:
            if res == self.reasons[g]:
                return g
        return None

    def callback_handler(self, call):
        if call.message.id not in self.old_queries:
            self.current_query = call.message.id

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

            elif self.question_index == 12:
                if int(call.data) == -5:

                    remove_tick_from_elements(self.bot, self.current_user, call.message.id, self.current_markup_elements, self.markup_page, self.pref_langs)
                    add_tick_to_elements(self.bot, self.current_user, self.current_inline_message_id, self.current_markup_elements, self.markup_page, self.chosen_langs)
                    for l in self.chosen_langs:
                        if l not in self.pref_langs:
                            add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id, self.current_markup_elements, self.markup_page, str(l))
                            self.pref_langs.append(l)
                    self.bot.answer_callback_query(call.id, self.localization['AddedMessage'])
                    self.bot.send_message(self.current_user,
                                          self.localization['SameAsMineMessage'],
                                          reply_markup=self.okMarkup)

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

            elif self.question_index == 14:
                if int(call.data) == -5:
                    if self.country not in self.pref_countries:
                        remove_tick_from_elements(self.bot, self.current_user, call.message.id, self.current_markup_elements, self.markup_page, self.pref_countries)
                        self.pref_countries.append(self.country)
                        self.bot.send_message(self.current_user,
                                              self.localization['SameAsMineMessage'],
                                              reply_markup=self.okMarkup)
                        self.bot.answer_callback_query(call.id, self.localization['AddedMessage'])

                        add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id, self.current_markup_elements, self.markup_page, str(self.country))

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

    def generate_personal_age_range(self):
        min_value = self.data["userAge"] - 3
        max_value = self.data["userAge"] + 5

        #Age below 12 is forbidden, thus, no need in that :)
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

    def destruct(self):
        self.bot.callback_query_handlers.remove(self.chCode)
        if self.hasVisited:
            Helpers.switch_user_busy_status(self.current_user, 12)
        if self.return_method:
            self.return_method()
        else:
            go_back_to_main_menu(self.bot, self.current_user, self.msg)
        del self