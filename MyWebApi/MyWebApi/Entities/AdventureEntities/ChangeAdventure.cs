using System.Collections.Generic;
using System;

namespace MyWebApi.Entities.AdventureEntities
{
    public class ChangeAdventure
    {
        public Guid Id { get; set; }
        public long UserId { get; set; }
        public bool? IsOnline { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public List<int> Languages { get; set; }
        public short? MinAge { get; set; }
        public short? MaxAge { get; set; }
        public string Description { get; set; }
        public string Adress { get; set; }
        public short? Capacity { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
    }
}
