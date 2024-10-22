import copy
from math import ceil
from typing import Union

from Core import HelpersMethodes as Helpers
from telebot.types import InlineKeyboardButton, InlineKeyboardMarkup

from Common.Keyboards import menu_markup, admin_menu_markup

from Requester import Requester


def go_back_to_main_menu(bot, user, message, shouldSwitch=True):
    response = None

    if shouldSwitch:
        response = Helpers.switch_user_busy_status(user, 14)

        response = Helpers.get_user_requests(user)
        request_list = response["requests"]

        if request_list:
            if message:
                Requester(bot, message, user, request_list, response["notification"])
            return False

    notification_list = Helpers.get_user_notifications(user)
    if notification_list:
        for notif in notification_list:  # TODO: Maybe create a separate module for handling that
            bot.send_message(notif["userId"], notif["description"])
            Helpers.delete_user_notification(notif["id"])
    else:
        if response and "comment" in response:
            bot.send_message(user, response["comment"])
    bot.send_message(user, "What are we doing next? 😊", reply_markup=menu_markup)


def show_admin_markup(bot, user):
    bot.send_message(user, "Sending you basic admin command set 😏", reply_markup=admin_menu_markup)


def start_program_in_debug_mode(userIds: list[str]):  # TODO: remove in production
    Helpers.start_program_in_debug_mode(userIds)


def count_pages(section_elements, current_markup_elements, markup_pages_count, prefs: bool = False,
                additional_buttons: Union[dict[str, str], None] = None):
    current_markup_elements.clear()
    section_elements = copy.copy(section_elements)
    markup = InlineKeyboardMarkup()
    i = 0
    element_count_on_page = 5
    elements_to_delete = []
    isLastElement = False

    count = ceil(len(section_elements) / 5)

    if prefs:
        markup.add(InlineKeyboardButton("⚡ Same as mine ⚡", callback_data=-5))
    for element in list(section_elements):
        if len(section_elements) > 5:
            element_count_on_page = 5
        else:
            element_count_on_page = len(section_elements)

        isLastElement = len(elements_to_delete) >= element_count_on_page - 1

        if len(markup.keyboard) < element_count_on_page and not isLastElement:
            markup.add(InlineKeyboardButton(section_elements[element].capitalize(), callback_data=f"{element}"))
            elements_to_delete.append(element)
        elif isLastElement:
            markup.add(InlineKeyboardButton(section_elements[element].capitalize(), callback_data=f"{element}"))
            elements_to_delete.append(element)

            i += 1
            markup.add(InlineKeyboardButton("<", callback_data="-1"),
                       # InlineKeyboardButton(f"{i} / {count}", callback_data=f"{i} / {count}"),
                       InlineKeyboardButton(f"{i} / {count}", callback_data=f"-3"),
                       InlineKeyboardButton(">", callback_data="-2"))

            if additional_buttons:
                for button in additional_buttons:
                    markup.add(InlineKeyboardButton(button, callback_data=additional_buttons[button]))

            current_markup_elements.append(copy.deepcopy(markup))
            markup_pages_count += 1
            markup.clear()
            for e in elements_to_delete:
                section_elements.pop(e)
            elements_to_delete.clear()

    return markup_pages_count


def assemble_markup(markup_page, current_markup_elements, index):
    if len(current_markup_elements) -1 >= (markup_page + index) - 1:
        return current_markup_elements[(markup_page + index) - 1]
    return current_markup_elements[-1]


def reset_pages(current_markup_elements, markup_last_element, markup_page, markup_pages_count):
    current_markup_elements.clear()
    markup_last_element = 0
    markup_page = 1
    markup_pages_count = 0


def paginate(current_markup_elements: list, markup_last_element: any, markup_page: int, markup_pages_count: int,
             section_elements: dict[str, str], index: int, prefs: bool = False,
                additional_buttons: Union[dict[str, str], None] = None):

    reset_pages(current_markup_elements, markup_last_element, markup_page, markup_pages_count)

    count_pages(section_elements, current_markup_elements, markup_pages_count, prefs=prefs,
                additional_buttons=additional_buttons)

    return assemble_markup(markup_page, current_markup_elements, index)


def index_converter(index):
    if index == "-1":
        return -1
    return 1


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


def add_tick_to_elements(bot, userId, messageId, current_markup_elements, markup_page, element_indexes):
    try:
        wasChanged = False
        for button in current_markup_elements[markup_page - 1].keyboard:
            if button[0].callback_data in element_indexes:
                wasChanged = True
                button[0].text += "✅"
                break

        if not wasChanged:
            for key in current_markup_elements:
                for button in key.keyboard:
                    if button[0].callback_data in element_indexes:
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


def remove_tick_from_elements(bot, userId, messageId, current_markup_elements, markup_page, element_indexes):
    try:
        wasChanged = False
        for button in current_markup_elements[markup_page - 1].keyboard:
            if button[0].callback_data in element_indexes:
                wasChanged = True
                button[0].text = button[0].text.replace("✅", "")
                break

        if not wasChanged:
            for key in current_markup_elements:
                for button in key.keyboard:
                    if button[0].callback_data in element_indexes:
                        wasChanged = True
                        button[0].text = button[0].text.replace("✅", "")
                        break

        if wasChanged:
            markup = assemble_markup(markup_page, current_markup_elements, 0)
            bot.edit_message_reply_markup(chat_id=userId, reply_markup=markup,
                                               message_id=messageId)
    except:
        return None
