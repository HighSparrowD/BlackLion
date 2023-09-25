from telebot.types import ReplyKeyboardMarkup, KeyboardButton, InlineKeyboardButton, InlineKeyboardMarkup

menu_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True) \
    .add(KeyboardButton("/search"),
         KeyboardButton("/random"),
         KeyboardButton("/feedback"),
         KeyboardButton("/settings"),
         KeyboardButton("/adventure"),
         KeyboardButton("/shop"))

admin_menu_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True) \
    .add(KeyboardButton("/switchstatus"),
         KeyboardButton("/showstatus"),
         KeyboardButton("/enteradmincabinet"))

register_markup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True) \
    .add(KeyboardButton("/registration"))
