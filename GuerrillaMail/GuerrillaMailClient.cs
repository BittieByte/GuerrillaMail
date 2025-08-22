using System.Net;
using System.Text.Json;

namespace GuerrillaMail
{
    public class GuerrillaMailClient
    {
        private const string BaseUrl = "http://api.guerrillamail.com/ajax.php";
        private readonly HttpClient _http;
        private readonly CookieContainer _cookies = new();
        private readonly string _ip;
        private readonly string _agent;

        public GuerrillaMailClient(string ip, string agent)
        {
            _ip = ip;
            _agent = agent;

            var handler = new HttpClientHandler
            {
                CookieContainer = _cookies,
                UseCookies = true
            };

            _http = new HttpClient(handler);
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString
        };

        private async Task<T?> CallAsync<T>(string function, Dictionary<string, string>? args = null)
        {
            args ??= [];
            args["f"] = function;
            args["ip"] = _ip;
            args["agent"] = _agent;

            var url = QueryHelpers.AddQueryString(BaseUrl, args);
            var response = await _http.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var raw = await response.Content.ReadAsStringAsync();

            // If the response starts with { or [, treat it as JSON
            raw = raw.Trim();
            if (raw.StartsWith('{') || raw.StartsWith('['))
            {
                using var doc = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(raw));
                return await JsonSerializer.DeserializeAsync<T>(doc, JsonOptions);
            }
            else
            {
                // Log or throw a descriptive exception
                throw new InvalidOperationException($"GuerrillaMail API returned non-JSON response: {raw}");
            }
        }

        // ---- API Methods ----

        public Task<GetEmailAddressResponse?> GetEmailAddressAsync(string lang = "en", string? subscr = null)
        {
            var args = new Dictionary<string, string> { { "lang", lang } };
            if (!string.IsNullOrEmpty(subscr)) args["SUBSCR"] = subscr;
            return CallAsync<GetEmailAddressResponse>("get_email_address", args);
        }

        public Task<GetEmailAddressResponse?> SetEmailUserAsync(string emailUser, string lang = "en")
        {
            var args = new Dictionary<string, string>
            {
                { "email_user", emailUser },
                { "lang", lang }
            };
            return CallAsync<GetEmailAddressResponse>("set_email_user", args);
        }

        public Task<CheckEmailResponse?> CheckEmailAsync(long seq = 0)
        {
            var args = new Dictionary<string, string> { { "seq", seq.ToString() } };
            return CallAsync<CheckEmailResponse>("check_email", args);
        }

        public Task<CheckEmailResponse?> GetEmailListAsync(int offset = 0, long? seq = null)
        {
            var args = new Dictionary<string, string> { { "offset", offset.ToString() } };
            if (seq.HasValue) args["seq"] = seq.Value.ToString();
            return CallAsync<CheckEmailResponse>("get_email_list", args);
        }

        public Task<EmailMessage?> FetchEmailAsync(long emailId)
        {
            var args = new Dictionary<string, string> { { "email_id", emailId.ToString() } };
            return CallAsync<EmailMessage>("fetch_email", args);
        }

        public Task<ForgetMeResponse?> ForgetMeAsync(string emailAddr)
        {
            var args = new Dictionary<string, string> { { "email_addr", emailAddr } };
            return CallAsync<ForgetMeResponse>("forget_me", args);
        }

        public Task<DeleteEmailResponse?> DeleteEmailAsync(long emailId)
        {
            // Call the existing batch method with a single-element array
            return DeleteEmailAsync([emailId]);
        }

        public Task<DeleteEmailResponse?> DeleteEmailAsync(IEnumerable<long> emailIds)
        {
            var args = new Dictionary<string, string>();
            int i = 0;
            foreach (var id in emailIds)
            {
                args[$"email_ids[{i++}]"] = id.ToString();
            }
            return CallAsync<DeleteEmailResponse>("del_email", args);
        }

        public Task<ExtendResponse?> ExtendAsync()
        {
            return CallAsync<ExtendResponse>("extend");
        }
    }
}
