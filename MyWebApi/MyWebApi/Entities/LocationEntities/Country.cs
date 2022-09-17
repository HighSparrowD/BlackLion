using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MyWebApi.Entities.LocationEntities
{
    public class Country
    {
        [Key]
        public int Id { get; set; }
        [Key]
        public int ClassLocalisationId { get; set; }
        public string CountryName { get; set; }
        public virtual List<City> Cities { get; set; }
    }
}
