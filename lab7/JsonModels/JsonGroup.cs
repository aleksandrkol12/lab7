using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace lab7.JsonModels
{
    public class JsonGroup
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("students")]
        public List<JsonStudent> Students { get; set; }
    }
}