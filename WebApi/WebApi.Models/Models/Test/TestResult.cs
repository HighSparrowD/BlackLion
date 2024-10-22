﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using WebApi.Enums.Enums.General;

namespace WebApi.Models.Models.Test;

public class TestResult
{
    [Key]
    [NotNull]
    public long Id { get; set; }
    [NotNull]
    public long TestId { get; set; }
    [NotNull]
    public AppLanguage TestLanguage { get; set; }
    [NotNull]
    public int? Score { get; set; }
    [NotNull]
    public string Result { get; set; }
    public List<long> Tags { get; set; }
}
