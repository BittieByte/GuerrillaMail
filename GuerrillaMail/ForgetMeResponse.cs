using System.Text.Json.Serialization;

namespace GuerrillaMail
{
    public class ForgetMeResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}
