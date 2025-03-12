using System.Collections.Generic;
using System.Reflection.Emit;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Aplikacja_tworzeniebazy.Models
{
    public class Modele_klas
    {
  


public class Przedmiot
    {
        public int PrzedmiotID { get; set; }
        public string NazwaPrzedmiotu { get; set; } = string.Empty;

        // Relacja wiele-do-wielu z prowadzącymi
        public List<PrzedmiotProwadzacy> PrzedmiotProwadzacy { get; set; } = new List<PrzedmiotProwadzacy>();
    }
        /// //////////////////////////////////////////////

        public class Prowadzacy
    {
        public int ProwadzacyID { get; set; }
        public string Imie { get; set; } = string.Empty;
        public string Nazwisko { get; set; } = string.Empty;

        // Relacja wiele-do-wielu z przedmiotami
        public List<PrzedmiotProwadzacy> PrzedmiotProwadzacy { get; set; } = new List<PrzedmiotProwadzacy>();
    }
       
        /// //////////////////////////////////////////////
      
    public class PrzedmiotProwadzacy
    {
        public int PrzedmiotID { get; set; }
        public Przedmiot Przedmiot { get; set; }

        public int ProwadzacyID { get; set; }
        public Prowadzacy Prowadzacy { get; set; }
    }

        /// //////////////////////////////////////////////
        public class AppDbContext : DbContext
    {
        private readonly string _connectionString;

        // Konstruktor umożliwiający przekazanie connection stringa
        public AppDbContext(string connectionString)
        {
            _connectionString = connectionString;
            }

        //  wczytanie connection string z pliku json 
        //"ConnectionStrings": {
   // "DefaultConnection": "Server=LUKASZ\\SQLEXPRESS;Database=UczelniaDB;Trusted_Connection=True;TrustServerCertificate=True;"
  //} i tworzy baze i tabele
        public AppDbContext() : this(GetConnectionStringFromConfiguration()) { }
        private static string GetConnectionStringFromConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            var configuration = builder.Build();
            return configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' nie został znaleziony.");
        }

        // Definicje tabel w bazie
        public DbSet<Przedmiot> Przedmioty { get; set; }
        public DbSet<Prowadzacy> Prowadzacy { get; set; }
        public DbSet<PrzedmiotProwadzacy> PrzedmiotProwadzacy { get; set; }

        // Konfiguracja połączenia z bazą
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(_connectionString);
        }

        // Konfiguracja relacji 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PrzedmiotProwadzacy>()
                .HasKey(pp => new { pp.PrzedmiotID, pp.ProwadzacyID });

            modelBuilder.Entity<PrzedmiotProwadzacy>()
                .HasOne(pp => pp.Przedmiot)
                .WithMany(p => p.PrzedmiotProwadzacy)
                .HasForeignKey(pp => pp.PrzedmiotID);

            modelBuilder.Entity<PrzedmiotProwadzacy>()
                .HasOne(pp => pp.Prowadzacy)
                .WithMany(p => p.PrzedmiotProwadzacy)
                .HasForeignKey(pp => pp.ProwadzacyID);
        }
    }

}
}
