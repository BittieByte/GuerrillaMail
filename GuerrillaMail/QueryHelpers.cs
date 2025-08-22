using System.Net;

namespace GuerrillaMail
{
    // ---- Utility ----
    public static class QueryHelpers
    {
        public static string AddQueryString(string uri, IDictionary<string, string> parameters)
        {
            if (parameters.Count == 0) return uri;
            var separator = uri.Contains('?') ? '&' : '?';
            var query = string.Join('&', Enumerable.Select(parameters, kvp =>
                $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));
            return uri + separator + query;
        }
    }
}
