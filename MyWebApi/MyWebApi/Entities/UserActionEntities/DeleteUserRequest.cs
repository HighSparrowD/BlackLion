﻿using System.Text.Json.Serialization;

namespace WebApi.Entities.UserActionEntities
{
    public class DeleteUserRequest
    {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}