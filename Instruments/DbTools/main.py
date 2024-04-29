import json
import math

import pandas
import requests
# from deep_translator import GoogleTranslator

countries_last_index = 0
cities_last_index = 0
languages_last_index = 0

langs = {
    1: "EN",
    2: "RU",
    3: "UK",
}

currencies = {
    12: "USD",
    13: "EUR",
    14: "UAH",
    15: "RUB",
    16: "CZK",
    17: "PLN",
    18: "Points"
}


initial_prompt = """У меня есть вопрос из психологического теста. Он звучит так: '{}' ответы на него: {}.
Мне нужно чтобы ты описал информацию полученную о человеке которую мы получим в случае если он ответит каждым из возможных вариантов ответа. 
Пример ответа: 
Да - боится темноты
Иногда - неоднозначно 
Нет - не боится темноты

Твои ответы должны содержать как можно меньше слов и как можно больше смысла. (не использую слов "возможно", "вероятно" и тд). Если ответ может быть только неоднозначным - пожалуйста, оставляй его пустым
"""


def generate_tag_prompt():
    file = pandas.read_csv(f"Inputs/TestsRU - Tests.csv")
    file = get_file_data(file)
    question_stacks = []
    prompts = ""

    count = 1

    for test in file:
        question_stacks.append(load_questions(test[0], "RU"))

    for question_stack in question_stacks:
        for question in question_stack:
            q = question["text"]
            answers = ""
            for answer in question["answers"]:
                answers += f' {answer["text"]} '

            prompts += f"{count}. {initial_prompt.format(q, answers)}\n\n"
            count += 1

    with open('prompts.txt', 'w+', encoding='utf-8') as f:
        f.write(prompts)


# def create_countries_resource(language, start_index=0, loc_index=0):
#     global countries_last_index
#     ids = []
#     countryNames = []
#     languages = []
#     priorities = []
#
#     try:
#         line_count = sum(1 for line in open(f"./Output/Countries{language.upper()}.csv", encoding='utf-8'))
#     except:
#         line_count = 0
#
#     # If all countries are already present
#     if line_count == 254:
#         return
#     elif line_count < 254 and line_count != 0:
#         file = pandas.read_csv(f"worldcitiesEN.csv", usecols=["country", "priority"], skiprows=range(1, line_count))
#     else:
#         file = pandas.read_csv(f"worldcitiesEN.csv", usecols=["country", "priority"])
#
#     countries = sorted(file.drop_duplicates(subset="country").values.tolist())
#     i = start_index
#
#     if loc_index == 0:
#         for country in countries:
#             ids.append(i)
#             countryNames.append(country[0].lower())
#             languages.append(langs[loc_index])
#             priorities.append(5 if (isinstance(country[1], float) and country[1] not in range(1, 5)) else int(country[1]))
#
#             i += 1
#     else:
#         translator = GoogleTranslator(source="auto", target=language)
#         for country in countries:
#             ids.append(i)
#             countryNames.append(translator.translate(country[0]).lower())
#             languages.append(langs[loc_index])
#             priorities.append(5 if (isinstance(country[1], float) and country[1] not in range(1, 5)) else int(country[1]))
#
#             i += 1
#
#             data = {
#                 "Id": ids,
#                 "Name": countryNames,
#                 "Language": languages,
#                 "Priority": priorities
#             }
#
#             # Save output
#             dataframe = pandas.DataFrame(data)
#             dataframe = dataframe.fillna(value=0)
#             dataframe.to_csv(f"./Output/Countries{language.upper()}.csv", index=False)
#
#             print(i)
#
#     data = {
#         "Id": ids,
#         "Name": countryNames,
#         "Language": languages,
#         "Priority": priorities
#     }
#
#     # Save output
#     dataframe = pandas.DataFrame(data)
#     dataframe = dataframe.fillna(value=0)
#     dataframe.to_csv(f"./Output/Countries{language.upper()}.csv", index=False)
#
#     countries_last_index = i + 1


# def create_cities_resource(language, start_index=0, loc_index=0):
#     global cities_last_index
#
#     existing_file_path = f"./Output/Cities{language.upper()}.csv"
#
#     ids = []
#     cityNames = []
#     cityIds = []
#     languages = []
#
#     try:
#         line_count = sum(1 for line in open(existing_file_path, encoding='utf-8'))
#     except:
#         line_count = 0
#
#     # Load previously translated data
#     if line_count != 0:
#         existing_cities = pandas.read_csv(existing_file_path).values.tolist()
#
#         for city in existing_cities:
#             ids.append(city[0])
#             cityNames.append(city[1])
#             cityIds.append(city[2])
#             languages.append(city[3])
#
#         cities = pandas.read_csv(f"worldcitiesEN.csv", usecols=["city", "country"], skiprows=range(1, line_count))
#         i = line_count
#     else:
#         cities = pandas.read_csv(f"worldcitiesEN.csv", usecols=["city", "country"])
#         i = 1
#
#     file = sorted(pandas.read_csv(f"worldcitiesEN.csv", usecols=["country"]).drop_duplicates().values.tolist())
#     countries = []
#
#     for f in file:
#         countries.append(f[0])
#
#     if loc_index == 0:
#         for index, data in cities.iterrows():
#             cName = data[0]
#
#             if isinstance(data[0], float):
#                 cName = "None"
#
#             ids.append(i)
#             cityNames.append(cName.lower())
#             cityIds.append(countries.index(data[1]) + 1)
#             languages.append(langs[loc_index])
#
#             print(f"{i} -> {cName.lower()}")
#             i += 1
#     else:
#         translator = GoogleTranslator(source="auto", target=language)
#         for index, data in cities.iterrows():
#             t = translator.translate(data[0]).lower()
#             ids.append(i)
#             cityNames.append(t)
#             cityIds.append(countries.index(data[1]) + 1)
#             languages.append(langs[loc_index])
#
#             i += 1
#
#             data = {
#                 "Id": ids,
#                 "Name": cityNames,
#                 "Country Id": cityIds,
#                 "Language": languages
#             }
#
#             # Save output
#             dataframe = pandas.DataFrame(data)
#             dataframe = dataframe.fillna(value=0)
#             dataframe.to_csv(f"./Output/Cities{language.upper()}.csv", index=False)
#
#             print(f"{i} -> {t}")
#
#     data = {
#         "Id": ids,
#         "Name": cityNames,
#         "Country Id": cityIds,
#         "Language": languages
#     }
#
#     # Save output
#     dataframe = pandas.DataFrame(data)
#     dataframe = dataframe.fillna(value=0)
#     dataframe.to_csv(f"./Output/Cities{language.upper()}.csv", index=False)
#
#     cities_last_index = i + 1
#
#
# def create_languages_resource(language, start_index=0, loc_index=0):
#     global languages_last_index
#
#     ids = []
#     languageNames = []
#     languageNativeNames = []
#     languages_translation = []
#     priorities = []
#
#     languages = pandas.read_csv("Languages.csv",
#                                 usecols=["Language name", "Native name", "Priority"]).drop_duplicates().values.tolist()
#     d = []
#     i = start_index
#
#     if loc_index == 0:
#         for lang in languages:
#             ids.append(i)
#             languageNames.append(lang[0].strip().lower())
#             languageNativeNames.append(lang[0].strip().lower())  # lang[1] TODO: Make it work with a native one
#             languages_translation.append(langs[loc_index])
#             priorities.append(5 if (isinstance(lang[2], float) and lang[2] not in range(1, 5)) else int(lang[2]))
#
#             i += 1
#
#     else:
#         translator = GoogleTranslator(source="en", target=language)
#         for lang in languages:
#             translated_lang = translator.translate(lang[0].strip()).lower()
#
#             ids.append(i)
#             languageNames.append(translated_lang)
#             languageNativeNames.append(lang[0].strip().lower())  # lang[1] TODO: Make it work with a native one
#             languages_translation.append(langs[loc_index])
#             priorities.append(5 if (isinstance(lang[2], float) and lang[2] not in range(1, 5)) else int(lang[2]))
#
#             d.append({
#                 "Id": i,
#                 "LanguageName": translated_lang,
#                 "LanguageNameNative": lang[0].strip(),
#                 "Language": loc_index,
#                 "Priority": priorities
#             }
#             )
#             i += 1
#
#     data = {
#         "Id": ids,
#         "Name": languageNames,
#         "Native Name": languageNativeNames,
#         "Language": languages_translation,
#         "Priority": priorities
#     }
#
#     # Save output
#     dataframe = pandas.DataFrame(data)
#     dataframe = dataframe.fillna(value=0)
#     dataframe.to_csv(f"./Output/Languages{language.upper()}.csv", index=False)
#
#     languages_last_index = i + 1


def update_countries(language):
    file = pandas.read_csv(f"./Output/Countries{language.upper()}.csv", usecols=["Id", "Name", "Language", "Priority"])
    countries = sorted(file.drop_duplicates(subset="Name").values.tolist())
    countries_dicts = []

    for country in countries:
        d = {
            "id": country[0],
            "countryName": country[1],
            "lang": country[2],
            "priority": country[3],
        }

        countries_dicts.append(d)

    # Convert data to json
    json_data = json.dumps(countries_dicts)

    s = requests.post("https://localhost:44381/UpdateCountries", json_data, headers={
        "Content-Type": "application/json"}, verify=False).text
    pass


def update_cities(language):
    file = pandas.read_csv(f"./Output/Cities{language.upper()}.csv", usecols=["Id", "Name", "Country Id", "Language"])
    cities = sorted(file.drop_duplicates(subset="Name").values.tolist())
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
        "Content-Type": "application/json"}, verify=False).text
    pass


def update_languages(language):
    file = pandas.read_csv(f"./Output/Languages{language.upper()}.csv",
                           usecols=["Id", "Name", "Native Name", "Language", "Priority"])
    languages = sorted(file.drop_duplicates(subset="Name").values.tolist())
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

    data = json.dumps(languages_dict)  # ensure_ascii=False
    s = requests.post("https://localhost:44381/UpdateLanguages", data, headers={
        "Content-Type": "application/json"}, verify=False).text
    pass


# create_countries_resource("ru", 1, 1)
# create_countries_resource("uk", 1, 2)
# create_cities_resource("ru", cities_last_index, 1)
# create_cities_resource("uk", cities_last_index, 2)
# create_languages_resource("ru", 1, 1)
# create_languages_resource("uk", 1, 2)


# def create_eng_localization():
#     create_countries_resource("en", 1, 0)
#     # create_cities_resource("en", 1, 0)
#     create_languages_resource("en", 1, 0)
#
#
# def create_ru_localization():
#     create_countries_resource("ru", 1, 1)
#     # create_cities_resource("ru", 1, 1)
#     create_languages_resource("ru", 1, 1)
#
#
# def create_uk_localization():
#     create_countries_resource("uk", 1, 1)
#     # create_cities_resource("uk", 1, 1)
#     create_languages_resource("uk", 1, 1)


def add_tests(lang):
    test_template = []

    test_template = load_test_data(test_template, lang.upper())

    data = json.dumps(test_template)
    print(requests.post("https://localhost:44381/UploadTests", data, headers={"Content-Type": "application/json"}, verify=False).text)


def generate_test_prices():
    file = pandas.read_csv(f"Inputs/Tests{langs[2]} - Tests.csv")
    file = get_file_data(file)

    data = {}

    currency_index = 5
    for currency in currencies.values():
        data[f"TestPrices{currency}"] = {}
        for test in file:
            data[f"TestPrices{currency}"][test[0]] = test[currency_index]

        currency_index += 1

    # Save output
    for index in data.keys():
        price_file = data[index]
        dataframe = pandas.DataFrame(price_file, index=[0])
        dataframe = dataframe.fillna(value=0)
        dataframe.to_csv(f"../../FrontEnd/Core/Resources/Prices/{index}.csv", index=False)

    print(f"Test prices are loaded")


def load_test_data(testTemplate, lang) -> dict:
    file = pandas.read_csv(f"Inputs/Tests{lang} - Tests.csv")
    file = get_file_data(file)

    for test in file:

        test_data = {
            "id": test[0],
            "language": lang,
            "name": test[1],
            "description": test[2] if type(test[2]) is not float else None,
            "testType": test[3] if type(test[3]) is not float else None,
            "canBePassedInDays": test[4],
            "questions": load_questions(test[0], lang),
            "results": load_results(test[0], lang),
            "scales": load_scales(test[0], lang)
        }
        testTemplate.append(test_data)

    return testTemplate


def load_questions(testId, lang) -> list:
    file = pandas.read_csv(f"Inputs/Tests{lang} - Questions.csv")
    file = get_file_data(file)

    questions = []

    for question in file:

        scale = None
        if not isinstance(question[3], float):
            scale = question[3]

        if question[2] == testId:
            q = {
                "text": question[0],
                "scale": scale,
                "answers": load_answers(question[1], lang)
            }

            # Nullable check
            if not isinstance(question[4], float):
                q["photo"] = question[4]

            questions.append(q)

    return questions


def load_answers(questionId, lang) -> list:
    file = pandas.read_csv(f"Inputs/Tests{lang} - Answers.csv")
    file = get_file_data(file)

    answers = []

    for answer in file:
        tags = None
        # Nullable check
        if not isinstance(answer[3], float):
            tags = answer[3]

        #If each question has its own answers
        if answer[2] == questionId:
            a = {
                "text": answer[0],
                "value": answer[1],
                "tags": tags,
            }

            answers.append(a)

    return answers


def load_results(testId, lang) -> list:
    file = pandas.read_csv(f"Inputs/Tests{lang} - Results.csv")
    file = get_file_data(file)

    results = []

    for result in file:
        if result[0] == testId:

            tags = None
            # Nullable check
            if not isinstance(result[3], float):
                tags = result[3]

            r = {
                "score": int(result[1]) if not math.isnan(result[1]) else None,
                "result": result[2],
                "tags": tags
            }

            results.append(r)

    return results


def load_scales(testId, lang) -> list:
    file = pandas.read_csv(f"Inputs/Tests{lang} - Scales.csv")
    file = get_file_data(file)

    scales = []

    for scale in file:
        if scale[1] == testId:

            s = {
                "scale": scale[0],
                "minValue": scale[2] if type(scale[2]) is not float else None,
            }

            scales.append(s)

    return scales


def get_file_data(file):
    return file.values.tolist()


def load_eng_localization():
    update_countries("en")
    update_cities("en")
    update_languages("en")
    # add_tests("en")

    print(f"Localization EN is loaded")


def load_ru_localization():
    update_countries("ru")
    update_cities("ru")
    update_languages("ru")
    add_tests("ru")

    print(f"Localization RU is loaded")


def load_uk_localization():
    update_countries("uk")
    update_cities("uk")
    update_languages("uk")
    # add_tests("uk")

    print(f"Localization UK is loaded")

load_eng_localization()
load_ru_localization()
load_uk_localization()

generate_test_prices()
