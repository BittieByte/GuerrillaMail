using System.Text.Json.Serialization;

namespace GuerrillaMail
{
    // ---- Response Models ----

    public class GetEmailAddressResponse
    {
        [JsonPropertyName("email_addr")]
        public string? EmailAddr { get; set; }

        [JsonPropertyName("email_timestamp")]
        public long EmailTimestamp { get; set; }

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
