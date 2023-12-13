namespace WebApi.Models.Models.User
{
    public class GetFullTestData
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Enums.Enums.User.OceanStats? TestType { get; set; }
    }
}
