using MVC_EduScanner.Enums;
using static MVC_EduScanner.Models.App_DbContext;

namespace MVC_EduScanner.Models
{
    public class Prowadzacy
    {
        public int ProwadzacyID { get; set; }
        public string Imie { get; set; } = string.Empty;
        public string Nazwisko { get; set; } = string.Empty;

        public Tytul_naukowy Tytul {  get; set; }

        // Relacja wiele-do-wielu z przedmiotami
        public List<PrzedmiotProwadzacy> PrzedmiotProwadzacy { get; set; } = new List<PrzedmiotProwadzacy>();
    }
}

