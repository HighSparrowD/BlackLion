import stripe
from telebot import TeleBot
from telebot.types import *
import Core.HelpersMethodes as Helpers
import Common.Menues as menues
import Settings
from Core.Api import ApiBase
from Core.Resources import Resources
from Helper import Helper
from TestModule import TestModule


class Shop:
    def __init__(self, bot: TeleBot, message: any, hasVisited: bool = False,
                 startingTransaction: int or str = None, returnMethod: any = None, active_message: any = None):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id
        self.hasVisited = hasVisited
        self.returnMethod = returnMethod
        self.startingTransaction = startingTransaction
        self.shouldGreet = True
        self.shouldStay = False
        self.activatedElsewhere = True
        self.user_language = Helpers.get_user_app_language(self.current_user)

        self.localization = Resources.get_shop_module_localization(self.user_language)
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
        self.current_invoice_id = None

        self.suggested_tips = [5_00, 50_00, 75_00, 100_00]

        self.userBalance = None
        self.user_currency = None

        self.currency_prices = None
        # Load price list in points
        self.points_prices = Resources.get_prices("Points")

        self.secondChanceDescription = self.localization["SC_Description"]
        self.valentineDescription = self.localization["V_Description"]
        self.detectorDescription = self.localization["D_Description"]
        self.nullifierDescription = self.localization["N_Description"]
        self.cardDeckMiniDescription = self.localization["CDM_Description"]
        self.cardDeckPlatinumDescription = self.localization["CdDP+Description"]

        self.YNmarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("yes", "no")
        self.currency_purchaseMarkup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add("1", "2", "3")

        self.start_markup = InlineKeyboardMarkup().add(InlineKeyboardButton(self.localization["Premium"], callback_data="1"))\
            .add(InlineKeyboardButton(self.localization["Coinds"], callback_data="2"), InlineKeyboardButton(self.localization["Effects"], callback_data="3"))\
            .add(InlineKeyboardButton(self.localization["OP_Points"], callback_data="4"))\
            .add(InlineKeyboardButton(self.localization["Tests"], callback_data="5"))\
            .add(InlineKeyboardButton(self.localization["Support_Us"], callback_data="6"))\
            .add(InlineKeyboardButton(self.localization["Exit"], callback_data="-1"))

        self.premiumBtn1 = InlineKeyboardButton("0", callback_data="10")
        self.premiumBtn2 = InlineKeyboardButton("0", callback_data="12")
        self.premiumBtn3 = InlineKeyboardButton("0", callback_data="14")

        self.OCbutton1 = InlineKeyboardButton("0", callback_data="31")
        self.OCbutton2 = InlineKeyboardButton("0", callback_data="33")
        self.OCbutton3 = InlineKeyboardButton("0", callback_data="35")

        self.pointsBtn1 = InlineKeyboardButton("0", callback_data="36")
        self.pointsBtn2 = InlineKeyboardButton("0", callback_data="37")
        self.pointsBtn3 = InlineKeyboardButton("0", callback_data="38")

        self.buy_premium_markup = InlineKeyboardMarkup().add(InlineKeyboardButton(self.localization["3Days"], callback_data="0"), InlineKeyboardButton(f"{self.points_prices['Premium7']} Coins", callback_data="9"), self.premiumBtn1)\
                                                        .add(InlineKeyboardButton(self.localization["21Days"], callback_data="0"), InlineKeyboardButton(f"{self.points_prices['Premium21']} Coins", callback_data="11"), self.premiumBtn2)\
                                                        .add(InlineKeyboardButton(self.localization["30Days"], callback_data="0"), InlineKeyboardButton(f"{self.points_prices['Premium30']} Coins", callback_data="13"), self.premiumBtn3)\
                                                        .add(InlineKeyboardButton(self.localization["GoBack"], callback_data="-1"))

        self.buyPP_markup = InlineKeyboardMarkup().add(InlineKeyboardButton(self.localization["Buy3"], callback_data="0"), InlineKeyboardButton(f"{self.points_prices['OCP3']} Coins", callback_data="30"), self.OCbutton1) \
            .add(InlineKeyboardButton(self.localization["Buy7"], callback_data="0"), InlineKeyboardButton(f"{self.points_prices['OCP7']} Coins", callback_data="32"), self.OCbutton2)\
            .add(InlineKeyboardButton(self.localization["Buy10"], callback_data="0"), InlineKeyboardButton(f"{self.points_prices['OCP10']} Coins", callback_data="34"), self.OCbutton3)\
            .add(InlineKeyboardButton(self.localization["GoBack"], callback_data="-1"))

        self.buy_points_markup = InlineKeyboardMarkup().add(InlineKeyboardButton(self.localization["Buy150"], callback_data="0"), self.pointsBtn1) \
            .add(InlineKeyboardButton(self.localization["Buy500"], callback_data="0"), self.pointsBtn2) \
            .add(InlineKeyboardButton(self.localization["Buy2000"], callback_data="0"), self.pointsBtn3) \
            .add(InlineKeyboardButton(self.localization["GoBack"], callback_data="-1"))

        self.effects_list_markup = InlineKeyboardMarkup().add(InlineKeyboardButton(self.localization["ScecondChance"], callback_data="16"))\
            .add(InlineKeyboardButton(self.localization["TheValentine"], callback_data="17"))\
            .add(InlineKeyboardButton(self.localization["TheDetector"], callback_data="18"))\
            .add(InlineKeyboardButton(self.localization["TheNullifier"], callback_data="19"))\
            .add(InlineKeyboardButton(self.localization["CardDeckMini"], callback_data="20"))\
            .add(InlineKeyboardButton(self.localization["CardDeckPlatinum"], callback_data="21"))\
            .add(InlineKeyboardButton(self.localization["GoBack"], callback_data="-1"))

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
            Settings.CurrencySetter(self.bot, self.current_user, self.first_time_handler, self.user_language)
        else:
            self.proceed_to_start(message)

    def proceed_to_start(self, message):
        self.ch = self.bot.register_callback_query_handler(message, self.callback_handler, user_id=self.current_user)
        self.mh = self.bot.register_message_handler(self.payment_handler, content_types=['successful_payment'], user_id=self.current_user)
        self.pre_checkout_h = self.bot.register_pre_checkout_query_handler(self.pre_checkout_handler, func=lambda query: True)

        self.hHandler = self.bot.register_message_handler(self.help_handler, commands=["help"], user_id=self.current_user)

        self.start(message)

    def start(self, message):
        self.previous_section = self.destruct
        self.current_section = self.start

        # Load price lists based on currency selected by user
        self.set_currency_prices()

        if self.startingTransaction:
            if isinstance(self.startingTransaction, str):
                self.callback_handler(DummyCallable(0, self.startingTransaction))
                return
            elif self.startingTransaction == 1:
                self.choose_pack_points(message)
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
            self.send_active_message(self.localization["Welcome"].format(self.get_balance_message()), markup=self.start_markup)
            self.shouldGreet = False
        else:
            self.send_active_message(self.get_balance_message(), self.start_markup)

    def buy_premium(self, message=None):
        self.current_section = self.buy_premium

        self.premiumBtn1.text = f"{self.currency_prices['Premium7']} {self.user_currency}"
        self.premiumBtn2.text = f"{self.currency_prices['Premium21']} {self.user_currency}"
        self.premiumBtn3.text = f"{self.currency_prices['Premium30']} {self.user_currency}"

        if not self.activatedElsewhere:
            self.previous_section = self.start
        else:
            self.previous_section = self.destruct

        self.send_active_message(self.localization["SelectPackAndCurrency"].format(self.get_balance_message()), markup=self.buy_premium_markup)

    def choose_effect_to_buy(self, message=None):
        self.current_section = self.choose_effect_to_buy

        if not self.activatedElsewhere:
            self.previous_section = self.start
        else:
            self.previous_section = self.destruct

        self.clear_screen(True)

        self.send_active_message(self.localization["SelectEffect"].format(self.get_balance_message()), markup=self.effects_list_markup)

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

        self.send_active_message(self.localization["SelectEffectPack"].format(self.get_balance_message()), markup=self.effect_pack_markup)

    def choose_pack_PP(self, message=None):

        self.active_first_option_price = self.points_prices["OCP3"]
        self.active_second_option_price = self.points_prices["OCP7"]
        self.active_third_option_price = self.points_prices["OCP10"]

        self.OCbutton1.text = f"{self.currency_prices['OCP3']} {self.user_currency}"
        self.OCbutton2.text = f"{self.currency_prices['OCP7']} {self.user_currency}"
        self.OCbutton3.text = f"{self.currency_prices['OCP10']} {self.user_currency}"

        self.send_active_message(self.localization["SelectPointPack"].format(self.get_balance_message()), markup=self.buyPP_markup)

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

        self.send_active_message(self.localization["SelectPointPack"].format(self.get_balance_message()), markup=self.buy_points_markup)

        self.current_section = self.choose_pack_points

        if not self.activatedElsewhere:
            self.previous_section = self.start
        else:
            self.previous_section = self.destruct

    def buy_tests(self):
        # Remove previous callback handler so that handlers do not collide
        self.bot.callback_query_handlers.remove(self.ch)
        self.bot.message_handlers.remove(self.mh)
        self.bot.message_handlers.remove(self.hHandler)

        self.bot.pre_checkout_query_handlers.remove(self.pre_checkout_h)

        self.ch = None
        self.mh = None
        self.hHandler = None
        self.pre_checkout_h = None

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
                    self.send_price_invoice(self.localization["Premium"], self.localization["3Premium"], self.chosen_pack_price, self.localization["Premium"])
                elif is_final:
                    result = Helpers.grant_premium_for_real_money(self.current_user, self.chosen_pack_price, 3, self.user_currency)
            elif transaction_type == "2":
                if currency == "1":
                    result = Helpers.grant_premium_for_points(self.current_user, self.chosen_pack_price, 21)
                elif currency == "2":
                    self.send_price_invoice(self.localization["Premium"], self.localization["21Premium"], self.chosen_pack_price, self.localization["Premium"])
                elif is_final:
                    result = Helpers.grant_premium_for_real_money(self.current_user, self.chosen_pack_price, 21, self.user_currency)
            elif transaction_type == "3":
                if currency == "1":
                    result = Helpers.grant_premium_for_points(self.current_user, self.chosen_pack_price, 30)
                elif currency == "2":
                    self.send_price_invoice(self.localization["Premium"], self.localization["30Premium"], self.chosen_pack_price, self.localization["Premium"])
                elif is_final:
                    result = Helpers.grant_premium_for_real_money(self.current_user, self.chosen_pack_price, 30, self.user_currency)
            elif transaction_type == "5":
                if currency == "1":
                    result = Helpers.purchase_effect_for_points(self.current_user, "5", self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    self.send_price_invoice(self.localization['ScecondChance'], f"{self.active_pack} {self.localization['ScecondChance']}\n\n{self.secondChanceDescription}", self.chosen_pack_price, "Effect")
                elif is_final:
                    result = Helpers.purchase_effect_for_real_money(self.current_user, "5", self.chosen_pack_price, self.user_currency, self.active_pack)
            elif transaction_type == "6":
                if currency == "1":
                    result = Helpers.purchase_effect_for_points(self.current_user, "6", self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    self.send_price_invoice(self.localization['TheValentine'], f"{self.active_pack} {self.localization['TheValentine']}\n\n{self.valentineDescription}", self.chosen_pack_price, "Effect")
                elif is_final:
                    result = Helpers.purchase_effect_for_real_money(self.current_user, "6", self.chosen_pack_price, self.user_currency, self.active_pack)
            elif transaction_type == "7":
                if currency == "1":
                    result = Helpers.purchase_effect_for_points(self.current_user, "7", self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    self.send_price_invoice(self.localization['TheDetector'], f"{self.active_pack} {self.localization['TheDetector']}\n\n{self.detectorDescription}", self.chosen_pack_price, "Effect")
                elif is_final:
                    result = Helpers.purchase_effect_for_real_money(self.current_user, "7", self.chosen_pack_price, self.user_currency, self.active_pack)
            elif transaction_type == "8":
                if currency == "1":
                    result = Helpers.purchase_effect_for_points(self.current_user, "8", self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    self.send_price_invoice(self.localization['TheNullifier'], f"{self.active_pack} {self.localization['TheNullifier']}\n\n{self.nullifierDescription}", self.chosen_pack_price, "Effect")
                elif is_final:
                    result = Helpers.purchase_effect_for_real_money(self.current_user, "8", self.chosen_pack_price, self.user_currency, self.active_pack)
            elif transaction_type == "9":
                if currency == "1":
                    result = Helpers.purchase_effect_for_points(self.current_user, "9", self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    self.send_price_invoice(self.localization['CardDeckMini'], f"{self.active_pack} {self.localization['CardDeckMini']}\n\n{self.cardDeckMiniDescription}", self.chosen_pack_price, "Effect")
                elif is_final:
                    result = Helpers.purchase_effect_for_real_money(self.current_user, "9", self.chosen_pack_price, self.user_currency, self.active_pack)
            elif transaction_type == "10":
                if currency == "1":
                    result = Helpers.purchase_effect_for_points(self.current_user, "10", self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    self.send_price_invoice(self.localization['CardDeckPlatinum'], f"{self.active_pack} {self.localization['CardDeckPlatinum']}\n\n{self.cardDeckPlatinumDescription}", self.chosen_pack_price, "Effect")
                elif is_final:
                    result = Helpers.purchase_effect_for_real_money(self.current_user, "10", self.chosen_pack_price, self.user_currency, self.active_pack)
            elif transaction_type == "100":
                if currency == "1":
                    result = Helpers.purchase_PP_for_points(self.current_user, self.chosen_pack_price, self.active_pack)
                elif currency == "2":
                    self.send_price_invoice(self.localization['OP_Points'], f"{self.active_pack} {self.localization['OP_Points']}", self.chosen_pack_price, "OceanPoints")
                elif is_final:
                    result = Helpers.purchase_PP_for_real_money(self.current_user, self.chosen_pack_price, self.user_currency, self.active_pack)
            elif transaction_type == "101":
                if currency == "2":
                    self.send_price_invoice(self.localization["Points"], f"{self.active_pack} {self.localization['Points']}", self.chosen_pack_price, "Points")
                elif is_final:
                    result = Helpers.purchase_points_for_real_money(self.current_user, self.chosen_pack_price, self.user_currency, self.active_pack)

            if type(result) is not bool and result.status_code == 200:
                if not is_final:
                    self.userBalance["points"] -= self.chosen_pack_price
                #self.display_user_balance()
                self.proceed(self.message)

                if currency != "2":
                    self.send_active_transaction_message(self.localization["Successfull"])

                # self.send_active_transaction_message("Transaction was successful")

                #Return to previous Module if it exists
                # if self.startingTransaction is not None:
                #     self.destruct()

            else:
                if currency != "2":
                    self.send_active_transaction_message(self.localization["SomethingWrong"])
        else:
            self.send_active_transaction_message(self.localization["NotEnoughCoins"])

    def send_active_transaction_message(self, text):
        if self.active_transaction_status_message is not None:
            self.bot.delete_message(self.current_user, self.active_transaction_status_message)

        self.active_transaction_status_message = self.bot.send_message(self.current_user, text).id

    def callback_handler(self, call):
        if call.id != 0:
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
            self.send_active_transaction_message(self.localization["PaymentSuccessfull"])
        # elif charge_info.status == "processing":
        #     pass
        elif charge_info.status == "payment_failed":
            self.send_active_transaction_message(self.localization["PaymentFail"])

    def construct_active_pack_markup(self):
        self.effect_pack_markup.clear()
        self.effect_pack_markup.add(InlineKeyboardButton(self.localization["Buy3"], callback_data="0"), InlineKeyboardButton(f"{self.active_first_option_price} Coins", callback_data="23"), InlineKeyboardButton(f"{self.active_currency_first_option_price} {self.user_currency}", callback_data="24"))\
            .add(InlineKeyboardButton(self.localization["Buy7"], callback_data="0"), InlineKeyboardButton(f"{self.active_second_option_price} Coins", callback_data="25"), InlineKeyboardButton(f"{self.active_currency_second_option_price} {self.user_currency}", callback_data="26"))\
            .add(InlineKeyboardButton(self.localization["Buy10"], callback_data="0"), InlineKeyboardButton(f"{self.active_third_option_price} Coins", callback_data="27"), InlineKeyboardButton(f"{self.active_currency_third_option_price} {self.user_currency}", callback_data="28"))\
            .add(InlineKeyboardButton(self.localization["GoBack"], callback_data="-1"))

    def get_balance_message(self):
        return self.localization["CurrentBalance"].format(self.userBalance['points'])

    def clear_screen(self, skipTransaction=False):
        # Clear screen of previous transaction message
        if not skipTransaction and self.active_transaction_status_message:
            try:
                self.bot.delete_message(self.current_user, self.active_transaction_status_message)
                self.active_transaction_status_message = None
            except:
                pass

        # Clear screen of a previous effect description message
        if self.active_description_message:
            try:
                self.bot.delete_message(self.current_user, self.active_description_message)
                self.active_description_message = None
            except:
                pass

    def send_active_message(self, text, markup=None):
        try:
            if self.active_message:
                self.bot.edit_message_text(text, self.current_user, self.active_message, reply_markup=markup)
                return
            self.active_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except:
            self.delete_active_message()
            self.send_active_message(text, markup)

    def send_price_invoice(self, title: str, description: str, price: str, invoice_payload: str):
        priceTag = LabeledPrice(self.localization["Price"], int(price.replace(",", "")))
        self.delete_price_invoice()
        self.current_invoice_id = self.bot.send_invoice(chat_id=self.current_user, currency=self.user_currency,
                         title=title,
                         description=description,
                         need_name=True,
                         invoice_payload=invoice_payload,
                         prices=[priceTag],
                         provider_token=ApiBase.payment_token,
                         need_email=True,
                         suggested_tip_amounts=self.suggested_tips,
                         max_tip_amount=100_000_000,
                         protect_content=True).message_id

    def delete_active_message(self):
        try:
            if self.active_message:
                self.bot.delete_message(self.current_user, self.active_message)
                self.active_message = None
        except:
            self.active_message = None

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
                self.ch = self.bot.register_callback_query_handler(message, self.callback_handler, user_id=self.current_user)
                self.mh = self.bot.register_message_handler(self.payment_handler, content_types=['successful_payment'],
                                                            user_id=self.current_user)
                self.pre_checkout_h = self.bot.register_pre_checkout_query_handler(self.pre_checkout_handler,
                                                                                   func=lambda query: True)

                self.hHandler = self.bot.register_message_handler(self.help_handler, commands=["help"],
                                                                  user_id=self.current_user)

            self.previous_section(message)

    def help_handler(self, message):
        self.bot.delete_message(self.current_user, message.id)
        Helper(self.bot, message, self.current_section, activeMessageId=self.active_message, secondaryMessageId=self.active_description_message)

    def destruct(self, message=None):
        self.clear_screen()

        self.bot.callback_query_handlers.remove(self.ch)
        self.bot.message_handlers.remove(self.mh)
        self.bot.message_handlers.remove(self.hHandler)
        self.bot.pre_checkout_query_handlers.remove(self.pre_checkout_h)

        self.delete_price_invoice()
        self.delete_active_message()

        if not self.activatedElsewhere:
            menues.go_back_to_main_menu(self.bot, self.current_user, self.message)
            return

        self.returnMethod(self.message, backFromShop=True)


class DummyCallable:
    def __init__(self, id: int, data: str):
        self.id = id
        self.data = data
        self.message = None
