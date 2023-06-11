
using System;
using System.Text.Json.Serialization;

namespace WebApi.Entities.AdventureEntities
{
    public class ManageAdventure
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("isOffline")]
        public bool IsOffline { get; set; }
        [JsonPropertyName("countryId")]
        public int? CountryId { get; set; }
        [JsonPropertyName("cityId")]
        public int? CityId { get; set; }
        [JsonPropertyName("media")]
        public string Media { get; set; }
        [JsonPropertyName("isMediaPhoto")]
        public bool IsMediaPhoto { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("experience")]
        public string Experience { get; set; }
        [JsonPropertyName("attendeesDescription")]
        public string AttendeesDescription { get; set; }
        [JsonPropertyName("unwantedAttendeesDescription")]
        public string UnwantedAttendeesDescription { get; set; }
        [JsonPropertyName("gratitude")]
        public string Gratitude { get; set; }
        [JsonPropertyName("date")]
        public string Date { get; set; }
        [JsonPropertyName("time")]
        public string Time { get; set; }
        [JsonPropertyName("duration")]
        public string Duration { get; set; }
        [JsonPropertyName("application")]
        public string Application { get; set; }
        [JsonPropertyName("address")]
        public string Address { get; set; }
        //If is null -> auto reply is not present at all
        [JsonPropertyName("isAutoReplyText")]
        public bool? IsAutoReplyText { get; set; }
        [JsonPropertyName("autoReply")]
        public string AutoReply { get; set; }
        [JsonPropertyName("isAwaiting")]
        public bool IsAwaiting { get; set; }
        [JsonPropertyName("groupId")]
        public long? GroupId { get; set; }

        public ManageAdventure()
        {}

        public ManageAdventure(Adventure adventure)
        {
            UserId = adventure.UserId;
            Name = adventure.Name;
            IsOffline = adventure.IsOffline;
            CountryId = adventure.CountryId;
            CityId = adventure.CityId;
            Media = adventure.Media;
            IsMediaPhoto = adventure.IsMediaPhoto;
            Description = adventure.Description;
            Experience = adventure.Experience;
            AttendeesDescription = adventure.AttendeesDescription;
            UnwantedAttendeesDescription = adventure.UnwantedAttendeesDescription;
            Gratitude = adventure.Gratitude;
            Date = adventure.Date;
            Time = adventure.Time;
            Duration = adventure.Duration;
            Application = adventure.Application;
            Address = adventure.Address;
            IsAutoReplyText = adventure.IsAutoReplyText;
            AutoReply = adventure.AutoReply;
            IsAwaiting = adventure.IsAwaiting;
            GroupId = adventure.GroupId;
        }
    }
}
