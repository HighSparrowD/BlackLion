import copy
import json
import random

import Common.Menues as Coms
import Core.HelpersMethodes as Helpers
import requests
from telebot.types import ReplyKeyboardMarkup, KeyboardButton, InlineKeyboardMarkup, InlineKeyboardButton

from Common.Menues import go_back_to_main_menu
from SponsorHandlerAdmin import SponsorHandlerAdmin


class SponsorHandler:
    def __init__(self, bot, message, sponsor_handlers, hasVisited=True):
        self.bot = bot
        self.current_user = message.from_user.id
        Helpers.switch_user_busy_status(self.current_user)
        self.current_userName = message.from_user.username
        self.isSponsor = Helpers.check_user_is_sponsor(self.current_user)
        self.isAwaiting = Helpers.check_user_is_awaiting_by_username(self.current_userName)
        if not self.isAwaiting: #Can mean that user was not found by its id
            self.isAwaiting = Helpers.check_user_is_awaiting(self.current_user)
        self.isPostponed = Helpers.check_user_is_postponed(self.current_user)
        self.userMaxAdCount = 0
        self.userMaxAdViewCount = 0
        self.sponsor_handlers = sponsor_handlers
        self.sponsor_handlers.append(self)

        self.chCode = None
        self.mhCode = None

        self.language_levels = ["A1", "A2", "B1", "B2", "C1", "C2"]

        self.dat = {}
        self.user_registration_data = {}
        self.ad_data = {"photo": "", "video": ""}

        self.current_inline_message_id = 0
        self.previous_item = ''
        self.country = 0
        self.city = 0

        self.current_markup_elements = []
        self.markup_last_element = 0
        self.markup_page = 1
        self.markup_pages_count = 0

        self.old_queries = []
        self.languages = {}
        self.chosen_languages = []
        self.chosen_languages_levels = []
        self.countries = {}
        self.cities = {}
        self.app_langs = {}

        self.markup_error_message = "No such option"
        self.frozen_warning_message = "\nWARNING!\n Your account has been frozen. It basically means, that your "
        self.registration_successful_message = "Congrats! Your registration has gone successfully"
        self.registration_interrupt_message = "Hey, you are already registered"
        self.permission_error_message = "You have no permission to use that command\nPlease, contact support team @GraphicGod"
        self.no_ads_message = "Hey! You have no ads yet...\nCreate your first ad using /createad command :-)"
        self.post_creation_start_message = "Please, input a text or description of an ad"
        self.post_creation_midd_message = "Please, send a photo video or a gif, you want users to receive"
        self.post_creation_show_message = "Here you go!\n This is how your ad will look like"
        self.post_creation_no_ad_text_message = "I have found no text here. Please try typing something again"
        self.post_creation_no_ad_data_message = "I cant see any media here! :-(, Are you sure, you want to show only text?"
        self.post_creation_success_message = "Ad had been created!"
        self.post_update_success_message = "Ad had been updated!"
        self.show_all_posts_message = "Here are all you ads! You can change them by clicking on a button with an ad description"
        self.action_list_message = ""
        self.command_list_message = "/myads - View your ads\n/createad - Create a new ad\n/exit - exit from this module"

        self.empty_markup = InlineKeyboardMarkup()
        self.okMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(KeyboardButton("Ok"))
        self.language_levels_markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(KeyboardButton("A1"), KeyboardButton("A2"), KeyboardButton("B1"), KeyboardButton("B2"), KeyboardButton("C1"), KeyboardButton("C2"))
        self.markup1 = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(KeyboardButton("/register"))
        self.app_langs_markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True)
        self.skip_markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(KeyboardButton("skip"))
        self.registration_checkout_markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(KeyboardButton("1"), KeyboardButton("2"), KeyboardButton("3"), KeyboardButton("4"), KeyboardButton("5"), KeyboardButton("6"), KeyboardButton("7"), KeyboardButton("8"), KeyboardButton("9"), KeyboardButton("10"))
        self.markup2 = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(KeyboardButton("/myads"), KeyboardButton("/createad"), KeyboardButton("/exit"))
        self.markupYN = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(KeyboardButton("Yes, text only"), KeyboardButton("No, i'll send media"))
        self.markup_delete = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(KeyboardButton("Yes"), KeyboardButton("No"))
        self.markup_ad_checkout = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(
            KeyboardButton("Ok"), KeyboardButton("Change Text"), KeyboardButton("Change Media"), KeyboardButton("Delete ad"))
        self.markup_create_ad = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(KeyboardButton("/createad"))

        self.greet_message1 = "Hey! I see you are a new one here. Lets create your sponsor account!"
        self.greet_message2 = f"Welcome back dear sponsor. What are we to do this time? \n{self.action_list_message}"
        self.registration_checkout_message = f"1. Change Username\n2.Change Age\n3. Change Tel. number\n4. Change Email\n5. Change Instagram Username\n6. Change Facebook Username\n7. Change App language\n8. Choose Location again\n9. Choose Languages again\n10. Finish Registration"
        self.greet_message3 = f"Welcome back dear sponsor. Reminding you my list of commands :-) \n{self.command_list_message}"
        self.greets = [self.greet_message2, self.greet_message3]

        for lang in json.loads(requests.get("https://localhost:44381/GetAppLanguages", verify=False).text):
            self.app_langs[lang["id"]] = lang["languageNameShort"]
            self.app_langs_markup.add(KeyboardButton(lang["languageNameShort"]))

        if self.isSponsor:
            if self.isPostponed:
                self.bot.send_message(self.current_user, self.frozen_warning_message)
            else:
                self.chCode = self.bot.register_callback_query_handler("", self.callback_handler, user_id=self.current_user)
                self.mhCode = self.bot.register_message_handler(self.message_handler, user_id=self.current_user, commands=["register", "myads", "createad"])
                self.eh = self.bot.register_message_handler(self.exit_handler, user_id=self.current_user, commands=["exit"])
                self.load_user_data()
                self.bot.send_message(self.current_user, random.choice(self.greets), reply_markup=self.markup2)

        else:
            self.bot.send_message(self.current_user, self.greet_message1, reply_markup=self.markup1)
            self.chCode = self.bot.register_callback_query_handler("", self.callback_handler, user_id=self.current_user)
            self.mhCode = self.bot.register_message_handler(self.message_handler, user_id=self.current_user, commands=["register", "myads", "createad", "switchstatus"])
            self.eh = self.bot.register_message_handler(self.exit_handler, user_id=self.current_user, commands=["exit"])

            self.load_user_data()

    def load_user_data(self):
        if self.isAwaiting:
            data = json.loads(
                requests.get(f"https://localhost:44381/GetAwaitingUser/{self.current_userName}", verify=False).text)
            self.userMaxAdCount = data["userMaxAdCount"]
            self.userMaxAdViewCount = data["userMaxAdViewCount"]

        elif self.isSponsor:
            data = json.loads(
                requests.get(f"https://localhost:44381/GetSponsorAsync/{self.current_user}", verify=False).text)
            self.userMaxAdCount = data["userMaxAdCount"]
            self.userMaxAdViewCount = data["userMaxAdViewCount"]
            self.isPostponed = data["isPostponed"]

    def remove_ad(self):
        self.bot.send_message(self.current_user, "Successfully deleted", reply_markup=self.markup2)
        return json.loads(requests.delete(f"https://localhost:44381/RemoveAd/{self.ad_data['id']}/{self.current_user}", verify=False).text)

    def message_handler(self, message):
        if message.text == "/register":
            if self.isAwaiting:
                self.bot.send_message(self.current_user, "Please, send me a keyword, you have received from the administration earlier")
                self.bot.register_next_step_handler(message, self.registration_codeword_step)
            else:
                self.bot.send_message(self.current_user, self.permission_error_message)

        elif message.text == "/myads":
            data = json.loads(requests.get(f"https://localhost:44381/GetSponsorAds/{self.current_user}", verify=False).text)
            if data:
                for d in data:
                    self.empty_markup.add(InlineKeyboardButton(d["description"], callback_data=d["id"]))

                self.bot.send_message(self.current_user, self.show_all_posts_message, reply_markup=self.empty_markup)
                self.empty_markup = InlineKeyboardMarkup()
            else:
                self.bot.send_message(self.current_user, self.no_ads_message, reply_markup=self.markup_create_ad)

        elif message.text == "/createad":
            if self.isSponsor:
                if not Helpers.check_sponsor_is_maxed(self.current_user):
                    self.bot.send_message(self.current_user, self.post_creation_start_message)
                    self.bot.register_next_step_handler(message, self.ad_creating1, chat_id=self.current_user)
                else:
                    self.bot.send_message(self.current_user, "Sorry, you have reached your max ad limit. Please delete one of your ads, or contact administration to buy more", reply_markup=self.markup2)

            else:
                self.bot.send_message(self.current_user, self.permission_error_message)
        elif message.text == "/switchstatus":
            self.bot.send_message(self.current_user, "Switching to admin section")
            self.destruct()
            SponsorHandlerAdmin(self.bot, message, self.sponsor_handlers)

    def registration_codeword_step(self, message):
        if Helpers.check_user_keyword_is_correct(self.current_user, message.text):
            if Helpers.check_user_exists(self.current_user):
                data = Helpers.get_user_info(self.current_user)
                self.dat = {
                    "id": self.current_user,
                    "codeWord": message.text,
                    "age": data["userDataInfo"]["userAge"],
                    "username": data["userBaseInfo"]["userName"],
                    "userAppLanguage": data["userDataInfo"]["languageId"],
                    "userCountryId": data["userDataInfo"]["location"]["countryId"],
                    "userCityId": data["userDataInfo"]["location"]["cityId"],
                    "languages": data["userLanguages"]
                }

                self.get_localisation(self.dat["userAppLanguage"])
                self.bot.send_message(self.current_user, "Now, lets get to a basic contact info :-). Send me your phone number", reply_markup=self.skip_markup)
                self.bot.register_next_step_handler(message, self.registration_contact_tel_step, chat_id=self.current_user)
                return False

        self.dat["codeWord"] = message.text,
        self.bot.send_message(self.current_user, "Please, choose an app language", reply_markup=self.app_langs_markup)
        self.bot.register_next_step_handler(message, self.registration_app_language_step, chat_id=self.current_user)

    def registration_app_language_step(self, message, revisit=False):
        if not revisit:
            lang = self.app_language_converter(message.text)
            if lang:
                self.get_localisation(lang)
                self.dat["userAppLanguage"] = lang
                self.return_to_registration_checkout(message)

            self.bot.send_message(self.current_user, "What is you name or nickname?")
            self.bot.register_next_step_handler(message, self.registration_name_step, chat_id=self.current_user)
            return

        lang = self.app_language_converter(message.text)
        if lang:
            self.dat["userAppLanguage"] = lang
            self.return_to_registration_checkout(message)
            return False
        self.bot.send_message(self.current_user, "There is no such option", reply_markup=self.app_langs_markup)
        self.bot.register_next_step_handler(message, self.registration_app_language_step, revisit=revisit, chat_id=self.current_user)

    def registration_name_step(self, message, revisit=False):
        if message.text:
            self.dat["username"] = message.text

            if not revisit:
                Coms.reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page, self.markup_pages_count)
                Coms.count_pages(self.cities, self.current_markup_elements, self.markup_pages_count)
                markup = Coms.assemble_markup(self.markup_page, self.current_markup_elements, 0)

                self.bot.send_message(self.current_user, "What languages do you speak?", reply_markup=markup)
                self.bot.send_message(self.current_user, "Chose one from above, or simply type to chat", reply_markup=self.okMarkup)
                self.bot.register_next_step_handler(message, self.registration_languages_step, chat_id=self.current_user)
                return False

            self.return_to_registration_checkout(message)

    def registration_languages_step(self, message, revisit):
        msg_text = message.text.lower().strip()
        if msg_text != "ok":
            language = self.spoken_languages_convertor(msg_text)
            if language:
                if language in self.chosen_languages:
                    Coms.remove_tick_from_element(self.bot, self.current_user, self.current_inline_message_id,
                                                  self.current_markup_elements,
                                                  self.markup_page, str(self.previous_item))
                    self.chosen_languages.remove(language)
                    self.bot.send_message(self.current_user, "Removed", reply_markup=self.okMarkup)
                else:

                    Coms.add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id, self.current_markup_elements,
                                                 self.markup_page, str(language))
                    self.chosen_languages.append(language)
                    self.bot.send_message(self.current_user, "Added", reply_markup=self.okMarkup)
            else:
                self.bot.send_message(self.current_user, "language was not recognized, try finding it in our list above")
                self.bot.register_next_step_handler(message, self.registration_languages_step, chat_id=self.current_user)
                return False

        if self.chosen_languages:
            self.dat["languages"] = self.chosen_languages

            #if not revisit: # Redundant, user has to choose language levels after changing the country as well
            self.previous_item = ''
            langs = copy.copy(self.chosen_languages)
            if len(langs) > 0:
                index = 0
                self.bot.send_message(self.current_user, f"Please, choose a level of {self.languages[langs[index]]}", reply_markup=self.language_levels_markup)
                self.bot.register_next_step_handler(message, self.registration_age_step, langs=langs, index=index, chat_id=self.current_user)
                return False

            if not revisit:
                self.bot.send_message(self.current_user, f"Something went wrong")
                self.bot.send_message(self.current_user, f"How old are you?")
                self.bot.register_next_step_handler(message, self.registration_age_step, chat_id=self.current_user)
                return False
            self.return_to_registration_checkout(message)

        else:
            self.bot.send_message(self.current_user, "You haven't chosen any languages!")
            self.bot.register_next_step_handler(message, self.registration_languages_step, chat_id=self.current_user)

    def registration_language_level_step(self, message, langs, index, revisit=False):
        if message.text in self.language_levels:
            self.chosen_languages.append(message.text)
            index += 1
        else:
            self.bot.send_message(self.current_user, "No such option", reply_markup=self.language_levels_markup)
            self.bot.register_next_step_handler(message, self.registration_languages_step, langs=langs, index=index, chat_id=self.current_user)
            return False

        if len(langs) >= index:
            self.bot.send_message(self.current_user, f"Please, choose a level of {self.languages[langs[index]]}", reply_markup=self.language_levels_markup)
            self.bot.register_next_step_handler(message, self.registration_age_step, langs=langs, index=index, chat_id=self.current_user)
            return False

        if not revisit:
            self.bot.send_message(self.current_user, "How old are you?")
            self.bot.register_next_step_handler(message, self.registration_age_step, chat_id=self.current_user)
            return False

        self.return_to_registration_checkout(message)

    def registration_age_step(self, message, revisit=False):
        try:
            age = int(message.text)
            self.dat["age"] = age
            if 100 >= age >= 16:
                if not revisit:
                    Coms.reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page,
                                     self.markup_pages_count)
                    Coms.count_pages(self.countries, self.current_markup_elements, self.markup_pages_count)
                    markup = Coms.assemble_markup(self.markup_page, self.current_markup_elements, 0)

                    self.previous_item = ''
                    self.current_inline_message_id = self.bot.send_message(self.current_user, "Where are you planning to conduct events in?", reply_markup=markup).json['message_id']
                    self.bot.register_next_step_handler(message, self.registration_country_step, chat_id=self.current_user)
                    return False
                self.return_to_registration_checkout(message)
                return False

            self.bot.send_message(self.current_user, "Your age is not corresponds to our age policies") #TODO: Write that policy and send it as a file
            self.bot.register_next_step_handler(message, self.registration_age_step, chat_id=self.current_user)

        except:
            self.bot.send_message(self.current_user, "Numbers please :-)")
            self.bot.register_next_step_handler(message, self.registration_age_step, chat_id=self.current_user)

    def registration_country_step(self, message, revisit=False):
        msg_text = message.text.lower().strip()
        if msg_text != "ok":
            country = self.country_convertor(msg_text)
            if country:
                if self.previous_item:
                    Coms.remove_tick_from_element(self.bot, self.current_user, self.current_inline_message_id,
                                                  self.current_markup_elements,
                                                  self.markup_page, str(self.previous_item))

                    self.bot.send_message(self.current_user, "Removed", reply_markup=self.okMarkup)


                self.country = country
                self.previous_item = country
                Coms.add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id, self.current_markup_elements,
                                             self.markup_page, str(country))
                self.bot.send_message(self.current_user, "Added", reply_markup=self.okMarkup)
            else:
                self.bot.send_message(self.current_user, "Country was not recognized, try finding it in our list above")
                self.bot.register_next_step_handler(message, self.registration_country_step, chat_id=self.current_user)
                return False

        if self.country:
            self.dat["userCountryId"] = self.country

            #if not revisit: # Redundant, user has to choose city after changing the country as well
            cities = json.loads(
                requests.get(f"https://localhost:44381/GetCities/{self.country}/{self.dat['userAppLanguage']}",
                             verify=False).text)
            for city in cities:
                self.cities[city["id"]] = city["cityName"].lower()

            Coms.reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page, self.markup_pages_count)
            Coms.count_pages(self.cities, self.current_markup_elements, self.markup_pages_count)
            markup = Coms.assemble_markup(self.markup_page, self.current_markup_elements, 0)

            self.previous_item = ''
            self.current_inline_message_id = self.bot.send_message(self.current_user, "Which city are you planning to conduct events in?", reply_markup=markup).json['message_id']
            self.bot.send_message(self.current_user, "Chose one from above, or simply type to chat", reply_markup=self.okMarkup)
            self.bot.register_next_step_handler(message, self.registration_contact_tel_step, revisit=revisit, chat_id=self.current_user)
            return False
        else:
            self.bot.send_message(self.current_user, "You haven't chosen a country!")
            self.bot.register_next_step_handler(message, self.registration_city_step, chat_id=self.current_user)

    def registration_city_step(self, message, revisit=False):
        msg_text = message.text.lower().strip()
        if msg_text != "ok":
            city = self.city_convertor(msg_text)
            if city:
                if self.previous_item:
                    Coms.remove_tick_from_element(self.bot, self.current_user, self.current_inline_message_id,
                                                  self.current_markup_elements,
                                                  self.markup_page, str(self.previous_item))

                    self.bot.send_message(self.current_user, "Removed", reply_markup=self.okMarkup)


                self.city = city
                self.previous_item = city
                Coms.add_tick_to_element(self.bot, self.current_user, self.current_inline_message_id, self.current_markup_elements,
                                             self.markup_page, str(city))
                self.bot.send_message(self.current_user, "Added", reply_markup=self.okMarkup)
            else:
                self.bot.send_message(self.current_user, "City was not recognized, try finding it in our list above")
                self.bot.register_next_step_handler(message, self.registration_country_step, chat_id=self.current_user)
                return False

        if self.city:
            self.dat["userCityId"] = self.city
            if not revisit:
                self.previous_item = ''
                self.bot.send_message(self.current_user,
                                      "Now, lets get to a basic contact info :-). Send me your phone number",
                                      reply_markup=self.skip_markup)
                self.bot.register_next_step_handler(message, self.registration_contact_tel_step,
                                                    chat_id=self.current_user)
                return False
            self.return_to_registration_checkout(message)

        else:
            self.bot.send_message(self.current_user, "You haven't chosen a city!")
            self.bot.register_next_step_handler(message, self.registration_city_step, chat_id=self.current_user)

    def registration_contact_tel_step(self, message, revisit=False):
        if message.text:
            if message.text != "skip":
                self.dat["tel"] = message.text
            if not revisit:
                self.bot.send_message(self.current_user, "Gotchas, now email", reply_markup=self.skip_markup)
                self.bot.register_next_step_handler(message, self.registration_contact_email_step, chat_id=self.current_user)
                return False
            self.return_to_registration_checkout(message)
            return False

        self.bot.send_message(self.current_user, "Something went wrong, Please try again", reply_markup=self.skip_markup)
        self.bot.register_next_step_handler(message, self.registration_contact_tel_step, chat_id=self.current_user)

    def registration_contact_email_step(self, message, revisit=False):
        if message.text:
            if message.text != "skip":
                self.dat["email"] = message.text
            if not revisit:
                self.bot.send_message(self.current_user, "Gotchas, now instagram",  reply_markup=self.skip_markup)
                self.bot.register_next_step_handler(message, self.registration_contact_instagram_step, chat_id=self.current_user)
                return False
            self.return_to_registration_checkout(message)
            return False

        self.bot.send_message(self.current_user, "Something went wrong, Please try again", reply_markup=self.skip_markup)
        self.bot.register_next_step_handler(message, self.registration_contact_email_step, chat_id=self.current_user)

    def registration_contact_instagram_step(self, message, revisit=False):
        if message.text:
            if message.text != "skip":
                self.dat["instagram"] = message.text
            if not revisit:
                self.bot.send_message(self.current_user, "Gotchas, now facebook username", reply_markup=self.skip_markup)
                self.bot.register_next_step_handler(message, self.registration_contact_facebook_step, chat_id=self.current_user)
                return False
            self.return_to_registration_checkout(message)
            return False

        self.bot.send_message(self.current_user, "Something went wrong, Please try again", reply_markup=self.skip_markup)
        self.bot.register_next_step_handler(message, self.registration_contact_instagram_step, chat_id=self.current_user)

    def registration_contact_facebook_step(self, message, revisit=False):
        if message.text:
            if message.text != "skip":
                self.dat["facebook"] = message.text
            if not revisit:
                self.bot.send_message(self.current_user, "Ok, we are done registering. Wanna change something?", reply_markup=self.registration_checkout_markup)
                self.bot.register_next_step_handler(message, self.registration_checkout_step, chat_id=self.current_user)
                return False
            self.return_to_registration_checkout(message)
            return False

        self.bot.send_message(self.current_user, "Something went wrong, Please try again")
        self.bot.register_next_step_handler(message, self.registration_contact_facebook_step, chat_id=self.current_user)

    def registration_checkout_step(self, message):
        if message.text == "1":
            self.bot.send_message(self.current_user, "Choose an app language")
            self.bot.register_next_step_handler(message, self.registration_app_language_step, revisit=True, chat_id=self.current_user)
        elif message.text == "2":
            self.bot.send_message(self.current_user, "What is you name or nickname?")
            self.bot.register_next_step_handler(message, self.registration_name_step, revisit=True, chat_id=self.current_user)
        elif message.text == "3":
            self.bot.send_message(self.current_user, "How old are you?")
            self.bot.register_next_step_handler(message, self.registration_age_step, revisit=True, chat_id=self.current_user)
        elif message.text == "4":
            self.bot.send_message(self.current_user, "Send me your phone number")
            self.bot.register_next_step_handler(message, self.registration_contact_tel_step, revisit=True, chat_id=self.current_user)
        elif message.text == "5":
            self.bot.send_message(self.current_user, "Send me your email")
            self.bot.register_next_step_handler(message, self.registration_contact_email_step, revisit=True, chat_id=self.current_user)
        elif message.text == "6":
            self.bot.send_message(self.current_user, "Send me your instagram name")
            self.bot.register_next_step_handler(message, self.registration_contact_instagram_step, revisit=True, chat_id=self.current_user)
        elif message.text == "7":
            self.bot.send_message(self.current_user, "Send me your facebook name")
            self.bot.register_next_step_handler(message, self.registration_contact_facebook_step, revisit=True, chat_id=self.current_user)
        elif message.text == "8":
            Coms.reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page,
                             self.markup_pages_count)
            Coms.count_pages(self.countries, self.current_markup_elements, self.markup_pages_count)
            markup = Coms.assemble_markup(self.markup_page, self.current_markup_elements, 0)

            self.previous_item = ''
            self.country = None
            self.current_inline_message_id = self.bot.send_message(self.current_user, "Where are you planning to conduct events in?", reply_markup=markup).json['message_id']
            self.bot.register_next_step_handler(message, self.registration_country_step, revisit=True, chat_id=self.current_user)
        elif message.text == "9":
            Coms.reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page,
                             self.markup_pages_count)
            Coms.count_pages(self.cities, self.current_markup_elements, self.markup_pages_count)
            markup = Coms.assemble_markup(self.markup_page, self.current_markup_elements, 0)

            self.bot.send_message(self.current_user, "What languages do you speak?", reply_markup=markup)
            self.bot.send_message(self.current_user, "Chose one from above, or simply type to chat",
                                  reply_markup=self.okMarkup)
            self.bot.register_next_step_handler(message, self.registration_languages_step, chat_id=self.current_user)
        elif message.text == "10":
            self.register_sponsor()

    def create_event_status_step(self, message, revisit): #Online or Offline
        pass
    def create_event_name_step(self, message, revisit):
        pass
    def create_event_languages_step(self, message, revisit):
        pass
    def create_event_min_age_step(self, message, revisit):
        pass
    def create_event_max_age_step(self, message, revisit):
        pass
    def create_event_description_step(self, message, revisit):
        pass
    def create_event_price_step(self, message, revisit):
        pass
    def create_event_country_step(self, message, revisit):
        pass
    def create_event_city_step(self, message, revisit):
        pass
    def create_event_start_date_step(self, message, revisit):
        pass
    def create_event_has_group_step(self, message, revisit):
        pass
    def create_event_group_link_step(self, message, revisit):
        pass
    def create_event_photo_step(self, message, revisit):
        pass
    def create_event_bounty_step(self, message, revisit):
        pass
    def create_event_checkout_step(self, message, revisit):
        pass
    def create_event_template_step(self, message, revisit):
        pass
    def create_template_name_step(self, message, revisit):
        pass

    def get_localisation(self, app_language):
        for country in json.loads(
                requests.get(f"https://localhost:44381/GetCountries/{app_language}", verify=False).text):
            self.countries[country["id"]] = country["countryName"].lower().strip()

        for language in json.loads(
                requests.get(f"https://localhost:44381/GetLanguages/{app_language}", verify=False).text):
            self.languages[language["id"]] = language["languageName"].lower().strip()

    def exit_handler(self, message):
        self.bot.send_message(self.current_user, "Exiting module...")
        self.destruct()

    def redundant_ad_creating(self, message):
        self.bot.send_message(self.current_user, self.post_creation_start_message)
        self.bot.register_next_step_handler(message, self.ad_creating1, revisit=True, chat_id=self.current_user)

    def ad_creating1(self, message, revisit=False, cb=False):
        if message.text:
            if revisit:
                self.ad_data["text"] = message.text
                self.ad_creating_show(message, cb=cb)
                return revisit

            self.ad_data["SponsorId"] = self.current_user
            self.ad_data["text"] = message.text
            self.bot.send_message(self.current_user, self.post_creation_midd_message)

            self.bot.register_next_step_handler(message, self.ad_creating2, chat_id=self.current_user)

        else:
            self.bot.send_message(self.current_user, self.post_creation_no_ad_text_message)
            self.bot.register_next_step_handler(message, self.ad_creating1, chat_id=self.current_user)

    def ad_creating2(self, message, cb=False):
        if message.video:
            self.ad_data["photo"] = ""
            self.ad_data["video"] = message.video[len(message.video) - 1].file_id
        elif message.photo:
            self.ad_data["video"] = ""
            self.ad_data["photo"] = message.photo[len(message.photo) - 1].file_id
        self.ad_creating_show(message, cb=cb)

    def ad_creating_show(self, message, cb=False):
        if self.ad_data["photo"]:
            self.bot.send_message(self.current_user, self.post_creation_show_message, reply_markup=self.markup_ad_checkout)
            self.bot.send_photo(self.current_user, self.ad_data["photo"], self.ad_data["text"])
            self.bot.register_next_step_handler(message, self.ad_creating_checkout, cb=cb, chat_id=self.current_user)
        elif self.ad_data["video"]:
            self.bot.send_message(self.current_user, self.post_creation_show_message, reply_markup=self.markup_ad_checkout)
            self.bot.send_video(self.current_user, self.ad_data["video"], self.ad_data["text"])
            self.bot.register_next_step_handler(message, self.ad_creating_checkout, cb=cb, chat_id=self.current_user)
        else:
            self.bot.send_message(self.current_user, self.post_creation_no_ad_data_message)
            self.bot.register_next_step_handler(message, self.ad_creating2, chat_id=self.current_user)

    def ad_creating_checkout(self, message, cb=False):
        if message.text == "Ok":
            if cb:
                self.update_ad()
                return cb
            self.create_ad()
        elif message.text == "Change Text":
            self.bot.send_message(self.current_user, self.post_creation_start_message)
            self.bot.register_next_step_handler(message, self.ad_creating1, revisit=True, cb=cb, chat_id=self.current_user)
        elif message.text == "Change Media":
            self.bot.send_message(self.current_user, self.post_creation_midd_message)
            self.bot.register_next_step_handler(message, self.ad_creating2, cb=cb, chat_id=self.current_user)
        elif message.text == "Delete ad":
            self.bot.send_message(self.current_user, "Are you sure that you want to delete an ad?",
                                  reply_markup=self.markup_delete)
            self.bot.register_next_step_handler(message, self.delete_ad_step, cb=cb, chat_id=self.current_user)
        else:
            self.bot.send_message(self.current_user, self.markup_error_message, reply_markup=self.markup_ad_checkout)
            self.bot.register_next_step_handler(message, self.ad_creating_checkout, cb=cb, chat_id=self.current_user)

    def delete_ad_step(self, message, cb):
        if message.text == "Yes":
            self.remove_ad()
        elif message.text == "No":
            self.bot.send_message(self.current_user, self.post_creation_show_message, reply_markup=self.markup_ad_checkout)
            self.bot.register_next_step_handler(message, self.ad_creating_checkout, cb=cb, chat_id=self.current_user)
        else:
            self.bot.send_message(self.current_user, "No such option")
            self.bot.send_message(self.current_user, "Are you sure that you want to delete an ad?",
                                  reply_markup=self.markupYN)
            self.bot.register_next_step_handler(message, self.delete_ad_step, cb=cb, chat_id=self.current_user)

    def callback_handler(self, call):
        if call.message.id not in self.old_queries:

            if call.data == "-1" or call.data == "-2":
                index = self.index_converter(call.data)
                if self.markup_page + index <= self.markup_pages_count or self.markup_page + index >= 1:
                    markup = Coms.assemble_markup(self.markup_page, self.current_markup_elements, index)
                    self.bot.edit_message_reply_markup(chat_id=call.message.chat.id, reply_markup=markup,
                                                       message_id=call.message.id)
                    self.markup_page += index
                return False

            elif "/" in call.data:      #TODO: Make it work another way... maybe
                self.bot.answer_callback_query(call.id, call.data)
                return False

            if call.message.text == "Where are you planning to conduct events in?":
                if call.data in self.countries.keys():
                    self.country = int(call.data)
                    self.bot.answer_callback_query(call.id, "Gotcha")
                    if self.previous_item:
                        Coms.remove_tick_from_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                                      self.markup_page, self.previous_item)
                    Coms.add_tick_to_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                             self.markup_page, call.data)
                    self.previous_item = call.data
                else:
                    self.bot.send_message(call.message.chat.id, "Incorrect country code")
                return False

            elif call.message.text == "Which city are you planning to conduct events in?":
                if int(call.data) in self.cities.keys():
                    self.city = int(call.data)
                    self.bot.answer_callback_query(call.id, "Gotcha")
                    if self.previous_item:
                        Coms.remove_tick_from_element(self.bot, self.current_user, call.message.id,
                                                      self.current_markup_elements,
                                                      self.markup_page, self.previous_item)
                    Coms.add_tick_to_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                             self.markup_page, call.data)
                    self.previous_item = call.data
                else:
                    self.bot.answer_callback_query(call.id, "Incorrect city code")

                return False
            elif call.maessage.text == "What languages do you speak?":
                if int(call.data) in self.languages.keys():
                    if call.data in self.chosen_languages:
                        self.chosen_languages.remove(int(call.data))
                        Coms.remove_tick_from_element(self.bot, self.current_user, call.message.id,
                                                      self.current_markup_elements,
                                                      self.markup_page, self.previous_item)
                        self.bot.answer_callback_query(call.id, "Removed")
                    else:
                        self.chosen_languages.append(int(call.data))
                        Coms.add_tick_to_element(self.bot, self.current_user, call.message.id, self.current_markup_elements,
                                                 self.markup_page, call.data)
                        self.bot.answer_callback_query(call.id, "Added")
                else:
                    self.bot.answer_callback_query(call.id, "Incorrect city code")

            self.old_queries.append(call.message.id)
            self.ad_data = json.loads(requests.get(f"https://localhost:44381/GetSponsorAd/{self.current_user}/{call.data}", verify=False).text)
            self.ad_creating_show(call.message, cb=True)


    def register_sponsor(self):
        if self.isSponsor:
            self.bot.send_message(self.current_user, self.registration_interrupt_message)
            return False

        self.dat["id"] = self.current_user
        d = json.dumps(self.dat)

        requests.post(f"https://localhost:44381/RegisterSponsor", d, headers={
            "Content-Type": "application/json"}, verify=False)
        self.isSponsor = Helpers.check_user_is_sponsor(self.current_user)
        self.isAwaiting = Helpers.check_user_is_awaiting(self.current_userName)
        self.bot.send_message(self.current_user, self.registration_successful_message)

    def update_ad(self):
        d = json.dumps(self.ad_data)
        requests.post(f"https://localhost:44381/AdUpdate", d, headers={
            "Content-Type": "application/json"}, verify=False)
        self.bot.send_message(self.current_user, self.post_update_success_message, reply_markup=self.markup2)

    def construct_sponsor_data_message(self):
        data = f"Username: {self.dat['username']}" \
               f"\nAge: {self.dat['age']}" \
               f"\nCountry: {self.countries[self.dat['userCountryId']]}" \
               f"\nCity: {self.cities[self.dat['userCityId']]}" \
               f"\nTel. number: {self.dat['tel']}" \
               f"\nEmail: {self.dat['email']}" \
               f"\nInstagram: {self.dat['instagram']}" \
               f"\nFacebook: {self.dat['facebook']}"
        if self.chosen_languages:
            i = 0
            for lang in self.chosen_languages:
                data += f"\n{self.languages[lang]} - {self.chosen_languages_levels[i]}"
                i += 1

        return data

    def create_ad(self):
        self.ad_data["Description"] = ""
        d = json.dumps(self.ad_data)
        requests.post(f"https://localhost:44381/AdAdd", d, headers={
            "Content-Type": "application/json"}, verify=False)
        self.bot.send_message(self.current_user, self.post_creation_success_message, reply_markup=self.markup2)

    def return_to_registration_checkout(self, message):
        self.previous_item = ''
        self.bot.send_message(self.current_user, "Done, something else?", reply_markup=self.registration_checkout_markup)
        self.bot.send_message(self.current_user, self.construct_sponsor_data_message())
        self.bot.register_next_step_handler(message, self.registration_checkout_step, chat_id=self.current_user)

    def app_language_converter(self, lang):
        for l in self.app_langs:
            if lang == self.app_langs[l]:
                return int(l)
        return None

    def country_convertor(self, country):
        for c in self.countries:
            if country == self.countries[c]:
                return int(c)
        return None

    def spoken_languages_convertor(self, lang):
        for l in self.languages:
            if lang == self.languages[l]:
                return int(l)
        return None

    def city_convertor(self, city):
        for c in self.cities:
            if city == self.cities[c]:
                return int(c)
        return None

    @staticmethod
    def index_converter(index):
        if index == "-1":
            return -1
        return 1

    def destruct(self):
        self.bot.callback_query_handlers.remove(self.chCode)
        self.bot.message_handlers.remove(self.mhCode)
        self.bot.message_handlers.remove(self.eh)
        self.sponsor_handlers.remove(self)
        go_back_to_main_menu(self.bot, self.current_user)
        Helpers.switch_user_busy_status(self.current_user)
        del self
