namespace MVC_EduScanner.Services
{
    public interface ITableScraper
    {
        Task<string> SubmitForm();
        Task<List<string>> RunAutomation();
    }
}
