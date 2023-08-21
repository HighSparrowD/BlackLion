import pandas


def get_registrator_localization(lang):
    path = f"Core/Resources/Registration{lang}.csv"

    file = pandas.read_csv(path, encoding="utf-8")
    resources = file.to_dict(orient='records')

    return resources[0]


def get_prices(currency):
    pass
