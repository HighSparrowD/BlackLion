class RecentUpdates:
    def __init__(self, data_dict):
        self.recentFeedbackCount: int = data_dict["recentFeedbackCount"]
        self.recentReportCount: int = data_dict["recentReportCount"]
        self.verificationRequestCount: int = data_dict["verificationRequestCount"]
        self.pendingAdvertisementCount: int = data_dict["pendingAdvertisementCount"]
        self.pendingAdventureCount: int = data_dict["pendingAdventureCount"]
