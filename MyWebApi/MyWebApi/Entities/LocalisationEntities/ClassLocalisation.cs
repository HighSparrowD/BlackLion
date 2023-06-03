using WebApi.Entities.LocationEntities;
using WebApi.Entities.SecondaryEntities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.LocalisationEntities
{
    public class ClassLocalisation
    {
        [Key]
        public int Id { get; set; }
        public string LanguageName { get; set; }
        //public virtual List<City> Cities { get; set; }
        public virtual List<UpdateCountry> Countries { get; set; }
        public virtual List<Language> Languages { get; set; }
        public virtual List<Gender> Genders { get; set; }
    }
}
