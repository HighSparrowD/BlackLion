using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WebApi.Entities.LocationEntities
{
    public class Country
    {
        [Key]
        public int Id { get; set; }
        [Key]
        public int ClassLocalisationId { get; set; }
        public string CountryName { get; set; }
        [AllowNull]
        public short? Priority { get; set; }
        public virtual List<City> Cities { get; set; }
    }
}
