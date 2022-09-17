using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApi.Entities.UserInfoEntities
{
    public class Encounter
    {
        [Key]
        public long Id{ get; set; }
        public long UserId{ get; set; }
        public long UserId1{ get; set; }
        public int SectionId { get; set; }
        public DateTime EncounterDate { get; set; }
        //[ForeignKey("UserId")]
        //public User User{ get; set; }
        //[ForeignKey("UserId1")]
        //public User User1{ get; set; }
    }
}
