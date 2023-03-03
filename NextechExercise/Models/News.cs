using System.Text.Json.Serialization;

namespace NextechExercise.Models
{
    public record class News
    {
        [property: JsonPropertyName("title")]
        public string title { get; set; }

        [property: JsonPropertyName("url")]
        public string? url { get; set; }
        [property: JsonPropertyName("id")]
        public int? id { get; set; }
    }
}