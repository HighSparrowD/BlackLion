import math

import pandas
import pandas as pd


def create_registration_resources():
    en = {}
    ru = {}
    uk = {}

    file = pandas.read_csv("./Inputs/Registration.csv", usecols=["Name", "ENG", "RUS", "UKR"])
    resources = file.drop_duplicates().values.tolist()

    for res in resources:
        en[res[0]] = res[1]
        ru[res[0]] = res[2]
        uk[res[0]] = res[3]

    en_dataframe = pandas.DataFrame(en, index=[0])
    ru_dataframe = pandas.DataFrame(ru, index=[0])
    uk_dataframe = pandas.DataFrame(uk, index=[0])

    # Save output
    en_dataframe.to_csv("./Outputs/RegistrationEN.csv", index=False)
    ru_dataframe.to_csv("./Outputs/RegistrationRU.csv", index=False)
    uk_dataframe.to_csv("./Outputs/RegistrationUK.csv", index=False)


def create_prices_resource():
    usd = {}
    eur = {}
    uah = {}
    czk = {}
    pln = {}
    points = {}

    file = pandas.read_csv("./Inputs/Prices.csv", usecols=["Name", "Currency", "Price"], )
    resources = file.values.tolist()

    for res in resources:
        currency = res[1]
        if currency == "USD":
            usd[res[0]] = res[2]
        elif currency == "EUR":
            eur[res[0]] = res[2]
        elif currency == "UAH":
            uah[res[0]] = res[2]
        elif currency == "CZK":
            czk[res[0]] = res[2]
        elif currency == "PLN":
            pln[res[0]] = res[2]
        elif currency == "Points":
            points[res[0]] = res[2]

    usd_dataframe = pandas.DataFrame(usd, index=[0])
    eur_dataframe = pandas.DataFrame(eur, index=[0])
    uah_dataframe = pandas.DataFrame(uah, index=[0])
    czk_dataframe = pandas.DataFrame(czk, index=[0])
    pln_dataframe = pandas.DataFrame(pln, index=[0])
    points_dataframe = pandas.DataFrame(points, index=[0])

    # Save output
    usd_dataframe.to_csv("./Outputs/PricesUSD.csv", index=False)
    eur_dataframe.to_csv("./Outputs/PricesEUR.csv", index=False)
    uah_dataframe.to_csv("./Outputs/PricesUAH.csv", index=False)
    czk_dataframe.to_csv("./Outputs/PricesCZK.csv", index=False)
    pln_dataframe.to_csv("./Outputs/PricesPLN.csv", index=False)
    points_dataframe.to_csv("./Outputs/PricesPoints.csv", index=False)


create_registration_resources()
# create_prices_resource()
