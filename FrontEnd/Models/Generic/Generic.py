from typing import Union

from Models.ApiModel import ApiModel


class LocalizedEnum(ApiModel):
    def __init__(self, enum_dict):
        self.id: Union[int, None] = enum_dict["id"]
        self.name: Union[str, None] = enum_dict["name"]
