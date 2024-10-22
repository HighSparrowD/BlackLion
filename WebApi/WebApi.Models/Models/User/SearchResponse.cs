﻿using System.Text.Json.Serialization;

namespace WebApi.Models.Models.User
{
    public class SearchResponse
    {
        [JsonPropertyName("users")]
        public List<GetUserData> Users { get; set; }

        public SearchResponse()
        { }

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
