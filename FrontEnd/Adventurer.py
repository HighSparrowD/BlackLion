from telebot.types import InlineKeyboardMarkup, InlineKeyboardButton, KeyboardButton, ReplyKeyboardMarkup
from Core import HelpersMethodes as Helpers
from Common.Menues import count_pages, assemble_markup, reset_pages, add_tick_to_element, remove_tick_from_element, index_converter
import requests
import json

from Common.Menues import go_back_to_main_menu


class Adventurer:
    def __init__(self, bot, message, hasVisited=False):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id
        self.user_info = Helpers.get_user_info(self.current_user)
        self.hasVisited = hasVisited
        self.hasPremium = self.user_info["hasPremium"]
        self.isIdentityConfirmed = self.user_info["isIdentityConfirmed"]
        self.user_localization = self.user_info["userDataInfo"]["languageId"]
        self.managingUsersAdventures = False
        self.managingSubscribedAdventures = False

        Helpers.switch_user_busy_status(self.current_user)

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

        self.countries = []
        self.cities = []
        self.country = None
        self.city = None

        self.start_markup = InlineKeyboardMarkup() \
            .add(InlineKeyboardButton("Manage my adventures", callback_data="1")) \
            .add(InlineKeyboardButton("Subscribed adventures", callback_data="2")) \
            .add(InlineKeyboardButton("Search adventures", callback_data="3")) \
            .add(InlineKeyboardButton("Go Back", callback_data="-20"))

        self.search_markup = InlineKeyboardMarkup() \
            .add(InlineKeyboardButton("➕Search➕", callback_data="3")) \
            .add(InlineKeyboardButton("Go Back", callback_data="-20"))

        self.adventure_state_markup = InlineKeyboardMarkup() \
            .add(InlineKeyboardButton("Online", callback_data="4"), InlineKeyboardButton("❓", callback_data="304")) \
            .add(InlineKeyboardButton("Offline", callback_data="5"), InlineKeyboardButton("❓", callback_data="305"))

        self.okMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("Ok")

        self.my_adventuresMarkup = InlineKeyboardMarkup()
        self.subscribed_adventuresMarkup = InlineKeyboardMarkup()

        count = json.loads(requests.get(f"https://localhost:44381/AdventureCount/{self.current_user}", verify=False).text)

        self.createdCount = count["created"]
        self.subscribedCount = count["subscribed"]

        self.creation_limit = 1
        self.subscription_limit = 1

        #Extend limitations for premium user
        if self.hasPremium:
            self.creation_limit = 50
            self.subscription_limit = 50

    def start(self):
        self.previous_section = self.destruct
        self.send_active_message("<b><i>Please, select an option</i></b>", markup=self.start_markup)

    def register_start(self):
        if self.createdCount + 1 > self.creation_limit:
            self.send_secondary_message(f"You reached the limit. You can create up to {self.creation_limit} adventures. Please, wait until one of your adventures ends, or delete it manually.")
        else:
            self.register_state_step(self.message)

    def register_state_step(self, acceptMode=False):
        if not acceptMode:
            self.send_active_message("What kind of adventure will it be?", markup=self.adventure_state_markup)

    def register_online(self, showDescription=False):
        if not showDescription:
            self.delete_active_message()
            self.delete_secondary_message()
            self.register_age_step()
        else:
            self.send_secondary_message("<b>Registering online adventure does NOT require any identity confirmation. Good example of online adventure is: -------</b>")

    def register_offline(self, showDescription=False):
        if not showDescription:
            #Only users with confirmed identity can create offline adventures
            if not self.isIdentityConfirmed:
                return

            self.delete_active_message()
            self.delete_secondary_message()
            self.register_country_step()

        else:
            self.send_secondary_message("<b>Registering offline adventure requires identity confirmation. It is described in user agreement (NUMBER OF PARAGRAPH) Good example of offline adventure is: -------</b>")

    def register_country_step(self, message=None, acceptMode=False, editMode=False):
        self.question_index = 4

        if not acceptMode:
            if self.countries is not None:
                countries = requests.get(f"https://localhost:44381/GetCountries/{self.user_localization}", verify=False)
                for country in countries:
                    self.countries[country["id"]] = country["countryName"].lower().strip()

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page,
                        self.markup_pages_count)
            count_pages(self.countries, self.current_markup_elements, self.markup_pages_count, additionalButton=True, buttonText="My country", buttonData="-10")
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

            self.previous_item = ''

            self.send_active_message("Please choose a country", markup=markup)

            if editMode:
                self.previous_item = str(self.country)
                add_tick_to_element(self.bot, self.current_user, self.active_message, self.current_markup_elements, self.markup_page, str(self.country))

            self.send_secondary_message("Choose one from above. Or simply type country name to chat", markup=self.okMarkup)
            self.bot.register_next_step_handler(message, self.register_country_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
            if not message.text:
                self.send_secondary_message("Country was not recognized, try finding it in our list above")
                self.bot.register_next_step_handler(message, self.register_country_step, acceptMode=acceptMode,
                                                    editMode=editMode, chat_id=self.current_user)
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
                    self.country = country
                    self.send_secondary_message("Gotcha", markup=self.okMarkup)
                    self.bot.register_next_step_handler(message, self.register_country_step, acceptMode=acceptMode,
                                                        editMode=editMode, chat_id=self.current_user)
                    return True

                self.register_city_step(message, editMode=editMode)
            else:
                self.send_secondary_message("You haven't chosen a country !", markup=self.okMarkup)
                self.bot.register_next_step_handler(message, self.register_country_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

    def register_city_step(self, message=None, acceptMode=False, editMode=False):
        if not acceptMode:
            self.question_index = 5
            self.markup_page = 1

            cities = json.loads(requests.get(f"https://localhost:44381/GetCities/{self.country}/{self.user_localization}", verify=False).text)

            #For edit purposes. If left as they are -> can result bugs
            self.cities.clear()

            for city in cities:
                self.cities[city["id"]] = city["cityName"].lower()

            self.previous_item = ''

            reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page, self.markup_pages_count)
            count_pages(self.cities, self.current_markup_elements, self.markup_pages_count, additionalButton=True, buttonData="-11", buttonText="My city")
            markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

            self.send_active_message("Which city do you live in?", markup=markup)

            if editMode:
                self.previous_item = str(self.city)
                add_tick_to_element(self.bot, self.current_user, self.active_message, self.current_markup_elements, self.markup_page, str(self.city))

            self.send_secondary_message("Chose one from above, or simply type to chat", markup=self.okMarkup)
            self.bot.register_next_step_handler(message, self.register_city_step, acceptMode=True, editMode=editMode, chat_id=self.current_user)
        else:
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
                    self.city = city
                    self.send_secondary_message("Gotcha", markup=self.okMarkup)
                    self.bot.register_next_step_handler(message, self.register_city_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)
                    return True
                else:
                    self.send_secondary_message("City was not recognized, try finding it in our list above")
                    self.bot.register_next_step_handler(message, self.register_city_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)
                    return False

            if self.city or self.city == 0:
                self.previous_item = ''
                # self.data["userCityCode"] = self.city

                #TODO: Apply code below to Adventurer functionality

                # if not editMode:
                #     self.name_step(msg)
                # else:
                #     self.checkout_step(msg)
                self.register_languages_step(message)

                # self.data["userCity"] = self.cities[self.country][self.city]
            else:
                self.send_secondary_message("You haven't chosen a city !", markup=self.okMarkup)
                self.bot.register_next_step_handler(message, self.register_city_step, acceptMode=acceptMode, editMode=editMode, chat_id=self.current_user)

    def register_languages_step(self, message=None, acceptMode=False):
        pass

    def register_age_step(self, message=None, acceptMode=False):
        pass

    def register_description_step(self, message=None, acceptMode=False):
        pass

    def register_place_count_step(self, message=None, acceptMode=False):
        pass

    def register_startDT_step(self, message=None, acceptMode=False):
        pass

    def register_endDT_step(self, message=None, acceptMode=False):
        pass

    def my_adventures_manager(self):
        self.previous_section = self.start
        self.assemble_my_adventures_markup()
        self.managingUsersAdventures = True
        self.managingSubscribedAdventures = False
        self.send_active_message("<b><i>Here are all adventures created by you</i></b>", markup=self.my_adventuresMarkup)

    def subscribed_adventures_manager(self):
        self.previous_section = self.start

        self.managingUsersAdventures = True
        self.managingSubscribedAdventures = False
        if len(self.subscribed_adventuresMarkup) > 1:
            self.send_active_message("<b><i>Here are all adventures, you are subscribed on</i></b>", markup=self.subscribed_adventuresMarkup)

        self.send_active_message("<b><i>You are not subscribed on any adventures!</b></i>", markup=self.search_markup)

    def recurring_adventure_search(self):
        self.delete_active_message()
        self.delete_secondary_message()
        self.previous_section = self.start
        #TODO: finish up

    def start_callback_handler(self, call):
        if call.data == "1":
            self.my_adventures_manager()
        elif call.data == "2":
            self.subscribed_adventures_manager()
        elif call.data == "3":
            self.recurring_adventure_search()

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

    def send_active_message(self, text, markup=None):
        if self.active_message:
            self.bot.edit_message_media(chat_id=self.current_user, message_id=self.active_message, reply_markup=markup)
            return

        self.active_message = self.bot.send_message(self.current_user, caption=text, reply_markup=markup).id

    def send_secondary_message(self, text, markup=None):
        if self.active_message:
            self.bot.edit_message_media(chat_id=self.current_user, message_id=self.secondary_message, reply_markup=markup)
            return

        self.secondary_message = self.bot.send_message(self.current_user, caption=text, reply_markup=markup).id

    def send_simple_message(self, text, markup=None):
        self.bot.send_message(self.current_user, text, reply_markup=markup)

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

    def assemble_my_adventures_markup(self):
        adventures = json.loads(requests.get(f"https://localhost:44381/GetUsersAdventures/{self.current_user}", verify=False).text)
        self.my_adventuresMarkup.clear()

        for adventure in adventures:
            self.my_adventuresMarkup.add(InlineKeyboardButton(adventure["name"], callback_data=adventure["id"]))

        self.my_adventuresMarkup.add(InlineKeyboardButton("➕Add➕", callback_data=""))
        self.my_adventuresMarkup.add(InlineKeyboardButton("Go Back", callback_data="-20"))

    def assemble_subscribed_adventures_markup(self):
        adventures = json.loads(requests.get(f"https://localhost:44381/GetUsersSubscribedAdventures/{self.current_user}", verify=False).text)

        self.subscribed_adventuresMarkup.clear()

        for adventure in adventures:
            self.subscribed_adventuresMarkup.add(InlineKeyboardButton(adventure["name"], callback_data=adventure["id"]))

        self.subscribed_adventuresMarkup.add(InlineKeyboardButton("Go Back", callback_data="-20"))

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

    def destruct(self):
        if self.current_callback_handler:
            self.bot.callback_query_handlers.remove(self.current_callback_handler)

        self.delete_active_message()
        Helpers.switch_user_busy_status(self.current_user)
        go_back_to_main_menu(self.bot, self.current_user, self.message)
        del self