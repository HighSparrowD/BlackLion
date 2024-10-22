﻿using System.Text.Json.Serialization;

#nullable enable
namespace WebApi.Models.Models.User
{
    public class GetRequests
    {
        [JsonPropertyName("requests")]
        public List<GetRequest> Requests { get; set; }
        [JsonPropertyName("notification")]
        public string? Notification { get; set; }

        public GetRequests(List<GetRequest> requests)
        {
            Requests = requests;
        }
    }
}
