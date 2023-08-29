using WebApi.Enums;

namespace WebApi.Entities.UserActionEntities
{
    public class GetFullTestData
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public OceanStats TestType { get; set; }
        public int Price{ get; set; }
    }
}
