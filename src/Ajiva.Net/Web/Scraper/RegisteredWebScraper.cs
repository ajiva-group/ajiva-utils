using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ajiva.Net.Web.Scraper
{
    public delegate Task<HttpResponseMessage> OptionsFunkDelegate(string url, HttpContent? content);

    public class RegisteredWebScraper<T> : WebScraper where T : Enum
    {
        /// <inheritdoc />
        public RegisteredWebScraper(Uri baseUri, CookieContainer? cookieContainer, UserAgent userAgent, AcceptHeader acceptHeader) : base(baseUri, cookieContainer, userAgent, acceptHeader)
        {
        }

        private readonly Dictionary<T, OptionsFunkDelegate> Options = new();

        public void Register(T key, string uri, HttpMethod method, AcceptHeader accept, params (string name, string value)[] headers)
        {
            var fullHeaders = headers.Append(accept.Tuple).ToList();
            Options.Add(key, (rUrl, content) => SendRequest(method, CombineUrl(uri, rUrl), content, fullHeaders));
        }

        private static string CombineUrl(string uri, string secondUri)
        {
            return string.IsNullOrEmpty(secondUri)
                ? uri.TrimEnd('/')
                : !secondUri.StartsWith("?")
                    ? uri.TrimEnd('/') + secondUri.TrimStart('/').TrimEnd('/')
                    : uri.EndsWith('/')
                        ? uri.TrimEnd('/') + secondUri
                        : uri + secondUri;
        }

        public async Task<HttpResponseMessage> ExecuteFor(T key, string optionalUrl, HttpContent? content = null)
        {
            return await Options[key].Invoke(optionalUrl, content);
        }

        public async Task<HttpResponseMessage> ExecuteFormFor(T key, string optionalUrl, params (string?, string?)[]? fields)
        {
            fields ??= Array.Empty<(string?, string?)>();
            return await ExecuteFormFor(key, optionalUrl, fields.Select(x => new KeyValuePair<string?, string?>(x.Item1, x.Item2)).ToArray());
        }

        public async Task<HttpResponseMessage> ExecuteFormFor(T key, string optionalUrl, params KeyValuePair<string?, string?>[]? fields)
        {
            fields ??= Array.Empty<KeyValuePair<string?, string?>>();
            return await Options[key].Invoke(optionalUrl, new FormUrlEncodedContent(fields));
        }
    }
}
