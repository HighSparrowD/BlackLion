using System;
using System.ComponentModel.DataAnnotations;
using WebApi.Enums;

namespace WebApi.Entities.UserInfoEntities
{
    public class Encounter
    {
        [Key]
        public Guid Id{ get; set; }
        public long UserId{ get; set; }
        public long EncounteredUserId{ get; set; }
        public Section Section { get; set; }
        public DateTime EncounterDate { get; set; }
        //[ForeignKey("UserId")]
        //public User User{ get; set; }
        //[ForeignKey("UserId1")]
        public User EncounteredUser{ get; set; }
    }
}
