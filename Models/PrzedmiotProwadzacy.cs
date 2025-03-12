namespace MVC_EduScanner.Models
{
    public class PrzedmiotProwadzacy
    {
        public int PrzedmiotID { get; set; }
        public Przedmiot Przedmiot { get; set; }

        public int ProwadzacyID { get; set; }
        public Prowadzacy Prowadzacy { get; set; }
    }
}
