import json
import requests
import telegram
import Core.HelpersMethodes as Helpers
import Common.Menues as Coms
from telebot.types import InlineKeyboardButton, InlineKeyboardMarkup, ReplyKeyboardMarkup, KeyboardButton


class AdminCabinet:
    def __init__(self, bot, message, admin_cabinets):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id
        Helpers.switch_user_busy_status(self.current_user)
        self.admin_greet_message = "Hello, dear admin. Commencing brief command description:\n\n/sendmessage is used to send a message to user by id. It should be consisting of 3 parts -> /sendmessage USERID MESSAGE. All should be written throw a space separator" \
                                   "\n\n/viewreports command is used to view all recent reports IF specified like that: /vievreports USERID -> retrieves recent reports about specific user" \
                                   "\n\n/getuserbyid command allows you to get and then manage user by his id: /getuserbyid USERID" \
                                   "\n\n/getuserbyusername command allows you to get and then manage user by his username: /getuserbyusername USERNAME" \
                                   "\n\n/managetickrequests command allows you to go through active tick requests and resolve them. FUN!" \
                                   "\n\n/recentfeedbacks command gives you recent feedbacks" \
                                   "\n\n/bannedusers command gives you users banned automatically" \
                                   "\n\n/exit"

        self.admin_cabinets = admin_cabinets
        self.admin_cabinets.append(self)
        self.current_request = None

        #self.start_markup = ReplyKeyboardMarkup(KeyboardButton(""))
        self.markup1 = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True)
        self.feedbacks_markup = InlineKeyboardMarkup()
        self.YNmarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(KeyboardButton("Yes"), KeyboardButton("No"))
        self.SilentMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(KeyboardButton("Leave empty"))
        self.YNExitmarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add(KeyboardButton("Yes"), KeyboardButton("No"), KeyboardButton("Exit"))

        self.isCheckout = False

        self.current_message = 0
        self.current_managed_user = 0

        self.request_data = {}

        self.markup1.add(KeyboardButton("Grant an achievement"),
                                           KeyboardButton("View all reports on this user"),
                                           KeyboardButton("Ban user"),
                                           KeyboardButton("Unban user"),
                                           KeyboardButton("View all user's feedbacks"),
                                           KeyboardButton("View all user's reports"),
                                           KeyboardButton("Delete user"),
                                           KeyboardButton("Top-up balance"),
                                           KeyboardButton("Go Back"))

        self.old_queries = []
        self.current_query = 0

        self.banned_users = []

        self.manage_user_message = "What do you want to do with this user?"

        self.ch = self.bot.register_callback_query_handler("", self.callback_handler, user_id=self.current_user)

        self.mh = bot.register_message_handler(self.message_handler, commands=["sendmessage", "viewreports", "getuserbyid", "getuserbyusername", "managetickrequests", "bannedusers", "recentfeedbacks"], user_id=self.current_user)
        self.eh = bot.register_message_handler(self.exit_handler, commands=["exit"], user_id=self.current_user)

        self.current_section = self.start

        self.start()

    def start(self):
        self.bot.send_message(self.current_user, self.admin_greet_message)
        self.bot.send_message(self.current_user, self.show_new_data_count())

    def show_new_data_count(self):
        return requests.get(f"https://localhost:44381/GetNewNotificationsCount/{self.current_user}", verify=False).text

    def tick_request_handler(self, message, acceptMode=False):
        if not acceptMode:
            requestText = requests.get(f"https://localhost:44381/GetTickRequest", verify=False).text
            if requestText:
                self.current_request = json.loads(requestText)

                try:
                    media = json.loads(requests.get(f"https://localhost:44381/get-user-media/{self.current_request['userId']}", verify=False).text)

                    if media["isPhoto"]:
                        self.bot.send_photo(self.current_user, media["media"], "That is how the user looks like")
                    else:
                        self.bot.send_video(self.current_user, video=media["media"], caption="That is how the user looks like")

                    if self.current_request["video"]:
                        self.bot.send_video(self.current_user, video=self.current_request["video"], caption="That is the message.")
                    elif self.current_request["circle"]:
                        self.bot.send_message(self.current_user, "That is the message.")
                        self.bot.send_video_note(self.current_user, data=self.current_request["circle"])
                    elif self.current_request["photo"]:
                        self.bot.send_message(self.current_user, "That is the message. IT MUST CONTAIN PASSPORT DATA.")
                        self.bot.send_photo(self.current_user, photo=self.current_request["photo"])

                    if self.current_request["gesture"]:
                        self.bot.send_message(self.current_user, f"That is a gesture this user had to send: \n{self.current_request['gesture']}")

                    self.bot.send_message(self.current_user, f"Would you like to grant the user identity confirmation type {self.current_request['type']} ?", reply_markup=self.YNExitmarkup)
                    self.bot.register_next_step_handler(message, self.tick_request_handler, acceptMode=True, chat_id=self.current_user)
                except:
                    #Proceed if we were unable to read user photo
                    response = requests.get(f"https://localhost:44381/NotifyFailierTickRequest/{self.current_request['id']}/{self.current_user}", verify=False).text
                    self.bot.send_message(self.current_user, "Unable to load users media")
                    self.tick_request_handler(message)
                    return False
            else:
                self.bot.send_message(self.current_user, "No more requests to handle :)")
                self.bot.send_message(self.current_user, self.admin_greet_message)
        else:
            data = {}

            isResolved = False
            if message.text == "Yes":
                isResolved = True
            elif message.text == "Exit":
                response = requests.get(f"https://localhost:44381/AbortTickRequest/{self.current_request['id']}", verify=False).text
                self.bot.send_message(self.current_user, self.admin_greet_message)
                return False
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.YNExitmarkup)
                self.bot.register_next_step_handler(message, self.tick_request_handler, acceptMode=acceptMode, chat_id=self.current_user)
                return False

            self.request_data["adminId"] = self.current_user
            self.request_data["id"] = self.current_request['id']
            self.request_data["isAccepted"] = isResolved


    def comment_request(self, message, acceptMode=False):
        if not acceptMode:
            self.bot.send_message(self.current_user, "Comment your decide. This comment will be shown to user, so, please, dont be to honest ;)", reply_markup=self.SilentMarkup)
            self.bot.register_next_step_handler(message, self.comment_request, acceptMode=True, chat_id=self.current_user)
        else:
            if message != "Leave empty":
                self.request_data["comment"] = message.text
                pass

            jdata = json.dumps(self.request_data)

            self.request_data = {}
            response = requests.post(f"https://localhost:44381/ResolveTickRequest", jdata,
                                     headers={"Content-Type": "application/json"},
                                     verify=False).text

            self.tick_request_handler(message)

    def message_handler(self, message):
        if not self.isCheckout:
            command = self.split_command(message.text)
            self.current_message = 0

            if "/sendmessage" in command:
                try:
                    if len(command) >= 3:
                        msg = self.construct_message(command)
                        self.bot.send_message(command[1], f"✨You have a new message from the administration ✨\n\n{msg}")
                        self.bot.send_message(self.current_user, f"Message had been sent to user {command[1]}")
                    else:
                        self.bot.send_message(self.current_user, "Wrong command consistence")
                except:
                    self.bot.send_message(self.current_user, "User was not found")

            elif "/getuserbyid" in command:
                if len(command) == 2:
                    try:
                        user = json.loads(requests.get(f"https://localhost:44381/UserInfo/{command[1]}", verify=False).text)
                        self.manage_user(user)
                    except:
                        self.bot.send_message(self.current_user, "User was not found")
                else:
                    self.bot.send_message(self.current_user, "Wrong command consistence")

            elif "/getuserbyusername" in command:
                if len(command) == 2:
                    try:
                        user = json.loads(requests.get(f"https://localhost:44381/GetUserByUsername/{command[1]}", verify=False).text)
                        self.manage_user(user)
                    except:
                        self.bot.send_message(self.current_user, "User was not found")
                else:
                    self.bot.send_message(self.current_user, "Wrong command consistence")
            elif "/recentfeedbacks" in command:
                self.get_recent_feedbacks()
            elif "/bannedusers" in command:
                self.get_banned_users()
            elif command[0] == "/managetickrequests":
                self.tick_request_handler(message)

    def exit_handler(self, message):
        self.destruct()

    def manage_user(self, user):
        if user:
            self.current_managed_user = user['userId']
            self.bot.send_message(self.current_user, self.manage_user_message)
            self.show_user(user)
        else:
            self.bot.send_message(self.current_user, "User was not found")
            self.current_section()

    def get_recent_feedbacks(self):
        self.feedbacks_markup = InlineKeyboardMarkup()
        data = json.loads(requests.get("https://localhost:44381/GetRecentFeedbacks", verify=False).text)
        if data:
            for d in data:
                self.feedbacks_markup.add(InlineKeyboardButton(f"{d['user']['userRealName']} -> {d['reasonId']}", callback_data=f"{d['id']}"))

            self.bot.send_message(self.current_user, "Here are some recent feedbacks about us", reply_markup=self.feedbacks_markup)
            return False

        self.bot.send_message(self.current_user, "No recent feedbacks !")

    def get_banned_users(self):
        try:
            self.banned_users = json.loads(requests.get(f"https://localhost:44381/banned-users", verify=False).text)
            self.manage_banned_users(self.message)
        except:
            self.bot.send_message(self.current_user, "Something went wrong")
            self.start()

    def manage_banned_users(self, message, acceptMode=False):
        if len(self.banned_users) > 0:
            if not acceptMode:
                self.bot.send_message(self.current_user, "Proceed?", reply_markup=self.YNmarkup)
                self.bot.register_next_step_handler(message, self.manage_banned_users, acceptMode=True, chat_id=self.current_user)
            else:
                if message.text == "Yes":
                    self.current_section = self.manage_banned_users
                    user = Helpers.get_user_info(self.banned_users[0])
                    self.manage_user(user)
                else:
                    self.start()
        else:
            self.start()


    def callback_handler(self, call):
        if int(call.data) == -4:
            self.show_user_achievement_list()
        if not self.isCheckout:
            # if call.message.id not in self.old_queries:
            if call.message.text == "Here are some recent feedbacks about us":
                self.bot.answer_callback_query(call.id)
                self.show_feedback(call.data)
            elif call.message.text == "Here are your reports":
                self.bot.answer_callback_query(call.id)
                self.show_report(call.data)
            elif call.message.text == "Here are your feedback":
                self.bot.answer_callback_query(call.id)
                self.show_feedback(call.data)
            elif call.message.text == "Choose an achievement":
                self.bot.answer_callback_query(call.id)
                self.show_achievement(call)

    def top_up_balance_step1(self, message):
        self.bot.send_message(self.current_user, "How many points do you want to add to user's balance?\n\n(If you want to remove certain amount of points - input number with minus. For instance: -5, -10, -100)")
        self.bot.register_next_step_handler(message, self.top_up_balance_step2, chat_id=self.current_user)

    def top_up_balance_step2(self, message):
        try:
            Helpers.top_up_user_balance(self.current_managed_user, int(message.text), "Admin top-up")
            self.bot.send_message(self.current_user, "User balance had been successfully changed")
            user = json.loads(requests.get(f"https://localhost:44381/UserInfo/{self.current_managed_user}", verify=False).text)
            self.manage_user(user)
        except:
            self.bot.send_message(self.current_user, "Something went wrong. Please try inputting NUMBER again")
            self.bot.register_next_step_handler(message, self.top_up_balance_step2, chat_id=self.current_user)

    def grant_achievement_checkout(self, message, achId):
        self.isCheckout = True
        if message.text == "Yes":
            msg = self.grant_achievement(achId, self.current_managed_user)
            self.bot.send_message(self.current_managed_user, msg)
            self.bot.send_message(self.current_user, "Achievement was granted to user. He was notified ;-)\nYou can grant more achievements to user using a list above, or do something else using a list of commands", reply_markup=self.markup1)
            self.bot.register_next_step_handler(message, self.user_actions_checkout, chat_id=self.current_user)
            self.isCheckout = False
        elif message.text == "No":
            self.bot.send_message(self.current_user, "Ok", reply_markup=self.markup1)
            self.bot.register_next_step_handler(message, self.user_actions_checkout, chat_id=self.current_user)
            self.isCheckout = False


    def user_actions_checkout(self, message):
        if message.text == "View all reports on this user":
            reports = self.get_all_reports_on_user(self.current_managed_user)
            if reports:
                markup = InlineKeyboardMarkup()
                for report in reports:
                    markup.add(InlineKeyboardButton(report['userBaseInfoId']['username'], callback_data=report['id']))
                self.bot.send_message(self.current_user, "Here are your reports", reply_markup=markup)
            else:
                self.bot.send_message(self.current_user, "No one had reported this user so far. He is clean ;-)", reply_markup=self.markup1)
                self.bot.register_next_step_handler(message, self.user_actions_checkout, chat_id=self.current_user)

        elif message.text == "Ban user":
            if int(requests.get(f"https://localhost:44381/BanUser/{self.current_managed_user}",
                                verify=False).text) > 0:
                self.bot.send_message(self.current_user, "User had been successfully banned", reply_markup=self.markup1)
                self.bot.register_next_step_handler(message, self.user_actions_checkout, chat_id=self.current_user)
                self.bot.send_message(self.current_managed_user, "Hello! Your account had been banned by administration. Any further actions are forbidden. Please contact the administration to reestablish your account")
                return False
            self.bot.send_message(self.current_user, "Unable to perform this action. User is already banned.", reply_markup=self.markup1)

        elif message.text == "Unban user":
            if int(requests.get(f"https://localhost:44381/UnbanUser/{self.current_managed_user}",
                                verify=False).text) > 0:
                self.bot.send_message(self.current_user, "User had been successfully unbanned", reply_markup=self.markup1)
                self.bot.register_next_step_handler(message, self.user_actions_checkout, chat_id=self.current_user)
                self.bot.send_message(self.current_managed_user, "Hello! Good news. Your account had been unbanned. You can continue using all bot's functionalities again!")
                return False
            self.bot.send_message(self.current_user, "Unable to perform this action. User is not banned.", reply_markup=self.markup1)

        elif message.text == "Top-up balance":
            self.top_up_balance_step1(message)

        elif message.text == "View all user's feedbacks":
            feedbacks = self.get_all_user_feedbacks(self.current_managed_user)

            if feedbacks:
                markup = InlineKeyboardMarkup()
                for feedback in feedbacks:
                    markup.add(InlineKeyboardButton(feedback['reason']['description'], callback_data=feedback['id']))
                self.bot.send_message(self.current_user, "Here are your feedback", reply_markup=markup)
                self.bot.send_message(self.current_user, "Whats next?", reply_markup=self.markup1)
                self.bot.register_next_step_handler(message, self.user_actions_checkout, chat_id=self.current_user)
            else:
                self.bot.send_message(self.current_user, "This user had not written any message to us so far", reply_markup=self.markup1)
                self.bot.register_next_step_handler(message, self.user_actions_checkout, chat_id=self.current_user)

        elif message.text == "View all user's reports":
            reports = self.get_all_reports_made_by_user(self.current_managed_user)
            if reports:
                markup = InlineKeyboardMarkup()
                for report in reports:
                    markup.add(InlineKeyboardButton(report['user']['userName'], callback_data=report['id']))
                self.bot.send_message(self.current_user, "Here are your reports", reply_markup=markup)
                self.bot.send_message(self.current_user, "Whats next?", reply_markup=self.markup1)
                self.bot.register_next_step_handler(message, self.user_actions_checkout, chat_id=self.current_user)
                return False
            self.bot.send_message(self.current_user, "No reports were made by this user so far :)", reply_markup=self.markup1)
            self.bot.register_next_step_handler(message, self.user_actions_checkout, chat_id=self.current_user)

        elif message.text == "Delete user":
            if int(requests.get(f"https://localhost:44381/DeleteUser/{self.current_managed_user}", verify=False).text) > 0:
                self.bot.send_message(self.current_user, "User had been successfully deleted", reply_markup=self.markup1)
                self.bot.register_next_step_handler(message, self.user_actions_checkout, chat_id=self.current_user)
                self.bot.send_message(self.current_managed_user, "Hello! Your account had been deleted by administration. Any further actions are forbidden. Please contact the administration to reestablish your account")
                return False
            self.bot.send_message(self.current_user, "Something went wrong", reply_markup=self.markup1)
            self.bot.register_next_step_handler(message, self.user_actions_checkout, chat_id=self.current_user)

        elif message.text == "Grant an achievement":
            try:
                achievements = Helpers.get_all_user_achievements_admin(self.current_managed_user)
                if achievements:
                    markup = InlineKeyboardMarkup()
                    for ach in achievements:
                        markup.add(InlineKeyboardButton(ach['shortDescription'], callback_data=ach['achievementId']))
                    self.bot.send_message(self.current_user, "Choose an achievement", reply_markup=markup)
                    return False
                self.bot.send_message(self.current_user, "It appears, that user has already acquired all possible achievements\nNothing to grant :-)", reply_markup=self.markup1)
                self.bot.register_next_step_handler(message, self.user_actions_checkout, chat_id=self.current_user)
            except:
                self.bot.send_message(self.current_user, "Something went wrong")

        elif message.text == "Go Back":
            self.current_section()

    def show_user(self, user):
        try:
            markup = InlineKeyboardMarkup()\
                .add(InlineKeyboardButton("Achievements", callback_data=-4))

            self.bot.send_message(self.current_user, "Processing...", reply_markup=self.markup1, parse_mode=telegram.ParseMode.HTML)
            self.bot.send_message(self.current_user, self.construct_user_data_message(user), reply_markup=markup)
            self.bot.register_next_step_handler(self.message, self.user_actions_checkout, chat_id=self.current_user)
        except:
            self.bot.send_message(self.current_user, "Something went wrong")

    def show_user_achievement_list(self):
        achievements = Helpers.get_all_user_achievements(self.current_managed_user)

        if achievements:
            markup = InlineKeyboardMarkup()
            for ach in achievements:
                markup.add(InlineKeyboardButton(ach['shortDescription'], callback_data=ach['achievementId']))
            self.bot.send_message(self.current_user, "Here are some achievements, user have acquired:", reply_markup=markup)
            return False
        self.bot.send_message(self.current_user, "User has not acquired any achievements yet")

    def show_feedback(self, id):
        feedback = json.loads(requests.get(f"https://localhost:44381/GetFeedbackById/{id}", verify=False).text)

        try:
            if self.current_message <= 0:
                msg = self.bot.send_message(self.current_user, self.construct_feedback_message(feedback), parse_mode=telegram.ParseMode.HTML)
                self.current_message = msg.message_id
                return False
            self.bot.edit_message_text(self.construct_feedback_message(feedback), self.current_user, self.current_message, parse_mode=telegram.ParseMode.HTML)
        except:
            msg = self.bot.send_message(self.current_user, self.construct_feedback_message(feedback), parse_mode=telegram.ParseMode.HTML)
            self.current_message = msg.message_id

    def show_report(self, reportId):
        report = json.loads(requests.get(f"https://localhost:44381/GetSingleUserReportById/{reportId}", verify=False).text)

        try:
            if self.current_message <= 0:
                msg = self.bot.send_message(self.current_user, self.construct_user_report_message(report), parse_mode=telegram.ParseMode.HTML)
                self.current_message = msg.message_id
                return False
            self.bot.edit_message_text(self.construct_user_report_message(report), self.current_user, self.current_message, parse_mode=telegram.ParseMode.HTML)
        except:
            msg = self.bot.send_message(self.current_user, self.construct_user_report_message(report), parse_mode=telegram.ParseMode.HTML)
            self.current_message = msg.message_id

    def show_achievement(self, call):
        achievement = json.loads(requests.get(f"https://localhost:44381/GetSingleUserAchievement/{self.current_managed_user}/{call.data}", verify=False).text)

        try:
            if self.current_message <= 0:
                self.bot.send_message(self.current_user,
                                      "Are you sure, you want to grant this achievement to user? It cannot be undone!")
                msg = self.bot.send_message(self.current_user, self.construct_achievement_message(achievement),
                                            reply_markup=self.YNmarkup, parse_mode=telegram.ParseMode.HTML)
                self.current_message = msg.message_id
                self.current_message = self.bot.register_next_step_handler(call.message, self.grant_achievement_checkout,
                                                                           call.data, chat_id=self.current_user)
                return False
            self.bot.edit_message_text(self.construct_user_report_message(achievement), self.current_user, self.current_message, parse_mode=telegram.ParseMode.HTML)

        except:
            msg = self.bot.send_message(self.current_user, self.construct_achievement_message(achievement),
                                        reply_markup=self.YNmarkup, parse_mode=telegram.ParseMode.HTML)
            self.current_message = msg.message_id
            self.current_message = self.bot.register_next_step_handler(call.message, self.grant_achievement_checkout, call.data, chat_id=self.current_user)

    def construct_user_report_message(self, report):
        return f"<b>Report id: {report['id']}\nWas given by{'userBaseInfoId'},  @{report['sender']['userName']}\nReported person: {report['userBaseInfoId1']}, @{report['user']['userName']}\nWith a reason {report['reason']['description']}\n\nReport Text: {report['text']}</b>"

    def construct_user_data_message(self, user):
        isSponsor = Helpers.check_user_is_sponsor(user['userBaseInfoId'])
        b = user['userBaseInfo']
        d = user['userDataInfo']
        p = user['userPreferences']
        balance = Helpers.get_active_user_balance(user['userBaseInfoId'])

        info = f"<b>Id: \n{b['id']}\nUsername: {b['userName']}\nReal Name: {b['userRealName']}\nDescription: {b['userDescription']}\nIsSponsor: {isSponsor}\n\n</b>" #TODO: Lengthen message by using data from other extracted entities
        if balance:
            info += f"UserBalance: {balance['amount']}\nLastBalancePurchase: {balance['pointInTime']}"

        return info

    def get_all_reports_on_user(self, userId):
        return json.loads(requests.get(f"https://localhost:44381/GetAllReportsOnUser/{userId}", verify=False).text)

    def get_all_reports_made_by_user(self, userId):
        return json.loads(requests.get(f"https://localhost:44381/GetAllUserReports/{userId}", verify=False).text)

    def get_all_user_feedbacks(self, userId):
        return json.loads(requests.get(f"https://localhost:44381/GetUsersRecentFeedbacks/{userId}", verify=False).text)

    def grant_achievement(self, userId,  achievementId):
        return str(requests.get(f"https://localhost:44381/GrantAchievementToUser/{achievementId}/{userId}", verify=False).text)

    def construct_feedback_message(self, feedback):
        return f"<b style='font-family: Arial, Helvetica, sans-serif;'>Sender: {feedback['user']['userRealName']}\nSender Id: {feedback['userBaseInfoId']}\nWants to: {feedback['reason']['description']}\n\nAnd says: {feedback['text']}</b>"

    def construct_achievement_message(self, achievement):
        ach = achievement['achievement']
        return f"<b>Name: {ach['name']}\nDescription: {ach['description']}\nPoints for acquiring {ach['value']}p\nUser Progress {achievement['progress']} / {ach['conditionValue']}</b>"

    def split_command(self, command):
        return command.split(" ")

    def construct_message(self, command):
        message = ""
        for c in command[2:]:
            message += f"{c} "

        return message.strip()

    def destruct(self):
        Helpers.switch_user_busy_status(self.current_user)
        Coms.show_admin_markup(self.bot, self.current_user)
        self.bot.message_handlers.remove(self.mh)
        self.bot.callback_query_handlers.remove(self.ch)
        self.bot.message_handlers.remove(self.eh)
        self.admin_cabinets.remove(self)
        del self