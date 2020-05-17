using System;
using System.Text.Json.Serialization;

namespace lab7.JsonModels
{
    public class JsonStudent
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; } = null;

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = null;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = null;

        [JsonPropertyName("groupId")]
        public int? GroupId { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime? CreatedAt { get; set; } = null;

        [JsonPropertyName("updatedAt")]
        public DateTime? UpdatedAt { get; set; } = null;
    }
}