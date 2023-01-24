using MyWebApi.Entities.TestEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MyWebApi.Entities.UserInfoEntities
{
    public class UserTest
    {
        [Key]
        [NotNull]
        public long UserId { get; set; }
        [Key]
        [NotNull]
        public long TestId { get; set; }
        [NotNull]
        public int TestClassLocalisationId { get; set; }
        [NotNull]
        public short TestType { get; set; }
        public double Result { get; set; }
        public DateTime? PassedOn { get; set; }
        public List<string> Tags { get; set; }
        public virtual Test Test { get; set; }
        public virtual User User { get; set; }
    }
}
