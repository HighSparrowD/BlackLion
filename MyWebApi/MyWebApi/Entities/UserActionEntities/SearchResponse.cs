using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApi.Entities.UserActionEntities
{
    public class SearchResponse
    {
        [JsonPropertyName("users")]
        public List<GetUserData> Users { get; set; } 
        [JsonPropertyName("response")]
        public string Response { get; set; } // Is not null if user list is empty

        public SearchResponse(string errorResponse)
        {
            Response = errorResponse;
        }

        public SearchResponse(GetUserData user)
        {
            Users = new List<GetUserData>();
            Users.Add(user);
        }

        public SearchResponse(List<GetUserData> users)
        {
            Users = users;
        }
    }
}
