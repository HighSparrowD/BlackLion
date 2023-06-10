namespace WebApi.Enums
{
    public static class SystemEnums
    {
        public enum Localisations
        {
            English = 0,
            Russian = 1,
            Ukrainian = 2
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
            Nullifier = 8,
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
