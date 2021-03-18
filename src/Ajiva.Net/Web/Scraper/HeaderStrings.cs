namespace Ajiva.Net.Web.Scraper
{
    public static class HeaderStrings
    {
        public const string Accept = "Accept";

        public const string XRequestWith = "X-Requested-With";
        public const string XRequestWithValueXmlHttpRequest = "XMLHttpRequest";

        public static (string, string) XhrRequestTuple { get; } = (XRequestWith, XRequestWithValueXmlHttpRequest);
        public const string Host = "Host";
        public const string UserAgent = "User-Agent";
    }
}
