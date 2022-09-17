using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.SponsorEntities
{
    public class SponsorLanguage
    {
        [Key]
        public long Id{ get; set; }
        public long SponsorId{ get; set; }
        public long LanguageId{ get; set; }
        public long LanguageClassLocalisationId{ get; set; }
        public short Level{ get; set; }
    }
}
