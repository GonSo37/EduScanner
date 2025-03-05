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

        public async Task<string> SubmitForm()
        {
            string url = "https://plany.ubb.edu.pl/right_menu_result_plan.php";
            var values = new Dictionary<string, string>
            {
                { "search", "plan" },
                { "word", "" }, 
                { "groups", "" }, 
                { "conductors", "1" }, 
                { "rooms", "" }
                
            };
            var content = new FormUrlEncodedContent(values);
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            string responseBody = await response.Content.ReadAsStringAsync();

            return responseBody;
        }
        public async Task<List<string>> RunAutomation()
        {
            List<string> links = new();

            string url = "https://plany.ath.bielsko.pl/main.php";
            string html = await _httpClient.GetStringAsync(url);

            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(html);

            string xpath = ".//table[@class='titles']//input[@id='submit']";
            HtmlNodeCollection linkNodes = htmlDocument.DocumentNode.SelectNodes(xpath);

            if (linkNodes != null)
            {
                foreach(var node in linkNodes)
                {
                    string href = node.GetAttributeValue("href", "");
                    if(!string.IsNullOrEmpty(href))
                    {
                        links.Add(href);
                        Console.WriteLine(href);
                    }
                }
            }

            return links;
        }
    }
}
