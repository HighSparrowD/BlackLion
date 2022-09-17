using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApi.Entities.LocalisationEntities
{
    public class SecondaryLocalisationModel
    {
        [Key]
        public int Id { get; set; }
        //[ForeignKey("localisation")]
        public int LocalisationId { get; set; }
        public int LocalisationSectionId { get; set; }
        public string ElementName { get; set; }
        public string ElementText { get; set; }
        //public virtual Localisation Localisation { get; set; }
    }
}
