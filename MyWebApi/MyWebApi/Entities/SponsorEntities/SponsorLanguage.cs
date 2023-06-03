using WebApi.Entities.SecondaryEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Entities.SponsorEntities
{
    public class SponsorLanguage
    {
        [Key]
        public long Id { get; set; }
        public long SponsorId { get; set; }
        public int LanguageId { get; set; }
        public int LanguageClassLocalisationId { get; set; }
        public short Level{ get; set; }
        public virtual Language Language { get; set; }
    }
}
