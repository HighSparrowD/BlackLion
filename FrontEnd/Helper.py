from telebot.types import ReplyKeyboardMarkup


class Helper:
    def __init__(self, bot, message, return_method, editMode=None, isEncountered=None):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id
        self.return_method = return_method
        self.editMode = editMode
        self.isEncountered = isEncountered

        self.functionality_message = "1. ❓PERSONALITY\n2. ❓PERSONALITY points\n3. ❓Achievements\n4. ❓Search by interests\n5. ❓Auto reply\n6. ❓Coins\n7. ❓Second chance\n8.❓Detector\n9. ❓Nullifier\n10. ❓Card Dec Mini\n11. ❓Card Dec Platinum\n12. 'Increased familiarity'\n13. Go Back"
        self.startMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("1", "2", "3", "4", "5", "6", "7", "8", "9")

    def start(self, message, acceptMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, self.functionality_message, reply_markup=self.startMarkup)
        else:
            #TODO: check input and give description
            if message.text == "13":
                self.destruct()
                return
            elif message.text == "/help":
                self.bot.send_message(self.current_user, "Do you need a help with a... help???\nExit to the main menu, hit /feedback button and describe your problem to us. We will contact you as soon as possible ;)")
            self.start(message)

    def destruct(self):
        if self.editMode is not None:
            self.return_method(self.message, editMode=self.editMode)
            return

        elif self.isEncountered is not None:
            self.return_method(self.message, isEncountered=self.isEncountered)
            return

        self.return_method(self.message)
        return

