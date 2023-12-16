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
    id: int
    text: str

    def __init__(self, advertisement_dict):
        self.id = advertisement_dict["id"]
        self.text = advertisement_dict["text"]
