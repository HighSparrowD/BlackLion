import json

import pandas
import requests


test_file_name = "TestUploadTemplate - Test"
questions_file_name = "TestUploadTemplate - Questions"
answers_file_name = "TestUploadTemplate - Answers"


def add_ps_test():
    test_template = []

    test_template = load_test_data(test_template)

    data = json.dumps(test_template)
    print(requests.post("https://localhost:44381/UploadPsTests", data, headers={"Content-Type": "application/json"}, verify=False).text)


def load_test_data(testTemplate):
    file = pandas.read_csv(f"{test_file_name}.csv")
    file = get_file_data(file)
    test_data = {}

    for test in file:
        test_data = {
            "id": test[0],
            "classLocalisationId": test[1],
            "name": test[2],
            "description": test[3],
            "questions": load_questions(test[0])
        }
        testTemplate.append(test_data)

    return testTemplate


def load_questions(testId):
    file = pandas.read_csv(f"{questions_file_name}.csv")
    file = get_file_data(file)

    questions = []

    for question in file:
        if question[2] == testId:
            questions.append({
                "text": question[0],
                "answers": load_answers(question[1])
            })

    return questions


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