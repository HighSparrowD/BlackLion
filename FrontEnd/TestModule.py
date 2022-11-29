import json

import requests
import Core.HelpersMethodes as Helpers
from telebot.types import ReplyKeyboardMarkup, InlineKeyboardButton, InlineKeyboardMarkup
from Common.Menues import count_pages, assemble_markup, reset_pages
from Common.Menues import go_back_to_main_menu


class TestModule:
    def __init__(self, bot, message, isActivatedFromShop=False, returnMethod=None):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id
        self.isActivatedFromShop = isActivatedFromShop
        self.isOnStart = True
        self.isDeciding = False
        self.isPassingTest = False
        self.is_about_to_leave = False
        self.is_about_handler_present = False
        self.returnMethod = returnMethod

        self.localisation = Helpers.get_user_app_language(self.current_user)

        self.current_markup_elements = []
        self.markup_last_element = 0
        self.markup_page = 1
        self.markup_pages_count = 0

        self.current_question_message = 0
        self.active_message_id = 0
        self.active_param = 0

        self.user_total = 0

        self.chCode = self.bot.register_callback_query_handler("", self.callback_handler, user_id=self.current_user)

        self.start_markup = InlineKeyboardMarkup().add(InlineKeyboardButton("Personality", callback_data="1"))\
                .add(InlineKeyboardButton("Emotional intellect", callback_data="2")) \
                .add(InlineKeyboardButton("Reliability", callback_data="3")) \
                .add(InlineKeyboardButton("Compassion", callback_data="4")) \
                .add(InlineKeyboardButton("Open-Mindedness", callback_data="5")) \
                .add(InlineKeyboardButton("Agreeableness", callback_data="6")) \
                .add(InlineKeyboardButton("Self-Awareness", callback_data="7")) \
                .add(InlineKeyboardButton("Levels of sense", callback_data="8")) \
                .add(InlineKeyboardButton("Intellect", callback_data="9")) \
                .add(InlineKeyboardButton("Nature", callback_data="10")) \
                .add(InlineKeyboardButton("Creativity", callback_data="11")) \

        self.YNmarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("Yes", "No")
        self.abortMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("/abort")
        self.ManageTestMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("1", "2")

        #TODO: Name the effect !
        self.manage_test_message = "1. Go Back\n2. Use an EFFECTNAME to re-pass this test again now"

        self.current_test_data = {}
        self.current_question = {}
        self.current_question_answers = {}
        self.tests = {}

        self.current_tests = None
        self.current_test = None

        Helpers.switch_user_busy_status(self.current_user)
        self.ah = self.bot.register_message_handler(self.abort_checkout, commands=["abort"], user_id=self.current_user)

        self.start()

    def start(self):
        self.active_message_id = self.bot.send_message(self.current_user, "Please choose the parameter, test will be sorted by", reply_markup=self.start_markup).id
        self.bot.send_message(self.current_user, "Type 'abort to leave'", reply_markup=self.abortMarkup)
        # self.is_about_to_leave = True
        # self.get_ready_to_abort(self.message)

    def manage_point_group(self):
        markup = self.get_markup()
        if markup:
            self.bot.edit_message_reply_markup(chat_id=self.current_user, reply_markup=markup, message_id=self.active_message_id)

    def get_markup(self):
        if self.isActivatedFromShop:
            self.load_new_tests_data_by_param(self.active_param)
        else:
            self.load_owned_tests_data_by_param(self.active_param)

        if not self.current_tests:
            self.bot.send_message(self.current_user, "No tests were found")
            return None

        self.isOnStart = False
        reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page, self.markup_pages_count)
        count_pages(self.tests, self.current_markup_elements, self.markup_pages_count, additionalButton=True, buttonText="Go Back", buttonData=-10)
        markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

        return markup

    def load_new_tests_data_by_param(self, param):
        try:
            self.current_tests = json.loads(requests.get(f"https://localhost:44381/GetTestDataByProperty/{self.current_user}/{param}", verify=False).text)
            self.tests.clear()
            for test in self.current_tests:
                self.tests[test["id"]] = test["name"]
        except:
            return None

    def load_owned_tests_data_by_param(self, param):
        try:
            self.current_tests = json.loads(requests.get(f"https://localhost:44381/GetUserTestDataByProperty/{self.current_user}/{param}", verify=False).text)
            self.tests.clear()
            for test in self.current_tests:
                self.tests[test["id"]] = test["name"]
        except:
            return None

    def show_current_test(self, message, acceptMode=False):
        # self.is_about_to_leave = False
        if not acceptMode:
            self.bot.send_message(self.current_user, self.get_current_test_data())
            if self.isActivatedFromShop:
                self.isDeciding = True
                self.bot.send_message(self.current_user, "Are you sure, you want to buy that test ?", reply_markup=self.YNmarkup)
                self.bot.register_next_step_handler(message, self.show_current_test, acceptMode=True, chat_id=self.current_user)
            else:
                canBePassedIn = int(requests.get(f"https://localhost:44381/GetPossibleTestPassRange/{self.current_user}/{self.current_test}", verify=False).text)

                if canBePassedIn == 0:
                    self.isDeciding = True
                    self.bot.send_message(self.current_user, "Do you want to pass the test now ?", reply_markup=self.YNmarkup)
                    self.bot.register_next_step_handler(message, self.show_current_test, acceptMode=True, chat_id=self.current_user)
                else:
                    self.manage_current_test(message, canBePassedIn=canBePassedIn)

        else:
            if message.text == "Yes":
                self.isDeciding = False
                if self.isActivatedFromShop:
                    canPurchase = bool(json.loads(requests.get(f"https://localhost:44381/PurchaseTest/{self.current_user}/{self.current_test}/{self.localisation}", verify=False).text))
                    if canPurchase:
                        self.bot.send_message(self.current_user, "Test had been successfully purchased :)")
                        self.test_pass_step_0(message)
                    else:
                        self.bot.send_message(self.current_user, "You dont have enough points to buy this test")
                        self.go_back_to_test_selection()
                else:
                    self.load_test_data()
                    self.recurring_test_pass()
            elif message.text == "No":
                self.isDeciding = False
                self.go_back_to_test_selection()
            else:
                self.bot.send_message(self.current_user, "No such option", reply_markup=self.YNmarkup)
                self.bot.register_next_step_handler(message, self.show_current_test, acceptMode=acceptMode, chat_id=self.current_user)

    def test_pass_step_0(self, message, acceptMode=False):
        if not acceptMode:
            self.isDeciding = True
            self.bot.send_message(self.current_user, "Would you like to pass it right away ?", reply_markup=self.YNmarkup)
            self.bot.register_next_step_handler(message, self.test_pass_step_0, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "Yes":
                self.isDeciding = False
                self.load_test_data()
                self.recurring_test_pass()
            else:
                self.isDeciding = False
                self.go_back_to_test_selection()

    def manage_current_test(self, message, canBePassedIn=0, acceptMode=False):
        if not acceptMode:
            self.isDeciding = True
            self.bot.send_message(self.current_user, f"You can pass this test again in {canBePassedIn} days")
            self.bot.send_message(self.current_user, self.manage_test_message, reply_markup=self.ManageTestMarkup)
            self.bot.register_next_step_handler(message, self.manage_current_test, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "1":
                self.isDeciding = False
                self.go_back_to_test_selection()
            elif message.text == "2":
                #TODO: Check if user has an effect
                self.isDeciding = False
                self.load_test_data()
                self.recurring_test_pass()
            else:
                self.bot.send_message(self.current_user, f"No such option", reply_markup=self.ManageTestMarkup)
                self.bot.register_next_step_handler(message, self.manage_current_test, acceptMode=True, chat_id=self.current_user)

    def recurring_test_pass(self):
        self.isPassingTest = True
        questions = self.current_test_data["questions"]
        if questions:
            markup = InlineKeyboardMarkup()
            self.current_question = questions[0]

            self.current_question_answers.clear()
            for answer in self.current_question["answers"]:
                self.current_question_answers[answer["id"]] = answer
                markup.add(InlineKeyboardButton(f"{answer['text']}", callback_data=answer["id"]))

            if self.current_question["photo"]:
                self.bot.send_photo(self.current_user, self.current_question["photo"], caption=self.current_question["text"])

            if not self.current_question_message:
                self.current_question_message = self.bot.send_message(self.current_user, f"❓ {self.current_question['text']} ❓", reply_markup=markup).id
            else:
                # self.bot.edit_message_reply_markup(chat_id=self.current_user, reply_markup=markup, message_id=self.current_question_message)
                self.bot.edit_message_text(text=f"❓ {self.current_question['text']} ❓", chat_id=self.current_user, message_id=self.current_question_message, reply_markup=markup)
            questions.pop(0)
        else:
            self.isPassingTest = False
            self.show_test_result()

    def show_test_result(self):
        data = self.create_test_payload()

        d = json.dumps(data)
        json.loads(requests.post(f"https://localhost:44381/UpdateUserPersonalityStats", d, headers={"Content-Type": "application/json"}, verify=False).text)

        active_answer = "-"

        default_answer = None
        smallest = 1000

        for result in self.current_test_data["results"]:
            #Will be sent if users score is even smaller than the smallest possible score
            if result["score"] < smallest:
                default_answer = result["result"]
            if self.user_total >= int(result["score"]):
                active_answer = result["result"]

        if active_answer:
            self.bot.send_message(self.current_user, active_answer)
        else:
            self.bot.send_message(self.current_user, default_answer)
        self.go_back_to_test_selection()

    def create_test_payload(self):
        data = {
            "userId": self.current_user,
            "testId": self.current_test,
            "personality": 0,
            "emotionalIntellect": 0,
            "reliability": 0,
            "compassion": 0,
            "openMindedness": 0,
            "agreeableness": 0,
            "selfAwareness": 0,
            "levelOfSense": 0,
            "intellect": 0,
            "nature": 0,
            "creativity": 0,
        }

        if self.active_param == 1:
            data["personality"] = self.user_total
        elif self.active_param == 2:
            data["emotionalIntellect"] = self.user_total
        elif self.active_param == 3:
            data["reliability"] = self.user_total
        elif self.active_param == 4:
            data["compassion"] = self.user_total
        elif self.active_param == 5:
            data["openMindedness"] = self.user_total
        elif self.active_param == 6:
            data["agreeableness"] = self.user_total
        elif self.active_param == 7:
            data["selfAwareness"] = self.user_total
        elif self.active_param == 8:
            data["levelOfSense"] = self.user_total
        elif self.active_param == 9:
            data["intellect"] = self.user_total
        elif self.active_param == 10:
            data["nature"] = self.user_total
        elif self.active_param == 11:
            data["creativity"] = self.user_total

        return data

    def get_current_test_data(self):
        if self.isActivatedFromShop:
            t = requests.get(f"https://localhost:44381/GetTestFullDataById/{self.current_test}/{self.localisation}", verify=False)
            data = json.loads(t.text)

            return f"{data['name']}\n\n{data['description']}\n\n{data['price']}"

        data = json.loads(requests.get(f"https://localhost:44381/GetUserTest/{self.current_user}/{self.current_test}", verify=False).text)

        return_string = f"{data['test']['name']}\n\n{data['test']['description']}\n\n"

        if data['passedOn']:
            #TODO: format timestamp correctly
            #passDate = datetime.strptime(data["passedOn"], '%Y-%m-%d %H:%M:%S %Z')
            return_string += f"Passed on: {data['passedOn']}"
        else:
            return_string += f"Test hadn't been passed yet"

        return return_string

    def go_back_to_test_selection(self):
        markup = self.get_markup()

        if markup:
            self.bot.delete_message(chat_id=self.current_user, message_id=self.active_message_id)
            self.active_message_id = self.bot.send_message(self.current_user, "Choose a test", reply_markup=markup).id
            self.bot.send_message(self.current_user, "Type '/abort' to leave", reply_markup=self.abortMarkup)
            # self.is_about_to_leave = True
            # self.get_ready_to_abort(self.message)

    def load_test_data(self):
        self.current_test_data = json.loads(requests.get(f"https://localhost:44381/GetSingleTest/{self.current_test}/{self.localisation}", verify=False).text)

    def callback_handler(self, call):
        self.bot.answer_callback_query(call.id, call.data)
        if call.data == "-1" or call.data == "-2":
            try:
                index = self.index_converter(call.data)
                if self.markup_page + index <= self.markup_pages_count or self.markup_page + index >= 1:
                    markup = assemble_markup(self.markup_page, self.current_markup_elements, index)
                    self.bot.edit_message_reply_markup(chat_id=call.message.chat.id, reply_markup=markup,
                                                       message_id=call.message.id)
                    self.markup_page += index
            except:
                pass

        elif "/" in call.data:
            self.bot.answer_callback_query(call.id, call.data)

        elif self.isDeciding:
            return False

        elif self.isOnStart:
            self.active_param = int(call.data)
            self.manage_point_group()

        elif self.isPassingTest:
            self.user_total += self.current_question_answers[int(call.data)]["value"]
            self.recurring_test_pass()

        #If user is going back to start
        elif call.data == '-10':
            self.isOnStart = True
            self.bot.edit_message_reply_markup(chat_id=self.current_user, reply_markup=self.start_markup, message_id=self.active_message_id)

        else:
            self.current_test = int(call.data)
            self.show_current_test(call.message)

    # def get_ready_to_abort(self, message):
    #     self.bot.register_next_step_handler(message, self.abort_checkout, chat_id=self.current_user)

    def abort_handler(self):
        self.destruct()

    def abort_checkout(self, message, acceptMode=False):
        if not self.is_about_to_leave:
            if not acceptMode:
                self.bot.send_message(self.current_user, "Are you sure, you want to leave ?", reply_markup=self.YNmarkup)
                self.bot.register_next_step_handler(message, self.abort_checkout, acceptMode=True, chat_id=self.current_user)
            else:
                if message.text == "Yes":
                    self.is_about_to_leave = False
                    self.abort_handler()
                elif message.text == "No":
                    self.is_about_to_leave = False
                    pass
                #     self.get_ready_to_abort(message)
                else:
                    self.bot.send_message(self.current_user, "No such option", reply_markup=self.YNmarkup)
                    self.bot.register_next_step_handler(message, self.abort_handler, acceptMode=acceptMode, chat_id=self.current_user)

    @staticmethod
    def index_converter(index):
        if index == "-1":
            return -1
        return 1

    def destruct(self):
        if self.chCode in self.bot.callback_query_handlers:
            self.bot.callback_query_handlers.remove(self.chCode)

        if self.ah in self.bot.message_handlers:
            self.bot.message_handlers.remove(self.ah)

        if self.returnMethod:
            self.returnMethod()
            return False

        go_back_to_main_menu(self.bot, self.current_user, self.message)