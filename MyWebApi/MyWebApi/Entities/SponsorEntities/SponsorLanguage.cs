using WebApi.Entities.SecondaryEntities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Enums;

namespace WebApi.Entities.SponsorEntities
{
    public class SponsorLanguage
    {
        [Key]
        public long Id { get; set; }
        public long SponsorId { get; set; }
        public AppLanguage Lang { get; set; }
        public short Level{ get; set; }
        public virtual Language Language { get; set; }
    }
}
