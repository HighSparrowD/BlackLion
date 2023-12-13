from telebot.types import InlineKeyboardMarkup, InlineKeyboardButton, ReplyKeyboardMarkup
from Core import HelpersMethodes as Helpers
from Common.Menues import count_pages, assemble_markup, reset_pages, add_tick_to_element, remove_tick_from_element, index_converter
import requests
import json

from Models import Advertisement as models
from Common.Menues import go_back_to_main_menu
from Helper import Helper
from ReportModule import ReportModule
from Settings import Settings
from Enums.AttendeeStatus import AttendeeStatus

class AdvertisementModule:
    def __init__(self, bot, message, hasVisited=False):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id
        self.user_info = Helpers.get_user_info(self.current_user)
        self.hasVisited = hasVisited
        self.user_language = self.user_info["language"]


