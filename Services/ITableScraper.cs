namespace MVC_EduScanner.Services
{
    public interface ITableScraper
    {
        Task<string> SubmitForm();
        Task<List<(string Link, string Name)>> GetAllLinks();

        Task<List<(string Link, string Name)>> GetActivePlans(List<(string Link, string Name)> links);
    }
}
