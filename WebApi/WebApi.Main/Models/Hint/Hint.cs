﻿using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.General;
using WebApi.Enums.Enums.Hint;

namespace WebApi.Main.Entities.Hint;

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
