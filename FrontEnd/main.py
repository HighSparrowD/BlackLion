from telebot import *

from Common.Menues import show_admin_markup, admin_menu_markup, menu_markup, register_markup
from Familiator import *
from RandomTalker import *
from Registration import *
from Reporter import Reporter
from Requester import *
from Shop import *

import Core.HelpersMethodes as Helpers
from Sponsor_Handler import SponsorHandler, SponsorHandlerAdmin
from AdminCabinet import AdminCabinet

bot = TeleBot("5488749379:AAEJ0t9RksogDD14zJLRYqSisBUpu2pS2WU") #TODO: relocate code to an .env file or db
Helpers.start_program_in_debug_mode(bot) #TODO: remove in production

familiators = []
random_talkers = []
registrators = []
requesters = []
sponsor_handlers = []
admin_sponsor_handlers = []
admin_cabinets = []
reporters = []
shops = []


@bot.message_handler(commands=["registration"], is_multihandler=True)
def Greet(message):
    if Helpers.check_user_is_registered(message.from_user.id):
        if not Helpers.check_user_is_busy(message.from_user.id):
            create_registrator(message)
        return False
    create_registrator(message)


@bot.message_handler(commands=["random"])
def RandomTalk(message):
    if not Helpers.check_user_is_busy(message.from_user.id):
        create_random_talker(message)


@bot.message_handler(commands=["search"])
def Search(message):
    if not Helpers.check_user_is_busy(message.from_user.id):
        create_familiator(message, message.from_user.id)


@bot.message_handler(commands=["feedback"], is_multihandler=True)
def Report(message):
    if not Helpers.check_user_is_busy(message.from_user.id):
        create_reporter(message)


@bot.message_handler(commands=["shop"])
def ShopC(message):
    if not Helpers.check_user_is_busy(message.from_user.id):
        create_shop(message)


@bot.message_handler(commands=["sponsoraccount"])
def Sponsor_Handler(message):
    if not Helpers.check_user_is_busy(message.from_user.id):
        create_sponsor_handler(message)


@bot.message_handler(commands=["switchstatus", "showstatus"], is_multihandler=True)
def SwitchAdminStatus(message):
    if message.text == "/switchstatus":
        user_was_admin = Helpers.check_user_is_admin(message.from_user.id)
        msg = Helpers.switch_admin_status(message.from_user.id)
        bot.send_message(message.from_user.id, msg)
        if Helpers.check_user_is_admin(message.from_user.id):
            show_admin_markup(bot, message.from_user.id)
        if user_was_admin:
            go_back_to_main_menu(bot, message.from_user.id)

        return False

    msg = Helpers.get_admin_status(message.from_user.id)
    if msg:
        bot.send_message(message.from_user.id, f"Your current admin status is: -> {msg} <-", reply_markup=admin_menu_markup)


@bot.message_handler(commands=["enteradmincabinet"])
def EnterAdminCabinet(message):
    if not Helpers.check_user_is_busy(message.from_user.id):
        create_admin_cabinet(message)


@bot.message_handler(commands=["test"])
def test(message):
    m = InlineKeyboardMarkup().add(InlineKeyboardButton("LLALA", callback_data=1))
    bot.send_message(message.from_user.id, "Try", reply_markup=m)


def create_registrator(message):
    visit = Helpers.check_user_has_visited_section(message.from_user.id, 1)
    return Registrator(bot, message, registrators, visit)
    #TODO: check if user exists, convert bool to short, pass it to registrator creating;
    # Add additional buttons depending on whether or not user exists


def create_familiator(message, userId):
    if Helpers.check_user_exists(message.from_user.id):
        if not Helpers.check_user_is_banned(message.from_user.id):
            if not Helpers.check_user_is_deleted(message.from_user.id):
                visit = Helpers.check_user_has_visited_section(message.from_user.id, 2)
                return Familiator(bot, message, userId, familiators, requesters, visit)
            else:
                bot.send_message(message.from_user.id, "Hey! your account had been deleted recently. Would you like to pass a quick registration and save all your lost data?\n Then hit /register !")
        else:
            bot.send_message(message.from_user.id, "Sorry, you had been banned. Please contact the support team")
    send_registration_warning(userId)


def create_random_talker(message):
    if Helpers.check_user_exists(message.from_user.id):
        if not Helpers.check_user_is_banned(message.from_user.id):
            if not Helpers.check_user_is_deleted(message.from_user.id):
                visit = Helpers.check_user_has_visited_section(message.from_user.id, 6)
                return RandomTalker(bot, message, random_talkers, visit)
            else:
                bot.send_message(message.from_user.id, "Hey! your account had been deleted recently. Would you like to pass a quick registration and save all your lost data?\n Then hit /register !")
        else:
            bot.send_message(message.from_user.id, "Sorry, you had been banned. Please contact the support team")
    send_registration_warning(message.from_user.id)


def create_shop(message):
    if Helpers.check_user_exists(message.from_user.id):
        if not Helpers.check_user_is_banned(message.from_user.id):
            visit = Helpers.check_user_has_visited_section(message.from_user.id, 10)
            return Shop(bot, message, shops, visit)
        else:
            bot.send_message(message.from_user.id, "Sorry, you had been banned. Please contact the support team")
    send_registration_warning(message.from_user.id)


def create_reporter(message):
    if Helpers.check_user_exists(message.from_user.id):
        if not Helpers.check_user_is_banned(message.from_user.id):
            if not Helpers.check_user_is_deleted(message.from_user.id):
                language = int(json.loads(requests.get(f"https://localhost:44381/GetUserLanguagePrefs/{message.from_user.id}", verify=False).text))
                visit = Helpers.check_user_has_visited_section(message.from_user.id, 7)
                return Reporter(bot, message, language, reporters, visit)
            else:
                bot.send_message(message.from_user.id, "Hey! your account had been deleted recently. Would you like to pass a quick registration and save all your lost data?\n Then hit /register !")
        else:
            bot.send_message(message.from_user.id, "Sorry, you had been banned. Please contact the support team")
    send_registration_warning(message.from_user.id)


def create_sponsor_handler(message):
    if Helpers.check_user_is_admin(message.from_user.id):
        return SponsorHandlerAdmin(bot, message, admin_sponsor_handlers)
    elif Helpers.check_user_exists(message.from_user.id):
        if not Helpers.check_user_is_banned(message.from_user.id):
            visit = Helpers.check_user_has_visited_section(message.from_user.id, 8)
            return SponsorHandler(bot, message, sponsor_handlers, visit)
        else:
            bot.send_message(message.from_user.id, "Sorry, you had been banned. Please contact the support team")
    send_registration_warning(message.from_user.id)


def create_admin_cabinet(message):
    if Helpers.check_user_is_admin(message.from_user.id):
        return AdminCabinet(bot, message, admin_cabinets)


def send_registration_warning(userId):
    bot.send_message(userId, "Please register before entering this section", reply_markup=register_markup)


def check_module_created(userId, module_array): #Probably redundant
    for module in module_array:
        if module.current_user == userId:
            return True
    return False


bot.polling()
