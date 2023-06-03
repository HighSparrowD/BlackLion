using WebApi.Entities.UserInfoEntities;
using WebApi.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.AdventureEntities
{
    public class Adventure
    {
        [Key]
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public string Name { get; set; }
        public bool IsOffline { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public string Media { get; set; }
        public bool IsMediaPhoto { get; set; }
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
        public string UniqueLink { get; set; }
        //Indicates, whether if adventure awaits for the group id
        public bool IsAwaiting { get; set; }
        public string GroupLink { get; set; }
        public long? GroupId { get; set; }
        public AdventureStatus Status { get; set; }

        public virtual User Creator { get; set; }
    }

}
