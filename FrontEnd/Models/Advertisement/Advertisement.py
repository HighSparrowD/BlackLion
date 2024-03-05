from typing import Union

from Models.ApiModel import ApiModel

# create, update
class AdvertisementNew(ApiModel):
    def __init__(self):
        self.sponsorId: Union[int, None]
        self.name: Union[str, None]
        self.text: Union[str, None]
        self.targetAudience: Union[str, None]
        self.media: Union[str, None]
        self.priority: Union[str, None]
        self.mediaType: Union[str, None]

class AdvertisementUpdate(ApiModel):
    def __init__(self, id: int, sponsorId: int, name: str, text: str, targetAudience: str, media: str, priority: str, mediaType: str):
        self.id = id
        self.sponsorId = sponsorId
        self.name = name
        self.text = text
        self.targetAudience = targetAudience
        self.media = media
        self.priority = priority
        self.mediaType = mediaType

class AdvertisementItem:
    def __init__(self, advertisement_dict):
        self.id = advertisement_dict["id"]
        self.name = advertisement_dict["name"]


class Advertisement:
    def __init__(self, advertisement_dict):
        self.id: int = advertisement_dict["id"]
        self.sponsorId: int = advertisement_dict["sponsorId"]
        self.text: str = advertisement_dict["text"]
        self.name: str = advertisement_dict["name"]
        self.targetAudience: Union[str, None] = advertisement_dict["targetAudience"]
        self.media: Union[str, None] = advertisement_dict["media"]
        self.show: bool = advertisement_dict["show"]
        self.updated: bool = advertisement_dict["updated"]
        self.priority: str = advertisement_dict["priority"]
        self.mediaType: str = advertisement_dict["mediaType"]
