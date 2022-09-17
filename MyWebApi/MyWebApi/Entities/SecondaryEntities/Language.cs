using System.ComponentModel.DataAnnotations;

namespace MyWebApi.Entities.SecondaryEntities
{
    public class Language
    {
        [Key]
        public int Id { get; set; }
        public string LanguageName { get; set; }
        public string LanguageNameNative { get; set; }
        public short Priority { get; set; }
        [Key]
        public int ClassLocalisationId { get; set; }
    }
}
