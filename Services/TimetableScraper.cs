﻿using HtmlAgilityPack;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MVC_EduScanner.Services
{
    public class TimetableScraper : ITableScraper
    {
        private readonly HttpClient _httpClient;
        HtmlDocument _htmlDocument = new();
        private string _htmlForm;
        string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Files", "activePlans.xlsx");
        List<(string teacherName, string lecture)> _finalResult = new();

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
                    List<string> lessons = await GetAllLessonsFromPlan();
                    List<string> filtredLessons = await GetAllLecturesFromPlan(lessons);
                    foreach(var filtredLesson in filtredLessons)
                    {
                        _finalResult.Add((plan.Name, filtredLesson));

                    }
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

                    for (int i = 2; i <= rowCount; i++)
                    {
                        string Name ="", Link = "";
                        for (int j = 1; j <= colCount; j++)
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

        public List<(string teachername, string lectrue)> GetLecturesFromFile()
        {
            List<(string teachername, string lectrue)> allLectures = new();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            try
            {
                using (var package = new ExcelPackage(new FileInfo(_filePath)))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;

                    for (int i = 2; i <= rowCount; i++)
                    {
                        string teacherName = "", lecture = "";
                        for (int j = 3; j <= 4; j++)
                        {
                            if (j == 3)
                            {
                                teacherName = worksheet.Cells[i, j].Text;
                            }
                            else if (j == 4)
                            {
                                lecture = worksheet.Cells[i, j].Text;
                            }
                        }
                        allLectures.Add((teacherName, lecture));

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }


            return allLectures;
        }
        public async Task<List<string>> GetAllLessonsFromPlan()
        {
            List<string> lessons = new();

            string xpath = "(//div[@name='course'])";
            HtmlNodeCollection courseNodes = _htmlDocument.DocumentNode.SelectNodes(xpath);

            if (courseNodes != null)
            {
                foreach (var node in courseNodes)
                {
                    string innerHtml = node.InnerHtml;

                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(innerHtml);

                    var textNodes = doc.DocumentNode.SelectNodes("//text()");
                    if (textNodes != null && textNodes.Count > 0)
                    {
                        string value = textNodes[0].InnerHtml.Trim();
                        string studyFormat = "";
                        foreach (var formatNode in textNodes )
                        {
                            string secondValue = formatNode.InnerHtml.Trim();
                            if (!string.IsNullOrEmpty(secondValue))
                            {
                                

                                if (secondValue.Contains("NZ"))
                                {
                                    studyFormat = "Niestacjonarne Zaoczne";
                                }
                                else if (secondValue.Contains("Niestacjonarne Wieczorowe"))
                                {
                                    studyFormat = "Niestacjonarne Wieczorowe";
                                }
                                else if (secondValue.Contains("Stacjonarne"))
                                {
                                    studyFormat = "Stacjonarne";
                                }
                                else if (secondValue.Contains("NZ-P"))
                                {
                                    studyFormat = "Niestacjonarne Zaoczne Parzyste";

                                }
                                else if (secondValue.Contains("S"))
                                {
                                    studyFormat = "Stacjonarne Parzyste";
                                }
                                else if (secondValue.Contains("NW Parzyste"))
                                {
                                    studyFormat = "Niestacjonarne Wieczorowe Parzyste";
                                }
                            }
                        }
                        if (!string.IsNullOrEmpty(studyFormat))
                        {
                            value += ", " + studyFormat;
                        }
                        lessons.Add(value);

                    }
                }
            }

            return lessons;
        }


        public async Task<List<string>> GetAllLecturesFromPlan(List<string> lessons)
        {
            Dictionary<string, string> uniqueLessons = new();
            foreach(var lesson in lessons)
            {
                string[] parts = lesson.Split(", ");

                if (parts.Length < 3)
                {
                    Console.WriteLine($"Incorrect lesson format: '{lesson}'");
                    continue;
                }

                string name = parts[0];
                string type = parts[1];
                string format = parts[2];

                if(!uniqueLessons.ContainsKey(name) || type == "wyk")
                {
                    uniqueLessons[name] = type + ", " + format;
                }

            }

            List<string> filteredLessons = uniqueLessons
                    .Select(kvp => $"{kvp.Key}, {kvp.Value}")
                    .ToList();

            foreach (var lesson in filteredLessons)
            {
                Console.WriteLine(lesson);
            }

                return filteredLessons;
        }

        public List<(string teacherName, string lecture)> GetTeacherLecture()
        {
            Console.WriteLine($"Кількість викладачів: {_finalResult.Count}");
            foreach (var item in _finalResult)
            {
                Console.WriteLine($"Викладач: {item.teacherName}, Лекція: {item.lecture}");
            }
            return _finalResult;
            
        }


        public void SavePlansInFile(List<(string Link, string Name)> activePlans)
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

        public void SaveLecturesInFile()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            try
            {
                using(var package = new ExcelPackage((new FileInfo(_filePath))))
                {
                    var worksheet = package.Workbook.Worksheets[0];

                    worksheet.Cells[1, 3].Value = "Teacher Name";
                    worksheet.Cells[1, 4].Value = "Lecture";
                    int row = 2;

                    foreach(var plan in _finalResult)
                    {
                        if (plan.teacherName != "A A")
                        {
                            worksheet.Cells[row, 3].Value = plan.teacherName;
                            worksheet.Cells[row, 4].Value = plan.lecture;
                            row++;
                        }
                    }
                    package.Save();

                    Console.WriteLine("The file was successfully saved");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

    }
}
