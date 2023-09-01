import pandas


def get_registrator_localization(lang):
    path = f"Core/Resources/Registration{lang}.csv"

    file = pandas.read_csv(path, encoding="utf-8")

    file = file.applymap(lambda x: x.replace("\\n", "\n") if isinstance(x, str) else x)

    resources = file.to_dict(orient='records')

    return resources[0]


def get_prices(currency):
    path = f"Core/Resources/Prices{currency}.csv"

    file = pandas.read_csv(path)
    resources = file.to_dict(orient='records')

    return resources[0]

def get_tests_prices(currency):
    path = f"Core/Resources/TestPrices{currency}.csv"

    file = pandas.read_csv(path)
    resources = file.to_dict(orient='records')

    return resources[0]
