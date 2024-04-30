import abc
import datetime
from Models.ApiModel import ApiModel


class RecentUpdates:
    def __init__(self, data_dict):
        self.recentFeedbackCount: int = data_dict["recentFeedbackCount"]
        self.recentReportCount: int = data_dict["recentReportCount"]
        self.verificationRequestCount: int = data_dict["verificationRequestCount"]
        self.pendingAdvertisementCount: int = data_dict["pendingAdvertisementCount"]
        self.pendingAdventureCount: int = data_dict["pendingAdventureCount"]


class AdminModuleModel(abc.ABC):
    @classmethod
    def unpack(cls, datas) -> list[any] | None:
        return [cls(data_dict) for data_dict in datas]


class RecentReports(AdminModuleModel):
    def __init__(self, data_dict):
        self.id: int = data_dict['id']
        self.senderId: int = data_dict['senderId']
        self.userId: int = data_dict['userId']
        self.adventureId: int = data_dict['adventureId']
        self.adminId: int = data_dict['adminId']
        self.text: str = data_dict['text']
        self.reason: str = data_dict['reason']
        self.insertedUtc: datetime.datetime = data_dict['insertedUtc']


class Feedbacks(AdminModuleModel):
    def __init__(self, data_dict):
        self.id: int = data_dict['id']
        self.userId: int = data_dict['userId']
        self.adminId: int | None = data_dict['adminId']
        self.text: str = data_dict['text']
        self.insertedUtc: str = data_dict['insertedUtc']
        self.reason: str = data_dict['reason']


class GroupedFeedback(AdminModuleModel):
    def __init__(self, data_dict):
        self.username: str = data_dict['username']
        self.feedbacks: list = Feedbacks.unpack(data_dict['feedbacks'])


class VerificationRequest(AdminModuleModel):
    def __init__(self, data_dict):
        self.id: int = data_dict['id']
        self.userId: int | None = data_dict['userId']
        self.adminId: int | None = data_dict['adminId']
        self.state: str | None = data_dict['state']
        self.media: str = data_dict['media']
        self.mediaType: str = data_dict['mediaType']
        self.gesture: str | None = data_dict['gesture']
        self.confirmationType: str = data_dict['confirmationType']


class UserMedia(AdminModuleModel):
    def __init__(self, data_dict):
        self.userId: int | None = data_dict['userId']
        self.media: str = data_dict['media']
        self.mediaType: str = data_dict['mediaType']


class ResolveVerificationRequest(ApiModel):
    def __init__(self, id: int, adminId: int, status: str, comment: str = None):
        self.id = id
        self.adminId = adminId
        self.status = status
        self.comment = comment
