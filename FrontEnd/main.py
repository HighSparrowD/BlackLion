import Common.Menues as Menus
from Adventurer import Adventurer
from Core.Api import ApiBase
from Familiator import *
from RandomTalker import *
from Settings import *
from Registration import *
from FeedbackModule import FeedbackModule
from Shop import *

import Core.HelpersMethodes as Helpers
from AdminCabinet import AdminCabinet
from StartModule import StartModule
from Common.Keyboards import register_markup
from Advertisement_Module import AdvertisementModule

import os
from dotenv import load_dotenv
from Functions import *

# Update resources
set_path_prefixes("../Instruments/Resources/Inputs/", "")
create_registration_resources()
create_prices_resource()
create_settings_resources()
create_currency_setter_resources()
create_shop_resources()

load_dotenv()
key = os.getenv("KEY")
payment_token = os.getenv("STRIPE_TOKEN")

users = os.getenv("DEBUG_USERS").split(",")
ApiBase.set_api_address(os.getenv("API_ADDRESS"))
Helpers.set_payment_token(os.getenv("STRIPE_TOKEN"))
Helpers.set_stripe_key(os.getenv("STRIPE_SECRET_KEY"))
stripe.api_key = Helpers.stripe_key

bot = TeleBot(key)
bot.parse_mode = telegram.ParseMode.HTML
Menus.start_program_in_debug_mode(users)  # TODO: remove in production?

for user in users:
    go_back_to_main_menu(bot, user, None, False)

random_talkers = []
admin_sponsor_handlers = []
admin_cabinets = []


@bot.message_handler(commands=["start"], func=lambda message: message.chat.type == 'private')
def Start(message):
    # Allow only if user is not registered
    if not Helpers.check_user_is_registered(message.from_user.id):
        StartModule(bot, message)
        # TODO: Maybe send a sticker or smth
    else:
        Menus.go_back_to_main_menu(bot, message.from_user.id, message)


@bot.message_handler(commands=["registration"], func=lambda message: message.chat.type == 'private')
def restore_user(message):
    result = Helpers.restore_user_profile(message.from_user.id)

    if result == "Success":
        bot.send_message(message.from_user.id, "Done! Your profile is successfully restored!")
        Menus.go_back_to_main_menu(bot, message.from_user.id, message)
    elif result == "DoesNotExist":
        bot.send_message(message.from_user.id, "You are not registered in the bot. Note, that profiles are completely removed from bot after a month (30 days) since they had been deleted\nPlease hìt /start to register your profile and meet THE person ;)")


@bot.message_handler(commands=["random"], func=lambda message: message.chat.type == 'private')
def RandomTalk(message):
    create_random_talker(message, message.from_user.id)


@bot.message_handler(commands=["search"], func=lambda message: message.chat.type == 'private')
def Search(message):
    create_familiator(message, message.from_user.id)


@bot.message_handler(commands=["feedback"], func=lambda message: message.chat.type == 'private')
def Report(message):
    create_reporter(message, message.from_user.id)


@bot.message_handler(commands=["shop"], func=lambda message: message.chat.type == 'private')
def ShopC(message):
    create_shop(message, message.from_user.id)

#TODO: Past mistake. Remake
@bot.message_handler(commands=["switchstatus", "showstatus"], func=lambda message: message.chat.type == 'private', is_multihandler=True)
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


@bot.message_handler(commands=["enteradmincabinet"], func=lambda message: message.chat.type == 'private')
def EnterAdminCabinet(message):
    create_admin_cabinet(message)


# bot.next_step_backend.handlers.popitem()
@bot.message_handler(commands=["settings"], func=lambda message: message.chat.type == 'private')
def settings(message):
    create_settings(message, message.from_user.id)


@bot.message_handler(commands=["adventure"], func=lambda message: message.chat.type == 'private')
def adventurer(message):
    create_adventurer(message, message.from_user.id)


@bot.message_handler(commands=["advertisements"], func=lambda message: message.chat.type == 'private')
def advertisements(message):
    create_advertisement_module(message, message.from_user.id)


@bot.message_handler(commands=["help"], func=lambda message: message.chat.type == 'private', is_multihandler=True)
def help(message):
    Helper(bot, message)


@bot.message_handler(commands=["test"], func=lambda message: message.chat.type == 'private', is_multihandler=True)
def test(message):
    ads = Helpers.get_advertisement_economy_monthly_statistics(1254647653, 6)
    adss = Helpers.get_advertisement_engagement_monthly_statistics(1254647653, 6)
    pass


# new_chat_member - present even upon changing permissions / adding new users to group
@bot.my_chat_member_handler(func=lambda message: message.new_chat_member and bot.get_me().id == message.new_chat_member.user.id)
def set_up_group_management(message):
    me = message.new_chat_member

    #TODO: Check if any adventure awaits
    if me.status != 'left':
        if me.status == 'administrator':
            if me.can_invite_users and me.can_restrict_members and me.can_pin_messages:
                #TODO: Ask for name
                chat_id = message.chat.id

                # expiration_date = datetime.datetime.now() + datetime.timedelta(days=365 * 2)
                # link = bot.create_chat_invite_link(chat_id, expire_date=expiration_date.timestamp())

                link = bot.export_chat_invite_link(chat_id)
                responseCode = Helpers.set_adventure_group_link(
                    {"userId": message.from_user.id, "groupLink": link, "groupId": chat_id, "adventureName": ""})

                if responseCode == 1:
                    bot.send_message(chat_id, "All set. Now I have the power I need :)")
                    return

                #TODO: Inform user that no adventures were found ?
            else:
                bot.send_message(message.chat.id, "Please, grant me all of those permissions: \n<b>1. Ban Users\n2. Invite users via link\n3. Pin messages</b>")
        else:
            bot.send_message(message.chat.id, "Hey! Please grant me administrator rights, so that I might be useful for you.\n\nPermissions I require:\n<b>1. Ban Users\n2. Invite users via link\n3. Pin messages</b>")

    # link = bot.create_chat_invite_link(-1001939151711, name="Chat")

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

    if status == "Success":  # Success
        return Familiator(bot, message, userId, response["hasVisited"])
    elif status == "Busy":  # Busy
        return
    elif status == "DoesNotExist":  # Does not exist
        send_registration_warning(userId)
    elif status == "IsDeleted":  # Is deleted
        bot.send_message(message.from_user.id, "Hey! your account had been deleted recently. Would you like to restore it?\n Then hit /registration !")


def create_random_talker(message, userId):
    response = Helpers.switch_user_busy_status(userId, 6)
    status = response["status"]

    if status == "Success":  # Success
        return RandomTalker(bot, message, random_talkers, response["hasVisited"])
    elif status == "Busy":  # Busy
        return
    elif status == "DoesNotExist":  # Does not exist
        send_registration_warning(userId)
    elif status == "IsDeleted":  # Is deleted
        bot.send_message(userId, "Hey! your account had been deleted recently. Would you like to restore it?\n Then hit /registration !")
    elif status == "IsBanned":  # Is banned
        bot.send_message(userId, "Your reputation is to low. Please contact the administration to resolve that", reply_markup=Menus.menu_markup)


def create_shop(message, userId):
    response = Helpers.switch_user_busy_status(userId, 10)
    status = response["status"]

    if status == "Success":  # Success
        return Shop(bot, message, response["hasVisited"])
    elif status == "Busy":  # Busy
        return
    elif status == "DoesNotExist":  # Does not exist
        send_registration_warning(userId)
    elif status == "IsDeleted":  # Is deleted
        bot.send_message(userId, "Hey! your account had been deleted recently. Would you like to restore it?\n Then hit /registration !")
    elif status == "IsBanned":  # Is banned
        bot.send_message(userId, "Your reputation is to low. Please contact the administration to resolve that", reply_markup=Menus.menu_markup)


def create_settings(message, userId):
    response = Helpers.switch_user_busy_status(userId, 11)
    status = response["status"]

    if status == "Success":  # Success
        return Settings.Settings(bot, message)
    elif status == "Busy":  # Busy
        return
    elif status == "DoesNotExist":  # Does not exist
        send_registration_warning(userId)
    elif status == "IsDeleted":  # Is deleted
        bot.send_message(userId, "Hey! your account had been deleted recently. Would you like to restore it?\n Then hit /registration !")


def create_reporter(message, userId):
    response = Helpers.switch_user_busy_status(userId, 7)
    status = response["status"]

    if status == "Success":  # Success
        return FeedbackModule(bot, message, response["hasVisited"])
    elif status == "Busy":  # Busy
        return
    elif status == "DoesNotExist":  # Does not exist
        send_registration_warning(userId)
    elif status == "IsDeleted":  # Is deleted
        bot.send_message(userId, "Hey! your account had been deleted recently. Would you like to restore it?\n Then hit /registration !")


def create_adventurer(message, userId):
    response = Helpers.switch_user_busy_status(userId, 13)
    status = response["status"]

    if status == "Success":  # Success
        return Adventurer(bot, message, response["hasVisited"])
    elif status == "Busy":  # Busy
        return
    elif status == "DoesNotExist":  # Does not exist
        send_registration_warning(userId)
    elif status == "IsDeleted":  # Is deleted
        bot.send_message(userId, "Hey! your account had been deleted recently. Would you like to restore it?\n Then hit /registration !")


def create_advertisement_module(message, userId):
    response = Helpers.switch_user_busy_status(userId, 13)
    status = response["status"]

    if status == "Success":  # Success
        return AdvertisementModule(bot, message, response["hasVisited"])
    elif status == "Busy":  # Busy
        return
    elif status == "DoesNotExist":  # Does not exist
        send_registration_warning(userId)
    elif status == "IsDeleted":  # Is deleted
        bot.send_message(userId, "Hey! your account had been deleted recently. Would you like to restore it?\n Then hit /registration !")


def create_admin_cabinet(message):
    if Helpers.check_user_is_admin(message.from_user.id):
        return AdminCabinet(bot, message, admin_cabinets)


def send_registration_warning(userId):
    bot.send_message(userId, "Please register before entering this section", reply_markup=register_markup)


bot.polling()
