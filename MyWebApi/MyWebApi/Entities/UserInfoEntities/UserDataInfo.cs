using MyWebApi.Entities.LocationEntities;
using MyWebApi.Entities.SecondaryEntities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
#nullable enable

namespace MyWebApi.Entities.UserInfoEntities
{
    public class UserDataInfo
    {
        [Key]
        public long Id { get; set; }
        public List<int>? UserLanguages { get; set; } 
        public short ReasonId { get; set; }
        public int UserAge { get; set; }
        public short UserGender { get; set; }
        public int LanguageId { get; set; } //AppLanguage
        public long LocationId { get; set; }
        public int ReasonClassLocalisationId { get; set; }
        public virtual Location? Location { get; set; }
        public virtual UserReason? Reason { get; set; }
        //public virtual Language? Language { get; set; }

    }
}
