﻿using System.ComponentModel.DataAnnotations;
using WebApi.Enums.Enums.Tag;

namespace WebApi.Main.Entities.User;

public class UserTag
{
    [Key]
    public long TagId { get; set; }

    [Key]
    public long UserId { get; set; }

    [Key]
    public TagType TagType { get; set; }

    public virtual  Tag.Tag Tag { get; set; }

    public UserTag()
    { }

    public UserTag(long id, long userId, TagType type)
    {
        TagId = id;
        UserId = userId;
        TagType = type;
    }
}
