﻿using System;
using System.Text.Json.Serialization;

namespace WebApi.Entities.AdventureEntities
{
    public class GetTemplateShort
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}