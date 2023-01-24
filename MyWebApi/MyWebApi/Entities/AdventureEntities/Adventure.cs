using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.AdventureEntities
{
    public class Adventure
    {
        [Key]
        public Guid Id{ get; set; }
        public long UserId { get; set; }
        public bool IsOnline{ get; set; }
        public int? CountryId{ get; set; }
        public int? CityId{ get; set; }
        public List<int> Languages{ get; set; }
        public short MinAge{ get; set; }
        public short MaxAge{ get; set; }
        public string Name{ get; set; }
        public string Description{ get; set; }
        public string Adress{ get; set; }
        public short Capacity{ get; set; }
        public DateTime StartDateTime{ get; set; }
        public DateTime EndDateTime{ get; set; }

        public bool IsOverlapping(Adventure model)
        {
            return ((StartDateTime <= model.EndDateTime && model.StartDateTime <= model.EndDateTime) || (model.StartDateTime <= StartDateTime && model.EndDateTime <= StartDateTime));
        }

        public bool IsOverlapping(ChangeAdventure model)
        {
            return ((StartDateTime <= model.EndDateTime && model.StartDateTime <= model.EndDateTime) || (model.StartDateTime <= StartDateTime && model.EndDateTime <= StartDateTime));
        }
    }

}
