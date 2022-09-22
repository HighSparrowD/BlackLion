import json

import Common.Menues as Coms
import Core.HelpersMethodes as Helpers
import requests
from telebot.types import ReplyKeyboardMarkup, KeyboardButton, InlineKeyboardMarkup, InlineKeyboardButton
from Sponsor_Handler import SponsorHandler

class SponsorHandlerAdmin:
    def __init__(self, bot, message, sponsor_handlers):
        self.bot = bot
        self.current_user = message.from_user.id
        Helpers.switch_user_busy_status(self.current_user)
        self.sponsor_handlers = sponsor_handlers
        self.sponsor_handlers.append(self)

        self.chCode = None
        self.mhCode = None

        self.old_queries = []

        self.data = {
            0: "/register - Register new awaiting user",
            1: "/viewsponsors"
        }
        self.dat = {}

        self.admin_greet_message = "Hello! Admin access granted. List of admin commands:\n/showstatus\n/switchstatus"
        self.admin_warning_message = "\nWARNING!\n This section id NOT debugged, please input only numbers, or registration will break!!!"
        self.admin_laugh_message = "Well, sure i have debugged this section. Did you really think that...? Do not mind, just continue to the previous step"
        self.checkout_message = "Wanna change smth?"
        self.registration_success_message = "User had been successfully registered. What now, your majesty?"
        self.upload_success_message = "Sponsors data had been successfully updated"
        self.delete_success_message = "Sponsor had been successfully deleted"

        self.markup1 = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(KeyboardButton("/register"), KeyboardButton("/viewsponsors"), KeyboardButton("/switchstatus"), KeyboardButton("/showstatus"), KeyboardButton("/exit"))
        self.markup3 = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(KeyboardButton("Ok"), KeyboardButton("Change username"), KeyboardButton("Change ad count"), KeyboardButton("Change view count"), KeyboardButton("Change code word"), KeyboardButton("Delete Sponsor"))
        self.markupYN = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(KeyboardButton("Yes"), KeyboardButton("No"))

        self.bot.send_message(self.current_user, self.admin_greet_message, reply_markup=self.markup1)
        self.chCode = self.bot.register_callback_query_handler("", self.admin_callback_handler, user_id=self.current_user)
        self.eh = self.bot.register_message_handler(self.admin_exit_handler, user_id=self.current_user, commands=["exit"])
        self.mhCode = self.bot.register_message_handler(self.admin_message_handler, user_id=self.current_user, commands=["register", "viewsponsors", "switchstatus"])

    def admin_message_handler(self, message):
        if message.text == "/register":
            self.dat.clear()
            self.bot.send_message(self.current_user, "Gimme user name:")
            self.bot.register_next_step_handler(message, self.ultimate_registration1, chat_id=self.current_user)

        elif message.text == "/viewsponsors":
            sponsors = json.loads(requests.get("https://localhost:44381/GetSponsors", verify=False).text)

            if not sponsors:
                self.bot.send_message(self.current_user, "There are no sponsors to handle", reply_markup=self.markup1)
                return False

            markup = InlineKeyboardMarkup()

            for sponsor in sponsors:
                username = self.pretiffy_username(sponsor)

                markup.add(InlineKeyboardButton(username, callback_data=sponsor["id"]))

            self.bot.send_message(self.current_user, "Here you go", reply_markup=markup)
        elif message.text == "/switchstatus":
            self.bot.send_message(self.current_user, "Admin access disabled\nSwitching to an ordinary sponsor module")
            self.destruct()
            SponsorHandler(self.bot, message, self.sponsor_handlers)
        elif message.text == "/exit":
            self.bot.send_message(self.current_user, "Admin access disabled\nSwitching to an ordinary sponsor module")
            SponsorHandler(self.bot, message, self.sponsor_handlers)
            self.destruct()

    def admin_callback_handler(self, call):
        if call.message.id not in self.old_queries:
            self.old_queries.append(call.message.id)
            self.dat.clear()
            self.dat = json.loads(requests.get(f"https://localhost:44381/GetSponsorAsync/{call.data}", verify=False).text)
            self.bot.send_message(self.current_user, "What do you want to do with this user?", reply_markup=self.markup3)
            self.bot.send_message(self.current_user, self.configure_sponsor_data())
            self.bot.register_next_step_handler(call.message, self.ultimate_registration_checkout, cb=True, chat_id=self.current_user)

    def admin_exit_handler(self, message):
        self.destruct()

    def ultimate_registration1(self, message, incorrect=False, revisit=False, cb=False):
        if not message.text:
            self.bot.send_message(self.current_user, "Gimme user name:")
            self.bot.register_next_step_handler(message, self.ultimate_registration1, chat_id=self.current_user)
            return False

        if not incorrect:
            self.dat["username"] = message.text

        if revisit:
            self.bot.send_message(self.current_user, "Changed!", reply_markup=self.markup3)
            self.bot.register_next_step_handler(message, self.ultimate_registration_checkout, cb=cb, chat_id=self.current_user)
            return revisit

        self.bot.send_message(message.from_user.id, f"Tell me the codeword user will use to register")
        self.bot.register_next_step_handler(message, self.ultimate_registration_codeword_step, incorrect=False, revisit=revisit, cb=cb, chat_id=self.current_user)

    def ultimate_registration_codeword_step(self, message, incorrect=False, revisit=False, cb=False):
        if not message.text:
            self.bot.send_message(self.current_user, "Tell me the codeword user will use to register")
            self.bot.register_next_step_handler(message, self.ultimate_registration1, chat_id=self.current_user)
            return False

        if not incorrect:
            self.dat["codeWord"] = message.text.strip()

        if revisit:
            self.bot.send_message(self.current_user, "Changed!", reply_markup=self.markup3)
            self.bot.register_next_step_handler(message, self.ultimate_registration_checkout, cb=cb,
                                                chat_id=self.current_user)
            return revisit

        self.bot.send_message(message.from_user.id, f"Tell me how many ads he will be able to host?:{self.admin_warning_message}")
        self.bot.register_next_step_handler(message, self.ultimate_registration2, incorrect=False, revisit=revisit, cb=cb,
                                            chat_id=self.current_user)

    def ultimate_registration2(self, message, incorrect=False, revisit=False, cb=False):
        try:
            if not incorrect:
                self.dat["userMaxAdCount"] = int(message.text)
            if revisit or cb:
                self.bot.send_message(self.current_user, "Changed!", reply_markup=self.markup3)
                self.bot.register_next_step_handler(message, self.ultimate_registration_checkout, cb=cb, chat_id=self.current_user)
                return revisit

            self.bot.send_message(message.from_user.id, f"Tell me how many times will the other users be able to see the ad?:{self.admin_warning_message}")
            self.bot.register_next_step_handler(message, self.ultimate_registration3, chat_id=self.current_user)

        except:
            self.bot.send_message(self.current_user, self.admin_laugh_message)
            self.bot.send_message(message.from_user.id,
                                    f"Tell me how many ads he will be able to host?:{self.admin_warning_message}")
            self.bot.register_next_step_handler(message, self.ultimate_registration2, incorrect=False, revisit=False, cb=cb, chat_id=self.current_user)
            return cb

    def ultimate_registration3(self, message, cb=False):
        try:
            self.dat["userMaxAdViewCount"] = int(message.text)
            self.bot.send_message(self.current_user, self.checkout_message, reply_markup=self.markup3)
            self.bot.send_message(self.current_user, self.configure_sponsor_data())
            self.bot.register_next_step_handler(message, self.ultimate_registration_checkout, cb=cb, chat_id=self.current_user)
        except:
            self.bot.send_message(self.current_user, self.admin_laugh_message)
            self.bot.send_message(message.from_user.id,
                                    f"Tell me how many times will the other users be able to see the ad?:{self.admin_warning_message}")
            self.bot.register_next_step_handler(message, self.ultimate_registration3, cb=cb, chat_id=self.current_user)
            return cb

    def ultimate_registration_delete(self, message, cb=False):
        if message.text == "Yes":
            if cb:
                self.delete_sponsor()
                return False
            self.bot.send_message(self.current_user, self.delete_success_message, self.markup1)
            return True #TODO: Implement exit command and use it here
            # (If it is not cb => it is just registration and sponsor is not in database, thus there is no need to call an API)
        elif message.text == "No":
            self.bot.send_message(self.current_user, self.checkout_message)
            self.bot.send_message(self.current_user, self.configure_sponsor_data())
            self.bot.register_next_step_handler(message, self.ultimate_registration_checkout, cb=cb, chat_id=self.current_user)

    def ultimate_registration_checkout(self, message, cb=False):
        if message.text == "Ok":
            if cb:
                self.update_sponsor()
                return cb
            self.register_sponsor()
        elif message.text == "Change username":
            self.bot.send_message(self.current_user, "Gimme user name:")
            self.bot.register_next_step_handler(message, self.ultimate_registration1, incorrect=False, revisit=True, cb=cb, chat_id=self.current_user)
        elif message.text == "Change ad count":
            self.bot.send_message(self.current_user, f"Tell me how many ads he will be able to host?:{self.admin_warning_message}")
            self.bot.register_next_step_handler(message, self.ultimate_registration2, incorrect=False, revisit=True, cb=cb, chat_id=self.current_user)
        elif message.text == "Change view count":
            self.bot.send_message(message.from_user.id, f"Tell me how many times will the other users be able to see the ad?:{self.admin_warning_message}")
            self.bot.register_next_step_handler(message, self.ultimate_registration3, cb=cb, chat_id=self.current_user)
        elif message.text == "Change code word":
            self.bot.send_message(message.from_user.id, f"Tell me a codeword user will use to register")
            self.bot.register_next_step_handler(message, self.ultimate_registration_codeword_step, revisit=True, cb=cb, chat_id=self.current_user)
        elif message.text == "Delete Sponsor":
            self.bot.send_message(message.from_user.id, f"Are you sure you want to delete sponsor?:{self.admin_warning_message}", reply_markup=self.markupYN)
            self.bot.register_next_step_handler(message, self.ultimate_registration_delete, cb=cb, chat_id=self.current_user)
        else:
            self.bot.send_message(self.current_user, "No such option", reply_markup=self.markup3)
            self.bot.register_next_step_handler(message, self.ultimate_registration_checkout, chat_id=self.current_user)

    def register_sponsor(self):
        d = json.dumps(self.dat)

        requests.post("https://localhost:44381/RegisterAwaitingUser", d, headers={
            "Content-Type": "application/json"}, verify=False)
        self.bot.send_message(self.current_user, self.registration_success_message, reply_markup=self.markup1)

    def delete_sponsor(self):
        sponsorId = self.dat["id"]
        requests.delete(f"https://localhost:44381/RemoveSponsor/{sponsorId}", verify=False)
        self.bot.send_message(self.current_user, self.delete_success_message, reply_markup=self.markup1)

    def update_sponsor(self):
        d = json.dumps(self.dat)

        requests.post("https://localhost:44381/SponsorUpdate", d, headers={
            "Content-Type": "application/json"}, verify=False)
        self.bot.send_message(self.current_user, self.upload_success_message, reply_markup=self.markup1)

    def configure_sponsor_data(self):
        data = ""
        if "id" in self.dat:
            data += f"UserId: {self.dat['id']}\n"
        if "username" in self.dat:
            data += f"Username: {self.dat['username']}\n"
        if "userMaxAdCount" in self.dat:
            data += f"UserMaxAdCount: {self.dat['userMaxAdCount']}\n"
        if "userMaxAdViewCount" in self.dat:
            data += f"UserMaxAdViewCount: {self.dat['userMaxAdViewCount']}\n"
        if "isPostponed" in self.dat:
            data += f"IsPostponed: {self.dat['isPostponed']}\n"
        if "isAwaiting" in self.dat:
            data += f"IsAwaiting: {self.dat['isAwaiting']}\n"
        return data

    def pretiffy_username(self, sponsor):
        if sponsor["isAwaiting"]:
            return f"{sponsor['username']} ❓"
        elif sponsor["isPostponed"]:
            return f"{sponsor['username']} ❗️"
        return f"{sponsor['username']} ✅"

    def destruct(self):
        self.bot.callback_query_handlers.remove(self.chCode)
        self.bot.message_handlers.remove(self.mhCode)
        self.bot.message_handlers.remove(self.eh)
        self.sponsor_handlers.remove(self)
        Coms.show_admin_markup(self.bot, self.current_user)
        Helpers.switch_user_busy_status(self.current_user)
        del self

