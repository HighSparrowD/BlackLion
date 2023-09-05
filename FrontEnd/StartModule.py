from Registration import *


class StartModule:
    def __init__(self, bot: TeleBot, message: any):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id

        self.active_message = None
        self.secondary_message = None

        self.user_localisation = 0

        self.hasEnteredPromo = False

        self.app_languages = Helpers.get_app_languages()
        terms_url = "https://telegra.ph/Personality-Bot--Terms-and-conditions-09-01"
        about_us_url = "https://telegra.ph/Personality-Bot--about-us-09-01"

        self.register_message = "Please note, that by registering you are agreeing to all the rules located in 'Rules section'. We are highly advise to read all the rules before registering\nAre you sure you want to continue?"

        self.startMarkup = InlineKeyboardMarkup(row_width=1).add(InlineKeyboardButton("✨Register profile✨", callback_data="4"),
                                                                 InlineKeyboardButton("About  us", url=about_us_url),
                                                                 InlineKeyboardButton("Rules", url=terms_url),
                                                                 InlineKeyboardButton("Enter promo code", callback_data="3"),
                                                                 InlineKeyboardButton("🔙Go Back", callback_data="-2"))

        self.registerMarkup = InlineKeyboardMarkup().add(InlineKeyboardButton("Yes, I agree to terms and conditions", callback_data="1"),
                                                         InlineKeyboardButton("🔙Go Back", callback_data="-1"))

        self.go_backMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("Go Back")

        self.app_languages_markup = InlineKeyboardMarkup()

        for lang in self.app_languages:
            self.app_languages_markup.add(InlineKeyboardButton(lang["name"], callback_data=lang["name"]))

        invitorId = self.get_invitor_id(message.html_text)
        if invitorId:
            if Helpers.user_invitation_link(invitorId, self.current_user):
                #TODO: Delete or Replace in production
                self.bot.send_message(self.current_user, "*")
            # bot.send_message(message.from_user.id, "Sorry. Something went wrong")

        self.ch = self.bot.register_callback_query_handler(message, self.callback_handler, user_id=self.current_user)

        self.app_language_step()

    def app_language_step(self):
        self.send_active_message("Welcome to our bot.\nPlease select a language before proceeding", markup=self.app_languages_markup)

    def start(self):
        self.send_active_message(f"Hello and welcome to Personality bot", markup=self.startMarkup)

    # def about_us(self, message):
    #     # self.bot.edit_message_text(self.about_us_message, self.current_user, self.active_message)
    #     self.bot.send_message(self.current_user, self.about_us_message)
    #     self.start(message)

    # def rules(self, message):
    #     # self.bot.edit_message_text(self.rules_message, self.current_user, self.active_message)
    #     self.bot.send_message(self.current_user, self.rules_message)
    #     self.start(message)

    def enter_promo(self, message, acceptMode=False):
        if not acceptMode:
            # self.bot.edit_message_text(, self.current_user, self.active_message, reply_markup=self.go_backMarkup)
            self.send_active_message("Please, enter your promo", markup=self.go_backMarkup)
            self.bot.register_next_step_handler(message, self.enter_promo, acceptMode=True, chat_id=self.current_user)
        else:
            if Helpers.check_promo_is_valid(self.current_user, message.text, True):
                self.send_secondary_message("Promo code is accepted :)")
                self.hasEnteredPromo = True
                self.start()
            else:
                if message.text == "Go Back":
                    self.start()
                    return

                self.send_secondary_message("Wrong promo code", markup=self.go_backMarkup)
                self.bot.register_next_step_handler(message, self.enter_promo, acceptMode=True, chat_id=self.current_user)

    def register(self, message, acceptMode=False):
        if not acceptMode:
            self.send_active_message(self.register_message, markup=self.registerMarkup)
        else:
            self.bot.callback_query_handlers.remove(self.ch)
            self.delete_active_message()
            self.delete_secondary_message()
            Registrator(self.bot, self.message, localizationIndex=self.user_localisation)

    def callback_handler(self, call):
        if call.data == "1":
            self.register(call.message, acceptMode=True)
        elif call.data == "-1":
            self.start()
        elif call.data == "-2":
            self.app_language_step()
        elif call.data == "3":
            self.enter_promo(call.message)
        elif call.data == "4":
            self.register(call.message)
        else:
            self.user_localisation = call.data
            self.get_localisations()
            self.start()

    def get_localisations(self):
        #TODO: Get localization from string localizer API
        pass

    def get_invitor_id(self, html_text):
        return html_text.strip("/start")

    def send_active_message(self, text, markup):
        try:
            if self.active_message is not None:
                self.bot.edit_message_text(text, self.current_user, self.active_message, reply_markup=markup)
                return
            self.active_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except Exception as ex:
            self.delete_active_message()
            self.send_active_message(text, markup)

    def delete_active_message(self):
        if self.active_message is not None:
            self.bot.delete_message(self.current_user, self.active_message)
            self.active_message = None

    def send_secondary_message(self, text, markup=None):
        try:
            if self.secondary_message:
                self.bot.edit_message_text(text, self.current_user, self.secondary_message, reply_markup=markup)
                return

            self.secondary_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except:
            self.delete_secondary_message()
            self.send_secondary_message(text, markup)

    def delete_secondary_message(self):
        try:
            if self.secondary_message:
                self.bot.delete_message(self.current_user, self.secondary_message)
                self.secondary_message = None

        except:
            self.secondary_message = None
