import json

import requests
from telebot import TeleBot

import Core.HelpersMethodes as Helpers
from telebot.types import ReplyKeyboardMarkup, InlineKeyboardButton, InlineKeyboardMarkup
from Common.Menues import index_converter
from Common.Menues import count_pages, assemble_markup, reset_pages
from Common.Menues import go_back_to_main_menu


# noinspection PyBroadException
class TestModule:
    def __init__(self, bot: TeleBot, message: any, isActivatedFromShop: bool = False, returnMethod: any = None,
                 active_message: int = 0):
        self.bot = bot
        self.message = message
        self.current_user = message.from_user.id
        self.isActivatedFromShop = isActivatedFromShop
        self.isOnStart = True
        self.isDeciding = False
        self.isPassingTest = False
        self.is_about_handler_present = False
        self.justEntered = True
        self.returnMethod = returnMethod

        self.current_question_index = -1
        self.previous_question = None
        self.answer_array = {}

        self.tags_list = []

        self.localisation = Helpers.get_user_app_language(self.current_user)

        self.current_markup_elements = []
        self.markup_last_element = 0
        self.markup_page = 1
        self.markup_pages_count = 0

        self.active_message = active_message
        self.secondary_message = None
        self.question_message = None

        self.active_param = 0
        self.active_media = None

        self.lie_scale = {}

        self.user_total = None

        #Represent a price of current test in different currencies
        self.current_price_C = 0
        self.current_price_RM = 0

        self.chCode = self.bot.register_callback_query_handler(message, self.callback_handler, user_id=self.current_user)

        self.questions = None
        self.questions_count = 0

        self.start_markup = InlineKeyboardMarkup().add(InlineKeyboardButton("Openness", callback_data="1"))\
                .add(InlineKeyboardButton("Conscientiousness", callback_data="2")) \
                .add(InlineKeyboardButton("Extroversion", callback_data="3")) \
                .add(InlineKeyboardButton("Agreeableness", callback_data="4")) \
                .add(InlineKeyboardButton("Neuroticism", callback_data="5")) \
                .add(InlineKeyboardButton("Nature", callback_data="6")) \
                .add(InlineKeyboardButton("🔙Go Back", callback_data="-15"))

        self.buy_markup = InlineKeyboardMarkup()
        self.pass_test_markup = InlineKeyboardMarkup().add(InlineKeyboardButton("Pass Now", callback_data="-11"))\
            .add(InlineKeyboardButton("🔙Go Back", callback_data="-5"))
        self.pass_test_again_markup = InlineKeyboardMarkup().add(InlineKeyboardButton("Pass again using 💥Nullifier💥", callback_data="-12"))\
            .add(InlineKeyboardButton("🔙Go Back", callback_data="-5"))

        self.YNmarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("Yes", "No")
        self.continueMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("Continue")
        self.abortMarkup = ReplyKeyboardMarkup(resize_keyboard=True, one_time_keyboard=True).add("/abort")

        self.current_test_data = {}
        self.current_question = {}
        self.current_question_answers = {}
        self.tests = {}

        self.current_tests = None
        self.current_test = None

        if not self.returnMethod:
            Helpers.switch_user_busy_status(self.current_user, 12)

        self.ah = self.bot.register_message_handler(self.abort_checkout, commands=["abort"], user_id=self.current_user)

        self.start()

    def start(self):
        self.isOnStart = True
        self.justEntered = False
        if not self.active_message:
            self.active_message = self.bot.send_message(self.current_user, "<i><b>Please select the parameter, test will be sorted by</b></i>", reply_markup=self.start_markup).id
        else:
            self.bot.edit_message_text("<i><b>Please select the parameter, test will be sorted by</b></i>", self.current_user, self.active_message, reply_markup=self.start_markup)

        self.send_secondary_message("<i>Type '/abort' to leave at any time</i>", markup=self.abortMarkup)
        # self.get_ready_to_abort(self.message)

    def manage_point_group(self):
        markup = self.get_markup()
        if markup:
            try:
                self.bot.edit_message_text(chat_id=self.current_user, text="<i><b>Select any test to view more details</b></i>", reply_markup=markup, message_id=self.active_message)
            except:
                pass

    def get_markup(self):
        if self.isActivatedFromShop:
            self.load_new_tests_data_by_param(self.active_param)
        else:
            self.load_owned_tests_data_by_param(self.active_param)

        if not self.current_tests:
            self.send_secondary_message("<i><b>No tests were found</b></i>")
            return

        self.isOnStart = False
        reset_pages(self.current_markup_elements, self.markup_last_element, self.markup_page, self.markup_pages_count)
        count_pages(self.tests, self.current_markup_elements, self.markup_pages_count, additionalButton=True, buttonText="🔙Go Back", buttonData=-10)
        markup = assemble_markup(self.markup_page, self.current_markup_elements, 0)

        return markup

    def load_new_tests_data_by_param(self, param):
        try:
            self.current_tests = json.loads(requests.get(f"https://localhost:44381/test-data-by-param/{self.current_user}/{param}", verify=False).text)
            self.tests.clear()
            for test in self.current_tests:
                self.tests[test["id"]] = test["name"]
        except:
            return None

    def load_owned_tests_data_by_param(self, param):
        try:
            self.current_tests = json.loads(requests.get(f"https://localhost:44381/test-data-by-prop/{self.current_user}/{param}", verify=False).text)
            self.tests.clear()
            for test in self.current_tests:
                self.tests[test["id"]] = test["name"]
        except:
            return None

    def show_current_test(self, message, acceptMode=False, decisionIndex="1"):
        if not acceptMode:
            if self.isActivatedFromShop:
                self.isDeciding = True
                self.send_active_message(f"{self.get_current_test_data()}\n\n<b>How would you like to purchase this test?</b>", self.buy_markup)
            else:
                canBePassedIn = int(requests.get(f"https://localhost:44381/test-pass-range/{self.current_user}/{self.current_test}", verify=False).text)

                if canBePassedIn == 0:
                    self.isDeciding = True
                    self.send_active_message(f"{self.get_current_test_data()}\n\n<b>Do you want to pass the test now ?</b>", self.pass_test_markup)
                else:
                    self.manage_current_test(canBePassedIn=canBePassedIn)

        else:
            #If is eager to buy in any currency
            if decisionIndex == "1" or decisionIndex == "2":
                self.isDeciding = False
                if self.isActivatedFromShop:
                    #Coins purchase
                    if decisionIndex == "1":
                        canPurchase = bool(json.loads(requests.get(f"https://localhost:44381/purchase-test/{self.current_user}/{self.current_test}/{self.localisation}", verify=False).text))
                    else:
                        canPurchase = False
                        pass
                        #TODO: Relocate user to real money purchase section
                        # canPurchase = bool(json.loads(requests.get(f"https://localhost:44381/PurchaseTest/{self.current_user}/{self.current_test}/{self.localisation}", verify=False).text))

                    if canPurchase:
                        self.test_pass_step_0(message)
                    else:
                        self.send_secondary_message("You dont have enough points to buy this test")
                else:
                    self.load_test_data()
                    self.recurring_test_pass()
            elif message.text == "2":
                self.isDeciding = False
                self.go_back_to_test_selection()
            elif decisionIndex == "3":
                self.go_back_to_test_selection()

    def test_pass_step_0(self, message, acceptMode=False):
        if not acceptMode:
            self.isDeciding = True
            self.bot.edit_message_text(f"{self.get_current_test_data()}\n\n<b>Test had been successfully purchased :)</b>", self.current_user, self.active_message,  reply_markup=self.pass_test_markup)
        else:
            if message.text == "Yes":
                self.isDeciding = False
                self.load_test_data()
                self.recurring_test_pass()
            else:
                self.isDeciding = False
                self.go_back_to_test_selection()

    def manage_current_test(self, canBePassedIn=0, acceptMode=False):
        if not acceptMode:
            self.isDeciding = True
            self.send_active_message(f"{self.get_current_test_data()}\n\n<b><i>You can pass this test again in {canBePassedIn} days</i></b>", self.pass_test_again_markup)
        else:
            if Helpers.check_user_has_effect(self.current_user, 8):
                self.isDeciding = False
                self.load_test_data()
                self.recurring_test_pass()
            else:
                self.send_secondary_message("Sorry, you dont have this effect")
            # elif decisionIndex == "2":
            #     self.isDeciding = False
            #     self.go_back_to_test_selection()

    def recurring_test_pass(self, gotoPrevious=False):
        if self.current_question_index + 1 < self.questions_count:
            if not gotoPrevious:
                self.current_question_index += 1

            markup = InlineKeyboardMarkup()
            self.current_question = self.questions[self.current_question_index]

            #Create question counter
            markup.add(InlineKeyboardButton(f"{self.current_question_index + 1} / {len(self.questions)}", callback_data="0"))

            self.current_question_answers.clear()

            for answer in self.current_question["answers"]:
                self.current_question_answers[answer["id"]] = answer
                markup.add(InlineKeyboardButton(f"{answer['text']}", callback_data=answer["id"]))

            #Create previous question button
            markup.add(InlineKeyboardButton("⬅ Previous question", callback_data="-8"))

            # Code below is redundant now. But may be useful in the future
            # if self.active_media is not None:
            #     #Remove photo from question
            #     self.active_media = None
            #     self.bot.edit_message_media(None, self.current_user, self.active_message)
            #
            # #Set question photo if one is required
            # if self.current_question["photo"]:
            #     self.active_media = self.current_question["photo"]
            #     self.bot.edit_message_media(self.active_media, self.current_user, self.active_message)

            if not self.question_message:
                self.delete_active_message()
                self.send_question_message(f"❓ {self.current_question['text']} ❓", markup)
                # self.question_message = self.bot.send_message(self, reply_markup=markup).id
                self.send_secondary_message("<i><b>You can leave by typing '/abort', but all your progress will be lost</b></i>", markup=self.abortMarkup)
            else:
                self.send_question_message(f"❓ {self.current_question['text']} ❓", markup)
                # self.bot.edit_message_text(text=, chat_id=self.current_user, message_id=self.question_message, reply_markup=markup)
        else:
            self.isPassingTest = False
            self.show_test_result(self.message)
            return

    def show_test_result(self, message, acceptMode=False):
        if not acceptMode:

            # Lie check
            isLying = self.hasLieScale and sum(self.lie_scale.values()) >= self.current_test_data["scales"][0]["minValue"]

            # Order results, so that the first one is the smallest (if scores are present at all)
            results = sorted(self.current_test_data["results"], key=lambda x: x['score'] if x["score"] is not None else x["id"])

            tags = None
            self.isDeciding = True

            self.delete_question_message()

            active_answer = None
            data = None

            # Set a default result (the smallest one)
            previous_result = results[0]

            if self.current_test == 49:
                self.user_total = {}
                y = sum(self.answer_array['1'].values())
                x = sum(self.answer_array['2'].values())

                # for scale in self.answer_array.keys():
                #     self.user_total[scale] = sum(self.answer_array[scale].values())

                # TODO: Provide percentages or a graph
                if y <= 12 <= x:
                    result = results[2]
                    score = 1
                elif y > 12 and x >= 12:
                    result = results[0]
                    score = 2
                elif y < 12 > x:
                    result = results[3]
                    score = 3
                else:
                    result = results[1]
                    score = 4

                data = self.create_test_payload(score)
                tags = result["tags"]
                active_answer = result["result"]
            else:
                self.user_total = sum(self.answer_array.values())
                score = float(self.user_total) / float(results[-1]["score"])
                data = self.create_test_payload(score)

                for result in results:
                    if self.user_total >= result["score"]:
                        previous_result = result
                        continue

                    if previous_result["tags"] is not None:
                        tags = previous_result["tags"]

                    active_answer = previous_result["result"]
                    break

            self.send_question_message(active_answer, markup=self.continueMarkup)
            self.active_message = self.question_message

            if not isLying:
                data["tags"] = tags
                d = json.dumps(data)
                requests.post(f"https://localhost:44381/update-ocean-stats", d,
                              headers={"Content-Type": "application/json"}, verify=False)
            else:
                self.send_secondary_message("According to your results you haven't been completely honest while passing this test\nYour results will not be registered. You are free to pass the test again without waiting, but, be more honest this time, please ;)")

            self.bot.register_next_step_handler(message, self.show_test_result, acceptMode=True, chat_id=self.current_user)
        else:
            self.isDeciding = False
            self.bot.delete_message(self.current_user, message.id)
            self.show_current_test(message)

    def create_test_payload(self, score):
        data = {
            "userId": self.current_user,
            "testId": self.current_test,
            "openness": 0,
            "conscientiousness": 0,
            "extroversion": 0,
            "agreeableness": 0,
            "neuroticism": 0,
            "nature": 0,
        }

        if self.active_param == 1:
            data["openness"] = score
        elif self.active_param == 2:
            data["conscientiousness"] = score
        elif self.active_param == 3:
            data["extroversion"] = score
        elif self.active_param == 4:
            data["agreeableness"] = score
        elif self.active_param == 5:
            data["neuroticism"] = score
        elif self.active_param == 6:
            data["nature"] = score

        return data

    def get_current_test_data(self):
        if self.isActivatedFromShop:
            t = requests.get(f"https://localhost:44381/test-data-by-id/{self.current_test}/{self.localisation}", verify=False)
            data = json.loads(t.text)

            self.current_price_C = data['price']
            self.current_price_RM = 0

            self.buy_markup.clear()
            self.buy_markup.add(InlineKeyboardButton(f"{data['price']} Coins", callback_data="-6"), InlineKeyboardButton(f"XXX CZK", callback_data="-3")).add(InlineKeyboardButton("🔙Go Back", callback_data="-5"))

            return f"{data['name']}\n\n{data['description']}"

        data = json.loads(requests.get(f"https://localhost:44381/user-test/{self.current_user}/{self.current_test}", verify=False).text)

        return_string = f"{data['test']['name']}\n\n{data['test']['description']}"

        if data['passedOn']:
            return_string += f"<b>Passed on: {data['passedOn']}</b>"
        else:
            return_string += f"\n\n<b>Test hadn't been passed yet</b>"

        return return_string

    def go_back_to_test_selection(self):
        markup = self.get_markup()

        if markup:
            if not self.active_message:
                self.active_message = self.bot.send_message(self.current_user, "<i><b>Select any test to view more details</b></i>", reply_markup=markup).id
            else:
                try:
                    self.bot.edit_message_text(chat_id=self.current_user, text="<i><b>Select any test to view more details</b></i>", reply_markup=markup, message_id=self.active_message)
                except:
                    pass
        else:
            self.start()

    def load_test_data(self):
        self.current_question_index = -1
        self.current_test_data = json.loads(requests.get(f"https://localhost:44381/single-test/{self.current_test}/{self.localisation}", verify=False).text)

        self.isPassingTest = True
        self.questions = self.current_test_data["questions"]
        self.questions_count = len(self.questions)
        self.hasLieScale = len(self.current_test_data["scales"]) != 0 and \
                           self.current_test_data["scales"][0]["scale"] == "L"

        for scale in self.current_test_data["scales"]:
            # Lie scale is handled separately
            if scale["scale"] != "L":
                self.answer_array[scale["scale"]] = {}

    def callback_handler(self, call):
        self.bot.answer_callback_query(call.id, "")
        if call.data == "-1" or call.data == "-2":
            try:
                index = index_converter(call.data)
                if self.markup_page + index <= self.markup_pages_count or self.markup_page + index >= 1:
                    markup = assemble_markup(self.markup_page, self.current_markup_elements, index)
                    self.bot.edit_message_reply_markup(chat_id=call.message.chat.id, reply_markup=markup,
                                                       message_id=call.message.id)
                    self.markup_page += index
            except:
                pass

        #Buy test for RM
        elif call.data == "-3":
            self.show_current_test(call.message, acceptMode=True, decisionIndex="2")
        #Buy test for Points
        elif call.data == "-6":
            self.show_current_test(call.message, acceptMode=True, decisionIndex="1")
        #Bo back from test selection
        elif call.data == "-5":
            self.isDeciding = False
            self.go_back_to_test_selection()

        #Pass test for the first time
        elif call.data == "-11":
            self.isDeciding = False
            self.load_test_data()
            self.recurring_test_pass()
            # self.show_current_test(call.message, acceptMode=True)

        elif call.data == "-12":
            self.manage_current_test(acceptMode=True)

        elif call.data == "-15":
            self.destruct()

        elif self.isDeciding:
            return False

        elif self.isOnStart:
            if not self.justEntered:
                self.active_param = int(call.data)
                self.manage_point_group()

        elif call.data == "0":
            return

        #If user is going back to start
        elif call.data == '-10':
            self.isOnStart = True
            self.bot.edit_message_text(chat_id=self.current_user, text="<i><b>Please select the parameter, test will be sorted by</b></i>", reply_markup=self.start_markup, message_id=self.active_message)

        elif call.data == '-8':
            if self.current_question_index > 0:
                self.current_question_index -= 1
                self.recurring_test_pass(gotoPrevious=True)

        elif self.isPassingTest:
            if self.questions_count > self.current_question_index:
                try:
                    answer_weight = int(self.current_question_answers[int(call.data)]["value"])
                    if self.current_question["scale"] == "L":
                        self.lie_scale[self.current_question_index] = answer_weight
                    else:
                        # Nature test is so different, it requires its own condition...
                        if self.current_test == 49:
                            self.answer_array[self.current_question["scale"]][self.current_question_index] = answer_weight
                        else:
                            self.answer_array[self.current_question_index] = answer_weight
                except Exception as ex:
                    return

                #Add tags from answers if they are present
                if self.current_question_answers[int(call.data)]["tags"] is not None:
                    self.tags_list.extend(self.current_question_answers[int(call.data)]["tags"])

                self.recurring_test_pass()
            # else:
            #     try:
            #         self.answer_array[self.current_question_index] = int(self.current_question_answers[int(call.data)]["value"])
            #         # self.answer_array.append(int(self.current_question_answers[int(call.data)]["value"]))
            #         # self.user_total += self.current_question_answers[int(call.data)]["value"]
            #         self.recurring_test_pass()
            #     except:
            #         pass
        else:
            self.current_test = int(call.data)
            self.show_current_test(call.message)

    # def get_ready_to_abort(self, message):
    #     self.bot.register_next_step_handler(message, self.abort_checkout, chat_id=self.current_user)

    def abort_handler(self):
        self.destruct()

    def abort_checkout(self, message, acceptMode=False):
        if not acceptMode:
            self.send_secondary_message("Are you sure, you want to leave ?", markup=self.YNmarkup)
            self.bot.register_next_step_handler(message, self.abort_checkout, acceptMode=True, chat_id=self.current_user)
        else:
            if message.text == "Yes":
                self.abort_handler()
            elif message.text == "No":
                pass
            #     self.get_ready_to_abort(message)
            else:
                self.send_secondary_message("No such option", markup=self.YNmarkup)
                self.bot.register_next_step_handler(message, self.abort_handler, acceptMode=acceptMode, chat_id=self.current_user)
        self.bot.delete_message(self.current_user, message.id)

    # def send_active_message(self, text, markup=None):
    #     try:
    #         if self.active_message:
    #             self.bot.edit_message_text(text, self.current_user, self.active_message, reply_markup=markup)
    #             return
    #
    #         self.active_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
    #     except:
    #         self.delete_active_message()
    #         self.send_active_message(text, markup)

    def send_question_message(self, text, markup=None):
        try:
            if self.question_message:
                self.bot.edit_message_text(text, self.current_user, self.question_message, reply_markup=markup)
                return

            self.question_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except:
            self.delete_question_message()
            self.send_question_message(text, markup)

    def send_active_message(self, text, markup=None):
        try:
            if self.active_message:
                self.bot.edit_message_text(text, self.current_user, self.active_message, reply_markup=markup)
                return

            self.active_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except:
            self.delete_question_message()
            self.send_active_message(text, markup)

    def send_secondary_message(self, text, markup=None):
        try:
            if self.secondary_message:
                self.bot.edit_message_text(text, self.current_user, self.secondary_message, reply_markup=markup)
                return

            self.secondary_message = self.bot.send_message(self.current_user, text, reply_markup=markup).id
        except:
            self.delete_secondary_message()
            self.send_secondary_message(text, markup)

    def delete_active_message(self):
        if self.active_message:
            self.bot.delete_message(self.current_user, self.active_message)
            self.active_message = None

    def delete_question_message(self):
        if self.question_message:
            self.bot.delete_message(self.current_user, self.question_message)
            self.question_message = None

    def delete_secondary_message(self):
        if self.secondary_message:
            self.bot.delete_message(self.current_user, self.secondary_message)
            self.secondary_message = None

    def destruct(self):
        if self.chCode in self.bot.callback_query_handlers:
            self.bot.callback_query_handlers.remove(self.chCode)

        if self.ah in self.bot.message_handlers:
            self.bot.message_handlers.remove(self.ah)

        self.delete_secondary_message()
        self.delete_question_message()

        if self.returnMethod:
            self.returnMethod(self.message, shouldSubscribe=True)
            return False

        self.delete_question_message()
        self.delete_active_message()
        self.delete_secondary_message()
        go_back_to_main_menu(self.bot, self.current_user, self.message)
