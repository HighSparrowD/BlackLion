from telebot.types import *
import Core.HelpersMethodes as Helpers
import Common.Menues as menues
from Helper import Helper
from TestModule import TestModule


class Shop:
    def __init__(self, bot, message, hasVisited=False, startingTransaction=None, returnMethod=None, active_message=None):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id
        self.hasVisited = hasVisited
        self.returnMethod = returnMethod
        self.startingTransaction = startingTransaction
        self.shouldGreet = True
        self.isDeciding = False
        self.shouldStay = False
        self.activatedElsewhere = True

        if returnMethod is None:
            self.activatedElsewhere = False

        self.active_first_option_price = 0
        self.active_second_option_price = 0
        self.active_third_option_price = 0
        self.active_pack = 0
        self.chosen_pack_price = 0

        self.current_effect_pack = ""
        self.active_message = active_message
        self.active_description_message = None
        self.active_transaction_status_message = None
        self.current_transaction = None

        # TODO: load price lists based on currency selected by user
        self.start_options_message = "1. Premium\n2. Coins\n3. Effects\n4. Personality Points\n5. Tests\n6. Support Us :)\n7. Exit"
        self.premium_price_list_message = "1. 3 days: 5,999 Coins - VALUE CURRENCY\n2. 21 days: 12,999 Coins - VALUE CURRENCY\n3. 30 days 20,999 Coins - VALUE CURRENCY\n4. Go Back"
        self.effects_list_message = "1. 💥Second Chance💥\n2. 💥The Valentine💥\n3. 💥The Detector💥\n4. 💥The Nullifier💥\n5. 💥Card Deck Mini💥\n6. 💥Card Deck Platinum💥\n7. Go Back"
        self.secondChance_price_list_message = "1. Buy 1: 599 Coins - VALUE CURRENCY\n2. Buy 5: 2,300 Coins - VALUE CURRENCY\n3. Buy 10: 4,000 Coins - VALUE CURRENCY\n4. Go Back"
        self.theValentine_price_list_message = "1. Buy 1: 599 Coins - VALUE CURRENCY\n2. Buy 5: 2,300 Coins - VALUE CURRENCY\n3. Buy 10: 4,000 Coins - VALUE CURRENCY\n4. Go Back"
        self.theDetector_price_list_message = "1. Buy 1: 399 Coins - VALUE CURRENCY\n2. Buy 5: 1,600 Coins - VALUE CURRENCY\n3. Buy 10: 2,600 Coins - VALUE CURRENCY\n4. Go Back"
        self.theNullifier_price_list_message = "1. Buy 1: 499 Coins - VALUE CURRENCY\n2. Buy 5: 2,000 Coins - VALUE CURRENCY\n3. Buy 10: 3,400 Coins - VALUE CURRENCY\n4. Go Back"
        self.cardDeckMini_price_list_message = "1. Buy 1: 399 Coins - VALUE CURRENCY\n2. Buy 5: 1,600 Coins - VALUE CURRENCY\n3. Buy 10: 2,600 Coins - VALUE CURRENCY\n4. Go Back"
        self.cardDeckPlatinum_price_list_message = "1. Buy 1: 499 Coins - VALUE CURRENCY\n2. Buy 5: 2,000 Coins - VALUE CURRENCY\n3. Buy 10: 3,400 Coins - VALUE CURRENCY\n4. Go Back"

        self.personalityPoints_price_list = "1. Buy 1: 1,000 Coins - VALUE CURRENCY\n2. Buy 5: 4,000 Coins - VALUE CURRENCY\n3. Buy 10: 7,000 Coins - VALUE CURRENCY\n4. Go Back"

        self.coins_price_list_message = "3 days: VALUE Coins - VALUE CURRENCY\n14 days: VALUE Coins - VALUE CURRENCY\n30 days VALUE Coins - VALUE CURRENCY"
        self.currency_list_message = "1. USD\n2. EUR\n3. UAH\n4. RUB\n5. CZK\n6. PLN"
        self.currency_purchase_message = "1. By Coins\n2. By Real USER_CURRENCY\n3. Go Back"

        self.userBalance = Helpers.get_active_user_balance(self.current_user)

        self.secondChanceDescription = "<i><b>Second chance allows you to 'like' another user once again. It can be used in the 'encounters' section</b></i>"
        self.valentineDescription = "<i><b>Doubles your Personality points for an hour</b></i>"
        self.detectorDescription = "<i><b>When matched, shows which PERSONALITY parameters were matched. Works for 1 hour</b></i>"
        self.nullifierDescription = "<i><b>Allows you to pass any test one more time, without waiting</b></i>"
        self.cardDeckMiniDescription = "<i><b>Instantly adds 20 profile views to your daily views</b></i>"
        self.cardDeckPlatinumDescription = "<i><b>Instantly adds 50 profile views to your daily views</b></i>"

        self.YNmarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("yes", "no")
        self.currency_choiceMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3", "4", "5", "6")
        self.currency_purchaseMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3")

        self.start_markup = InlineKeyboardMarkup().add(InlineKeyboardButton("Premium", callback_data="1"))\
            .add(InlineKeyboardButton("Coins", callback_data="2"), InlineKeyboardButton("Effects", callback_data="3"))\
            .add(InlineKeyboardButton("Personality Points", callback_data="4"))\
            .add(InlineKeyboardButton("Tests", callback_data="5"))\
            .add(InlineKeyboardButton("Support us :)", callback_data="6"))\
            .add(InlineKeyboardButton("Exit", callback_data="-1"))

        self.buy_premium_markup = InlineKeyboardMarkup().add(InlineKeyboardButton("3 days", callback_data="0"), InlineKeyboardButton("5,999 Coins", callback_data="9"), InlineKeyboardButton("VALUE CURRENCY", callback_data="10"))\
                                                        .add(InlineKeyboardButton("21 days", callback_data="0"), InlineKeyboardButton("12,999 Coins", callback_data="11"), InlineKeyboardButton("VALUE CURRENCY", callback_data="12"))\
                                                        .add(InlineKeyboardButton("30 days", callback_data="0"), InlineKeyboardButton("20,999 Coins", callback_data="13"), InlineKeyboardButton("VALUE CURRENCY", callback_data="14"))\
                                                        .add(InlineKeyboardButton("Go Back", callback_data="-1"))

        self.buyPP_markup = InlineKeyboardMarkup().add(InlineKeyboardButton("Buy 1", callback_data="0"), InlineKeyboardButton("1000 Coins", callback_data="30"), InlineKeyboardButton("VALUE CURRENCY", callback_data="31")) \
            .add(InlineKeyboardButton("Buy 5", callback_data="0"), InlineKeyboardButton("4000 Coins", callback_data="32"), InlineKeyboardButton("VALUE CURRENCY", callback_data="33"))\
            .add(InlineKeyboardButton("Buy 10", callback_data="0"), InlineKeyboardButton("7000 Coins", callback_data="34"), InlineKeyboardButton("VALUE CURRENCY", callback_data="35"))\
            .add(InlineKeyboardButton("Go Back", callback_data="-1"))

        self.effects_list_markup = InlineKeyboardMarkup().add(InlineKeyboardButton("💥Second Chance💥", callback_data="16"))\
            .add(InlineKeyboardButton("💥The Valentine💥", callback_data="17"))\
            .add(InlineKeyboardButton("💥The Detector💥", callback_data="18"))\
            .add(InlineKeyboardButton("💥The Nullifier💥", callback_data="19"))\
            .add(InlineKeyboardButton("💥Card Deck Mini💥", callback_data="20"))\
            .add(InlineKeyboardButton("💥Card Deck Platinum💥", callback_data="21"))\
            .add(InlineKeyboardButton("Go Back", callback_data="-1"))

        self.effect_pack_markup = InlineKeyboardMarkup()

        self.previous_section = self.destruct
        self.current_section = None

        self.ch = self.bot.register_callback_query_handler("", self.callback_handler, user_id=self.current_user)

        if not hasVisited:
            self.first_time_handler(message)
            return

        self.helpHandler = self.bot.register_message_handler(self.help_handler, commands=["help"], user_id=self.current_user)

        self.start(message)

    def first_time_handler(self, message, acceptMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, "Choose the currency you feel most comfortable using to make payments before we continue")
            self.bot.send_message(self.current_user, self.currency_list_message, reply_markup=self.currency_choiceMarkup)
            self.bot.register_next_step_handler(message, self.first_time_handler, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1" or message.text == "2" or message.text == "3" or message.text == "4" or message.text == "5" or message.text == "6":
                if Helpers.set_user_currency(self.current_user, message.text):
                    self.bot.send_message(self.current_user, "Done. You can change selected currency at any time in Settings :)")
                else:
                    self.bot.send_message(self.current_user, "Something went wrong. Please, contact the administration")

                self.start(message)
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.currency_choiceMarkup)
                self.bot.register_next_step_handler(message, self.first_time_handler, acceptMode=acceptMode, chat_id=self.current_user)

    def start(self, message):
        self.previous_section = self.destruct
        self.current_section = self.start

        if self.startingTransaction == 1:
            self.buy_coins(message)
            return
        elif self.startingTransaction == 2:
            self.choose_effect_to_buy(message)
            return
        elif self.startingTransaction == 3:
            self.buy_premium(message)
            return
        elif self.startingTransaction == 4:
            self.choose_pack_PP(message)
            return
        elif self.startingTransaction == 5:
            self.buy_tests()
            return

        if self.shouldGreet:
            self.send_active_message(f"Welcome to the shop!\n{self.get_balance_message()}", markup=self.start_markup)
            self.shouldGreet = False
        else:
            try:
                self.bot.edit_message_text(f"{self.get_balance_message()}", self.current_user, self.active_message, reply_markup=self.start_markup)
            except:
                pass

    def buy_coins(self, message, acceptMode=False):
        self.current_section = self.buy_coins

        if not self.activatedElsewhere:
            self.previous_section = self.start
        else:
            self.previous_section = self.destruct

        if not acceptMode:
            #TODO: Implement
            self.send_active_transaction_message("Not implemented yet :)")
            # self.bot.register_next_step_handler(message, self.buy_premium, acceptMode=True, chat_id=self.current_user)
            self.proceed(message)
        else:
            pass

    def buy_premium(self, message=None):
        self.current_section = self.buy_premium

        if not self.activatedElsewhere:
            self.previous_section = self.start
        else:
            self.previous_section = self.destruct

        self.send_active_message(f"<i><b>Please, select a pack and currency by clicking on a corresponding button</b></i>\n{self.get_balance_message()}", markup=self.buy_premium_markup)

    def choose_effect_to_buy(self, message=None):
        self.current_section = self.choose_effect_to_buy

        if not self.activatedElsewhere:
            self.previous_section = self.start
        else:
            self.previous_section = self.destruct

        self.clear_screen(True)

        self.send_active_message(f"<i><b>Please, select an effect</b></i>\n{self.get_balance_message()}", markup=self.effects_list_markup)

    def choose_effect_pack(self, message=None):
        self.current_section = self.choose_effect_pack

        if not self.activatedElsewhere:
            self.previous_section = self.choose_effect_to_buy
        else:
            self.previous_section = self.destruct

        effectId = self.current_effect_pack
        transaction = "0"
        if effectId == "1":
            self.active_first_option_price = 599
            self.active_second_option_price = 2300
            self.active_third_option_price = 4000
            transaction = "5"
        elif effectId == "2":
            self.active_first_option_price = 599
            self.active_second_option_price = 2300
            self.active_third_option_price = 4000
            transaction = "6"
        elif effectId == "3":
            self.active_first_option_price = 399
            self.active_second_option_price = 1600
            self.active_third_option_price = 2600
            transaction = "7"
        elif effectId == "4":
            self.active_first_option_price = 499
            self.active_second_option_price = 2000
            self.active_third_option_price = 3400
            transaction = "8"
        elif effectId == "5":
            self.active_first_option_price = 399
            self.active_second_option_price = 1600
            self.active_third_option_price = 2600
            transaction = "9"
        elif effectId == "6":
            self.active_first_option_price = 499
            self.active_second_option_price = 2000
            self.active_third_option_price = 3400
            transaction = "10"
        self.current_transaction = transaction

        self.construct_active_pack_markup()

        self.send_active_message(f"<i><b>Please, select pack of effects. Click on the according price to choose currency</b></i>\n{self.get_balance_message()}", markup=self.effect_pack_markup)

    def choose_pack_PP(self, message=None):

        self.active_first_option_price = 1000
        self.active_second_option_price = 4000
        self.active_third_option_price = 7000

        self.send_active_message(f"<i><b>Please, select Points pack. Click on the according price to choose currency</b></i>\n{self.get_balance_message()}", markup=self.buyPP_markup)

        self.current_section = self.choose_pack_PP

        if not self.activatedElsewhere:
            self.previous_section = self.start
        else:
            self.previous_section = self.destruct

    def buy_tests(self):
        # Remove previous callback handler so that handlers do not collide
        self.bot.callback_query_handlers.remove(self.ch)
        self.ch = None

        self.current_section = self.buy_tests

        if not self.activatedElsewhere:
            self.previous_section = self.start
        else:
            self.previous_section = self.destruct

        TestModule(self.bot, self.message, True, self.proceed, active_message=self.active_message)
        return

    def process_transaction(self, transaction_type, currency):
        result = False

        if (currency == "1" and self.userBalance["points"] >= self.chosen_pack_price) or currency == "2":
            if transaction_type == "1":
                if currency == "1":
                    result = Helpers.grant_premium_for_points(self.current_user, self.chosen_pack_price, 3)
                elif currency == "2":
                    pass
                    # TODO: Redirect user to real money purchase section
            elif transaction_type == "2":
                if currency == "1":
                    result = Helpers.grant_premium_for_points(self.current_user, self.chosen_pack_price, 21)
                elif currency == "2":
                    pass
                    # TODO: Redirect user to real money purchase section
            elif transaction_type == "3":
                if currency == "1":
                    result = Helpers.grant_premium_for_points(self.current_user, self.chosen_pack_price, 30)
                elif currency == "2":
                    pass
                    #TODO: Redirect user to real money purchase section
            elif transaction_type == "5":
                if currency == "1":
                    result = Helpers.purchase_effect_for_points(self.current_user, "5", self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    pass
                    #TODO: Redirect user to real money purchase section
            elif transaction_type == "6":
                if currency == "1":
                    result = Helpers.purchase_effect_for_points(self.current_user, "6", self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    pass
                    #TODO: Redirect user to real money purchase section
            elif transaction_type == "7":
                if currency == "1":
                    result = Helpers.purchase_effect_for_points(self.current_user, "7", self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    pass
                    #TODO: Redirect user to real money purchase section
            elif transaction_type == "8":
                if currency == "1":
                    result = Helpers.purchase_effect_for_points(self.current_user, "8", self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    pass
                    #TODO: Redirect user to real money purchase section
            elif transaction_type == "9":
                if currency == "1":
                    result = Helpers.purchase_effect_for_points(self.current_user, "9", self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    pass
                    #TODO: Redirect user to real money purchase section
            elif transaction_type == "10":
                if currency == "1":
                    result = Helpers.purchase_effect_for_points(self.current_user, "10", self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    pass
                    #TODO: Redirect user to real money purchase section
            elif transaction_type == "100":
                if currency == "1":
                    result = Helpers.purchase_PP_for_points(self.current_user, self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    pass
                    #TODO: Redirect user to real money purchase section

            if result:
                self.userBalance["points"] -= self.chosen_pack_price
                #self.display_user_balance()
                self.proceed(self.message)

                self.send_active_transaction_message("Transaction was successful")

                #Return to previous Module if it exists
                # if self.startingTransaction is not None:
                #     self.destruct()

            else:
                #TODO: tell what had gone wrong
                self.send_active_transaction_message("Something went wrong")
        else:
            self.send_active_transaction_message("You dont have enough coins to buy this item")

    def send_active_transaction_message(self, text):
        if self.active_transaction_status_message is not None:
            self.bot.delete_message(self.current_user, self.active_transaction_status_message)

        self.active_transaction_status_message = self.bot.send_message(self.current_user, text).id

    def callback_handler(self, call):
        self.bot.answer_callback_query(call.id, "")
        #Exit / Go Back
        if call.data == "-1":
            self.proceed(call.message, shouldClearChat=True)
        else:
            if not self.isDeciding:
                if call.data == "1":
                    self.buy_premium(call.message)
                elif call.data == "2":
                    self.buy_coins(call.message)
                elif call.data == "3":
                    self.choose_effect_to_buy(call.message)
                elif call.data == "4":
                    self.choose_pack_PP(call.message)
                elif call.data == "5":
                    self.buy_tests()
                elif call.data == "6":
                    pass
                elif call.data == "9":
                    self.chosen_pack_price = 5999
                    self.process_transaction("1", "1")
                elif call.data == "10":
                    #TODO: Implement Payment
                    return
                elif call.data == "11":
                    self.chosen_pack_price = 12999
                    self.process_transaction("2", "1")
                elif call.data == "12":
                    #TODO: Implement Payment
                    return
                elif call.data == "13":
                    self.chosen_pack_price = 20999
                    self.process_transaction("3", "1")
                elif call.data == "16":
                    self.current_effect_pack = "1"
                    self.current_transaction = "5"
                    self.active_description_message = self.bot.send_message(self.current_user, self.secondChanceDescription).id
                    self.choose_effect_pack(call.message)
                elif call.data == "17":
                    self.current_effect_pack = "2"
                    self.active_description_message = self.bot.send_message(self.current_user, self.valentineDescription).id
                    self.choose_effect_pack(call.message)
                elif call.data == "18":
                    self.current_effect_pack = "3"
                    self.current_transaction = "6"
                    self.active_description_message = self.bot.send_message(self.current_user, self.detectorDescription).id
                    self.choose_effect_pack(call.message)
                elif call.data == "19":
                    self.current_effect_pack = "4"
                    self.current_transaction = "7"
                    self.active_description_message = self.bot.send_message(self.current_user, self.nullifierDescription).id
                    self.choose_effect_pack(call.message)
                elif call.data == "20":
                    self.current_effect_pack = "5"
                    self.current_transaction = "8"
                    self.active_description_message = self.bot.send_message(self.current_user, self.cardDeckMiniDescription).id
                    self.choose_effect_pack(call.message)
                elif call.data == "21":
                    self.current_effect_pack = "6"
                    self.current_transaction = "9"
                    self.active_description_message = self.bot.send_message(self.current_user, self.cardDeckPlatinumDescription).id
                    self.choose_effect_pack(call.message)
                elif call.data == "23":
                    self.active_pack = 1
                    self.chosen_pack_price = self.active_first_option_price
                    self.process_transaction(self.current_transaction, "1")
                elif call.data == "24":
                    # TODO: Implement Payment
                    return
                elif call.data == "25":
                    self.active_pack = 5
                    self.chosen_pack_price = self.active_second_option_price
                    self.process_transaction(self.current_transaction, "1")
                elif call.data == "26":
                    # TODO: Implement Payment
                    return
                elif call.data == "27":
                    self.active_pack = 10
                    self.chosen_pack_price = self.active_third_option_price
                    self.process_transaction(self.current_transaction, "1")
                elif call.data == "28":
                    # TODO: Implement Payment
                    return
                elif call.data == "30":
                    self.active_pack = 1
                    self.chosen_pack_price = self.active_first_option_price
                    self.process_transaction("100", "1")
                elif call.data == "31":
                    # TODO: Implement Payment
                    return
                elif call.data == "32":
                    self.active_pack = 5
                    self.chosen_pack_price = self.active_second_option_price
                    self.process_transaction("100", "1")
                elif call.data == "33":
                    # TODO: Implement Payment
                    return
                elif call.data == "34":
                    self.active_pack = 10
                    self.chosen_pack_price = self.active_third_option_price
                    self.process_transaction("100", "1")
                elif call.data == "35":
                    # TODO: Implement Payment
                    return

    def construct_active_pack_markup(self):
        self.effect_pack_markup.clear()
        self.effect_pack_markup.add(InlineKeyboardButton(f"Buy 1:", callback_data="0"), InlineKeyboardButton(f"{self.active_first_option_price} coins", callback_data="23"), InlineKeyboardButton("VALUE CURRENCY", callback_data="24"))\
            .add(InlineKeyboardButton(f"Buy 5:", callback_data="0"), InlineKeyboardButton(f"{self.active_second_option_price} coins", callback_data="25"), InlineKeyboardButton("VALUE CURRENCY", callback_data="26"))\
            .add(InlineKeyboardButton(f"Buy 10:", callback_data="0"), InlineKeyboardButton(f"{self.active_third_option_price} coins", callback_data="27"), InlineKeyboardButton("VALUE CURRENCY", callback_data="28"))\
            .add(InlineKeyboardButton("Go Back", callback_data="-1"))

    def get_balance_message(self):
        return "<i><b>Your current points balance: {}</b></i>".format(self.userBalance['points'])

    def clear_screen(self, skipTransaction=False):
        # Clear screen of previous transaction message
        if not skipTransaction and self.active_transaction_status_message:
            try:
                self.bot.delete_message(self.current_user, self.active_transaction_status_message)
                self.active_transaction_status_message = None
            except:
                pass

        # Clear screen of previous effect description message
        if self.active_description_message:
            try:
                self.bot.delete_message(self.current_user, self.active_description_message)
                self.active_description_message = None
            except:
                pass

    def send_active_message(self, text, markup=None):
        if self.active_message:
            self.bot.edit_message_text(text, self.current_user, self.active_message, reply_markup=markup)
            return
        self.active_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id

    def display_user_balance(self):
        if self.active_message:
            self.bot.edit_message_text(self.get_balance_message(), self.current_user, self.active_message)
            return

    def proceed(self, message, **kwargs):
        #Re-subscribe callback handler upon returning from Tester
        if kwargs.get("shouldClearChat") and self.active_transaction_status_message is not None:
            try:
                self.bot.delete_message(self.current_user, self.active_transaction_status_message)
                self.active_transaction_status_message = None
            except:
                pass

        if self.previous_section:
            if kwargs.get("shouldSubscribe"):
                self.ch = self.bot.register_callback_query_handler("", self.callback_handler, user_id=self.current_user)

            self.previous_section(message)

    def help_handler(self, message):
        self.bot.delete_message(self.current_user, message.id)
        Helper(self.bot, message, self.current_section, activeMessageId=self.active_message, secondaryMessageId=self.active_description_message)

    def destruct(self, message=None):
        self.clear_screen()

        if not self.activatedElsewhere:
            self.bot.delete_message(self.current_user, self.active_message)
            self.bot.callback_query_handlers.remove(self.ch)
            menues.go_back_to_main_menu(self.bot, self.current_user, self.message)
            return

        self.returnMethod(self.message, backFromShop=True)
