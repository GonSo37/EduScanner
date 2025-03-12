using static MVC_EduScanner.Models.App_DbContext;

namespace MVC_EduScanner.Models
{
    public class Prowadzacy
    {
        public int ProwadzacyID { get; set; }
        public string Imie { get; set; } = string.Empty;
        public string Nazwisko { get; set; } = string.Empty;

        // Relacja wiele-do-wielu z przedmiotami
        public List<PrzedmiotProwadzacy> PrzedmiotProwadzacy { get; set; } = new List<PrzedmiotProwadzacy>();
    }
}

