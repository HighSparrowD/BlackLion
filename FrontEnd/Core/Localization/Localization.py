import pandas


def get_registrator_localization(lang):
    path = f"Core/Localization/Registration{lang}.csv"

    file = pandas.read_csv(path)
    resources = file.to_dict(orient='records')

    return resources[0]
