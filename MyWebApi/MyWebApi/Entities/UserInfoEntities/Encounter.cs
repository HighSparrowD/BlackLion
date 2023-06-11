using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApi.Enums;

namespace WebApi.Entities.UserInfoEntities
{
    public class Encounter
    {
        [Key]
        public long Id{ get; set; }
        [ForeignKey("User")]
        public long UserId{ get; set; }
        [ForeignKey("EncounteredUser")]
        public long EncounteredUserId{ get; set; }
        public Section Section { get; set; }
        public DateTime EncounterDate { get; set; }
        public User User { get; set; }
        public User EncounteredUser { get; set; }
    }
}
