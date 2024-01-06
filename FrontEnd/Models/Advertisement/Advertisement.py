from typing import Union

from Models.ApiModel import ApiModel

# create, update
class AdvertisementNew(ApiModel):
    def __init__(self, sponsor_id: int, text: str, target_audience: str, media: str, priority: str, media_type: str):
        self.sponsorId: int = sponsor_id
        self.text: str = text
        self.targetAudience: str = target_audience
        self.media: str = media
        self.priority: str = priority
        self.mediaType: str = media_type

class AdvertisementItem:
    def __init__(self, advertisement_dict):
        self.id = advertisement_dict["id"]
        self.text = advertisement_dict["text"]


class Advertisement:
    def __init__(self, advertisement_dict):
        self.id: int = advertisement_dict["id"]
        self.userId: int = advertisement_dict["userId"]
        self.text: str = advertisement_dict["text"]
        self.targetAudience: Union[str, None] = advertisement_dict["targetAudience"]
        self.media: Union[str, None] = advertisement_dict["media"]
        self.show: bool = advertisement_dict["show"]
        self.updated: bool = advertisement_dict["updated"]
        self.priority: str = advertisement_dict["priority"]
        self.mediaType: str = advertisement_dict["mediaType"]
