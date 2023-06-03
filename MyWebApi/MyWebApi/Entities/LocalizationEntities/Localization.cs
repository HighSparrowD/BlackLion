using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace WebApi.Entities.LocalisationEntities
{
    public class Localization
    {
        [Key]
        public int Id { get; set; }
        [Key]
        public int SectionId { get; set; }
        public string SectionName { get; set; }
        public string LanguageName { get; set; }
        public virtual List<SecondaryLocalizationModel> Loc { get; set; }
    }
}
