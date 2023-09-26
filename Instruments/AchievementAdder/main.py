import json

import pandas
import requests


def generate_achievement_resource():
    file = pandas.read_csv("./Inputs/Achievements.csv")
    achievements = sorted(file.drop_duplicates().values.tolist())

    ids = []
    namesEN = []
    namesRU = []
    namesUK = []
    descriptionsEN = []
    descriptionsRU = []
    descriptionsUK = []
    conditions = []
    rewards = []

    for a in achievements:
        ids.append(a[0])
        namesEN.append(a[1])
        descriptionsEN.append(a[2])
        namesRU.append(a[3])
        descriptionsRU.append(a[4])
        namesUK.append(a[5])
        descriptionsUK.append(a[6])
        conditions.append(a[7])
        rewards.append(a[8])

    dataEN = {
        "Id": ids,
        "Name": namesEN,
        "Description": descriptionsEN,
        "Condition": conditions,
        "Reward": rewards
    }

    dataRU = {
        "Id": ids,
        "Name": namesRU,
        "Description": descriptionsRU,
        "Condition": conditions,
        "Reward": rewards
    }

    dataUK = {
        "Id": ids,
        "Name": namesUK,
        "Description": descriptionsUK,
        "Condition": conditions,
        "Reward": rewards
    }

    dataframeEN = pandas.DataFrame(dataEN)
    dataframeEN = dataframeEN.fillna(value=0)

    dataframeRU = pandas.DataFrame(dataRU)
    dataframeRU = dataframeRU.fillna(value=0)

    dataframeUK = pandas.DataFrame(dataUK)
    dataframeUK = dataframeUK.fillna(value=0)

    dataframeEN.to_csv("./Outputs/AchievementsEN.csv", index=False)
    dataframeRU.to_csv("./Outputs/AchievementsRU.csv", index=False)
    dataframeUK.to_csv("./Outputs/AchievementsUK.csv", index=False)


def load_achievement_resource(lang):
    file = pandas.read_csv(f"./Outputs/Achievements{lang.upper()}.csv")
    achievements = sorted(file.drop_duplicates().values.tolist())

    achievement_list = []

    for a in achievements:
        d = {
            "id": a[0],
            "name": a[1],
            "language": lang.upper(),
            "description": a[2],
            "condition": a[3],
            "reward": a[4]
        }

        achievement_list.append(d)

    data = json.dumps(achievement_list)
    requests.post("https://localhost:44381/upload-achievements", data, headers={"Content-Type": "application/json"}, verify=False)

# generate_achievement_resource()

load_achievement_resource("en")
load_achievement_resource("ru")
load_achievement_resource("uk")
