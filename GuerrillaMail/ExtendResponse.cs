using System.Text.Json.Serialization;

namespace GuerrillaMail
{
    public class ExtendResponse
    {
        [JsonPropertyName("expired")]
        public bool Expired { get; set; }

        [JsonPropertyName("email_timestamp")]
        public long EmailTimestamp { get; set; }

        [JsonPropertyName("affected")]
        public int Affected { get; set; }
    }
}
