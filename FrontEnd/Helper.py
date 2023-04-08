from telebot.types import InlineKeyboardMarkup, InlineKeyboardButton
from Common import Menues as menues


class Helper:
    def __init__(self, bot, message, return_method=None, editMode=None, isEncounter=None, activeMessageId=None, secondaryMessageId=None):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id
        self.return_method = return_method
        self.editMode = editMode
        self.isEncounter = isEncounter

        self.shouldCleanUp = not activeMessageId

        self.active_message_id = activeMessageId
        self.secondary_message_id = secondaryMessageId

        self.functionality_Markup = InlineKeyboardMarkup()\
            .add(InlineKeyboardButton("❓PERSONALITY❓", callback_data="1001"))\
            .add(InlineKeyboardButton("❓PERSONALITY points❓", callback_data="1002"))\
            .add(InlineKeyboardButton("❓Achievements❓", callback_data="1003"))\
            .add(InlineKeyboardButton("❓Search by interests❓", callback_data="1004"))\
            .add(InlineKeyboardButton("❓Auto reply❓", callback_data="1005"))\
            .add(InlineKeyboardButton("❓Coins❓", callback_data="1006"))\
            .add(InlineKeyboardButton("❓Second chance❓", callback_data="1007"))\
            .add(InlineKeyboardButton("❓Detector❓", callback_data="1008"))\
            .add(InlineKeyboardButton("❓Nullifier❓", callback_data="1009"))\
            .add(InlineKeyboardButton("❓Card Dec Mini❓", callback_data="1010"))\
            .add(InlineKeyboardButton("❓Card Dec Platinum❓", callback_data="1011"))\
            .add(InlineKeyboardButton("❓'Increased familiarity'❓", callback_data="1012"))\
            .add(InlineKeyboardButton("Go Back", callback_data="-10"))\

        self.current_callback_handler = self.bot.register_callback_query_handler("", self.callback_handler, user_id=self.current_user)
        self.start()

    def start(self):
        self.send_active_message("<b>What is it you need help with ?</b>", markup=self.functionality_Markup)
        pass

    # TODO: probably rewrite to follow pattern like that: send... Dict[call.data]
    def callback_handler(self, call):
        self.bot.answer_callback_query(call.id, "")
        if call.data == "1001":
            self.send_secondary_message("LALALa")
        elif call.data == "1002":
            self.send_secondary_message("LA La")
        elif call.data == "-10":
            self.destruct()

    def send_active_message(self, text, markup=None):
        try:
            if self.active_message_id:
                self.bot.edit_message_text(text, self.current_user, self.active_message_id, reply_markup=markup)
                return
            self.active_message_id = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except:
            pass

    def send_secondary_message(self, text):
        try:
            if self.secondary_message_id:
                self.bot.edit_message_text(text, self.current_user, self.secondary_message_id)
                return
            self.secondary_message_id = self.bot.send_message(self.current_user, text).id
        except:
            pass

    def destruct(self):
        if self.current_callback_handler:
            self.bot.callback_query_handlers.remove(self.current_callback_handler)

        if self.shouldCleanUp:
            try:
                self.bot.delete_message(self.current_user, self.active_message_id)
            except:
                pass

        if self.secondary_message_id:
            try:
                self.bot.delete_message(self.current_user, self.secondary_message_id)
            except:
                pass

        # Simultaneously means, that return method is present
        if self.editMode is not None:
            self.return_method(self.message, editMode=self.editMode)
            return

        # Simultaneously means, that return method is present
        elif self.isEncounter is not None:
            self.return_method(self.message, isEncountered=self.isEncounter)
            return

        if self.return_method:
            self.return_method(self.message)
            return

        try:
            self.bot.delete_message(self.current_user, self.active_message_id)
        except:
            pass

        menues.go_back_to_main_menu(self.bot, self.current_user, self.message, shouldSwitch=False)
        return

