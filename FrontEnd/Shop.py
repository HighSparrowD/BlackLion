from telebot.types import *
import Core.HelpersMethodes as Helpers
import Common.Menues as menues


class Shop:
    def __init__(self, bot, message, hasVisited=False, startingTransaction=None, returnMethod=None):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id
        self.hasVisited = hasVisited
        self.returnMethod = returnMethod
        self.startingTransaction = startingTransaction
        Helpers.switch_user_busy_status(self.current_user)

        #TODO: load price lists based on currency selected by user
        self.start_options_message = "1. Premium\n2. Coins\n3. Effects\n4. Support us ;)\n5. Exit"
        self.premium_price_list_message = "3 days: VALUE Coins - VALUE CURRENCY\n14 days: VALUE Coins - VALUE CURRENCY\n30 days VALUE Coins - VALUE CURRENCY"
        self.effects_price_list_message = "3 days: VALUE Coins - VALUE CURRENCY\n14 days: VALUE Coins - VALUE CURRENCY\n30 days VALUE Coins - VALUE CURRENCY"
        self.coins_price_list_message = "3 days: VALUE Coins - VALUE CURRENCY\n14 days: VALUE Coins - VALUE CURRENCY\n30 days VALUE Coins - VALUE CURRENCY"
        self.currency_list_message = "1. USD\n2. EUR\n3. UAH\n4. RUB\n5. CZK\n6. PLN"
        self.currency_purchase_message = "1. By Coins\n2. By Real USER_CURRENCY"

        self.userBalance = Helpers.get_active_user_balance(self.current_user)

        self.YNmarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("yes", "no")
        self.currency_choiceMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3", "4", "5", "6")
        self.currency_purchaseMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2")
        self.start_markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3", "4") #TODO: Expand in the future
        self.buy_premium_markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3")

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
            self.bot.send_message(self.current_user, "Welcome to the shop!", reply_markup=self.start_markup)
            self.bot.send_message(self.current_user, f"Your current points balance: {self.userBalance['points']}", reply_markup=self.start_markup)
            self.bot.send_message(self.current_user, self.start_options_message, reply_markup=self.start_markup)
            self.bot.register_next_step_handler(message, self.start, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                self.buy_premium(message)
            elif message.text == "2":
                self.proceed(message)
            elif message.text == "3":
                self.proceed(message)
            elif message.text == "4":
                self.proceed(message)
            elif message.text == "5":
                self.destruct()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.start_markup )
                self.proceed(message)

    def buy_premium(self, message, acceptMode=False):
        self.previous_section = self.start
        if not acceptMode:
            self.bot.send_message(self.current_user, self.premium_price_list_message, reply_markup=self.buy_premium_markup)
            self.bot.register_next_step_handler(message, self.buy_premium, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text.lower() == "1":
                self.choose_pay_method(message, "1")
            elif message.text.lower() == "2":
                self.choose_pay_method(message, "2")
            elif message.text.lower() == "3":
                self.choose_pay_method(message, "3")
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.buy_premium_markup)
                self.bot.register_next_step_handler(message, self.buy_premium, acceptMode=acceptMode, chat_id=self.current_user)
                return


    def choose_pay_method(self, message, transaction_type, acceptMode=False):
        self.previous_section = self.buy_premium
        if not acceptMode:
            self.bot.send_message(self.current_user, "How would you like to make a purchase?")
            self.bot.send_message(self.current_user, self.currency_purchase_message, reply_markup=self.currency_purchaseMarkup)
            self.bot.register_next_step_handler(message, self.choose_pay_method, transaction_type=transaction_type, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                self.process_transaction(transaction_type, "1")
            elif message.text == "2":
                self.process_transaction(transaction_type, "2")
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.currency_purchaseMarkup)
                self.bot.register_next_step_handler(message, self.choose_pay_method, transaction_type=transaction_type, acceptMode=acceptMode, chat_id=self.current_user)

    def process_transaction(self, transaction_type, currency):
        result = False
        if transaction_type == "1":
            if currency == "1":
                result = Helpers.grant_premium_for_points(self.current_user, 6999, 3)
            elif currency == "2":
                pass
                # TODO: Redirect user to real money purchase section
        elif transaction_type == "2":
            if currency == "1":
                result = Helpers.grant_premium_for_points(self.current_user, 10999, 14)
            elif currency == "2":
                pass
                # TODO: Redirect user to real money purchase section
        elif transaction_type == "3":
            if currency == "1":
                result = Helpers.grant_premium_for_points(self.current_user, 18999, 30)
            elif currency == "2":
                pass
                #TODO: Redirect user to real money purchase section
        elif transaction_type == "4":
            if currency == "1":
                #TODO: Create effect list to purchase
                pass
                result = Helpers.purchase_effect_for_points(self.current_user, 5, )

        if result:
            self.bot.send_message(self.current_user, "Transaction was successful")
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
