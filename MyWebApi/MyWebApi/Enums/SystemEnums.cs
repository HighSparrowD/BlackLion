namespace MyWebApi.Enums
{
    public static class SystemEnums
    {
        public enum Sections
        {
            Registration = 1,
            Familiator = 2,
            Requester = 3,
            TesterPs = 4,
            TesterIn = 4,
            RT = 5,
            Reporet = 6,
            Sponsor = 7,
            Eventer = 8,
            Shop = 9, 
            Settings = 10
        }

        public enum Localisations
        {
            English = 0,
            Russian = 1,
            Ukrainian = 2
        }

        public enum Severities
        {
            Minor = 1, 
            Moderate = 2,
            Urgent = 3
        }

        public enum LanguageLevels
        {
            A1 = 1,
            A2 = 2,
            B1 = 3,
            B2 = 4,
            C1 = 5, 
            C2 = 6
        }

        public enum Ratings
        {
            VeryPoor = 1,
            Poor = 2,
            Good = 3,
            VeryGood = 4,
            Excellent = 5
        }

        public enum EventStatuses
        {
            Created = 1,
            Updated = 2,
            Running = 3,
            Canceled = 4,
            Stopped = 5,
            Postponed = 6,
            Ended = 7
        }

        public enum NotificationReasons
        {
            Subscription = 1,
            Unsubscription = 2,
        }
    }
}
