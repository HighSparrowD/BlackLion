using MyWebApi.Entities.SecondaryEntities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
# nullable enable

namespace MyWebApi.Entities.UserInfoEntities
{
    public class UserPreferences
    {
        [Key]
        public long Id { get; set; }
        public List<int>? UserLanguagePreferences { get; set; }
        public List<int>? UserLocationPreferences { get; set; }
        //public long LanguageId { get; set; }
        public List<int>? AgePrefs { get; set; }
        public int CommunicationPrefs { get; set; }
        public short UserGenderPrefs { get; set; }

        public UserPreferences(long id, List<int>? userLanguagePreferences, List<int>? userLocationPreferences, List<int>? agePrefs, int communicationPrefs, short userGenderPrefs)
        {
            Id = id;
            UserLanguagePreferences = userLanguagePreferences;
            UserLocationPreferences = userLocationPreferences;
            AgePrefs = agePrefs;
            CommunicationPrefs = communicationPrefs;
            UserGenderPrefs = userGenderPrefs;
        }
    }
}
