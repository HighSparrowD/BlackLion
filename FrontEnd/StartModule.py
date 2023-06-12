from Registration import *


class StartModule:
    def __init__(self, bot, message):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id

        self.active_message = None

        self.user_localisation = 0

        self.hasEnteredPromo = False

        # TODO: load from API
        self.app_languages = ["EN", "RU", "UK"]

        self.startMessage = "1. About us\n2. Read rules\n3. Enter Promo code\n4. Register profile"
        self.registerMessage = "1. Yes\n2. Go back"
        self.about_us_message = "--About US-- TODO: Fill up"
        self.rules_message = "--Rules-- TODO: Fill up"
        self.register_message = "Please note, that by registering you are agreeing to all the rules located in 'Rules section'. We are highly advise to read all the rules before registering\nAre you sure you want to continue?"
        self.startMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("1", "2", "3", "4")
        self.registerMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("Yes", "Go Back")
        self.go_backMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("Go Back")
        self.app_languages_markup = None

        invitorId = self.get_invitor_id(message.html_text)
        if invitorId:
            if Helpers.user_invitation_link(invitorId, self.current_user):
                #TODO: Delete or Replace in production
                self.bot.send_message(self.current_user, "*")
            # bot.send_message(message.from_user.id, "Sorry. Something went wrong")

        self.app_language_step(message)

    def app_language_step(self, msg, acceptMode=False):
        if not acceptMode:

            self.app_languages_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)
            start_message = "Welcome to our bot.\nPlease select a language before proceeding"

            self.app_languages_markup.add(self.app_languages[0], self.app_languages[1], self.app_languages[2])
            self.bot.send_message(msg.chat.id, start_message, reply_markup=self.app_languages_markup)
            self.bot.register_next_step_handler(msg, self.app_language_step, acceptMode=True, chat_id=self.current_user)

        else:
            self.user_localisation = msg.text
            if self.user_localisation in self.app_languages:
                self.get_localisations()
                self.start(msg)
            else:
                self.bot.send_message(self.current_user, "There is no such language. Try again", reply_markup=self.app_languages_markup)
                self.bot.register_next_step_handler(msg, self.app_language_step, acceptMode=acceptMode, chat_id=self.current_user)

    def start(self, message, acceptMode=False):
        if not acceptMode:
            self.active_message = self.bot.send_message(self.current_user, f"Hello and welcome to Personality bot\n--Tell more about us--\n{self.startMessage}", reply_markup=self.startMarkup)
            self.bot.register_next_step_handler(message, self.start, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                self.about_us(message)
            elif message.text == "2":
                self.rules(message)
            elif message.text == "3":
                if not self.hasEnteredPromo:
                    self.enter_promo(message)
                else:
                    self.start(message)
            elif message.text == "4":
                self.register(message)
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.startMarkup)
                self.bot.register_next_step_handler(message, self.start, acceptMode=acceptMode, chat_id=self.current_user)

    def about_us(self, message):
        # self.bot.edit_message_text(self.about_us_message, self.current_user, self.active_message)
        self.bot.send_message(self.current_user, self.about_us_message)
        self.start(message)

    def rules(self, message):
        # self.bot.edit_message_text(self.rules_message, self.current_user, self.active_message)
        self.bot.send_message(self.current_user, self.rules_message)
        self.start(message)

    def enter_promo(self, message, acceptMode=False):
        if not acceptMode:
            #Nullify active message to avoid inconveniences
            self.active_message = None
            # self.bot.edit_message_text(, self.current_user, self.active_message, reply_markup=self.go_backMarkup)
            self.bot.send_message(self.current_user, "Please, enter your promo", reply_markup=self.go_backMarkup)
            self.bot.register_next_step_handler(message, self.enter_promo, acceptMode=True, chat_id=self.current_user)
        else:
            if Helpers.check_promo_is_valid(self.current_user, message.text, True):
                self.bot.send_message(self.current_user, "Promo code is accepted :)")
                self.hasEnteredPromo = True
                self.start(message)
            else:
                if message.text == "Go Back":
                    self.start(message)
                    return

                self.bot.send_message(self.current_user, "Wrong promo code", reply_markup=self.go_backMarkup)
                self.bot.register_next_step_handler(message, self.enter_promo, acceptMode=True, chat_id=self.current_user)

    def register(self, message, acceptMode=False):
        if not acceptMode:
            self.active_message = None
            self.bot.send_message(self.current_user, self.register_message, reply_markup=self.registerMarkup)
            self.bot.register_next_step_handler(message, self.register, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "Yes":
                Registrator(self.bot, message, localizationIndex=self.user_localisation)
            elif message.text == "Go Back":
                self.start(message)
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.registerMarkup)
                self.bot.register_next_step_handler(message, self.register, acceptMode=acceptMode, chat_id=self.current_user)

    def get_localisations(self):
        #TODO: Get localization from string localizer API
        pass

    def get_invitor_id(self, html_text):
        return html_text.strip("/start")