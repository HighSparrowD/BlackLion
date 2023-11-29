using System.ComponentModel.DataAnnotations;
using WebApi.Enums;

namespace WebApi.Entities.AdventureEntities
{
    public class AdventureTemplate
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public bool IsOffline { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public string Media { get; set; }
        public MediaType MediaType { get; set; }
        public string Description { get; set; }
        public string Experience { get; set; }
        public string AttendeesDescription { get; set; }
        public string UnwantedAttendeesDescription { get; set; }
        public string Gratitude { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Duration { get; set; }
        public string Application { get; set; }
        public string Address { get; set; }
        public bool? IsAutoReplyText { get; set; }
        public string AutoReply { get; set; }
    }

}
