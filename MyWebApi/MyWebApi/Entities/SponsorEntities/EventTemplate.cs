using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;

namespace WebApi.Entities.SponsorEntities
{
    public class EventTemplate
    {
        [Key]
        public long Id { get; set; }
        public long SponsorId { get; set; }
        public string TemplateName { get; set; }
        public string Name { get; set; }
        public List<int?> Languages { get; set; }
        public short MinAge { get; set; }
        public short MaxAge { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public bool IsOnline { get; set; }
        public string Photo { get; set; }
        public int Bounty { get; set; }
    }
}
