using MVC_EduScanner.Enums;
using static MVC_EduScanner.Models.App_DbContext;


namespace MVC_EduScanner.Models
{
    public class Przedmiot
    {
        public int PrzedmiotID { get; set; }
        public string NazwaPrzedmiotu { get; set; } = string.Empty;

        public Format_nauczania Format {  get; set; }

        // Relacja wiele-do-wielu z prowadzącymi
        public List<PrzedmiotProwadzacy> PrzedmiotProwadzacy { get; set; } = new List<PrzedmiotProwadzacy>();
    }
}
