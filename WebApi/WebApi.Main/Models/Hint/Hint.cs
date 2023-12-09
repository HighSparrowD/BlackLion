using System.ComponentModel.DataAnnotations;
using WebApi.Main.Enums.General;
using WebApi.Main.Enums.Hint;

namespace WebApi.Main.Models.Hint;

public class Hint
{
    [Key]
    public int Id { get; set; }
    [Key]
    public AppLanguage Localization { get; set; }
    public Section? Section { get; set; }
    public HintType Type { get; set; }
    public string Text { get; set; }
}
