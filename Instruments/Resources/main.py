import pandas


def create_registration_resources():
    en = {}
    ru = {}
    uk = {}

    file = pandas.read_csv("./Inputs/Registration - Sheet1.csv", usecols=["Name", "EN", "RU", "UKR"])
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


create_registration_resources()