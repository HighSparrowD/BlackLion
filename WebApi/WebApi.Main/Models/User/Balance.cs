using System;
using System.ComponentModel.DataAnnotations;
using WebApi.Main.Enums.General;

namespace WebApi.Main.Models.User;

public class Balance
{
    [Key]
    public long Id { get; set; }
    public long UserId { get; set; }
    public float Points { get; set; }
    public int OceanPoints { get; set; }
    public int SecondChances { get; set; }
    public int Valentines { get; set; }
    public int Detectors { get; set; }
    public int Nullifiers { get; set; }
    public int CardDecksMini { get; set; }
    public int CardDecksPlatinum { get; set; }
    public int ThePersonalities { get; set; }
    public Currency? Currency { get; set; }
    public DateTime PointInTime { get; set; }

    public Balance()
    { }

    public Balance(long userId, float points, DateTime pointInTime)
    {
        UserId = userId;
        Points = points;
        OceanPoints = 15;
        PointInTime = pointInTime;
        SecondChances = 0;
        Valentines = 0;
        Detectors = 0;
        Nullifiers = 0;
        CardDecksMini = 0;
        CardDecksPlatinum = 0;
        ThePersonalities = 0;
    }
}
