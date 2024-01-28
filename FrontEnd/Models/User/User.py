from typing import Union

from Models.ApiModel import ApiModel


class UserNew(ApiModel):
    def __init__(self):
        self.id: Union[int, None] = None
        self.name: Union[str, None] = None
        self.realName: Union[str, None] = None
        self.description: Union[str, None] = None
        self.appLanguage: Union[str, None] = None
        self.media: Union[str, None] = None
        self.mediaType: Union[str, None] = None
        self.country: Union[int, None] = None
        self.city: Union[int, None] = None
        self.languages: Union[list[int], None] = None
        self.reason: Union[str, None] = None
        self.age: Union[int, None] = None
        self.gender: Union[str, None] = None
        self.languagePreferences: Union[list[int], None] = None
        self.locationPreferences: Union[list[int], None] = None
        self.agePrefs: Union[list[int], None] = None
        self.communicationPrefs: Union[int, None] = None
        self.genderPrefs: Union[str, None] = None
        self.voice: Union[str, None] = None
        self.text: Union[str, None] = None
        self.tags: Union[str, None] = None
        self.usesOcean: Union[bool, None] = None
        self.promo: Union[str, None] = None
        self.wasChanged: Union[bool, None] = None


class UserSettings:
    def __init__(self, settings_dict: dict[str, any]):
        self.usesOcean: bool = settings_dict["usesOcean"]
        self.shouldFilterUsersWithoutRealPhoto: bool = settings_dict["shouldFilterUsersWithoutRealPhoto"]
        self.shouldConsiderLanguages: bool = settings_dict["shouldConsiderLanguages"]
        self.shouldComment: bool = settings_dict["shouldComment"]
        self.shouldSendHints: bool = settings_dict["shouldSendHints"]
        self.increasedFamiliarity: bool = settings_dict["increasedFamiliarity"]
        self.isFree: bool = settings_dict["isFree"]
        self.hasPremium: bool = settings_dict["hasPremium"]
        self.language: bool = settings_dict["language"]


class UserInfo:
    def __init__(self, user_dict: dict[str, any]):
        self.id: Union[int, None] = user_dict["id"]
        self.username: Union[str, None] = user_dict["username"]
        self.realName: Union[str, None] = user_dict["realName"]
        self.description: Union[str, None] = user_dict["description"]
        self.rawDescription: Union[str, None] = user_dict["rawDescription"]
        self.language: Union[str, None] = user_dict["language"]
        self.media: Union[str, None] = user_dict["media"]
        self.mediaType: Union[str, None] = user_dict["mediaType"]
        self.country: Union[int, None] = user_dict["country"]
        self.city: Union[int, None] = user_dict["city"]
        self.countryLang: Union[int, None] = user_dict["countryLang"]
        self.cityLang: Union[int, None] = user_dict["cityLang"]
        self.languages: Union[list[int], None] = user_dict["languages"]
        self.reason: Union[str, None] = user_dict["reason"]
        self.age: Union[int, None] = user_dict["age"]
        self.gender: Union[str, None] = user_dict["gender"]
        self.languagePreferences: Union[list[int], None] = user_dict["languagePreferences"]
        self.locationPreferences: Union[list[int], None] = user_dict["locationPreferences"]
        self.agePrefs: Union[list[int], None] = user_dict["agePrefs"]
        self.communicationPrefs: Union[int, None] = user_dict["communicationPrefs"]
        self.genderPrefs: Union[str, None] = user_dict["genderPrefs"]
        self.voice: Union[str, None] = user_dict["voice"]
        self.text: Union[str, None] = user_dict["text"]
        self.tags: Union[str, None] = user_dict["tags"]
        self.identityType: Union[str, None] = user_dict["identityType"]
        self.hasPremium: Union[str, None] = user_dict["hasPremium"]
