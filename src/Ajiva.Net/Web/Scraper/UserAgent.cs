namespace Ajiva.Net.Web.Scraper
{
    public sealed class UserAgent
    {
        public static UserAgent Chrome75 { get; } = new("Mozilla/5.0 (X11; CrOS armv7l 12105.100.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.144 Safari/537.36");
        public static UserAgent Firefox86 { get; } = new("Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:86.0) Gecko/20100101 Firefox/86.0");
        public static UserAgent NagiosHttpMonitor { get; } = new("check_http/v2.2.1 (nagios-plugins 2.2.1)");
        public static UserAgent PythonRequests { get; } = new("python-requests/2.20.1");
        public static UserAgent SemRushCrawler { get; } = new("Mozilla/5.0 (compatible; SemrushBot/2~bl; +http://www.semrush.com/bot.html)");
        public static UserAgent CodeWiseBot { get; } = new("Mozilla/5.0 (compatible; Codewisebot/2.0; +http://www.nosite.com/somebot.htm)");
        public static UserAgent UnknownBot { get; } = new("Mozilla/5.0 (compatible; Linux x86_64; Mail.RU_Bot/2.0; +http://go.mail.ru/help/robots)");
        public static UserAgent Googlebot { get; } = new("Mozilla/5.0 (compatible; Googlebot/2.1; +http://www.google.com/bot.html)");
        public static UserAgent Safari12 { get; } = new("Mozilla/5.0 (iPhone; CPU iPhone OS 12_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.1 Mobile/15E148 Safari/604.1");
        public static UserAgent Huawei { get; } = new("Mozilla/5.0 (Linux; Android 4.3; MediaPad 7 Youth 2 Build/HuaweiMediaPad) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.83 Safari/537.36");
        public static UserAgent Thunderbird { get; } = new("Mozilla/5.0 (X11; Linux x86_64; rv:45.0) Gecko/20100101 Thunderbird/45.8.0");
        public static UserAgent InternetExplorer11 { get; } = new("Mozilla/5.0 CK={} (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko");
        public static UserAgent FaceBockAndroid { get; } = new("Mozilla/5.0 (Linux; Android 10; moto g(7) power Build/QCOS30.85-18-6; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/86.0.4240.185 Mobile Safari/537.36 [FB_IAB/FB4A;FBAV/294.0.0.39.118;]");

        // ReSharper disable once MemberCanBePrivate.Global
        public UserAgent(string agent)
        {
            Agent = agent;
        }

        public string Agent { get; }
    }
}
