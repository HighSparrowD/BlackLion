using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace WebApi.Entities.SecondaryEntities
{
    public class Language
    {
        [Key]
        public int Id { get; set; }
        public string LanguageName { get; set; }
        public string LanguageNameNative { get; set; }
        [AllowNull]
        public short? Priority { get; set; }
        [Key]
        public int ClassLocalisationId { get; set; }
    }
}
