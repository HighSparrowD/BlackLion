namespace WebApi.Entities.SecondaryEntities
{
    public class PromoCode
    {
        public int Id { get; set; }
        public bool UsedOnlyInRegistration { get; set; }
        public string Promo { get; set; }
        public int Points { get; set; }
        public int PersonalityPoints { get; set; }
        public int SecondChance { get; set; }
        public int TheValentine { get; set; }
        public int TheDetector { get; set; }
        public int Nullifier { get; set; }
        public int CardDeckMini { get; set; }
        public int CardDeckPlatinum { get; set; }
    }
}
