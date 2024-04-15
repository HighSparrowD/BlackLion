using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebApi.Enums.Enums.General;

namespace WebApi.Main.Entities.Language
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
        public AppLanguage Lang { get; set; }
    }
}
