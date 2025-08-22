using System.Text.Json.Serialization;

namespace GuerrillaMail
{
    public class DeleteEmailResponse
    {
        [JsonPropertyName("deleted_ids")]
        public List<long>? DeletedIds { get; set; }
    }
}
