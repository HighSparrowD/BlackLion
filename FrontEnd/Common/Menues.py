import copy
from math import ceil
from Core import HelpersMethodes as Helpers
from telebot.types import ReplyKeyboardMarkup, KeyboardButton, InlineKeyboardButton, InlineKeyboardMarkup

from Requester import Requester

menu_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True) \
    .add(KeyboardButton("/search"),
         KeyboardButton("/random"),
         KeyboardButton("/feedback"),
         KeyboardButton("/sponsoraccount"),
         KeyboardButton("/random"),
         KeyboardButton("/shop"))

admin_menu_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True) \
    .add(KeyboardButton("/switchstatus"),
         KeyboardButton("/showstatus"),
         KeyboardButton("/enteradmincabinet"))

register_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True) \
    .add(KeyboardButton("/registration"))


def go_back_to_main_menu(bot, user, message):
    if Helpers.check_user_has_requests(user):
        request_list = Helpers.get_user_requests(user)
        if message:
            Requester(bot, message, user, request_list)
        return False
    bot.send_message(user, "What are we doing next? 😊", reply_markup=menu_markup)


def show_admin_markup(bot, user):
    bot.send_message(user, "Sending you basic admin command set 😏", reply_markup=admin_menu_markup)


def start_program_in_debug_mode(bot): # TODO: remove in production
    users = Helpers.start_program_in_debug_mode(bot)
    for user in users:
        go_back_to_main_menu(bot, user, None)


def count_pages(section_elements, current_markup_elements, markup_pages_count, prefs=False):
    current_markup_elements.clear()
    section_elements = copy.copy(section_elements)
    markup = InlineKeyboardMarkup()
    i = 0
    element_count_on_page = 5
    elements_to_delete = []
    isLastElement = False

    count = ceil(len(section_elements) / 5)  # TODO: count properly

    if prefs:
        markup.add(InlineKeyboardButton("Same as mine", callback_data=-5))
    for element in list(section_elements):
        if len(section_elements) > 5:
            element_count_on_page = 5
        else:
            element_count_on_page = len(section_elements)

        isLastElement = len(elements_to_delete) >= element_count_on_page - 1

        if len(markup.keyboard) < element_count_on_page and not isLastElement:  # TODO: List is loosing few last elements, make it work
            markup.add(InlineKeyboardButton(section_elements[element].capitalize(), callback_data=f"{element}"))
            elements_to_delete.append(element)
        elif isLastElement:
            markup.add(InlineKeyboardButton(section_elements[element].capitalize(), callback_data=f"{element}"))
            elements_to_delete.append(element)

            i += 1
            markup.add(InlineKeyboardButton("<", callback_data="-1"),
                       InlineKeyboardButton(f"{i} / {count}", callback_data=f"{i} / {count}"),
                       InlineKeyboardButton(">", callback_data="-2"))
            current_markup_elements.append(copy.deepcopy(markup))
            markup_pages_count += 1
            markup.clear()
            for e in elements_to_delete:
                section_elements.pop(e)
            elements_to_delete.clear()


def assemble_markup(markup_page, current_markup_elements, index):
    return current_markup_elements[(markup_page + index) - 1]


def reset_pages(current_markup_elements, markup_last_element, markup_page, markup_pages_count):
    current_markup_elements.clear()
    markup_last_element = 0
    markup_page = 1
    markup_pages_count = 0


def add_tick_to_element(bot, userId, messageId, current_markup_elements, markup_page, element_index):
    try:
        wasChanged = False
        for button in current_markup_elements[markup_page - 1].keyboard:
            if button[0].callback_data == element_index:
                wasChanged = True
                button[0].text += "✅"
                break

        if not wasChanged:
            for key in current_markup_elements:
                for button in key.keyboard:
                    if button[0].callback_data == element_index:
                        wasChanged = True
                        button[0].text += "✅"
                        break

        if wasChanged:
            markup = assemble_markup(markup_page, current_markup_elements, 0)
            bot.edit_message_reply_markup(chat_id=userId, reply_markup=markup,
                                               message_id=messageId)
    except:
        return None


def remove_tick_from_element(bot, userId, messageId, current_markup_elements, markup_page, element_index):
    try:
        wasChanged = False
        for button in current_markup_elements[markup_page - 1].keyboard:
            if button[0].callback_data == element_index:
                wasChanged = True
                button[0].text = button[0].text.replace("✅", "")
                break

        if not wasChanged:
            for key in current_markup_elements:
                for button in key.keyboard:
                    if button[0].callback_data == element_index:
                        wasChanged = True
                        button[0].text = button[0].text.replace("✅", "")
                        break

        if wasChanged:
            markup = assemble_markup(markup_page, current_markup_elements, 0)
            bot.edit_message_reply_markup(chat_id=userId, reply_markup=markup,
                                               message_id=messageId)
    except:
        return None
