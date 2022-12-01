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
            TesterIn = 5,
            RT = 6,
            Reporter = 7,
            Sponsor = 8,
            Eventer = 9,
            Shop = 10, 
            Settings = 11,
            Neutral = 12
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
            Comment = 3,
            Report = 4,
        }

        public enum Currencies
        {
            Points = 1,
            PersonalityPoints = 2,
            Premium = 3, // Maybe remove in the future
            RealMoney = 4,
            SecondChance = 5,
            TheValentine = 6,
            TheDetector = 7,
            TheWhiteDetector = 8,
            CardDeckMini = 9,
            CardDeckPlatinum = 10,
            ThePersonality = 11
        }

        public enum TaskTypes
        {
            Common = 1,
            Rare = 2,
            Premium = 3
        }

        public enum PersonalityStats
        {
            PersonalityType = 1,
            EmotionalIntellect = 2,
            Reliability = 3,
            Compassion = 4,
            OpenMindedness = 5,
            Agreeableness = 6,
            SelfAwareness = 7,
            LevelsOfSense = 8,
            Intellect = 9,
            Nature = 10,
            Creativity = 11
        }

        public enum TickRequestStatus
        {
            Added = 1,
            Changed = 2,
            InProcess = 3,
            Declined = 4,
            Accepted = 5,
            Aborted = 6,
            Failed = 7
        }

        public enum PaymentCurrencies
        {
            USD = 1,
            EUR = 2,
            UAH = 3,
            RUB = 4,
            CZK = 5,
            PLN = 6,
        }
    }
}
