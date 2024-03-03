from typing import Union

from Models.ApiModel import ApiModel

# create, update
class AdvertisementNew(ApiModel):
    def __init__(self, sponsorId = None, name = None, text = None, targetAudience = None, media = None, priority = None, mediaType = None):
        self.sponsorId: Union[int, None] = sponsorId  # TODO: temp solution for line 329 Adv_Mod (above)
        self.name: Union[str, None] = name
        self.text: Union[str, None] = text
        self.targetAudience: Union[str, None] = targetAudience
        self.media: Union[str, None] = media
        self.priority: Union[str, None] = priority
        self.mediaType: Union[str, None] = mediaType

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
