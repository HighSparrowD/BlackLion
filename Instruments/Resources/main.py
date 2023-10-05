import pandas


def create_registration_resources():
    create_generic_resources("Registration")


def create_settings_resources():
    create_generic_resources("Settings")


def create_currency_setter_resources():
    create_generic_resources("CurrencySetter")


def create_shop_resources():
    create_generic_resources("Shop")


def create_rt_resources():
    create_generic_resources("RT")


def create_feedback_module_resources():
    create_generic_resources("FeedbackModule")


def create_report_module_resources():
    create_generic_resources("ReportModule")


def create_requester_module_resources():
    create_generic_resources("Requester")


def create_familiator_resources():
    create_generic_resources("Familiator")


def create_adventurer_resources():
    create_generic_resources("Adventurer")


def create_generic_resources(location):
    en = {}
    ru = {}
    uk = {}

    file = pandas.read_csv(f"./Inputs/{location}.csv", usecols=["Name", "ENG", "RUS", "UKR"])
    resources = file.drop_duplicates().values.tolist()

    for res in resources:
        en[res[0]] = res[1]
        ru[res[0]] = res[2]
        uk[res[0]] = res[3]

    en_dataframe = pandas.DataFrame(en, index=[0])
    ru_dataframe = pandas.DataFrame(ru, index=[0])
    uk_dataframe = pandas.DataFrame(uk, index=[0])

    # Save output
    en_dataframe.to_csv(f"./Outputs/{location}EN.csv", index=False)
    ru_dataframe.to_csv(f"./Outputs/{location}RU.csv", index=False)
    uk_dataframe.to_csv(f"./Outputs/{location}UK.csv", index=False)


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


# create_registration_resources()
# create_prices_resource()
# create_settings_resources()
create_currency_setter_resources()
