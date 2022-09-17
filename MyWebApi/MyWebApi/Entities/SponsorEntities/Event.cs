using MyWebApi.Entities.UserInfoEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.SponsorEntities
{
    public class Event
    {
        [Key]
        public long Id{ get; set; }
        public long SponsorId{ get; set; }
        public string Name{ get; set; }
        public short Status{ get; set; }
        public short MinAge { get; set; }
        public short MaxAge { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public long CityId { get; set; }
        public DateTime StartDateTime { get; set; }
        public bool IsOnline { get; set; }
        public bool HasGroup { get; set; }
        public string GroupLink { get; set; }
        public string Photo { get; set; }
        public int Bounty { get; set; }
        public string Comment { get; set; }
    }
}
