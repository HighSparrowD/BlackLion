﻿using WebApi.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Entities.AdventureEntities
{
    public class AdventureAttendee
    {
        [Key]
        public long UserId{ get; set; }
        [Key]
        public Guid AdventureId { get; set; }
        public string Username { get; set; }
        public AdventureAttendeeStatus Status { get; set; }
        public virtual Adventure Adventure { get; set; }
    }
}