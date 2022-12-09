import json

import pandas
import requests


test_file_name = "TestUploadTemplate - Test"
questions_file_name = "TestUploadTemplate - Questions"
answers_file_name = "TestUploadTemplate - Answers"
results_file_name = "TestUploadTemplate - Results"


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
            "testType": test[4],
            "price": test[5],
            "questions": load_questions(test[0]),
            "results": load_results(test[0])
        }
        testTemplate.append(test_data)

    return testTemplate


def load_questions(testId):
    file = pandas.read_csv(f"{questions_file_name}.csv")
    file = get_file_data(file)

    questions = []
    q = {}

    for question in file:
        if question[2] == testId:
            q = {
                "text": question[0],
                "answers": load_answers(question[1], question[2])
            }

            if not isinstance(question[3], float):
               q["photo"] = question[3]

            questions.append(q)

    return questions


def load_answers(questionId, testId=None):
    file = pandas.read_csv(f"{answers_file_name}.csv")
    file = get_file_data(file)

    answers = []

    for answer in file:
        a = {}

        #If each question has its own answers
        if answer[3] > 0:
            if answer[3] == questionId:
                a = {
                    "text": answer[0],
                    "value": answer[1],
                }
        else:
            if answer[2] == testId:
                a = {
                    "text": answer[0],
                    "value": answer[1],
                }

        if not isinstance(answer[3], float):
            a["isCorrect"] = answer[3]

        answers.append(a)

    return answers


def load_results(testId):
    file = pandas.read_csv(f"{results_file_name}.csv")
    file = get_file_data(file)

    results = []

    for result in file:
        if result[0] == testId:
            r = {
                "score": result[1],
                "result": result[2]
            }

            results.append(r)

    return results


def get_file_data(file):
    return file.values.tolist()


add_ps_test()