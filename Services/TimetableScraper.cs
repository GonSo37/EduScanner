using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MVC_EduScanner.Services
{
    public class TimetableScraper : ITableScraper
    {
        private readonly HttpClient _httpClient;
        HtmlDocument _htmlDocument = new();
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
                { "groups", "1" },
                { "conductors", "1" },
                { "rooms", "" }

            };
            var content = new FormUrlEncodedContent(values);
            HttpResponseMessage response = await _httpClient.PostAsync(url, content);
            _htmlForm = await response.Content.ReadAsStringAsync();

            return _htmlForm;
        }
        public async Task<List<(string Link, string Name)>> GetAllLinks()
        {
            List<(string Link, string Name)> results = new();

            HtmlDocument htmlDocument = new();
            htmlDocument.LoadHtml(_htmlForm);

            string xpath = "//a[@href]";
            HtmlNodeCollection linkNodes = htmlDocument.DocumentNode.SelectNodes(xpath);

            if (linkNodes != null)
            {
                foreach (var node in linkNodes)
                {
                    string href = node.GetAttributeValue("href", "");
                    string name = node.InnerText.Trim();
                    if (!string.IsNullOrEmpty(href))
                    {
                        results.Add((href, name));
                    }
                }
            }


            return results;
        }

        public async Task<List<(string Link, string Name)>> GetActivePlans(List<(string Link, string Name)> allPlans)
        {
            List<(string Link, string Name)> activePlans = new();

            foreach (var plan in allPlans)
            {
                string url = $"https://plany.ubb.edu.pl/{plan.Link}&winW=847&winH=607&loadBG=000000";
                var html = await _httpClient.GetStringAsync(url);
                _htmlDocument.LoadHtml(html);

                string xpath = "(//div[@class='cd'])[1]"; 
                HtmlNodeCollection linkNodes = _htmlDocument.DocumentNode.SelectNodes(xpath);

                if (linkNodes != null)
                {
                    activePlans.Add((plan.Link, plan.Name));
                }
            }

            return activePlans; 
        }



    }
}
