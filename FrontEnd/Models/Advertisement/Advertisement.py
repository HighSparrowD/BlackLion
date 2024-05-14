import json
from datetime import datetime
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


class StatisticsGet(ApiModel):
    def __init__(self):
        self.from_: datetime = datetime.now()
        self.to: datetime = datetime.now()

    def to_json(self):
        dict = {
            "from": self.from_.strftime("%Y-%m-%d"),
            "to": self.to.strftime("%Y-%m-%d")
        }

        return json.dumps(dict)


class AdvertisementItem:
    def __init__(self, advertisement_dict):
        self.id = advertisement_dict["id"]
        self.name = advertisement_dict["name"]


class Advertisement:
    def __init__(self, advertisement_dict):
        self.id: int = advertisement_dict["id"]
        self.sponsorId: int = advertisement_dict["userId"]
        self.text: str = advertisement_dict["text"]
        self.name: str = advertisement_dict["name"]
        self.targetAudience: Union[str, None] = advertisement_dict["targetAudience"]
        self.media: Union[str, None] = advertisement_dict["media"]
        self.show: bool = advertisement_dict["show"]
        self.updated: bool = advertisement_dict["updated"]
        self.priority: str = advertisement_dict["priority"]
        self.mediaType: str = advertisement_dict["mediaType"]


class StatisticsEconomy:
    def __init__(self, stats_dict):
        self.id: int = stats_dict["id"]
        self.advertisementId: int = stats_dict["advertisementId"]
        self.payback: float = stats_dict["payback"]
        self.pricePerClick: float = stats_dict["pricePerClick"]
        self.totalPrice: float = stats_dict["totalPrice"]
        self.income: float = stats_dict["income"]
        self.created: str = stats_dict["created"]

    @staticmethod
    def unpack(stats) -> list[any] | None:
        return [StatisticsEconomy(stat) for stat in stats]

class StatisticsEngagement:
    def __init__(self, stats_dict):
        self.id: int = stats_dict["id"]
        self.advertisementId: int = stats_dict["advertisementId"]
        self.viewCount: int = stats_dict["viewCount"]
        self.averageStayInSeconds: int = stats_dict["averageStayInSeconds"]
        self.clickCount: int = stats_dict["linkClickCount"]
        self.peoplePercentage: int = stats_dict["peoplePercentage"]
        self.created: str = stats_dict["created"]

    @staticmethod
    def unpack(stats) -> list[any] | None:
        return [StatisticsEngagement(stat) for stat in stats]