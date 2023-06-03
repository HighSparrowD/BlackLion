using WebApi.Entities.AdminEntities;
using System.Collections.Generic;
using WebApi.Enums;

namespace WebApi.Entities.TestEntities
{
    public class UploadTest
    {
        public long Id { get; set; }
        public AppLanguage ClassLocalisationId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public short TestType { get; set; }
        public int Price{ get; set; }
        public int CanBePassedInDays { get; set; }
        public List<UploadTestQuestion> Questions { get; set; }
        public List<UploadTestResult> Results { get; set; }
    }
}
