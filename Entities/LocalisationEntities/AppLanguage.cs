using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.LocalisationEntities
{
    public class AppLanguage
    {
        [Key]
        public int Id { get; set; }
        public string LanguageName { get; set; }
        public string LanguageNameShort { get; set; }
    }
}
