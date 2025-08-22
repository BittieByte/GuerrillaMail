using System.Text.Json.Serialization;

namespace GuerrillaMail
{
    public class EmailListItem
    {
        [JsonPropertyName("mail_id")]
        public long Id { get; set; }

        [JsonPropertyName("mail_from")]
        public string? From { get; set; }

        [JsonPropertyName("mail_subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("mail_excerpt")]
        public string? Excerpt { get; set; }

        [JsonPropertyName("mail_timestamp")]
        public long Timestamp { get; set; }

        [JsonPropertyName("mail_read")]
        public int Read { get; set; }

        [JsonPropertyName("mail_date")]
        public string? Date { get; set; }
    }
}
