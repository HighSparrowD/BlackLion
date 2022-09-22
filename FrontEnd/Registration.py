import copy

from telebot.types import KeyboardButton, ReplyKeyboardMarkup
from Core import HelpersMethodes as Helpers
from Common.Menues import count_pages, assemble_markup, reset_pages, add_tick_to_element, remove_tick_from_element
import requests
import json

from Common.Menues import go_back_to_main_menu


class Registrator:
    def __init__(self, bot=None, msg=None, registrators=None, hasVisited=True):
        self.bot = bot
        self.msg = msg
        self.previous_item = '' #Is used to remove a tick from single-type items (country, city, etc..)
        self.current_inline_message_id = 0 #Represents current message with inline markup
        self.current_user = self.msg.from_user.id
        Helpers.switch_user_busy_status(self.current_user)
        self.hasVisited = hasVisited
        self.okMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(KeyboardButton("Ok"))
        self.chCode = self.bot.register_callback_query_handler("", self.callback_handler, user_id=self.current_user)
        self.bot.register_next_step_handler(msg, self.spoken_language_step, chat_id=self.current_user)
        self.localisation = json.loads(requests.get("https://localhost:44381/GetLocalisation/0", verify=False).text)

        self.current_query = 0
        self.old_queries = []

        self.current_markup_elements = []
        self.markup_last_element = 0
        self.markup_page = 1
        self.markup_pages_count = 0

        self.app_languages_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)
        self.gender_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)
        self.reason_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)
        self.communication_pref_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)
        self.age_pref_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)

        self.app_language = None

        self.data = {}
        self.country = None
        self.city = None

        self.current_step_id = self.spoken_language_step

        self.languages = {}

        self.genders = {}
        self.countries = {}
        self.cities = {}

        self.reasons = {}
        self.age_pref = {}
        self.communication_pref = {}
        self.app_langs = {}

        self.registrators = registrators
        self.registrators.append(self)
        self.chosen_langs = []
        self.pref_langs = []
        self.pref_countries = []

        for lang in json.loads(requests.get("https://localhost:44381/GetAppLanguages", verify=False).text):
            self.app_langs[lang["id"]] = lang["languageNameShort"]

        self.app_languages_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)
        start_message = self.localisation[0]["loc"][0]["elementText"]
        b = []

        for lang in self.app_langs.values():
            b.append(KeyboardButton(lang))

        self.app_languages_markup.add(b[0], b[1], b[2])
        self.bot.send_message(msg.chat.id, start_message, reply_markup=self.app_languages_markup)

    # def app_language_step(self, msg):

    def spoken_language_step(self, msg):
        self.msg = msg
        self.app_language = self.app_language_converter(msg.text)
        if self.app_language or self.app_language == 0:
            self.get_localisations()

            self.data = {"id": msg.from_user.id, "userName": msg.from_user.username, "userAppLanguageId": self.app_language}

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page, self.markup_pages_count)
            count_pages(self.languages, self.current_markup_elements, self.markup_pages_count)
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)
            self.markup_page = 1

            self.current_inline_message_id = self.bot.send_message(self.msg.chat.id, "Please state all languages you speak", reply_markup=markup).json['message_id']

            self.bot.send_message(self.msg.chat.id, "Choose few from above, or simply write language to chat :-)", reply_markup=self.okMarkup)
            self.bot.register_next_step_handler(msg, self.gender_step, chat_id=self.current_user)
        else:
            self.bot.send_message(self.current_user, "There is no such language. Try again", reply_markup=self.app_languages_markup)
            self.bot.register_next_step_handler(msg, self.spoken_language_step, chat_id=self.current_user)

    def gender_step(self, msg):
        msg_text = msg.text.lower().strip()
        if msg_text != "ok":
            lang = self.spoken_languages_convertor(msg_text)
            if lang: #TODO: Get string, separate by , and process it
                if lang not in self.chosen_langs:
                    self.chosen_langs.append(lang)
                    add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id, self.current_markup_elements,
                                        self.markup_page, str(lang))
                    self.bot.send_message(self.current_user, "Added", reply_markup=self.okMarkup)
                else:
                    self.chosen_langs.remove(lang)
                    remove_tick_from_element(self.bot, self.current_user, self.current_inline_message_id, self.current_markup_elements,
                                        self.markup_page, str(lang))
                    self.bot.send_message(self.current_user, "Removed", reply_markup=self.okMarkup)
                self.bot.register_next_step_handler(msg, self.gender_step, chat_id=self.current_user)
                return True
            else:
                self.bot.send_message(self.current_user, "Language was not recognized, try finding it in our list above")
                self.bot.register_next_step_handler(msg, self.gender_step, chat_id=self.current_user)
                return False

        if self.chosen_langs:
            self.old_queries.append(self.current_query)
            self.gender_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)
            self.msg = msg
            self.data["userLanguages"] = self.chosen_langs

            for g in self.genders.keys():
                self.gender_markup.row().add(KeyboardButton(self.genders[g]))

            self.bot.send_message(self.msg.chat.id, "What is your gender?", reply_markup=self.gender_markup)
            self.bot.register_next_step_handler(msg, self.location_step, chat_id=self.current_user)
        else:
            self.bot.send_message(self.current_user, "You haven't chosen any languages !", reply_markup=self.okMarkup)
            self.bot.register_next_step_handler(msg, self.gender_step, chat_id=self.current_user)

    def location_step(self, msg):
        self.msg = msg
        gender = self.gender_converter(msg.text)
        if gender or gender == 0:
            self.data["userGender"] = self.gender_converter(msg.text)

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page, self.markup_pages_count)
            count_pages(self.countries, self.current_markup_elements, self.markup_pages_count)
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)
            self.previous_item = ''

            self.current_inline_message_id = self.bot.send_message(self.msg.chat.id, "Which country do you live in?", reply_markup=markup).json['message_id']
            self.bot.send_message(self.msg.chat.id, "Choose one from above", reply_markup=self.okMarkup)
            self.bot.register_next_step_handler(msg, self.city_step, chat_id=self.current_user)
        else:
            self.bot.send_message(self.current_user, "Please choose only ones in a given list !", reply_markup=self.gender_markup)
            self.bot.register_next_step_handler(msg, self.location_step, chat_id=self.current_user)

    def city_step(self, msg):
        msg_text = msg.text.lower().strip()
        if msg_text != "ok":
            country = self.country_convertor(msg_text)
            if country:
                if self.previous_item:
                    remove_tick_from_element(self.bot, self.current_user, self.current_inline_message_id, self.current_markup_elements,
                                             self.markup_page, self.previous_item)
                self.previous_item = str(country)
                add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id, self.current_markup_elements,
                                    self.markup_page, str(country))
                self.country = country
                self.bot.send_message(self.current_user, "Gotcha", reply_markup=self.okMarkup)
                self.bot.register_next_step_handler(msg, self.city_step, chat_id=self.current_user)
                return True
            else:
                self.bot.send_message(self.current_user, "Country was not recognized, try finding it in our list above")
                self.bot.register_next_step_handler(msg, self.city_step, chat_id=self.current_user)
                return False

        if self.country or self.country == 0:
            self.old_queries.append(self.current_query)
            self.msg = msg
            self.data["userCountryCode"] = self.country
            self.previous_item = ''

            cities = json.loads(
                requests.get(f"https://localhost:44381/GetCities/{self.country}/{self.app_language}", verify=False).text)
            for city in cities:
                self.cities[city["id"]] = city["cityName"].lower()

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page, self.markup_pages_count)
            count_pages(self.cities, self.current_markup_elements, self.markup_pages_count)
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

            self.current_inline_message_id = self.bot.send_message(self.msg.chat.id, "Which city do you live in?", reply_markup=markup).json['message_id']
            self.bot.send_message(self.msg.chat.id, "Chose one from above, or simply type to chat", reply_markup=self.okMarkup)
            self.bot.register_next_step_handler(msg, self.name_step, chat_id=self.current_user)
        else:
            self.bot.send_message(self.current_user, "You haven't chosen a country !", reply_markup=self.okMarkup)
            self.bot.register_next_step_handler(msg, self.city_step, chat_id=self.current_user)

    def name_step(self, msg):
        msg_text = msg.text.lower().strip()
        if msg_text != "ok":
            city = self.city_convertor(msg_text)
            if city:
                if self.previous_item:
                    remove_tick_from_element(self.bot, self.current_user, self.current_inline_message_id, self.current_markup_elements,
                                             self.markup_page, self.previous_item)
                self.previous_item = str(city)
                add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id, self.current_markup_elements,
                                    self.markup_page, str(city))
                self.city = city
                self.bot.send_message(self.current_user, "Gotcha", reply_markup=self.okMarkup)
                self.bot.register_next_step_handler(msg, self.name_step, chat_id=self.current_user)
                return True
            else:
                self.bot.send_message(self.current_user, "City was not recognized, try finding it in our list above")
                self.bot.register_next_step_handler(msg, self.name_step, chat_id=self.current_user)
                return False

        if self.city or self.city == 0:
            self.old_queries.append(self.current_query)
            self.msg = msg
            self.previous_item = ''
            self.data["userCityCode"] = self.city
            self.previous_item = ''
            # self.data["userCity"] = self.cities[self.country][self.city]

            self.bot.send_message(msg.chat.id, "What is your name?")
            self.bot.register_next_step_handler(msg, self.age_step, chat_id=self.current_user)
        else:
            self.bot.send_message(self.current_user, "You haven't chosen a city !", reply_markup=self.okMarkup)
            self.bot.register_next_step_handler(msg, self.name_step, chat_id=self.current_user)

    def age_step(self, msg):
        if msg.text:
            if "userRealName" not in self.data:
                self.data["userRealName"] = msg.text
        else:
            self.bot.send_message(self.current_user, "Name cannot be empty !")
            self.bot.register_next_step_handler(msg, self.age_step, chat_id=self.current_user)

        self.msg = msg
        self.bot.send_message(msg.chat.id, "How old are you?")
        self.bot.register_next_step_handler(msg, self.description_step, chat_id=self.current_user)

    def description_step(self, msg):
        try:
            self.data["userAge"] = int(msg.text)
            self.msg = msg
            self.bot.send_message(msg.chat.id, "Tell something about yourself !")
            self.bot.register_next_step_handler(msg, self.reason_step, chat_id=self.current_user)

        except:
            self.bot.send_message(msg.chat.id, "Age please, only numbers")
            self.bot.register_next_step_handler(msg, self.description_step, chat_id=self.current_user)

    def reason_step(self, msg):
        if msg.text:
            for reason in self.reasons.values():
                self.reason_markup.add(KeyboardButton(reason))

            self.msg = msg
            self.data["userDescription"] = msg.text
            self.bot.send_message(msg.chat.id, "What are you searching for?", reply_markup=self.reason_markup)
            self.bot.register_next_step_handler(msg, self.photo_step, chat_id=self.current_user)
        else:
            self.bot.send_message(msg.chat.id, "Description cannot be empty. Just  describe yourself in a few words ;-)")
            self.bot.register_next_step_handler(msg, self.reason_step, chat_id=self.current_user)

    def photo_step(self, msg):
        reason = self.reason_convertor(msg.text)
        if reason or reason == 0:
            self.msg = msg
            self.bot.send_message(msg.chat.id, "Send your photo")
            self.data["reasonId"] = reason
            self.bot.register_next_step_handler(msg, self.gender_preferences_step, chat_id=self.current_user)
        else:
            self.bot.send_message(msg.chat.id, "No-no, you can't search for something else here ;-)", reply_markup=self.reason_markup)
            self.bot.register_next_step_handler(msg, self.photo_step, chat_id=self.current_user)

    def gender_preferences_step(self, msg):
        if msg.photo:
            self.msg = msg
            self.data["userPhoto"] = msg.photo[len(msg.photo) - 1].file_id  # TODO: troubleshoot photos

            self.bot.send_message(self.msg.chat.id, "Who are you searching for", reply_markup=self.gender_markup)
            self.bot.register_next_step_handler(msg, self.language_preferences_step, chat_id=self.current_user)
        else:
            self.bot.send_message(self.current_user, "I can't find a photo in your message")
            self.bot.register_next_step_handler(msg, self.gender_preferences_step, chat_id=self.current_user)

    def language_preferences_step(self, msg):
        gender = self.gender_converter(msg.text)
        if gender or gender == 0:
            self.msg = msg
            self.data["userGenderPrefs"] = gender

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page, self.markup_pages_count)
            count_pages(self.languages, self.current_markup_elements, self.markup_pages_count, True)
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

            self.current_inline_message_id = self.bot.send_message(msg.chat.id, "What languages are you willing to speak with people?",
                                                                   reply_markup=markup).json['message_id']
            self.bot.send_message(self.msg.chat.id, "Choose few from above, or simply type to chat", reply_markup=self.okMarkup)

            self.bot.register_next_step_handler(msg, self.communication_preferences_step, chat_id=self.current_user)
        else:
            self.bot.send_message(self.current_user, "Please chose only ones in the list", reply_markup=self.gender_markup)
            self.bot.register_next_step_handler(msg, self.language_preferences_step, chat_id=self.current_user)

    def communication_preferences_step(self, msg):
        msg_text = msg.text.lower().strip()
        if msg_text != "ok":
            lang = self.spoken_languages_convertor(msg_text)
            if lang: #TODO: Get string, separate by , and process it
                if lang not in self.chosen_langs:
                    self.chosen_langs.append(lang)
                    add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id, self.current_markup_elements,
                                        self.markup_page, str(lang))
                    self.bot.send_message(self.current_user, "Added", reply_markup=self.okMarkup)
                else:
                    self.chosen_langs.remove(lang)
                    remove_tick_from_element(self.bot, self.current_user, self.current_inline_message_id, self.current_markup_elements,
                                        self.markup_page, str(lang))
                    self.bot.send_message(self.current_user, "Removed", reply_markup=self.okMarkup)
                self.bot.register_next_step_handler(msg, self.communication_preferences_step, chat_id=self.current_user)
                return True
            else:
                self.bot.send_message(self.current_user, "Language was not recognized, try finding it in our list above")
                self.bot.register_next_step_handler(msg, self.communication_preferences_step, chat_id=self.current_user)
                return False
        elif msg_text == "Same as mine":
            self.pref_langs = copy.copy(self.chosen_langs)
            self.bot.send_message(self.current_user, "Got it! Press OK to move to the next step or add more languages if you want ;-)", reply_markup=self.okMarkup)
            self.bot.register_next_step_handler(msg, self.communication_preferences_step, chat_id=self.current_user)
            return False

        if self.pref_langs:
            self.old_queries.append(self.current_query)
            b1 = KeyboardButton("In real life")
            b2 = KeyboardButton("Online")
            b3 = KeyboardButton("Does not matter")

            self.communication_pref_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(b1, b2, b3)
            self.msg = msg

            self.bot.send_message(msg.chat.id, "How would you like to communicate?", reply_markup=self.communication_pref_markup)
            self.data["userLanguagePreferences"] = self.pref_langs
            self.bot.register_next_step_handler(msg, self.location_preferences_step, chat_id=self.current_user)
        else:
            self.bot.send_message(self.current_user, "You haven't chosen any languages !")
            self.bot.register_next_step_handler(msg, self.communication_preferences_step, chat_id=self.current_user)

    def location_preferences_step(self, msg):
        comm_pref = self.communication_pref_convertor(msg.text)
        if comm_pref or comm_pref == 0:
            self.old_queries.append(self.current_query)
            self.msg = msg
            self.data["communicationPrefs"] = comm_pref

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page, self.markup_pages_count)
            count_pages(self.countries, self.current_markup_elements, self.markup_pages_count, True)
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

            self.current_inline_message_id = self.bot.send_message(self.msg.chat.id, "People from which countries would you like to communicate with?",
                                  reply_markup=markup).json['message_id']
            self.bot.send_message(self.msg.chat.id, "Choose few from above or simply type to chat ;-)", reply_markup=self.okMarkup)
            self.bot.register_next_step_handler(msg, self.age_pref_step, chat_id=self.current_user)
        else:
            self.bot.send_message(self.current_user, "Please, choose only ones in the list !", reply_markup=self.communication_pref_markup)
            self.bot.register_next_step_handler(msg, self.location_preferences_step, chat_id=self.current_user)

    def age_pref_step(self, msg):
        msg_text = msg.text.lower().strip()
        if msg_text != "ok":
            country = self.country_convertor(msg_text)
            if country:  # TODO: Get string, separate by , and process it
                if country not in self.pref_countries:
                    self.pref_countries.append(country)
                    add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id, self.current_markup_elements,
                                        self.markup_page, str(country))
                    self.bot.send_message(self.current_user, "Added", reply_markup=self.okMarkup)
                else:
                    self.pref_countries.remove(country)
                    remove_tick_from_element(self.bot, self.current_user, self.current_inline_message_id, self.current_markup_elements,
                                             self.markup_page, str(country))
                    self.bot.send_message(self.current_user, "Removed", reply_markup=self.okMarkup)
                self.bot.register_next_step_handler(msg, self.location_preferences_step, chat_id=self.current_user)
                return True
            else:
                self.bot.send_message(self.current_user, "Country was not recognized, try finding it in our list above")
                self.bot.register_next_step_handler(msg, self.location_preferences_step, chat_id=self.current_user)
                return False
        elif msg.text == "Same as mine":
            if self.country not in self.pref_countries:
                self.pref_countries.append(self.country)
                self.bot.send_message(self.current_user,
                                      "Got it! Press OK to move to the next step or add more languages if you want ;-)",
                                      reply_markup=self.okMarkup)
            self.bot.register_next_step_handler(msg, self.location_preferences_step, chat_id=self.current_user)

        if self.pref_countries:
            self.old_queries.append(self.current_query)
            self.age_pref_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)

            for ap in self.age_pref.keys():
                self.age_pref_markup.add(KeyboardButton(self.age_pref[ap]))

            self.msg = msg
            self.data["userLocationPreferences"] = self.pref_countries

            self.bot.send_message(msg.chat.id, "What age would you like your companion to be?", reply_markup=self.age_pref_markup)
            self.bot.register_next_step_handler(msg, self.tests_step, chat_id=self.current_user)
        else:
            self.bot.send_message(self.current_user, "You haven't chosen any countries !")
            self.bot.register_next_step_handler(msg, self.age_pref_step, chat_id=self.current_user)

    def tests_step(self, msg):
        age_pref = self.age_prefs_converter(msg.text)

        if age_pref or age_pref == 0:
            self.msg = msg
            self.data["agePrefs"] = age_pref

            d = json.dumps(self.data)
            userId = requests.post("https://localhost:44381/RegisterUser", d, headers={
                "Content-Type": "application/json"}, verify=False)

            self.bot.send_message(self.current_user, userId.text)
            self.destruct()
        else:
            self.bot.send_message(self.current_user, "Please, chose only ones in the list!", reply_markup=self.age_pref_markup)
            self.bot.register_next_step_handler(msg, self.tests_step, chat_id=self.current_user)

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

    def age_prefs_converter(self, pref):
        for p in self.age_pref:
            if pref == self.age_pref[p]:
                return p
        return None

    def photo_handler(self, message):
        if message.from_user.id == self.current_user:
            self.bot.send_photo(message.chat.id, message.photo[len(message.photo) - 1].file_id)  # id is a string

    def callback_handler(self, call):
        if call.message.id not in self.old_queries:
            self.current_query = call.message.id

            if call.data == "-1" or call.data == "-2":
                index = self.index_converter(call.data)
                if self.markup_page + index <= self.markup_pages_count or self.markup_page + index >= 1:
                    markup = assemble_markup(self.markup_page, self.current_markup_elements, index)
                    self.bot.edit_message_reply_markup(chat_id=call.message.chat.id, reply_markup=markup,
                                                       message_id=call.message.id)
                    self.markup_page += index

            elif "/" in call.data:      #TODO: Make it work another way... maybe
                self.bot.answer_callback_query(call.id, call.data)

            elif call.message.text == "Please state all languages you speak":
                if int(call.data) not in self.chosen_langs:
                    # self.bot.send_message(chatId, call.data)
                    self.chosen_langs.append(int(call.data))
                    self.bot.answer_callback_query(call.id, "Added")
                    add_tick_to_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                        self.markup_page, call.data)

                else:
                    self.chosen_langs.remove(int(call.data))
                    self.bot.answer_callback_query(call.id, "Removed")
                    remove_tick_from_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                        self.markup_page, call.data)

            elif call.message.text == "Which country do you live in?":
                if int(call.data) in self.countries.keys():
                    self.country = int(call.data)
                    self.bot.answer_callback_query(call.id, "Gotcha")
                    if self.previous_item:
                        remove_tick_from_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                            self.markup_page, self.previous_item)
                    add_tick_to_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                        self.markup_page, call.data)
                    self.previous_item = call.data
                else:  # call.data in self.countries.values():
                    self.bot.send_message(call.message.chat.id, "Incorrect country code")

            elif call.message.text == "Which city do you live in?":
                if int(call.data) in self.cities.keys():
                    self.city = int(call.data)
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

            elif call.message.text == "What languages are you willing to speak with people?":
                if int(call.data) == -5:
                    if self.data["userLanguages"][0] not in self.pref_langs:
                        self.pref_langs = copy.copy(self.chosen_langs)
                        self.bot.send_message(self.current_user,
                                              "Got it! Press OK to move to the next step or add more languages if you want ;-)",
                                              reply_markup=self.okMarkup)
                        self.bot.answer_callback_query(call.id, "Added")

                        for element in self.chosen_langs:
                            add_tick_to_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                                self.markup_page, element)

                elif int(call.data) not in self.pref_langs:
                    # self.bot.send_message(chatId, call.data)
                    self.pref_langs.append(int(call.data))
                    self.bot.answer_callback_query(call.id, "Added")
                    add_tick_to_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                        self.markup_page, call.data)

                else:
                    self.pref_langs.remove(int(call.data))
                    self.bot.answer_callback_query(call.id, "Removed")
                    remove_tick_from_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                        self.markup_page, call.data)

            elif call.message.text == "People from which countries would you like to communicate with?":
                if int(call.data) == -5:
                    if self.country not in self.pref_countries:
                        self.pref_countries.append(self.country)
                        self.bot.send_message(self.current_user,
                                              "Got it! Press OK to move to the next step or add more countries if you want ;-)",
                                              reply_markup=self.okMarkup)
                        self.bot.answer_callback_query(call.id, "Added")

                        add_tick_to_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                            self.markup_page, self.country)

                elif int(call.data) not in self.pref_countries:
                    self.pref_countries.append(int(call.data))
                    self.bot.answer_callback_query(call.id, "Added")
                    add_tick_to_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                        self.markup_page, call.data)
                else:
                    self.pref_langs.remove(int(call.data))
                    self.bot.answer_callback_query(call.id, "Removed")
                    remove_tick_from_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                        self.markup_page, call.data)

    def get_localisations(self):
        for language in json.loads(
                requests.get(f"https://localhost:44381/GetLanguages/{self.app_language}", verify=False).text):
            self.languages[language["id"]] = language["languageName"].lower().strip()

        for gender in json.loads(
                requests.get(f"https://localhost:44381/GetGenders/{self.app_language}", verify=False).text):
            self.genders[gender["id"]] = gender["genderName"].strip()

        for country in json.loads(
                requests.get(f"https://localhost:44381/GetCountries/{self.app_language}", verify=False).text):
            self.countries[country["id"]] = country["countryName"].lower().strip()

        for reason in json.loads(
                requests.get(f"https://localhost:44381/GetReasons/{self.app_language}", verify=False).text):
            self.reasons[reason["id"]] = reason["reasonName"].strip()

        for pref in json.loads(
                requests.get(f"https://localhost:44381/GetAgePreferences/{self.app_language}", verify=False).text):
            self.age_pref[pref["id"]] = pref["agePrefName"].strip()

        for pref in json.loads(requests.get(f"https://localhost:44381/GetCommunicationPreferences/{self.app_language}",
                                            verify=False).text):
            self.communication_pref[pref["id"]] = pref["communicationPrefName"].strip()

    @staticmethod
    def index_converter(index):
        if index == "-1":
            return -1
        return 1

    def destruct(self):
        self.bot.callback_query_handlers.remove(self.chCode)
        self.registrators.remove(self)
        if self.hasVisited:
            Helpers.switch_user_busy_status(self.current_user)
        go_back_to_main_menu(self.bot, self.current_user)
        del self

    def __del__(self):
        self.bot.callback_query_handlers.remove(self.chCode)
        self.registrators.remove(self)
        del self
        # TODO: Make it work
