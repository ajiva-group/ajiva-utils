using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Ajiva.Net.Web.Scraper
{
    internal class Program : RegisteredWebScraper<Program.WebOptions>
    {
        public static async Task Main(string[] args)
        {
            var debug = new Program(new("https://httpbin.org/"), new(), UserAgent.Firefox86, AcceptHeader.All);
            debug.Register(WebOptions.Post, "post", HttpMethod.Post, AcceptHeader.HtmlXHtmlXmlQ09WebpAllQ08);
            var prog = new Program(new("https://stackoverflow.com/"), new(), UserAgent.Firefox86, AcceptHeader.All);
            prog.Register(WebOptions.Questions, "questions", HttpMethod.Get, AcceptHeader.HtmlAllQ001);

            var ps = await debug.ExecuteFormFor(WebOptions.Post, "?asd=2", ("test11", "value2"), ("sda","dsa"));

            Console.WriteLine(await ps.Content.ReadAsStringAsync());

            var res = await prog.ExecuteFor(WebOptions.Questions, "9873873");
            Console.WriteLine(await res.Content.ReadAsStringAsync());
        }

        /// <inheritdoc />
        private Program(Uri baseUri, CookieContainer? cookieContainer, UserAgent userAgent, AcceptHeader acceptHeader) : base(baseUri, cookieContainer, userAgent, acceptHeader)
        {
        }
        
        internal enum WebOptions
        {
            Questions,
            Post
        }
    }
}
