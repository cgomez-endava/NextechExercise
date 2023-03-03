using System.Text.Json.Serialization;

namespace NextechExercise.Models
{
    public record class Response
    {
        [property: JsonPropertyName("news")]
        public IEnumerable<News> news { get; set; }
        [property: JsonPropertyName("totalNews")]
        public int totalNews { get; set; }
        [property: JsonPropertyName("pageNumber")]
        public int pageNumber { get; set; }
    }
}
