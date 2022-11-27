from telebot.types import *
import Core.HelpersMethodes as Helpers
import Common.Menues as menues


class Shop:
    def __init__(self, bot, message, visit):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id
        Helpers.switch_user_busy_status(self.current_user)
        self.price_list = f"\n1 day of premium costs 500p\n7 days of premium cost 1500p\n1 month (always 30 days) costs 7000p"

        self.start_markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True, row_width=3)\
            .add(KeyboardButton("Buy premium"), KeyboardButton("/exit")) #TODO: Expand in the future
        self.buy_premium_markup = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True, row_width=3)\
            .add(KeyboardButton("1 day"), KeyboardButton("7 days"), KeyboardButton("1 month"))

        self.mh = self.bot.register_message_handler(self.message_handler, user_id=self.current_user, commands=["exit"])

        self.bot.send_message(self.current_user, "Welcome to shop! What are you willing to buy?", reply_markup=self.start_markup)
        self.bot.register_next_step_handler(message, self.start, chat_id=self.current_user)

    def start(self, message):
        if message.text.lower() == "buy premium":
            self.bot.send_message(self.current_user, f"Nice! today price list looks like this:{self.price_list}", reply_markup=self.buy_premium_markup)
            self.bot.register_next_step_handler(message, self.buy_premium_step_1, chat_id=self.current_user)
        elif message.text.lower() == "/exit":
            self.message_handler(message)
        else:
            self.return_to_start(message, "No such option")

    def buy_premium_step_1(self, message):
        duration = 0
        price = 0
        if message.text.lower() == "1 day":
            duration = 1
            price = 500
        elif message.text.lower() == "7 days":
            duration = 7
            price = 1500
        elif message.text.lower() == "1 month":
            duration = 30
            price = 7000

        if duration > 0:
            if Helpers.check_user_balance_is_sufficient(self.current_user, price):
                try:
                    Helpers.grant_premium(self.current_user, price, duration)
                    self.return_to_start(message, "Transaction was successful!")
                except:
                    self.return_to_start(message, "Something went wrong...\nContact our technical support @GraphicGod")
            else:
                self.bot.send_message(self.current_user, "You have not enough points to buy this item\nYou can unlock an achievement, invite some friends using your personal link or qrcode to receive some more points. Also you can buy them in the shop ;-)")
                self.bot.send_message(self.current_user, "What are you willing to buy?", reply_markup=self.start_markup)
                self.bot.register_next_step_handler(message, self.start, chat_id=self.current_user)

    def return_to_start(self, message, message_to_user):
        self.bot.send_message(self.current_user, message_to_user, reply_markup=self.start_markup)
        self.bot.register_next_step_handler(message, self.start, chat_id=self.current_user)

    def message_handler(self, message):
        self.destruct()

    def destruct(self):
        self.bot.message_handlers.remove(self.mh)
        Helpers.switch_user_busy_status(self.current_user)
        menues.go_back_to_main_menu(self.bot, self.current_user, self.message)
