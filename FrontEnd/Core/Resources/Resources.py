import pandas


def get_registrator_localization(lang) -> dict[str, str]:
    return get_generic_localization("Registration", lang)


def get_settings_localization(lang) -> dict[str, str]:
    return get_generic_localization("Settings", lang)


def get_currency_setter_localization(lang) -> dict[str, str]:
    return get_generic_localization("CurrencySetter", lang)


def get_adventurer_localization(lang) -> dict[str, str]:
    return get_generic_localization("Adventurer", lang)


def get_familiator_localization(lang) -> dict[str, str]:
    return get_generic_localization("Familiator", lang)


def get_feedback_module_localization(lang) -> dict[str, str]:
    return get_generic_localization("FeedbackModule", lang)


def get_report_module_localization(lang) -> dict[str, str]:
    return get_generic_localization("ReportModule", lang)


def get_requester_module_localization(lang) -> dict[str, str]:
    return get_generic_localization("Requester", lang)


def get_rt_module_localization(lang) -> dict[str, str]:
    return get_generic_localization("RT", lang)


def get_shop_module_localization(lang) -> dict[str, str]:
    return get_generic_localization("Shop", lang)


def get_test_module_localization(lang) -> dict[str, str]:
    return get_generic_localization("TestModule", lang)


def get_generic_localization(location, lang) -> dict[str, str]:
    path = f"Core/Resources/{location}/{location}{lang}.csv"

    file = pandas.read_csv(path, encoding="utf-8")

    file = file.applymap(lambda x: x.replace("\\n", "\n") if isinstance(x, str) else x)

    resources = file.to_dict(orient='records')

    return resources[0]


def get_prices(currency):
    path = f"Core/Resources/Prices/Prices{currency}.csv"

    file = pandas.read_csv(path)
    resources = file.to_dict(orient='records')

    return resources[0]


def get_tests_prices(currency):
    path = f"Core/Resources/Prices/TestPrices{currency}.csv"

    file = pandas.read_csv(path)
    resources = file.to_dict(orient='records')

    return resources[0]
