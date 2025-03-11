using HtmlAgilityPack;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MVC_EduScanner.Services
{
    public class TimetableScraper : ITableScraper
    {
        private readonly HttpClient _httpClient;
        HtmlDocument _htmlDocument = new();
        private string _htmlForm;
        private List<(string Link, string Name)> collectionOfLinks;

        string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "activePlans.xlsx");

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
        public async Task<List<(string Link, string Name)>> GetActivePlansFromWebsite(List<(string Link, string Name)> allPlans)
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

        public async Task<List<(string Link, string Name)>> GetActivePlansFromFile()
        {
            List < (string Link, string Name) > activePlans = new();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                using (var package = new ExcelPackage(new FileInfo(_filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;

                    for (int i = 2; i < rowCount; i++)
                    {
                        string Name ="", Link = "";
                        for (int j = 1; j < colCount; j++)
                        {
                            if(j == 1)
                            {
                                Name = worksheet.Cells[i, j].Text;
                            }
                            else if(j == 2)
                            {
                                Link = worksheet.Cells[i, j].Text;
                            }
                        }
                        activePlans.Add((Link, Name));

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return activePlans;
        }

        public void SaveInFile(List<(string Link, string Name)> activePlans)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                using (var package = new ExcelPackage()) 
                {
                    var worsheet = package.Workbook.Worksheets.Add("Active Plans");

                    worsheet.Cells[1, 1].Value = "Name";
                    worsheet.Cells[1, 2].Value = "Link";
                    int row = 2;
                   
                    foreach(var plan in activePlans)
                    {
                        worsheet.Cells[row, 1].Value = plan.Name;
                        worsheet.Cells[row, 2].Value = plan.Link;
                        row++;
                    }

                    package.SaveAs(new FileInfo(_filePath));
                }

                Console.WriteLine("The file was successfully saved");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            
        }



    }
}
