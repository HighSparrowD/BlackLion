import stripe
from telebot import TeleBot
from telebot.types import *
import Core.HelpersMethodes as Helpers
import Common.Menues as menues
import Settings
from Core.Resources import Resources
from Helper import Helper
from TestModule import TestModule


class Shop:
    def __init__(self, bot: TeleBot, message: any, hasVisited: bool = False,
                 startingTransaction: int = None, returnMethod: classmethod = None, active_message: any = None):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id
        self.hasVisited = hasVisited
        self.returnMethod = returnMethod
        self.startingTransaction = startingTransaction
        self.shouldGreet = True
        self.shouldStay = False
        self.activatedElsewhere = True

        if returnMethod is None:
            self.activatedElsewhere = False

        self.active_first_option_price = 0
        self.active_second_option_price = 0
        self.active_third_option_price = 0

        self.active_currency_first_option_price = 0
        self.active_currency_second_option_price = 0
        self.active_currency_third_option_price = 0

        self.active_pack = 0
        self.chosen_pack_price = None

        self.current_effect_pack = ""
        self.active_message = active_message
        self.active_description_message = None
        self.active_transaction_status_message = None
        self.current_transaction = None

        self.suggested_tips = [5_00, 50_00, 75_00, 100_00]

        self.userBalance = None
        self.user_currency = None

        self.currency_prices = None
        # Load price list in points
        self.points_prices = Resources.get_prices("Points")

        self.secondChanceDescription = "<i><b>Second chance allows you to 'like' another user once again. It can be used in the 'encounters' section</b></i>"
        self.valentineDescription = "<i><b>Doubles your Personality points for an hour</b></i>"
        self.detectorDescription = "<i><b>When matched, shows which PERSONALITY parameters were matched. Works for 1 hour</b></i>"
        self.nullifierDescription = "<i><b>Allows you to pass any test one more time, without waiting</b></i>"
        self.cardDeckMiniDescription = "<i><b>Instantly adds 20 profile views to your daily views</b></i>"
        self.cardDeckPlatinumDescription = "<i><b>Instantly adds 50 profile views to your daily views</b></i>"

        self.YNmarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("yes", "no")
        self.currency_purchaseMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3")

        self.start_markup = InlineKeyboardMarkup().add(InlineKeyboardButton("Premium", callback_data="1"))\
            .add(InlineKeyboardButton("Coins", callback_data="2"), InlineKeyboardButton("Effects", callback_data="3"))\
            .add(InlineKeyboardButton("Personality Points", callback_data="4"))\
            .add(InlineKeyboardButton("Tests", callback_data="5"))\
            .add(InlineKeyboardButton("Support us :)", callback_data="6"))\
            .add(InlineKeyboardButton("Exit", callback_data="-1"))

        self.premiumBtn1 = InlineKeyboardButton("0", callback_data="10")
        self.premiumBtn2 = InlineKeyboardButton("0", callback_data="12")
        self.premiumBtn3 = InlineKeyboardButton("0", callback_data="14")

        self.OCbutton1 = InlineKeyboardButton("0", callback_data="31")
        self.OCbutton2 = InlineKeyboardButton("0", callback_data="33")
        self.OCbutton3 = InlineKeyboardButton("0", callback_data="35")

        self.pointsBtn1 = InlineKeyboardButton("0", callback_data="36")
        self.pointsBtn2 = InlineKeyboardButton("0", callback_data="37")
        self.pointsBtn3 = InlineKeyboardButton("0", callback_data="38")

        self.buy_premium_markup = InlineKeyboardMarkup().add(InlineKeyboardButton("3 days", callback_data="0"), InlineKeyboardButton(self.points_prices["Premium7"], callback_data="9"), self.premiumBtn1)\
                                                        .add(InlineKeyboardButton("21 days", callback_data="0"), InlineKeyboardButton(self.points_prices["Premium21"], callback_data="11"), self.premiumBtn2)\
                                                        .add(InlineKeyboardButton("30 days", callback_data="0"), InlineKeyboardButton(self.points_prices["Premium30"], callback_data="13"), self.premiumBtn3)\
                                                        .add(InlineKeyboardButton("Go Back", callback_data="-1"))

        self.buyPP_markup = InlineKeyboardMarkup().add(InlineKeyboardButton("Buy 3", callback_data="0"), InlineKeyboardButton(self.points_prices["OCP3"], callback_data="30"), self.OCbutton1) \
            .add(InlineKeyboardButton("Buy 7", callback_data="0"), InlineKeyboardButton(self.points_prices["OCP7"], callback_data="32"), self.OCbutton2)\
            .add(InlineKeyboardButton("Buy 10", callback_data="0"), InlineKeyboardButton(self.points_prices["OCP10"], callback_data="34"), self.OCbutton3)\
            .add(InlineKeyboardButton("Go Back", callback_data="-1"))

        self.buy_points_markup = InlineKeyboardMarkup().add(InlineKeyboardButton("Buy 150", callback_data="0"), self.pointsBtn1) \
            .add(InlineKeyboardButton("Buy 500", callback_data="0"), self.pointsBtn2) \
            .add(InlineKeyboardButton("Buy 2000", callback_data="0"), self.pointsBtn3) \
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

        self.ch = None
        self.mh = None
        self.pre_checkout_h = None
        self.hHandler = None

        self.first_time_handler(message)

    def first_time_handler(self, message):
        self.get_user_balance()

        if self.user_currency is None:
            Settings.CurrencySetter(self.bot, self.current_user, self.first_time_handler)
        else:
            self.proceed_to_start(message)

    def proceed_to_start(self, message):
        self.ch = self.bot.register_callback_query_handler("", self.callback_handler, user_id=self.current_user)
        self.mh = self.bot.register_message_handler(self.payment_handler, content_types=['successful_payment'], user_id=self.current_user)
        self.pre_checkout_h = self.bot.register_pre_checkout_query_handler(self.pre_checkout_handler, func=lambda query: True)

        self.hHandler = self.bot.register_message_handler(self.help_handler, commands=["help"], user_id=self.current_user)

        self.start(message)

    def start(self, message):
        self.previous_section = self.destruct
        self.current_section = self.start

        # Load price lists based on currency selected by user
        self.set_currency_prices()

        if self.startingTransaction == 1:
            self.choose_pack_points(message)
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

    def buy_premium(self, message=None):
        self.current_section = self.buy_premium

        self.premiumBtn1.text = f"{self.currency_prices['Premium7']} {self.user_currency}"
        self.premiumBtn2.text = f"{self.currency_prices['Premium21']} {self.user_currency}"
        self.premiumBtn3.text = f"{self.currency_prices['Premium30']} {self.user_currency}"

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
            self.active_first_option_price = self.points_prices["SecondChance3"]
            self.active_second_option_price = self.points_prices["SecondChance7"]
            self.active_third_option_price = self.points_prices["SecondChance10"]

            self.active_currency_first_option_price = self.currency_prices["SecondChance3"]
            self.active_currency_second_option_price = self.currency_prices["SecondChance7"]
            self.active_currency_third_option_price = self.currency_prices["SecondChance10"]

            transaction = "5"
        elif effectId == "2":
            self.active_first_option_price = self.points_prices["Valentine3"]
            self.active_second_option_price = self.points_prices["Valentine7"]
            self.active_third_option_price = self.points_prices["Valentine10"]

            self.active_currency_first_option_price = self.currency_prices["Valentine3"]
            self.active_currency_second_option_price = self.currency_prices["Valentine7"]
            self.active_currency_third_option_price = self.currency_prices["Valentine10"]

            transaction = "6"
        elif effectId == "3":
            self.active_first_option_price = self.points_prices["Detector3"]
            self.active_second_option_price = self.points_prices["Detector7"]
            self.active_third_option_price = self.points_prices["Detector10"]

            self.active_currency_first_option_price = self.currency_prices["Detector3"]
            self.active_currency_second_option_price = self.currency_prices["Detector7"]
            self.active_currency_third_option_price = self.currency_prices["Detector10"]

            transaction = "7"
        elif effectId == "4":
            self.active_first_option_price = self.points_prices["Nullifier3"]
            self.active_second_option_price = self.points_prices["Nullifier7"]
            self.active_third_option_price = self.points_prices["Nullifier10"]

            self.active_currency_first_option_price = self.currency_prices["Nullifier3"]
            self.active_currency_second_option_price = self.currency_prices["Nullifier7"]
            self.active_currency_third_option_price = self.currency_prices["Nullifier10"]

            transaction = "8"
        elif effectId == "5":
            self.active_first_option_price = self.points_prices["DecMini3"]
            self.active_second_option_price = self.points_prices["DecMini7"]
            self.active_third_option_price = self.points_prices["DecMini10"]

            self.active_currency_first_option_price = self.currency_prices["DecMini3"]
            self.active_currency_second_option_price = self.currency_prices["DecMini7"]
            self.active_currency_third_option_price = self.currency_prices["DecMini10"]

            transaction = "9"
        elif effectId == "6":
            self.active_first_option_price = self.points_prices["DecPlatinum3"]
            self.active_second_option_price = self.points_prices["DecPlatinum7"]
            self.active_third_option_price = self.points_prices["DecPlatinum10"]

            self.active_currency_first_option_price = self.currency_prices["DecPlatinum3"]
            self.active_currency_second_option_price = self.currency_prices["DecPlatinum7"]
            self.active_currency_third_option_price = self.currency_prices["DecPlatinum10"]

            transaction = "10"
        self.current_transaction = transaction

        self.construct_active_pack_markup()

        self.send_active_message(f"<i><b>Please, select pack of effects. Click on the according price to choose currency</b></i>\n{self.get_balance_message()}", markup=self.effect_pack_markup)

    def choose_pack_PP(self, message=None):

        self.active_first_option_price = self.points_prices["OCP3"]
        self.active_second_option_price = self.points_prices["OCP7"]
        self.active_third_option_price = self.points_prices["OCP10"]

        self.OCbutton1.text = f"{self.currency_prices['OCP3']} {self.user_currency}"
        self.OCbutton2.text = f"{self.currency_prices['OCP7']} {self.user_currency}"
        self.OCbutton3.text = f"{self.currency_prices['OCP10']} {self.user_currency}"

        self.send_active_message(f"<i><b>Please, select Points pack. Click on the according price to choose currency</b></i>\n{self.get_balance_message()}", markup=self.buyPP_markup)

        self.current_section = self.choose_pack_PP

        if not self.activatedElsewhere:
            self.previous_section = self.start
        else:
            self.previous_section = self.destruct

    def choose_pack_points(self, message=None):
        self.active_first_option_price = self.points_prices["OCP3"]
        self.active_second_option_price = self.points_prices["OCP7"]
        self.active_third_option_price = self.points_prices["OCP10"]

        self.pointsBtn1.text = f"{self.currency_prices['Points150']} {self.user_currency}"
        self.pointsBtn2.text = f"{self.currency_prices['Points500']} {self.user_currency}"
        self.pointsBtn3.text = f"{self.currency_prices['Points2000']} {self.user_currency}"

        self.send_active_message(f"<i><b>Please, select Points pack. Click on the according price to choose currency</b></i>\n{self.get_balance_message()}", markup=self.buy_points_markup)

        self.current_section = self.choose_pack_points

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

    def process_transaction(self, transaction_type, currency, is_final=False):
        # is_final - True when real money transaction had been confirmed. Used to let the API log it to the DB

        result = False

        if (currency == "1" and self.userBalance["points"] >= self.chosen_pack_price) or currency == "2" or currency is None:
            if transaction_type == "1":
                if currency == "1":
                    result = Helpers.grant_premium_for_points(self.current_user, self.chosen_pack_price, 3)
                elif currency == "2":
                    self.send_price_invoice("Premium", "3 days of Premium class benefits", self.chosen_pack_price, "Premium")
                elif is_final:
                    result = Helpers.grant_premium_for_real_money(self.current_user, self.chosen_pack_price, 3, self.user_currency)
            elif transaction_type == "2":
                if currency == "1":
                    result = Helpers.grant_premium_for_points(self.current_user, self.chosen_pack_price, 21)
                elif currency == "2":
                    self.send_price_invoice("Premium", "21 days of Premium class benefits", self.chosen_pack_price, "Premium")
                elif is_final:
                    result = Helpers.grant_premium_for_real_money(self.current_user, self.chosen_pack_price, 21, self.user_currency)
            elif transaction_type == "3":
                if currency == "1":
                    result = Helpers.grant_premium_for_points(self.current_user, self.chosen_pack_price, 30)
                elif currency == "2":
                    self.send_price_invoice("Premium", "30 days of Premium class benefits", self.chosen_pack_price, "Premium")
                elif is_final:
                    result = Helpers.grant_premium_for_real_money(self.current_user, self.chosen_pack_price, 30, self.user_currency)
            elif transaction_type == "5":
                if currency == "1":
                    result = Helpers.purchase_effect_for_points(self.current_user, "5", self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    self.send_price_invoice("Second Chance", f"{self.active_pack} Second Chance\n\n{self.secondChanceDescription}", self.chosen_pack_price, "Effect")
                elif is_final:
                    result = Helpers.purchase_effect_for_real_money(self.current_user, "5", self.chosen_pack_price, self.user_currency, self.active_pack)
            elif transaction_type == "6":
                if currency == "1":
                    result = Helpers.purchase_effect_for_points(self.current_user, "6", self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    self.send_price_invoice("The Valentine", f"{self.active_pack} The Valentine\n\n{self.valentineDescription}", self.chosen_pack_price, "Effect")
                elif is_final:
                    result = Helpers.purchase_effect_for_real_money(self.current_user, "6", self.chosen_pack_price, self.user_currency, self.active_pack)
            elif transaction_type == "7":
                if currency == "1":
                    result = Helpers.purchase_effect_for_points(self.current_user, "7", self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    self.send_price_invoice("The Detector", f"{self.active_pack} The Detector\n\n{self.detectorDescription}", self.chosen_pack_price, "Effect")
                elif is_final:
                    result = Helpers.purchase_effect_for_real_money(self.current_user, "7", self.chosen_pack_price, self.user_currency, self.active_pack)
            elif transaction_type == "8":
                if currency == "1":
                    result = Helpers.purchase_effect_for_points(self.current_user, "8", self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    self.send_price_invoice("Nullifier", f"{self.active_pack} Nullifier\n\n{self.nullifierDescription}", self.chosen_pack_price, "Effect")
                elif is_final:
                    result = Helpers.purchase_effect_for_real_money(self.current_user, "8", self.chosen_pack_price, self.user_currency, self.active_pack)
            elif transaction_type == "9":
                if currency == "1":
                    result = Helpers.purchase_effect_for_points(self.current_user, "9", self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    self.send_price_invoice("Card Dec Mini", f"{self.active_pack} Card Dec Mini\n\n{self.cardDeckMiniDescription}", self.chosen_pack_price, "Effect")
                elif is_final:
                    result = Helpers.purchase_effect_for_real_money(self.current_user, "9", self.chosen_pack_price, self.user_currency, self.active_pack)
            elif transaction_type == "10":
                if currency == "1":
                    result = Helpers.purchase_effect_for_points(self.current_user, "10", self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    self.send_price_invoice("Card Dec Premium", f"{self.active_pack} Card Dec Platinum\n\n{self.cardDeckPlatinumDescription}", self.chosen_pack_price, "Effect")
                elif is_final:
                    result = Helpers.purchase_effect_for_real_money(self.current_user, "10", self.chosen_pack_price, self.user_currency, self.active_pack)
            elif transaction_type == "100":
                if currency == "1":
                    result = Helpers.purchase_PP_for_points(self.current_user, self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    self.send_price_invoice("OCEAN+ points", f"{self.active_pack} OCEAN+ points", self.chosen_pack_price, "OceanPoints")
                elif is_final:
                    result = Helpers.purchase_PP_for_real_money(self.current_user, self.chosen_pack_price, self.user_currency, self.active_pack)
            elif transaction_type == "101":
                if currency == "2":
                    self.send_price_invoice("Points", f"{self.active_pack} Points", self.chosen_pack_price, "Points")
                elif is_final:
                    result = Helpers.purchase_points_for_real_money(self.current_user, self.chosen_pack_price, self.user_currency, self.active_pack)

            if result:
                if not is_final:
                    self.userBalance["points"] -= self.chosen_pack_price
                #self.display_user_balance()
                self.proceed(self.message)

                # self.send_active_transaction_message("Transaction was successful")

                #Return to previous Module if it exists
                # if self.startingTransaction is not None:
                #     self.destruct()

            else:
                #TODO: tell what had gone wrong
                if currency != "2": #TODO: Remove when payment system is fully integrated
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
            if call.data == "1":
                self.buy_premium(call.message)
            elif call.data == "2":
                self.choose_pack_points(call.message)
            elif call.data == "3":
                self.choose_effect_to_buy(call.message)
            elif call.data == "4":
                self.choose_pack_PP(call.message)
            elif call.data == "5":
                self.buy_tests()
            elif call.data == "6":
                pass
            elif call.data == "9":
                self.current_transaction = "1"
                self.chosen_pack_price = self.points_prices["Premium7"]
                self.process_transaction(self.current_transaction, "1")
            elif call.data == "10":
                self.current_transaction = "1"
                self.chosen_pack_price = self.currency_prices["Premium7"]
                self.process_transaction(self.current_transaction, "2")
            elif call.data == "11":
                self.current_transaction = "2"
                self.chosen_pack_price = self.points_prices["Premium21"]
                self.process_transaction(self.current_transaction, "1")
            elif call.data == "12":
                self.current_transaction = "2"
                self.chosen_pack_price = self.currency_prices["Premium21"]
                self.process_transaction(self.current_transaction, "2")
            elif call.data == "13":
                self.current_transaction = "3"
                self.chosen_pack_price = self.points_prices["Premium30"]
                self.process_transaction(self.current_transaction, "1")
            elif call.data == "14":
                self.current_transaction = "3"
                self.chosen_pack_price = self.currency_prices["Premium30"]
                self.process_transaction(self.current_transaction, "2")
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
                self.active_pack = 3
                self.chosen_pack_price = self.active_first_option_price
                self.process_transaction(self.current_transaction, "1")
            elif call.data == "24":
                self.active_pack = 3
                self.chosen_pack_price = self.active_currency_first_option_price
                self.process_transaction(self.current_transaction, "2")
            elif call.data == "25":
                self.active_pack = 7
                self.chosen_pack_price = self.active_second_option_price
                self.process_transaction(self.current_transaction, "1")
            elif call.data == "26":
                self.active_pack = 7
                self.chosen_pack_price = self.active_currency_second_option_price
                self.process_transaction(self.current_transaction, "2")
                return
            elif call.data == "27":
                self.active_pack = 10
                self.chosen_pack_price = self.active_third_option_price
                self.process_transaction(self.current_transaction, "1")
            elif call.data == "28":
                self.active_pack = 10
                self.chosen_pack_price = self.active_currency_third_option_price
                self.process_transaction(self.current_transaction, "2")
            # Buy OCEAN+ points
            elif call.data == "30":
                self.active_pack = 3
                self.chosen_pack_price = self.active_first_option_price
                self.current_transaction = "100"
                self.process_transaction(self.current_transaction, "1")
            elif call.data == "31":
                self.active_pack = 3
                self.chosen_pack_price = self.currency_prices["OCP3"]
                self.current_transaction = "100"
                self.process_transaction(self.current_transaction, "2")
            elif call.data == "32":
                self.active_pack = 7
                self.chosen_pack_price = self.active_second_option_price
                self.current_transaction = "100"
                self.process_transaction(self.current_transaction, "1")
            elif call.data == "33":
                self.active_pack = 7
                self.chosen_pack_price = self.currency_prices["OCP7"]
                self.current_transaction = "100"
                self.process_transaction(self.current_transaction, "2")
            elif call.data == "34":
                self.active_pack = 10
                self.chosen_pack_price = self.active_third_option_price
                self.current_transaction = "100"
                self.process_transaction(self.current_transaction, "1")
            elif call.data == "35":
                self.active_pack = 10
                self.chosen_pack_price = self.currency_prices["OCP10"]
                self.current_transaction = "100"
                self.process_transaction(self.current_transaction, "2")
            # Buy points
            elif call.data == "36":
                self.active_pack = 150
                self.chosen_pack_price = self.currency_prices["Points150"]
                self.current_transaction = "101"
                self.process_transaction(self.current_transaction, "2")
            elif call.data == "37":
                self.active_pack = 500
                self.chosen_pack_price = self.currency_prices["Points500"]
                self.current_transaction = "101"
                self.process_transaction(self.current_transaction, "2")
            elif call.data == "38":
                self.active_pack = 2000
                self.chosen_pack_price = self.currency_prices["Points2000"]
                self.current_transaction = "101"
                self.process_transaction(self.current_transaction, "2")

    def pre_checkout_handler(self, query):
        self.bot.answer_pre_checkout_query(query.id, ok=True)
        # self.bot.register_next_step_handler(None, self.payment_handler, chat_id=self.current_user)

    def payment_handler(self, message):
        charge_info = stripe.Charge.retrieve(message.successful_payment.provider_payment_charge_id)
        # intent_info = stripe.PaymentIntent.retrieve(charge_info.payment_intent)

        if charge_info.status == "succeeded":
            self.delete_price_invoice()
            self.process_transaction(self.current_transaction, None, is_final=True)
            self.send_active_transaction_message("Payment was successful!")
        else:
            # TODO: Find out what gone wrong ?
            self.send_active_transaction_message("Payment failed. Please try again or contact the administration")

    def construct_active_pack_markup(self):
        self.effect_pack_markup.clear()
        self.effect_pack_markup.add(InlineKeyboardButton(f"Buy 1:", callback_data="0"), InlineKeyboardButton(f"{self.active_first_option_price} coins", callback_data="23"), InlineKeyboardButton(f"{self.active_currency_first_option_price} {self.user_currency}", callback_data="24"))\
            .add(InlineKeyboardButton(f"Buy 5:", callback_data="0"), InlineKeyboardButton(f"{self.active_second_option_price} coins", callback_data="25"), InlineKeyboardButton(f"{self.active_currency_second_option_price} {self.user_currency}", callback_data="26"))\
            .add(InlineKeyboardButton(f"Buy 10:", callback_data="0"), InlineKeyboardButton(f"{self.active_third_option_price} coins", callback_data="27"), InlineKeyboardButton(f"{self.active_currency_third_option_price} {self.user_currency}", callback_data="28"))\
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

    def send_price_invoice(self, title: str, description: str, price: str, invoice_payload: str):
        # TODO: achieve the same result using another method
        priceTag = LabeledPrice("Price", int(price.replace(",", "")))
        self.delete_price_invoice()
        self.current_invoice_id = self.bot.send_invoice(chat_id=self.current_user, currency=self.user_currency,
                         title=title,
                         description=description,
                         need_name=True,
                         invoice_payload=invoice_payload,
                         prices=[priceTag],
                         provider_token=Helpers.payment_token,
                         need_email=True,
                         suggested_tip_amounts=self.suggested_tips,
                         max_tip_amount=100_000_000,
                         protect_content=True).message_id

    def delete_price_invoice(self):
        try:
            if self.current_invoice_id is not None:
                self.bot.delete_message(self.current_user, self.current_invoice_id)
                self.current_invoice_id = None
        except:
            pass

    def display_user_balance(self):
        if self.active_message:
            self.bot.edit_message_text(self.get_balance_message(), self.current_user, self.active_message)
            return

    def set_currency_prices(self):
        self.currency_prices = Resources.get_prices(self.user_currency)

    def get_user_balance(self):
        self.userBalance = Helpers.get_active_user_balance(self.current_user)

        self.user_currency = self.userBalance["currency"]

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

        self.bot.callback_query_handlers.remove(self.ch)
        self.bot.message_handlers.remove(self.mh)
        self.bot.message_handlers.remove(self.hHandler)

        #TODO: Find out why throws an exception
        try:
            self.bot.pre_checkout_query_handlers.remove(self.pre_checkout_h)
        except:
            pass

        self.delete_price_invoice()

        if not self.activatedElsewhere:
            self.bot.delete_message(self.current_user, self.active_message)

            menues.go_back_to_main_menu(self.bot, self.current_user, self.message)
            return

        self.returnMethod(self.message, backFromShop=True)
