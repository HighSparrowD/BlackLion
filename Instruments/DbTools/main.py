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


def update_countries(language, start_index=0, loc_index=0):
    global countries_last_index
    ids = []
    countryNames = []
    languages = []
    priorities = [] #Dummy column. Set manually afterwards


    file = pandas.read_csv("worldcities.csv", usecols=["country", "priority"])
    countries = sorted(file.drop_duplicates().values.tolist())
    countries_dicts = []
    i = start_index

    if loc_index == 0:
        for country in countries:

            ids.append(i)
            countryNames.append(country[0])
            languages.append(langs[loc_index])
            priorities.append("")

            d = {
                "id": i,
                "countryName": country[0],
                "lang": langs[loc_index]
            }
            countries_dicts.append(d)
            i += 1
    else:
        for country in countries:

            translated_name = ts.deepl(country[0], from_language="en", to_language=language)

            ids.append(i)
            countryNames.append(translated_name)
            languages.append(langs[loc_index])
            priorities.append("")

            d = {
                "id": i,
                "countryName": translated_name,
                "lang": langs[loc_index]
            }
            countries_dicts.append(d)
            i += 1

    data = {
        "Id": ids,
        "Name": countryNames,
        "Language": languages,
        "Priority": priorities
    }

    #Save output
    dataframe = pandas.DataFrame(data)
    dataframe.to_csv(f"./Output/Countries{language.upper()}.csv", index=False)

    #Convert data to json
    # json_data = json.dumps(countries_dicts)
    #
    # s = requests.post("https://localhost:44381/UpdateCountries", json_data, headers={
    #         "Content-Type": "application/json"},   verify=False).text

    countries_last_index = i + 1


def update_cities(language, start_index=0, loc_index=0):
    global cities_last_index

    ids = []
    countryNames = []
    countryIds = []
    languages = []

    cities = pandas.read_csv("worldcities.csv", usecols=["city", "country"])
    file = sorted(pandas.read_csv("worldcities.csv", usecols=["country"]).drop_duplicates().values.tolist())
    countries = []
    d = []
    i = start_index

    for f in file:
        countries.append(f[0])

    if loc_index == 0:
        for index, data in cities.iterrows():

            ids.append(i)
            countryNames.append(data[0])
            countryIds.append(countries.index(data[1]) + 1)
            languages.append(langs[loc_index])

            d.append({
                "Id": i,
                "CityName": data[0],
                "CountryId": countries.index(data[1]) + 1,
                "lang": langs[loc_index]
            })
            i += 1

    else:
        for index, data in cities.iterrows():

            translated_name = ts.deepl(data[0], from_language="en", to_language=language)

            ids.append(i)
            countryNames.append(translated_name)
            countryIds.append(countries.index(data[1]) + 1)
            languages.append(langs[loc_index])

            d.append({
                "Id": i,
                "CityName": translated_name,
                "CountryId": countries.index(data[1]) + 1,
                "lang": langs[loc_index]
            })
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
    dataframe.to_csv(f"./Output/Cities{language.upper()}.csv", index=False)

    # json_data = json.dumps(d)
    #
    # s = requests.post("https://localhost:44381/UpdateCities", json_data, headers={
    #         "Content-Type": "application/json"},   verify=False).text

    cities_last_index = i + 1


def update_languages(language, start_index=0, loc_index=0):
    global languages_last_index

    ids = []
    languageNames = []
    languageNativeNames = []
    languages_translation = []
    priorities = []

    languages = pandas.read_csv("Languages.csv", usecols=["Language name", "Native name", "Priority"]).drop_duplicates().values.tolist()
    d = []
    i = start_index

    if loc_index == 0:
        for lang in languages:

            ids.append(i)
            languageNames.append(lang[0].strip())
            languageNativeNames.append(lang[0].strip())
            languages_translation.append(langs[loc_index])
            priorities.append(lang[2])

            d.append({
                "Id": i,
                "LanguageName": lang[0].strip(),
                "LanguageNameNative": lang[0].strip(),  # lang[1] TODO: Make it work with a native one
                "lang": langs[loc_index],
                "Priority": lang[2]
            })
            i += 1

    else:
        for lang in languages:

            translated_lang = ts.deepl(lang[0].strip(), from_language="en", to_language=language)

            ids.append(i)
            languageNames.append(translated_lang)
            languageNativeNames.append(lang[0].strip())
            languages_translation.append(langs[loc_index])
            priorities.append(lang[2])

            d.append({
                    "Id": i,
                    "LanguageName": translated_lang,
                    "LanguageNameNative": lang[0].strip(), #lang[1] TODO: Make it work with a native one
                    "lang": loc_index,
                    "Priority": lang[2]
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
    dataframe.to_csv(f"./Output/Languages{language.upper()}.csv", index=False)

    # data = json.dumps(d) #ensure_ascii=False
    # s = requests.post("https://localhost:44381/UpdateLanguages", data, headers={
    #     "Content-Type": "application/json"}, verify=False).text

    languages_last_index = i + 1


def Do():
    data = ts.deepl("Me", from_language="en", to_language="ru")
    print(data)


update_countries("en", 1, 0)
# update_countries("ru", 1, 1)
# update_countries("uk", 1, 2)
update_cities("en", 1, 0)
# update_cities("ru", cities_last_index, 1)
# update_cities("uk", cities_last_index, 2)
update_languages("en", 1, 0)
# update_languages("ru", 1, 1)
# update_languages("uk", 1, 2)