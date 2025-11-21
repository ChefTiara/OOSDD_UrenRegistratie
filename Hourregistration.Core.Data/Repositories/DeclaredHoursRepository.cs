using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Models;

namespace Hourregistration.Core.Data.Repositories
{
    public class DeclaredHoursRepository : IDeclaredHoursRepository
    {
        private readonly List<DeclaredHours> declaredHoursList;
        public DeclaredHoursRepository()
        {
            declaredHoursList = [
                new DeclaredHours(1,"Kees", "Janssen", "Medewerker", new DateOnly(2025, 11, 17), new TimeOnly(8, 20), new TimeOnly(16, 20), "Medewerker"),
                new DeclaredHours(2, "Jeroen", "de Boom", "Medewerker", new DateOnly(2025, 11, 17), new TimeOnly(7, 20), new TimeOnly(18, 20), "Medewerker"),
                new DeclaredHours(3, "Teun", "van Kampen", "Teamleider", new DateOnly(2025, 11, 17), new TimeOnly(8, 20), new TimeOnly(17, 20), "Teamleider"),
                new DeclaredHours(4, "Bilal", "Hout", "Medewerker", new DateOnly(2025, 11, 17), new TimeOnly(8, 20), new TimeOnly(16, 20), "Medewerker"),
                new DeclaredHours(5, "Karsten", "de Lange", "Medewerker", new DateOnly(2025, 11, 17), new TimeOnly(9, 20), new TimeOnly(17, 20), "Medewerker"),
                new DeclaredHours(6, "Bas", "de Graaf", "Medewerker", new DateOnly(2025, 11, 17), new TimeOnly(9, 20), new TimeOnly(17, 20), "Medewerker"),
                new DeclaredHours(7, "Rodi", "Verschoor", "Teamleider", new DateOnly(2025, 11, 17), new TimeOnly(8, 20), new TimeOnly(17, 20), "Teamleider"),
                new DeclaredHours(8, "Tyrone", "van Blokken", "Medewerker", new DateOnly(2025, 11, 17), new TimeOnly(9, 20), new TimeOnly(17, 20), "Medewerker"),
                new DeclaredHours(9, "Daan", "de Vries", "Medewerker", new DateOnly(2025, 11, 17), new TimeOnly(8, 20), new TimeOnly(16, 20), "Medewerker"),
                new DeclaredHours(10, "Luuk", "Jansen", "Medewerker", new DateOnly(2025, 11, 17), new TimeOnly(7, 20), new TimeOnly(15, 20), "Medewerker"),
                new DeclaredHours(11, "Sven", "Klaassen", "Medewerker", new DateOnly(2025, 11, 17), new TimeOnly(8, 20), new TimeOnly(16, 20), "Medewerker"),
                new DeclaredHours(12, "Milan", "de Wit", "Teamleider", new DateOnly(2025, 11, 17), new TimeOnly(8, 20), new TimeOnly(17, 20), "Teamleider"),
                new DeclaredHours(13, "Jesse", "van den Berg", "Medewerker", new DateOnly(2025, 11, 17), new TimeOnly(9, 20), new TimeOnly(17, 20), "Medewerker"),
                new DeclaredHours(14, "Finn", "Smits", "Medewerker", new DateOnly(2025, 11, 17), new TimeOnly(8, 20), new TimeOnly(16, 20), "Medewerker"),


            ];
        }

        public DeclaredHours? Get(int id)
        {
            return declaredHoursList.FirstOrDefault(dh => dh.Id == id);
        }
        public List<DeclaredHours> GetAll()
        {
            return declaredHoursList;
        }
        public List<DeclaredHours> GetByState(DeclaredState state)
        {
            return declaredHoursList.Where(dh => dh.State == state).ToList();
        }
        public DeclaredHours Add(DeclaredHours declaredHour)
        {
            declaredHoursList.Add(declaredHour);
            return declaredHour;
        }
        public DeclaredHours Update(DeclaredHours declaredHour)
        {
            DeclaredHours? existingDeclaredHour = Get(declaredHour.Id) ?? throw new ArgumentException("Declared hour not found");
            declaredHoursList.Remove(existingDeclaredHour);
            declaredHoursList.Add(declaredHour);
            return declaredHour;
        }
        public DeclaredHours Delete(int id)
        {
            DeclaredHours? existingDeclaredHour = Get(id) ?? throw new ArgumentException("Declared hour not found");
            declaredHoursList.Remove(existingDeclaredHour);
            return existingDeclaredHour;
        }
    }
}