﻿using System.Text.Json.Serialization;
using WebApi.Enums;

namespace WebApi.Entities.UserActionEntities
{
    public class AddFeedback
    {
        [JsonPropertyName("userId")]
        public long UserId { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("reason")]
        public FeedbackReason Reason { get; set; }
    }
}