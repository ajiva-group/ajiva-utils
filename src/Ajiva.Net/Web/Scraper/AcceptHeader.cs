namespace Ajiva.Net.Web.Scraper
{
    public sealed class AcceptHeader
    {
        public static AcceptHeader HtmlXHtmlXmlQ09WebpAllQ08 { get; } = new("text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
        public static AcceptHeader All { get; } = new("*/*");
        public static AcceptHeader HtmlAllQ001 { get; } = new("text/html,*/*;q=0.01");

        public string Accept { get; }
        public (string name, string value) Tuple => (HeaderStrings.Accept, Accept);

        public AcceptHeader(string accept)
        {
            Accept = accept;
        }
    }
}
