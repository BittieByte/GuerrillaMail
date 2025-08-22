using System.Text.Json.Serialization;

namespace GuerrillaMail
{
    public class CheckEmailResponse
    {
        [JsonPropertyName("list")]
        public List<EmailListItem>? Emails { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("ts")]
        public long Timestamp { get; set; }

        [JsonPropertyName("s_active")]
        public string? SubscriptionActive { get; set; }

        [JsonPropertyName("s_date")]
        public string? SubscriptionDate { get; set; }

        [JsonPropertyName("s_time")]
        public long SubscriptionTimestamp { get; set; }

        [JsonPropertyName("s_time_expires")]
        public long SubscriptionExpires { get; set; }
    }
}
