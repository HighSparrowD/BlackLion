﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Main.Entities.User;

public class BlackList
{
    [Key]
    public long Id { get; set; }
    public long UserId { get; set; }
    [ForeignKey("BannedUser")]
    public long BannedUserId { get; set; }
    public virtual User BannedUser { get; set; }
}
