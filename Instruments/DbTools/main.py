import json

import pandas
import requests
import translators as ts

countries_last_index = 0
cities_last_index = 0
languages_last_index = 0

langs = {
    0: "Eng",
    1: "Rus",
    2: "Ukr",
}


def create_countries_resource(language, start_index=0, loc_index=0):
    global countries_last_index
    ids = []
    countryNames = []
    languages = []
    priorities = [] #Dummy column. Set manually afterwards


    file = pandas.read_csv("worldcities.csv", usecols=["country", "priority"])
    countries = sorted(file.drop_duplicates().values.tolist())
    i = start_index

    if loc_index == 0:
        for country in countries:

            ids.append(i)
            countryNames.append(country[0])
            languages.append(langs[loc_index])
            priorities.append(5)

            i += 1
    else:
        for country in countries:

            translated_name = ts.google(country[0], from_language="en", to_language=language)

            ids.append(i)
            countryNames.append(translated_name)
            languages.append(langs[loc_index])
            priorities.append(5)

            i += 1

    data = {
        "Id": ids,
        "Name": countryNames,
        "Language": languages,
        "Priority": priorities
    }

    #Save output
    dataframe = pandas.DataFrame(data)
    dataframe = dataframe.fillna(value=0)
    dataframe.to_csv(f"./Output/Countries{language.upper()}.csv", index=False)

    countries_last_index = i + 1


def create_cities_resource(language, start_index=0, loc_index=0):
    global cities_last_index

    ids = []
    countryNames = []
    countryIds = []
    languages = []

    cities = pandas.read_csv("worldcities.csv", usecols=["city", "country"])
    file = sorted(pandas.read_csv("worldcities.csv", usecols=["country"]).drop_duplicates().values.tolist())
    countries = []
    i = start_index

    for f in file:
        countries.append(f[0])

    if loc_index == 0:
        for index, data in cities.iterrows():

            ids.append(i)
            countryNames.append(data[0])
            countryIds.append(countries.index(data[1]) + 1)
            languages.append(langs[loc_index])

            i += 1

    else:
        for index, data in cities.iterrows():

            translated_name = ts.google(data[0], from_language="en", to_language=language)

            ids.append(i)
            countryNames.append(translated_name)
            countryIds.append(countries.index(data[1]) + 1)
            languages.append(langs[loc_index])

            print(i)
            i += 1

    data = {
        "Id": ids,
        "Name": countryNames,
        "Country Id": countryIds,
        "Language": languages
    }

    #Save output
    dataframe = pandas.DataFrame(data)
    dataframe = dataframe.fillna(value=0)
    dataframe.to_csv(f"./Output/Cities{language.upper()}.csv", index=False)

    cities_last_index = i + 1


def create_languages_resource(language, start_index=0, loc_index=0):
    global languages_last_index

    ids = []
    languageNames = []
    languageNativeNames = []
    languages_translation = []
    priorities = []

    languages = pandas.read_csv("Languages.csv", usecols=["Language name", "Native name"]).drop_duplicates().values.tolist()
    d = []
    i = start_index

    if loc_index == 0:
        for lang in languages:

            ids.append(i)
            languageNames.append(lang[0].strip())
            languageNativeNames.append(lang[0].strip()) # lang[1] TODO: Make it work with a native one
            languages_translation.append(langs[loc_index])
            priorities.append(5)

            i += 1

    else:
        for lang in languages:

            translated_lang = ts.google(lang[0].strip(), from_language="en", to_language=language)

            ids.append(i)
            languageNames.append(translated_lang)
            languageNativeNames.append(lang[0].strip()) #lang[1] TODO: Make it work with a native one
            languages_translation.append(langs[loc_index])
            priorities.append(5)

            d.append({
                    "Id": i,
                    "LanguageName": translated_lang,
                    "LanguageNameNative": lang[0].strip(),
                    "Language": loc_index,
                    "Priority": priorities
                }
            )
            i += 1

    data = {
        "Id": ids,
        "Name": languageNames,
        "Native Name": languageNativeNames,
        "Language": languages_translation,
        "Priority": priorities
    }

    #Save output
    dataframe = pandas.DataFrame(data)
    dataframe = dataframe.fillna(value=0)
    dataframe.to_csv(f"./Output/Languages{language.upper()}.csv", index=False)

    languages_last_index = i + 1


def update_countries(language):
    file = pandas.read_csv(f"./Output/Countries{language.upper()}.csv", usecols=["Id", "Name", "Language", "Priority"])
    countries = sorted(file.drop_duplicates().values.tolist())
    countries_dicts = []

    for country in countries:
        d = {
            "id": country[0],
            "countryName": country[1],
            "lang": country[2],
            "priority": country[3] or 5,
        }

        countries_dicts.append(d)

    #Convert data to json
    json_data = json.dumps(countries_dicts)

    s = requests.post("https://localhost:44381/UpdateCountries", json_data, headers={
            "Content-Type": "application/json"},   verify=False).text
    pass


def update_cities(language):
    file = pandas.read_csv(f"./Output/Cities{language.upper()}.csv", usecols=["Id", "Name", "Country Id", "Language"])
    cities = sorted(file.drop_duplicates().values.tolist())
    cities_dict = []

    for city in cities:
        d = {
            "id": city[0],
            "cityName": city[1],
            "countryId": city[2],
            "lang": city[3]
        }

        cities_dict.append(d)

    json_data = json.dumps(cities_dict)

    s = requests.post("https://localhost:44381/UpdateCities", json_data, headers={
            "Content-Type": "application/json"},   verify=False).text


def update_languages(language):
    file = pandas.read_csv(f"./Output/Languages{language.upper()}.csv", usecols=["Id", "Name", "Native Name", "Language", "Priority"])
    languages = sorted(file.drop_duplicates().values.tolist())
    languages_dict = []

    for lang in languages:
        d = {
            "id": lang[0],
            "languageName": lang[1],
            "languageNameNative": lang[2],
            "lang": lang[3],
            "priority": lang[4]
        }

        languages_dict.append(d)

    data = json.dumps(languages_dict) #ensure_ascii=False
    s = requests.post("https://localhost:44381/UpdateLanguages", data, headers={
        "Content-Type": "application/json"}, verify=False).text
    pass


def Do():
    data = ts.google("Me", from_language="en", to_language="ru")
    # print(ts._google.api_url)
    print(data)


# create_countries_resource("ru", 1, 1)
# create_countries_resource("uk", 1, 2)
# create_cities_resource("ru", cities_last_index, 1)
# create_cities_resource("uk", cities_last_index, 2)
# create_languages_resource("ru", 1, 1)
# create_languages_resource("uk", 1, 2)


def create_eng_localization():
    create_countries_resource("en", 1, 0)
    create_cities_resource("en", 1, 0)
    create_languages_resource("en", 1, 0)


def load_eng_localization():
    update_countries("en")
    update_cities("en")
    update_languages("en")


create_eng_localization()
# load_eng_localization()
update_languages("en")