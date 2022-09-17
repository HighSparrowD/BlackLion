
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.SponsorEntities
{
    public class RegisterSponsor
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public int UserMaxAdCount { get; set; }
        public int UserMaxAdViewCount { get; set; }
        public bool IsPostponed { get; set; }
        public bool IsAwaiting { get; set; }
        public int UserAppLanguage { get; set; }
        public long SponsorContactInfoId { get; set; }
        public double AverageRating { get; set; }
        public long SponsorId { get; set; }
        [MaxLength(255)]
        public string Tel { get; set; }
        [MaxLength(255)]
        public string Email { get; set; }
        [MaxLength(255)]
        public string Instagram { get; set; }
        [MaxLength(255)]
        public string Facebook { get; set; }
    }
}
