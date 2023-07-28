from enum import Enum

class AttendeeStatus(Enum):
    New = "New",
    NewByCode = "NewByCode",
    Accepted = "Accepted",
    Declined = "Declined",
    Unsubscribed = "Unsubscribed"
