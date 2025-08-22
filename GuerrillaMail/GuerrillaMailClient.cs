using System.Net;
using System.Text;
using System.Text.Json;

namespace GuerrillaMail
{
    /// <summary>
    /// Client for interacting with GuerrillaMail temporary email API.
    /// </summary>
    public class GuerrillaMailClient
    {
        private const string BaseUrl = "http://api.guerrillamail.com/ajax.php";
        private readonly HttpClient _http;
        private readonly CookieContainer _cookies = new();
        private readonly string _ip;
        private readonly string _agent;

        /// <summary>
        /// Initializes a new instance of the <see cref="GuerrillaMailClient"/> class.
        /// </summary>
        /// <param name="ip">Optional IP address to use for the API.</param>
        /// <param name="agent">User-Agent string to identify the client.</param>
        public GuerrillaMailClient(string ip, string agent)
        {
            _ip = ip;
            _agent = agent;

            var handler = new HttpClientHandler
            {
                CookieContainer = _cookies,
                UseCookies = true,
                AutomaticDecompression = DecompressionMethods.All
            };

            _http = new HttpClient(handler);
        }

        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true, NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString };

        /// <summary>
        /// Sends a request to the GuerrillaMail API and deserializes the JSON response.
        /// </summary>
        /// <typeparam name="T">Type of the expected response object.</typeparam>
        /// <param name="function">API function name.</param>
        /// <param name="args">Optional query arguments.</param>
        /// <returns>The deserialized response object.</returns>
        /// <exception cref="InvalidOperationException">Thrown if API returns non-JSON response.</exception>
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
            raw = raw.Trim();
            if (raw.StartsWith('{') || raw.StartsWith('['))
            {
                using var doc = new MemoryStream(Encoding.UTF8.GetBytes(raw));
                return await JsonSerializer.DeserializeAsync<T>(doc, JsonOptions);
            }
            else
            {
                throw new InvalidOperationException($"GuerrillaMail API returned non-JSON response: {raw}");
            }
        }

        /// <summary>
        /// Gets or creates a temporary email address.
        /// </summary>
        /// <param name="lang">Optional language code (default "en").</param>
        /// <param name="subscr">Optional subscription ID.</param>
        /// <returns>The email address info.</returns>
        public Task<GetEmailAddressResponse?> GetEmailAddressAsync(string lang = "en", string? subscr = null) =>
            CallAsync<GetEmailAddressResponse>("get_email_address", new Dictionary<string, string> { { "lang", lang } });


        /// <summary>
        /// Attempt to set a custom username for the temporary email.
        /// Note: You cannot choose the domain; Guerrilla Mail will assign it automatically from their pool.
        /// If the username is taken, the API may assign a different one.
        /// </summary>
        /// <param name="emailUser">The desired username part of the email (before the @)</param>
        /// <param name="lang">Language code (default "en")</param>
        /// <param name="throwIfMismatch">Whether to throw an exception if the returned email does not start with the requested username</param>
        /// <returns>Returns the resulting email address info, including the assigned domain and subscription status.</returns>
        public async Task<GetEmailAddressResponse?> SetEmailUserAsync(string emailUser, string lang = "en", bool throwIfMismatch = false)
        {
            if (string.IsNullOrWhiteSpace(emailUser))
                throw new ArgumentException("Username cannot be empty", nameof(emailUser));

            var args = new Dictionary<string, string>
            {
                { "email_user", emailUser.Trim() },
                { "lang", lang }
            };

            var result = await CallAsync<GetEmailAddressResponse>("set_email_user", args);
            if (!result.EmailAddr.StartsWith(emailUser, StringComparison.OrdinalIgnoreCase))
            {
                if (throwIfMismatch)
                    throw new InvalidOperationException($"Requested username '{emailUser}' was not available. Assigned email: {result.EmailAddr}");
            }
            return result;
        }

        /// <summary>
        /// Sets the username for the temporary email address.
        /// </summary>
        /// <param name="emailUser">The desired email username.</param>
        /// <param name="lang">Optional language code.</param>
        /// <returns>The updated email address info.</returns>
        public Task<GetEmailAddressResponse?> SetEmailUserAsync(string emailUser, string lang = "en") =>
            CallAsync<GetEmailAddressResponse>("set_email_user", new Dictionary<string, string> { { "email_user", emailUser }, { "lang", lang } });

        /// <summary>
        /// Checks for new emails since the given sequence number.
        /// </summary>
        /// <param name="seq">Sequence number to start checking from.</param>
        /// <returns>List of emails and metadata.</returns>
        public Task<CheckEmailResponse?> CheckEmailAsync(long seq = 0) =>
            CallAsync<CheckEmailResponse>("check_email", new Dictionary<string, string> { { "seq", seq.ToString() } });

        /// <summary>
        /// Retrieves a list of emails with optional offset and sequence.
        /// </summary>
        public Task<CheckEmailResponse?> GetEmailListAsync(int offset = 0, long? seq = null)
        {
            var args = new Dictionary<string, string> { { "offset", offset.ToString() } };
            if (seq.HasValue) args["seq"] = seq.Value.ToString();
            return CallAsync<CheckEmailResponse>("get_email_list", args);
        }

        /// <summary>
        /// Fetches the full content of a single email.
        /// </summary>
        /// <param name="emailId">ID of the email to fetch.</param>
        /// <returns>The email message.</returns>
        public Task<EmailMessage?> FetchEmailAsync(long emailId) =>
            CallAsync<EmailMessage>("fetch_email", new Dictionary<string, string> { { "email_id", emailId.ToString() } });

        /// <summary>
        /// Deletes a single email by ID.
        /// </summary>
        public Task<DeleteEmailResponse?> DeleteEmailAsync(long emailId) =>
            DeleteEmailAsync([emailId]);

        /// <summary>
        /// Deletes multiple emails by their IDs.
        /// </summary>
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

        /// <summary>
        /// Extends the life of the temporary email by 1 hour.
        /// </summary>
        /// <returns>Extension response metadata.</returns>
        public Task<ExtendResponse?> ExtendAsync() => CallAsync<ExtendResponse>("extend");
    }
}