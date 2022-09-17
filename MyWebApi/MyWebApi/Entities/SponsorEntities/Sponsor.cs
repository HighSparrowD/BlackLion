using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.SponsorEntities
{
    public class Sponsor
    {
        [Key]
        public long Id { get; set; }
        public string Username { get; set; }
        public int UserMaxAdCount { get; set; }
        public int UserMaxAdViewCount { get; set; }
        public bool IsPostponed { get; set; }
        public bool IsAwaiting { get; set; }
        public int UserAppLanguage { get; set; }
        public long SponsorContactInfoId{ get; set; }
        public double AverageRating { get; set; }
        public virtual List<SponsorLanguage> Languages { get; set; }
        public virtual ContactInfo ContactInfo { get; set; }
        public virtual List<Ad> SponsorAds { get; set; }
        public virtual List<Event> SponsorEvents { get; set; }
    }
}
