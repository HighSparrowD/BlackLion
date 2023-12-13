namespace WebApi.Enums.Enums.Notification;

public enum NotificationType : short
{
    Like = 1, // Means request
    LikeNotification = 2, // Means the notification itself
    PremiumAcquire = 3,
    TickRequest = 4,
    PremiumEnd = 5,
    AdventureParticipation = 6,
    AdventureParticipationByCode = 7,
    ReferentialRegistration = 9,
    Other = 10 // Random achievement, Added to adventure, Removed from Adventure
               // and other stuff that does not need to be grouped
}

