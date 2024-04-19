import datetime


class RecentUpdates:
    def __init__(self, data_dict):
        self.recentFeedbackCount: int = data_dict["recentFeedbackCount"]
        self.recentReportCount: int = data_dict["recentReportCount"]
        self.verificationRequestCount: int = data_dict["verificationRequestCount"]
        self.pendingAdvertisementCount: int = data_dict["pendingAdvertisementCount"]
        self.pendingAdventureCount: int = data_dict["pendingAdventureCount"]

class RecentReports:
    def __init__(self, data_dict):
        self.id: int = data_dict['id']
        self.senderId: int = data_dict['senderId']
        self.userId: int = data_dict['userId']
        self.adventureId: int = data_dict['adventureId']
        self.adminId: int = data_dict['adminId']
        self.text: str = data_dict['text']
        self.reason: str = data_dict['reason']
        self.insertedUtc: datetime.datetime = data_dict['insertedUtc']

    @staticmethod
    def unpack(datas) -> list[any] | None:
        return [RecentReports(data_dict) for data_dict in datas]
