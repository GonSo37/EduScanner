using HtmlAgilityPack;
using System.Net.Http;
using System.Threading.Tasks;

namespace MVC_EduScanner.Services
{
    public class TimetableScraper : ITableScraper
    {
        private readonly HttpClient _httpClient;
        private string _htmlForm;

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
            _htmlForm = await response.Content.ReadAsStringAsync();

            return _htmlForm;
        }
        public async Task<List<(string Link, string Name)>> RunAutomation()
        {
            List<(string Link, string Name)> results = new();

            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(_htmlForm);

            string xpath = "//a[@href]";
            HtmlNodeCollection linkNodes = htmlDocument.DocumentNode.SelectNodes(xpath);

            if (linkNodes != null)
            {
                foreach(var node in linkNodes)
                {
                    string href = node.GetAttributeValue("href", "");
                    string name = node.InnerText.Trim();
                    if(!string.IsNullOrEmpty(href))
                    {
                        results.Add((href, name));
                    }
                }
            }
            

            return results;
        }
    }
}
