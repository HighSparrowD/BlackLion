import json
import random

import Common.Menues as Coms
import Core.HelpersMethodes as Helpers
import requests
from telebot.types import ReplyKeyboardMarkup, KeyboardButton, InlineKeyboardMarkup, InlineKeyboardButton

from Common.Menues import go_back_to_main_menu


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
        self.markup3 = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(KeyboardButton("Ok"), KeyboardButton("Change username"), KeyboardButton("Change ad count"), KeyboardButton("Change view count"), KeyboardButton("Delete Sponsor"))
        self.markupYN = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(KeyboardButton("Yes"), KeyboardButton("No"))

        self.bot.send_message(self.current_user, self.admin_greet_message, reply_markup=self.markup1)
        self.chCode = self.bot.register_callback_query_handler("", self.admin_callback_handler, user_id=self.current_user)
        self.eh = self.bot.register_message_handler(self.admin_exit_handler, user_id=self.current_user, commands=["exit"])
        self.mhCode = self.bot.register_message_handler(self.admin_message_handler, self.current_user, commands=["register", "viewsponsors"])

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
            self.bot.register_next_step_handler(call.message, self.ultimate_registration_checkout, True, chat_id=self.current_user)

    def admin_exit_handler(self):
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
            self.bot.register_next_step_handler(message, self.ultimate_registration_checkout, cb, chat_id=self.current_user)
            return revisit

        self.bot.send_message(message.from_user.id, f"Tell me how many ads he will be able to host?:{self.admin_warning_message}")
        self.bot.register_next_step_handler(message, self.ultimate_registration2, False, revisit, cb, chat_id=self.current_user)

    def ultimate_registration2(self, message, incorrect=False, revisit=False, cb=False):
        try:
            if not incorrect:
                self.dat["userMaxAdCount"] = int(message.text)
            if revisit or cb:
                self.bot.send_message(self.current_user, "Changed!", reply_markup=self.markup3)
                self.bot.register_next_step_handler(message, self.ultimate_registration_checkout, cb, chat_id=self.current_user)
                return revisit

            self.bot.send_message(message.from_user.id, f"Tell me how many times will the other users be able to see the ad?:{self.admin_warning_message}")
            self.bot.register_next_step_handler(message, self.ultimate_registration3, chat_id=self.current_user)

        except:
            self.bot.send_message(self.current_user, self.admin_laugh_message)
            self.bot.send_message(message.from_user.id,
                                    f"Tell me how many ads he will be able to host?:{self.admin_warning_message}")
            self.bot.register_next_step_handler(message, self.ultimate_registration2, False, False, cb, chat_id=self.current_user)
            return cb

    def ultimate_registration3(self, message, cb=False):
        try:
            self.dat["userMaxAdViewCount"] = int(message.text)
            self.bot.send_message(self.current_user, self.checkout_message, reply_markup=self.markup3)
            self.bot.send_message(self.current_user, self.configure_sponsor_data())
            self.bot.register_next_step_handler(message, self.ultimate_registration_checkout, cb, chat_id=self.current_user)
        except:
            self.bot.send_message(self.current_user, self.admin_laugh_message)
            self.bot.send_message(message.from_user.id,
                                    f"Tell me how many times will the other users be able to see the ad?:{self.admin_warning_message}")
            self.bot.register_next_step_handler(message, self.ultimate_registration3, cb, chat_id=self.current_user)
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
            self.bot.register_next_step_handler(message, self.ultimate_registration_checkout, cb, chat_id=self.current_user)

    def ultimate_registration_checkout(self, message, cb=False):
        if message.text == "Ok":
            if cb:
                self.update_sponsor()
                return cb
            self.register_sponsor()
        elif message.text == "Change username":
            self.bot.send_message(self.current_user, "Gimme user name:")
            self.bot.register_next_step_handler(message, self.ultimate_registration1, False, True, cb, chat_id=self.current_user)
        elif message.text == "Change ad count":
            self.bot.send_message(self.current_user, f"Tell me how many ads he will be able to host?:{self.admin_warning_message}")
            self.bot.register_next_step_handler(message, self.ultimate_registration2, False, True, cb, chat_id=self.current_user)
        elif message.text == "Change view count":
            self.bot.send_message(message.from_user.id, f"Tell me how many times will the other users be able to see the ad?:{self.admin_warning_message}")
            self.bot.register_next_step_handler(message, self.ultimate_registration3, cb, chat_id=self.current_user)
        elif message.text == "Delete Sponsor":
            self.bot.send_message(message.from_user.id, f"Are you sure you want to delete sponsor?:{self.admin_warning_message}", reply_markup=self.markupYN)
            self.bot.register_next_step_handler(message, self.ultimate_registration_delete, cb, chat_id=self.current_user)
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


class SponsorHandler:
    def __init__(self, bot, message, sponsor_handlers, hasVisited=True):
        self.bot = bot
        self.current_user = message.from_user.id
        Helpers.switch_user_busy_status(self.current_user)
        self.current_userName = message.from_user.username
        self.isSponsor = Helpers.check_user_is_sponsor(self.current_user)
        self.isAwaiting = Helpers.check_user_is_awaiting_by_username(self.current_userName)
        if not self.isAwaiting: #Can mean that user was not found by its id
            self.isAwaiting = Helpers.check_user_is_awaiting(self.current_user)
        self.isPostponed = Helpers.check_user_is_postponed(self.current_user)
        self.userMaxAdCount = 0
        self.userMaxAdViewCount = 0
        self.sponsor_handlers = sponsor_handlers
        self.sponsor_handlers.append(self)

        self.chCode = None
        self.mhCode = None

        self.dat = {}
        self.user_registration_data = {}
        self.ad_data = {"photo": "", "video": ""}

        self.old_queries = []

        self.markup_error_message = "No such option"
        self.frozen_warning_message = "\nWARNING!\n Your account has been frozen. It basically means, that your "
        self.registration_successful_message = "Congrats! Your registration has gone successfully"
        self.registration_interrupt_message = "Hey, you are already registered"
        self.permission_error_message = "You have no permission to use that command\nPlease, contact support team @GraphicGod"
        self.no_ads_message = "Hey! You have no ads yet...\nCreate your first ad using /createad command :-)"
        self.post_creation_start_message = "Please, input a text or description of an ad"
        self.post_creation_midd_message = "Please, send a photo video or a gif, you want users to receive"
        self.post_creation_show_message = "Here you go!\n This is how your ad will look like"
        self.post_creation_no_ad_text_message = "I have found no text here. Please try typing something again"
        self.post_creation_no_ad_data_message = "I cant see any media here! :-(, Are you sure, you want to show only text?"
        self.post_creation_success_message = "Ad had been created!"
        self.post_update_success_message = "Ad had been updated!"
        self.show_all_posts_message = "Here are all you ads! You can change them by clicking on a button with an ad description"
        self.action_list_message = ""
        self.command_list_message = "/myads - View your ads\n/createad - Create a new ad\n/exit - exit from this module"

        self.empty_markup = InlineKeyboardMarkup()
        self.markup1 = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(KeyboardButton("/register"))
        self.markup2 = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(KeyboardButton("/myads"), KeyboardButton("/createad"), KeyboardButton("/exit"))
        self.markupYN = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(KeyboardButton("Yes, text only"), KeyboardButton("No, i'll send media"))
        self.markup_delete = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(KeyboardButton("Yes"), KeyboardButton("No"))
        self.markup_ad_checkout = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(
            KeyboardButton("Ok"), KeyboardButton("Change Text"), KeyboardButton("Change Media"), KeyboardButton("Delete ad"))
        self.markup_create_ad = ReplyKeyboardMarkup(one_time_keyboard=True, resize_keyboard=True).add(KeyboardButton("/createad"))

        self.greet_message1 = "Hey! I see you are a new one here. Lets create your sponsor account!"
        self.greet_message2 = f"Welcome back dear sponsor. What are we to do this time? \n{self.action_list_message}"
        self.greet_message3 = f"Welcome back dear sponsor. Reminding you my list of commands :-) \n{self.command_list_message}"
        self.greets = [self.greet_message2, self.greet_message3]

        if self.isSponsor:
            if self.isPostponed:
                self.bot.send_message(self.current_user, self.frozen_warning_message)
            else:
                self.chCode = self.bot.register_callback_query_handler("", self.callback_handler, user_id=self.current_user)
                self.mhCode = self.bot.register_message_handler(self.message_handler, user_id=self.current_user, commands=["register", "myads", "createad"])
                self.eh = self.bot.register_message_handler(self.exit_handler, user_id=self.current_user, commands=["exit"])
                self.load_user_data()
                self.bot.send_message(self.current_user, random.choice(self.greets), reply_markup=self.markup2)

        else:
            self.bot.send_message(self.current_user, self.greet_message1, reply_markup=self.markup1)
            self.chCode = self.bot.register_callback_query_handler("", self.callback_handler, user_id=self.current_user)
            self.mhCode = self.bot.register_message_handler(self.message_handler, user_id=self.current_user, commands=["register", "myads", "createad"])
            self.eh = self.bot.register_message_handler(self.exit_handler, user_id=self.current_user, commands=["exit"])

            self.load_user_data()

    def load_user_data(self):
        if self.isAwaiting:
            data = json.loads(
                requests.get(f"https://localhost:44381/GetAwaitingUser/{self.current_userName}", verify=False).text)
            self.userMaxAdCount = data["userMaxAdCount"]
            self.userMaxAdViewCount = data["userMaxAdViewCount"]

        elif self.isSponsor:
            data = json.loads(
                requests.get(f"https://localhost:44381/GetSponsorAsync/{self.current_user}", verify=False).text)
            self.userMaxAdCount = data["userMaxAdCount"]
            self.userMaxAdViewCount = data["userMaxAdViewCount"]
            self.isPostponed = data["isPostponed"]

    def remove_ad(self):
        self.bot.send_message(self.current_user, "Successfully deleted", reply_markup=self.markup2)
        return json.loads(requests.delete(f"https://localhost:44381/RemoveAd/{self.ad_data['id']}/{self.current_user}", verify=False).text)

    def message_handler(self, message):
        if message.text == "/register":
            if self.isAwaiting:
                self.register_sponsor()
                self.bot.send_message(self.current_user, self.greet_message3)
            else:
                self.bot.send_message(self.current_user, self.permission_error_message)

        elif message.text == "/myads":
            data = json.loads(requests.get(f"https://localhost:44381/GetSponsorAds/{self.current_user}", verify=False).text)
            if data:
                for d in data:
                    self.empty_markup.add(InlineKeyboardButton(d["description"], callback_data=d["id"]))

                self.bot.send_message(self.current_user, self.show_all_posts_message, reply_markup=self.empty_markup)
                self.empty_markup = InlineKeyboardMarkup()
            else:
                self.bot.send_message(self.current_user, self.no_ads_message, reply_markup=self.markup_create_ad)

        elif message.text == "/createad":
            if self.isSponsor:
                if not Helpers.check_sponsor_is_maxed(self.current_user):
                    self.bot.send_message(self.current_user, self.post_creation_start_message)
                    self.bot.register_next_step_handler(message, self.ad_creating1, chat_id=self.current_user)
                else:
                    self.bot.send_message(self.current_user, "Sorry, you have reached your max ad limit. Please delete one of your ads, or contact administration to buy more", reply_markup=self.markup2)

            else:
                self.bot.send_message(self.current_user, self.permission_error_message)

    def exit_handler(self):
        self.bot.send_message(self.current_user, "Exiting module...")
        self.destruct()

    def redundant_ad_creating(self, message):
        self.bot.send_message(self.current_user, self.post_creation_start_message)
        self.bot.register_next_step_handler(message, self.ad_creating1, True, chat_id=self.current_user)

    def ad_creating1(self, message, revisit=False, cb=False):
        if message.text:
            if revisit:
                self.ad_data["text"] = message.text
                self.ad_creating_show(message, cb=cb)
                return revisit

            self.ad_data["SponsorId"] = self.current_user
            self.ad_data["text"] = message.text
            self.bot.send_message(self.current_user, self.post_creation_midd_message)

            self.bot.register_next_step_handler(message, self.ad_creating2, chat_id=self.current_user)

        else:
            self.bot.send_message(self.current_user, self.post_creation_no_ad_text_message)
            self.bot.register_next_step_handler(message, self.ad_creating1, chat_id=self.current_user)

    def ad_creating2(self, message, cb=False):
        if message.video:
            self.ad_data["photo"] = ""
            self.ad_data["video"] = message.video[len(message.video) - 1].file_id
        elif message.photo:
            self.ad_data["video"] = ""
            self.ad_data["photo"] = message.photo[len(message.photo) - 1].file_id
        self.ad_creating_show(message, cb=cb)

    def ad_creating_show(self, message, cb=False):
        if self.ad_data["photo"]:
            self.bot.send_message(self.current_user, self.post_creation_show_message, reply_markup=self.markup_ad_checkout)
            self.bot.send_photo(self.current_user, self.ad_data["photo"], self.ad_data["text"])
            self.bot.register_next_step_handler(message, self.ad_creating_checkout, cb, chat_id=self.current_user)
        elif self.ad_data["video"]:
            self.bot.send_message(self.current_user, self.post_creation_show_message, reply_markup=self.markup_ad_checkout)
            self.bot.send_video(self.current_user, self.ad_data["video"], self.ad_data["text"])
            self.bot.register_next_step_handler(message, self.ad_creating_checkout, cb, chat_id=self.current_user)
        else:
            self.bot.send_message(self.current_user, self.post_creation_no_ad_data_message)
            self.bot.register_next_step_handler(message, self.ad_creating2, chat_id=self.current_user)

    def ad_creating_checkout(self, message, cb=False):
        if message.text == "Ok":
            if cb:
                self.update_ad()
                return cb
            self.create_ad()
        elif message.text == "Change Text":
            self.bot.send_message(self.current_user, self.post_creation_start_message)
            self.bot.register_next_step_handler(message, self.ad_creating1, True, cb=cb, chat_id=self.current_user)
        elif message.text == "Change Media":
            self.bot.send_message(self.current_user, self.post_creation_midd_message)
            self.bot.register_next_step_handler(message, self.ad_creating2, cb=cb, chat_id=self.current_user)
        elif message.text == "Delete ad":
            self.bot.send_message(self.current_user, "Are you sure that you want to delete an ad?",
                                  reply_markup=self.markup_delete)
            self.bot.register_next_step_handler(message, self.delete_ad_step, cb, chat_id=self.current_user)
        else:
            self.bot.send_message(self.current_user, self.markup_error_message, reply_markup=self.markup_ad_checkout)
            self.bot.register_next_step_handler(message, self.ad_creating_checkout, cb=cb, chat_id=self.current_user)

    def delete_ad_step(self, message, cb):
        if message.text == "Yes":
            self.remove_ad()
        elif message.text == "No":
            self.bot.send_message(self.current_user, self.post_creation_show_message, reply_markup=self.markup_ad_checkout)
            self.bot.register_next_step_handler(message, self.ad_creating_checkout, cb, chat_id=self.current_user)
        else:
            self.bot.send_message(self.current_user, "No such option")
            self.bot.send_message(self.current_user, "Are you sure that you want to delete an ad?",
                                  reply_markup=self.markupYN)
            self.bot.register_next_step_handler(message, self.delete_ad_step, cb, chat_id=self.current_user)

    def callback_handler(self, call):
        if call.from_user.id == self.current_user:
            if call.message.id not in self.old_queries:
                self.old_queries.append(call.message.id)
                self.ad_data = json.loads(requests.get(f"https://localhost:44381/GetSponsorAd/{self.current_user}/{call.data}", verify=False).text)
                self.ad_creating_show(call.message, cb=True)


    def register_sponsor(self):
        if self.isSponsor:
            self.bot.send_message(self.current_user, self.registration_interrupt_message)
            return False

        self.user_registration_data = {"id": self.current_user, "userName": self.current_userName,
                                       "userMaxAdCount": self.userMaxAdCount,
                                       "userMaxAdViewCount": self.userMaxAdViewCount,
                                       "isPostponed": False,
                                       "UserAppLanguage": 0, #TODO: if user does not exist in system - ask for his language prefs in the beginning
                                       }

        d = json.dumps(self.user_registration_data)

        requests.post(f"https://localhost:44381/RegisterSponsor", d, headers={
            "Content-Type": "application/json"}, verify=False)
        self.isSponsor = Helpers.check_user_is_sponsor(self.current_user)
        self.isAwaiting = Helpers.check_user_is_awaiting(self.current_userName)
        self.bot.send_message(self.current_user, self.registration_successful_message)

    def update_ad(self):
        d = json.dumps(self.ad_data)
        requests.post(f"https://localhost:44381/AdUpdate", d, headers={
            "Content-Type": "application/json"}, verify=False)
        self.bot.send_message(self.current_user, self.post_update_success_message, reply_markup=self.markup2)

    def create_ad(self):
        self.ad_data["Description"] = ""
        d = json.dumps(self.ad_data)
        requests.post(f"https://localhost:44381/AdAdd", d, headers={
            "Content-Type": "application/json"}, verify=False)
        self.bot.send_message(self.current_user, self.post_creation_success_message, reply_markup=self.markup2)

    def destruct(self):
        self.bot.callback_query_handlers.remove(self.chCode)
        self.bot.message_handlers.remove(self.mhCode)
        self.bot.message_handlers.remove(self.eh)
        self.sponsor_handlers.remove(self)
        go_back_to_main_menu(self.bot, self.current_user)
        Helpers.switch_user_busy_status(self.current_user)
        del self
