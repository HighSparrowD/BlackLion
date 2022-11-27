import json

import pandas
import requests
import translators as ts

countries_last_index = 0
cities_last_index = 0
languages_last_index = 0

def update_countries(language, start_index=0, loc_index=0):
    global countries_last_index

    file = pandas.read_csv("worldcities.csv", usecols=["country", "priority"])
    countries = sorted(file.drop_duplicates().values.tolist())
    countries_dicts = []
    i = start_index

    if loc_index == 0:
        for country in countries:
            d = {
                "id": i,
                "countryName": country[0],
                "classLocalisationId": loc_index
            }
            countries_dicts.append(d)
            i += 1
    else:
        for country in countries:
            d = {
                "id": i,
                "countryName": ts.google(country[0], from_language="en", to_language=language),
                "classLocalisationId": loc_index
            }
            countries_dicts.append(d)
            i += 1

    data = json.dumps(countries_dicts)

    s = requests.post("https://localhost:44381/UpdateCountries", data, headers={
            "Content-Type": "application/json"},   verify=False).text

    countries_last_index = i + 1


def update_cities(language, start_index=0, loc_index=0):
    global cities_last_index

    cities = pandas.read_csv("worldcities.csv", usecols=["city", "country"])
    file = sorted(pandas.read_csv("worldcities.csv", usecols=["country"]).drop_duplicates().values.tolist())
    countries = []
    d = []
    i = start_index

    for f in file:
        countries.append(f[0])

    if loc_index == 0:
        for index, data in cities.iterrows():
            d.append({
                "Id": i,
                "CityName": data[0],
                "CountryId": countries.index(data[1]) + 1,
                "ClassLocalisationId": loc_index
            })
            i += 1

    else:
        for index, data in cities.iterrows():
            d.append({
                "Id": i,
                "CityName": ts.google(data[0], from_language="en", to_language=language),
                "CountryId": countries.index(data[1]) + 1,
                "ClassLocalisationId": loc_index
            })
            print(i)
            i += 1

    data = json.dumps(d)

    s = requests.post("https://localhost:44381/UpdateCities", data, headers={
            "Content-Type": "application/json"},   verify=False).text

    cities_last_index = i + 1


def update_languages(language, start_index=0, loc_index=0):
    global languages_last_index

    languages = pandas.read_csv("Languages.csv", usecols=["Language name ", "Native name ", "Priority "]).drop_duplicates().values.tolist()
    d = []
    i = start_index

    if loc_index == 0:
        for lang in languages:
            d.append({
                "Id": i,
                "LanguageName": lang[0],
                "LanguageNameNative": lang[0],  # lang[1] TODO: Make it work with a native one
                "ClassLocalisationId": loc_index,
                "Priority": lang[2]
            }
            )
            i += 1

    else:
        for lang in languages:
            d.append({
                    "Id": i,
                    "LanguageName": ts.google(lang[0], from_language="en", to_language=language),
                    "LanguageNameNative": lang[0], #lang[1] TODO: Make it work with a native one
                    "ClassLocalisationId": loc_index,
                    "Priority": lang[2]
                }
            )
            i += 1

    data = json.dumps(d) #ensure_ascii=False
    s = requests.post("https://localhost:44381/UpdateLanguages", data, headers={
        "Content-Type": "application/json"}, verify=False).text

    languages_last_index = i + 1


def Do():
    data = ts.google("Me", from_language="en", to_language="ru")
    print(data)


update_countries("en", 1, 0)
# update_countries("ru", 1, 1)
# update_countries("uk", 1, 2)
# update_cities("en", 1, 0)
update_cities("ru", cities_last_index, 1)
update_cities("uk", cities_last_index, 2)
# update_languages("en", 1, 0)
# update_languages("ru", 1, 1)
# update_languages("uk", 1, 2)