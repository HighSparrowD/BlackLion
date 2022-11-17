import json

import pandas
import requests


test_file_name = "TestUploadTemplate - Test"
questions_file_name = "TestUploadTemplate - Questions"
answers_file_name = "TestUploadTemplate - Answers"


def add_ps_test():
    test_template = {}

    test_template = load_test_data(test_template)
    test_template = load_questions(test_template)

    data = json.dumps(test_template)
    print(requests.post("https://localhost:44381/UploadPsTest", data, headers={"Content-Type": "application/json"}, verify=False).text)


def load_test_data(testTemplate):
    file = pandas.read_csv(f"{test_file_name}.csv")
    file = get_file_data(file)[0]

    testTemplate["id"] = file[0]
    testTemplate["classLocalisationId"] = file[1]
    testTemplate["name"] = file[2]
    testTemplate["description"] = file[3]

    return testTemplate


def load_questions(testTemplate):
    file = pandas.read_csv(f"{questions_file_name}.csv")
    file = get_file_data(file)

    questions = []

    for question in file:
        questions.append({
            "text": question[0],
            "answers": load_answers(question[1])
        })

    testTemplate["questions"] = questions

    return testTemplate


def load_answers(questionId):
    file = pandas.read_csv(f"{answers_file_name}.csv")
    file = get_file_data(file)

    answers = []

    for answer in file:
        if answer[2] == questionId:
            answers.append({
                "text": answer[0],
                "value": answer[1]
            })

    return answers


def get_file_data(file):
    return file.values.tolist()


add_ps_test()