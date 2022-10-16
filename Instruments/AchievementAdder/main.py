import json

import pandas
import requests


def add_achievements():
    file = pandas.read_csv("achievements.csv", usecols=["country"])
    achievements = sorted(file.drop_duplicates().values.tolist())
    i = 1

    for ach in achievements:
        ach["id"] = i
        i += 1

    if len(achievements) > 0:
        data = json.dumps(achievements)
        print(requests.post("", data, headers={"Content-Type": "application/json"}, verify=False).text)