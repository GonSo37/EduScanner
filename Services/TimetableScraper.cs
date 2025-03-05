using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;

namespace MVC_EduScanner.Services
{
    public class TimetableScraper : ITableScraper
    {
        private readonly HttpClient _httpClient;

        public TimetableScraper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> RunAutomation()
        {
            int recordsUpdateTotal = 0;

            string url = "https://plany.ubb.edu.pl/";
            string html = await _httpClient.GetStringAsync(url);

            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(html);

            string xpath = ".//table[@class='branch2']//li[@class='closed']";
            HtmlNodeCollection allLinks = htmlDocument.DocumentNode.SelectNodes(xpath);

            return 0;
        }
    }
}
