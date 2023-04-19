from telebot import *

import Common.Menues as Menus
from Adventurer import Adventurer
from Familiator import *
from RandomTalker import *
from Settings import *
from Registration import *
from FeedbackModule import FeedbackModule
from Shop import *

import Core.HelpersMethodes as Helpers
# from Sponsor_Handler import SponsorHandler
# from SponsorHandlerAdmin import SponsorHandlerAdmin
from AdminCabinet import AdminCabinet
from StartModule import StartModule

bot = TeleBot("5488749379:AAEJ0t9RksogDD14zJLRYqSisBUpu2pS2WU") #TODO: relocate code to an .env file or db
bot.parse_mode = telegram.ParseMode.HTML
Menus.start_program_in_debug_mode(bot) #TODO: remove in production

random_talkers = []
sponsor_handlers = []
admin_sponsor_handlers = []
admin_cabinets = []


@bot.message_handler(commands=["start"])
def Start(message):
    #Allow only if user is not registered
    if not Helpers.check_user_is_registered(message.from_user.id):
        StartModule(bot, message)
        #TODO: Maybe send a sticker or smth
    else:
        Menus.go_back_to_main_menu(bot, message.from_user.id, message)


@bot.message_handler(commands=["registration"], is_multihandler=True)
def Greet(message):
    create_registrator(message)


@bot.message_handler(commands=["random"])
def RandomTalk(message):
    create_random_talker(message, message.from_user.id)


@bot.message_handler(commands=["search"])
def Search(message):
    create_familiator(message, message.from_user.id)


@bot.message_handler(commands=["feedback"], is_multihandler=True)
def Report(message):
    create_reporter(message, message.from_user.id)


@bot.message_handler(commands=["shop"])
def ShopC(message):
    create_shop(message, message.from_user.id)


@bot.message_handler(commands=["sponsoraccount"])
def Sponsor_Handler(message):
    create_sponsor_handler(message)


@bot.message_handler(commands=["switchstatus", "showstatus"], is_multihandler=True)
def SwitchAdminStatus(message):
    if message.text == "/switchstatus":
        user_was_admin = Helpers.check_user_is_admin(message.from_user.id)
        msg = Helpers.switch_admin_status(message.from_user.id)
        bot.send_message(message.from_user.id, msg)
        if Helpers.check_user_is_admin(message.from_user.id):
            Menus.show_admin_markup(bot, message.from_user.id)
        if user_was_admin:
            go_back_to_main_menu(bot, message.from_user.id, message)

        return False

    msg = Helpers.get_admin_status(message.from_user.id)
    if msg:
        bot.send_message(message.from_user.id, f"Your current admin status is: -> {msg} <-", reply_markup=Menus.admin_menu_markup)


@bot.message_handler(commands=["enteradmincabinet"])
def EnterAdminCabinet(message):
    create_admin_cabinet(message)


# bot.next_step_backend.handlers.popitem()
@bot.message_handler(commands=["settings"])
def settings(message):
    create_settings(message, message.from_user.id)


@bot.message_handler(commands=["adventure"])
def adventurer(message):
    create_adventurer(message, message.from_user.id)


@bot.message_handler(commands=["help"], is_multihandler=True)
def help(message):
    Helper(bot, message)


# @bot.message_handler(commands=["test"])
# def test(message):
#     bot.add_chat_member()

# @bot.message_handler()
# def test(message):
#     try:
#         t = bot.get_sticker_set(message.text.replace("/test", ""))
#         bot.send_sticker(message.chat.id, t.stickers[0].file_id)
#     except:
#         pass
    # bot.send_message(message.from_user.id, "* A list item With multiple paragraphs* Bar", parse_mode=telegram.ParseMode.MARKDOWN_V2)
    # bot.send_message(message.from_user.id, "term: definition")


def create_registrator(message):
    visit = Helpers.check_user_is_registered(message.from_user.id)
    return Registrator(bot, message, visit)


def create_familiator(message, userId):
    response = Helpers.switch_user_busy_status(userId, 2)
    status = response["status"]

    if status == 1: # Success
        return Familiator(bot, message, userId, response["hasVisited"])
    elif status == 2: # Busy
        return
    elif status == 3: # Does not exist
        send_registration_warning(userId)
    elif status == 4: # Is deleted
        bot.send_message(message.from_user.id, "Hey! your account had been deleted recently. Would you like to pass a quick registration and save all your lost data?\n Then hit /register !")


def create_random_talker(message, userId):
    response = Helpers.switch_user_busy_status(userId, 6)
    status = response["status"]

    if status == 1: # Success
        return RandomTalker(bot, message, random_talkers, response["hasVisited"])
    elif status == 2: # Busy
        return
    elif status == 3: # Does not exist
        send_registration_warning(userId)
    elif status == 4: # Is deleted
        bot.send_message(userId, "Hey! your account had been deleted recently. Would you like to pass a quick registration and save all your lost data?\n Then hit /register !")
    elif status == 5:
        bot.send_message(userId, "Your reputation is to low. Please contact the administration to resolve that", reply_markup=Menus.menu_markup)


def create_shop(message, userId):
    response = Helpers.switch_user_busy_status(userId, 10)
    status = response["status"]

    if status == 1: # Success
        return Shop(bot, message, response["hasVisited"])
    elif status == 2: # Busy
        return
    elif status == 3: # Does not exist
        send_registration_warning(userId)
    elif status == 4: # Is deleted
        bot.send_message(userId, "Hey! your account had been deleted recently. Would you like to pass a quick registration and save all your lost data?\n Then hit /register !")
    elif status == 5:
        bot.send_message(userId, "Your reputation is to low. Please contact the administration to resolve that", reply_markup=Menus.menu_markup)


def create_settings(message, userId):
    response = Helpers.switch_user_busy_status(userId, 11)
    status = response["status"]

    if status == 1: # Success
        return Settings(bot, message)
    elif status == 2: # Busy
        return
    elif status == 3: # Does not exist
        send_registration_warning(userId)
    elif status == 4: # Is deleted
        bot.send_message(userId, "Hey! your account had been deleted recently. Would you like to pass a quick registration and save all your lost data?\n Then hit /register !")


def create_reporter(message, userId):
    response = Helpers.switch_user_busy_status(userId, 7)
    status = response["status"]

    if status == 1: # Success
        return FeedbackModule(bot, message, response["hasVisited"])
    elif status == 2: # Busy
        return
    elif status == 3: # Does not exist
        send_registration_warning(userId)
    elif status == 4: # Is deleted
        bot.send_message(userId, "Hey! your account had been deleted recently. Would you like to pass a quick registration and save all your lost data?\n Then hit /register !")


def create_adventurer(message, userId):
    response = Helpers.switch_user_busy_status(userId, 13)
    status = response["status"]

    if status == 1: # Success
        return Adventurer(bot, message, response["hasVisited"])
    elif status == 2: # Busy
        return
    elif status == 3: # Does not exist
        send_registration_warning(userId)
    elif status == 4: # Is deleted
        bot.send_message(userId, "Hey! your account had been deleted recently. Would you like to pass a quick registration and save all your lost data?\n Then hit /register !")


#TODO: Finish up using new functionality (viz another create methods)
def create_sponsor_handler(message):
    if Helpers.check_user_is_admin(message.from_user.id):
        pass
        # return SponsorHandlerAdmin(bot, message, admin_sponsor_handlers)
    elif Helpers.check_user_exists(message.from_user.id):
        visit = Helpers.check_user_has_visited_section(message.from_user.id, 8)
        # return SponsorHandler(bot, message, sponsor_handlers, visit)
    send_registration_warning(message.from_user.id)


def create_admin_cabinet(message):
    if Helpers.check_user_is_admin(message.from_user.id):
        return AdminCabinet(bot, message, admin_cabinets)


def send_registration_warning(userId):
    bot.send_message(userId, "Please register before entering this section", reply_markup=Menus.register_markup)


bot.polling()