using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable enable

namespace MyWebApi.Entities.SponsorEntities
{
    public class Sponsor
    {
        [Key]
        public long Id { get; set; }
        public string? Username { get; set; }
        public string? CodeWord { get; set; }
        public int? Age { get; set; }
        public int UserMaxAdCount { get; set; }
        public int UserMaxAdViewCount { get; set; }
        public bool IsPostponed { get; set; }
        public bool IsAwaiting { get; set; }
        public int UserAppLanguage { get; set; }
        public long? ContactInfoId { get; set; }
        public long? StatsId { get; set; }
        public virtual List<SponsorLanguage>? SponsorLanguages { get; set; }
        [ForeignKey("ContactInfoId")]
        public virtual SponsorContactInfo? SponsorContactInfo { get; set; }
        [ForeignKey("StatsId")]
        public virtual Stats? Stats { get; set; }
        public virtual List<Ad>? SponsorAds { get; set; }
        public virtual List<Event>? SponsorEvents { get; set; }
    }
}
