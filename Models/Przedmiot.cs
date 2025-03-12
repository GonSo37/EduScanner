using static Aplikacja_tworzeniebazy.Models.Modele_klas;

namespace MVC_EduScanner.Models
{
    public class Przedmiot
    {
        public int PrzedmiotID { get; set; }
        public string NazwaPrzedmiotu { get; set; } = string.Empty;

        // Relacja wiele-do-wielu z prowadzącymi
        public List<PrzedmiotProwadzacy> PrzedmiotProwadzacy { get; set; } = new List<PrzedmiotProwadzacy>();
    }
}
