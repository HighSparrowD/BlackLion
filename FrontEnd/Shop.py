from telebot.types import *
import Core.HelpersMethodes as Helpers
import Common.Menues as menues
from TestModule import TestModule


class Shop:
    def __init__(self, bot, message, hasVisited=False, startingTransaction=None, returnMethod=None):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id
        self.hasVisited = hasVisited
        self.returnMethod = returnMethod
        self.startingTransaction = startingTransaction
        Helpers.switch_user_busy_status(self.current_user)

        self.active_first_option_price = 0
        self.active_second_option_price = 0
        self.active_third_option_price = 0
        self.active_pack = 0
        self.chosen_pack_price = 0

        self.current_effect_pack = ""

        #TODO: load price lists based on currency selected by user
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

        self.secondChanceDescription = "Second chance allows you to 'like' another user once again. It can be used in the 'encounters' section"
        self.valentineDescription = "Doubles your Personality points for an hour"
        self.detectorDescription = "When matched, shows which PERSONALITY parameters were matched. Works for 1 hour"
        self.nullifierDescription = "Allows you to pass any test one more time, without waiting"
        self.cardDeckMiniDescription = "Instantly adds 20 profile views to your daily views"
        self.cardDeckPlatinumDescription = "Instantly adds 50 profile views to your daily views"

        self.YNmarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("yes", "no")
        self.currency_choiceMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3", "4", "5", "6")
        self.currency_purchaseMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3")
        self.start_markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3", "4", "5", "6", "7") #TODO: Expand in the future
        self.buy_premium_markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3", "4")
        self.effects_list_markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3", "4", "5", "6", "7")
        self.effect_pack_markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3", "4")

        self.previous_section = self.start

        if not hasVisited:
            self.first_time_handler(message)
            return

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

                if not self.startingTransaction:
                    self.start(message)

                else:
                    self.choose_pay_method(message, self.startingTransaction)
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.currency_choiceMarkup)
                self.bot.register_next_step_handler(message, self.first_time_handler, acceptMode=acceptMode, chat_id=self.current_user)

    def start(self, message, acceptMode=False):
        if not acceptMode:
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

            self.bot.send_message(self.current_user, "Welcome to the shop!", reply_markup=self.start_markup)
            self.bot.send_message(self.current_user, f"Your current points balance: {self.userBalance['points']}", reply_markup=self.start_markup)
            self.bot.send_message(self.current_user, self.start_options_message, reply_markup=self.start_markup)
            self.bot.register_next_step_handler(message, self.start, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                self.buy_premium(message)
            elif message.text == "2":
                self.buy_coins(message)
            elif message.text == "3":
                self.choose_effect_to_buy(message)
            elif message.text == "4":
                self.choose_pack_PP(message)
            elif message.text == "5":
                self.buy_tests()
            elif message.text == "6":
                #TODO: Implement
                self.proceed(message)
            elif message.text == "7":
                self.destruct()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.start_markup )
                self.proceed(message)

    def buy_coins(self, message, acceptMode=False):
        self.previous_section = self.start
        if not acceptMode:
            #TODO: Implement
            self.bot.send_message(self.current_user, "Not implemented yet :)")
            # self.bot.register_next_step_handler(message, self.buy_premium, acceptMode=True, chat_id=self.current_user)
            self.proceed(message)
        else:
            pass

    def buy_premium(self, message, acceptMode=False):
        self.previous_section = self.start
        if not acceptMode:
            self.bot.send_message(self.current_user, self.premium_price_list_message, reply_markup=self.buy_premium_markup)
            self.bot.register_next_step_handler(message, self.buy_premium, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                self.choose_pay_method(message, "1")
            elif message.text == "2":
                self.choose_pay_method(message, "2")
            elif message.text == "3":
                self.choose_pay_method(message, "3")
            elif message.text == "4":
                self.proceed(message)
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.buy_premium_markup)
                self.bot.register_next_step_handler(message, self.buy_premium, acceptMode=acceptMode, chat_id=self.current_user)
                return

    def choose_effect_to_buy(self, message, acceptMode=False):
        self.previous_section = self.start
        if not acceptMode:
            self.bot.send_message(self.current_user, self.effects_list_message, reply_markup=self.effects_list_markup)
            self.bot.register_next_step_handler(message, self.choose_effect_to_buy, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                self.current_effect_pack = message.text
                self.bot.send_message(self.current_user, self.secondChanceDescription)
                self.choose_effect_pack(message)
            elif message.text == "2":
                self.current_effect_pack = message.text
                self.bot.send_message(self.current_user, self.valentineDescription)
                self.choose_effect_pack(message)
            elif message.text == "3":
                self.current_effect_pack = message.text
                self.bot.send_message(self.current_user, self.detectorDescription)
                self.choose_effect_pack(message)
            elif message.text == "4":
                self.current_effect_pack = message.text
                self.bot.send_message(self.current_user, self.nullifierDescription)
                self.choose_effect_pack(message)
            elif message.text == "5":
                self.current_effect_pack = message.text
                self.bot.send_message(self.current_user, self.cardDeckMiniDescription)
                self.choose_effect_pack(message)
            elif message.text == "6":
                self.current_effect_pack = message.text
                self.bot.send_message(self.current_user, self.cardDeckPlatinumDescription)
                self.choose_effect_pack(message)
            elif message.text == "7":
                self.proceed(message)
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.effects_list_markup)
                self.bot.register_next_step_handler(message, self.choose_effect_to_buy, acceptMode=acceptMode, chat_id=self.current_user)

    def choose_effect_pack(self, message, acceptMode=False):
        self.previous_section = self.choose_effect_to_buy
        effectId = self.current_effect_pack
        if not acceptMode:
            if effectId == "1":
                self.active_first_option_price = 599
                self.active_first_option_price = 2300
                self.active_first_option_price = 4000
                self.bot.send_message(self.current_user, self.secondChance_price_list_message, reply_markup=self.effect_pack_markup)
            elif effectId == "2":
                self.active_first_option_price = 599
                self.active_first_option_price = 2300
                self.active_first_option_price = 4000
                self.bot.send_message(self.current_user, self.theValentine_price_list_message, reply_markup=self.effect_pack_markup)
            elif effectId == "3":
                self.active_first_option_price = 399
                self.active_first_option_price = 1600
                self.active_first_option_price = 2600
                self.bot.send_message(self.current_user, self.theDetector_price_list_message, reply_markup=self.effect_pack_markup)
            elif effectId == "4":
                self.active_first_option_price = 499
                self.active_first_option_price = 2000
                self.active_first_option_price = 3400
                self.bot.send_message(self.current_user, self.theNullifier_price_list_message, reply_markup=self.effect_pack_markup)
            elif effectId == "5":
                self.active_first_option_price = 399
                self.active_first_option_price = 1600
                self.active_first_option_price = 2600
                self.bot.send_message(self.current_user, self.cardDeckMini_price_list_message, reply_markup=self.effect_pack_markup)
            elif effectId == "6":
                self.active_first_option_price = 499
                self.active_first_option_price = 2000
                self.active_first_option_price = 3400
                self.bot.send_message(self.current_user, self.cardDeckPlatinum_price_list_message, reply_markup=self.effect_pack_markup)
            self.bot.register_next_step_handler(message, self.choose_effect_pack, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                self.active_pack = 1
                self.chosen_pack_price = self.active_first_option_price
            elif message.text == "2":
                self.active_pack = 5
                self.chosen_pack_price = self.active_second_option_price
            elif message.text == "3":
                self.active_pack = 10
                self.chosen_pack_price = self.active_third_option_price
            elif message.text == "4":
                self.proceed(message)
                return
            self.choose_pay_method(message, effectId)

    def choose_pack_PP(self, message, acceptMode=False):
        self.previous_section = self.start
        if not acceptMode:
            self.active_first_option_price = 1000
            self.active_second_option_price = 4000
            self.active_third_option_price = 7000

            self.bot.send_message(self.current_user, self.personalityPoints_price_list, reply_markup=self.effect_pack_markup)
            self.bot.register_next_step_handler(message, self.choose_pack_PP, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                self.active_pack = 1
                self.chosen_pack_price = self.active_first_option_price
                self.choose_pay_method(message, "100")
            elif message.text == "2":
                self.active_pack = 5
                self.chosen_pack_price = self.active_second_option_price
                self.choose_pay_method(message, "100")
            elif message.text == "3":
                self.active_pack = 10
                self.chosen_pack_price = self.active_third_option_price
                self.choose_pay_method(message, "100")
            elif message.text == "4":
                self.proceed(message)

    def buy_tests(self):
        TestModule(self.bot, self.message, True, self.proceed)
        return

    def choose_pay_method(self, message, transaction_type, acceptMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, "How would you like to make a purchase?")
            self.bot.send_message(self.current_user, self.currency_purchase_message, reply_markup=self.currency_purchaseMarkup)
            self.bot.register_next_step_handler(message, self.choose_pay_method, transaction_type=transaction_type, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                #Transaction type represents effect id in case of buying any effect
                self.process_transaction(transaction_type, "1")
            elif message.text == "2":
                self.process_transaction(transaction_type, "2")
            elif message.text == "3":
                self.proceed(message)
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.currency_purchaseMarkup)
                self.bot.register_next_step_handler(message, self.choose_pay_method, transaction_type=transaction_type, acceptMode=acceptMode, chat_id=self.current_user)

    def process_transaction(self, transaction_type, currency):
        self.previous_section = self.choose_pack_PP
        result = False
        if transaction_type == "1":
            if currency == "1":
                result = Helpers.grant_premium_for_points(self.current_user, 5999, 3)
            elif currency == "2":
                pass
                # TODO: Redirect user to real money purchase section
        elif transaction_type == "2":
            if currency == "1":
                result = Helpers.grant_premium_for_points(self.current_user, 12999, 21)
            elif currency == "2":
                pass
                # TODO: Redirect user to real money purchase section
        elif transaction_type == "3":
            if currency == "1":
                result = Helpers.grant_premium_for_points(self.current_user, 20999, 30)
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
            self.bot.send_message(self.current_user, "Transaction was successful")

            if self.startingTransaction is not None:
                self.destruct()
            else:
                self.proceed(self.message)
        else:
            #TODO: tell what had gone wrong
            self.bot.send_message(self.current_user, "Something went wrong")
        self.proceed(self.message)

    def proceed(self, message):
        if self.previous_section:
            self.previous_section(message)

    def destruct(self):
        Helpers.switch_user_busy_status(self.current_user)

        if not self.returnMethod:
            menues.go_back_to_main_menu(self.bot, self.current_user, self.message)
            return

        self.returnMethod(self.message)
