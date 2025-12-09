using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Models;

namespace Hourregistration.Core.Data.Repositories
{
    public class DeclaredHoursRepository : IDeclaredHoursRepository
    {
        private readonly List<DeclaredHours> declaredHoursList;
        private readonly List<DeclaredHoursEmployee> declaredHoursList4;
        public DeclaredHoursRepository()
        {
            declaredHoursList = [
                new DeclaredHours(1, new DateOnly(2025, 11, 3), new TimeOnly(8, 20), new TimeOnly(16, 20), "Boodschappenapp", "Boodschappen", 0),
                new DeclaredHours(2, new DateOnly(2025, 11, 4), new TimeOnly(7, 20), new TimeOnly(18, 20), "Boodschappenapp", "Het is etenstijd waar ben je >:(", 0),
                new DeclaredHours(3, new DateOnly(2025, 11, 5), new TimeOnly(8, 20), new TimeOnly(17, 20), "Boodschappenapp", "", 0),
                new DeclaredHours(4, new DateOnly(2025, 11, 6), new TimeOnly(8, 20), new TimeOnly(16, 20), "Boodschappenapp", "", 0),
                new DeclaredHours(5, new DateOnly(2025, 11, 7), new TimeOnly(9, 20), new TimeOnly(17, 20), "Boodschappenapp", "Werk jij op vrijdag??", 0),

                new DeclaredHours(6, new DateOnly(2025, 11, 10), new TimeOnly(9, 20), new TimeOnly(17, 20), "Urenregistratie", "", 0) { State = DeclaredState.Akkoord },
                new DeclaredHours(7, new DateOnly(2025, 11, 11), new TimeOnly(8, 20), new TimeOnly(17, 20), "Boodschappenapp", "", 0),
                new DeclaredHours(8, new DateOnly(2025, 11, 12), new TimeOnly(9, 20), new TimeOnly(17, 20), "Urenregistratie", "", 0) { State = DeclaredState.Geweigerd },
            ];

            declaredHoursList4 =
            [
                new DeclaredHoursEmployee(9, "Kees", "Janssen", "Medewerker", 0, new DateOnly(2025, 11, 17), new TimeOnly(8, 20), new TimeOnly(16, 20)),
                new DeclaredHoursEmployee(10, "Jeroen", "de Boom", "Medewerker", 1, new DateOnly(2025, 11, 17), new TimeOnly(7, 20), new TimeOnly(18, 20)),
                new DeclaredHoursEmployee(11, "Teun", "van Kampen", "Teamleider", 2, new DateOnly(2025, 11, 17), new TimeOnly(8, 20), new TimeOnly(17, 20)),
                new DeclaredHoursEmployee(12, "Bilal", "Hout", "Medewerker", 3, new DateOnly(2025, 11, 17), new TimeOnly(8, 20), new TimeOnly(16, 20)),
                new DeclaredHoursEmployee(13, "Karsten", "de Lange", "Medewerker", 4, new DateOnly(2025, 11, 17), new TimeOnly(9, 20), new TimeOnly(17, 20)),
                new DeclaredHoursEmployee(14, "Bas", "de Graaf", "Medewerker", 5, new DateOnly(2025, 11, 17), new TimeOnly(9, 20), new TimeOnly(17, 20)),
                new DeclaredHoursEmployee(15, "Rodi", "Verschoor", "Teamleider", 6, new DateOnly(2025, 11, 17), new TimeOnly(8, 20), new TimeOnly(17, 20)),
                new DeclaredHoursEmployee(16, "Tyrone", "van Blokken", "Medewerker", 7, new DateOnly(2025, 11, 17), new TimeOnly(9, 20), new TimeOnly(17, 20)),
                new DeclaredHoursEmployee(17, "Daan", "de Vries", "Medewerker", 8, new DateOnly(2025, 11, 17), new TimeOnly(8, 20), new TimeOnly(16, 20)),
                new DeclaredHoursEmployee(18, "Luuk", "Jansen", "Medewerker", 9, new DateOnly(2025, 11, 17), new TimeOnly(7, 20), new TimeOnly(15, 20)),
                new DeclaredHoursEmployee(19, "Sven", "Klaassen", "Medewerker", 10, new DateOnly(2025, 11, 17), new TimeOnly(8, 20), new TimeOnly(16, 20)),
                new DeclaredHoursEmployee(20, "Milan", "de Wit", "Teamleider", 11, new DateOnly(2025, 11, 17), new TimeOnly(8, 20), new TimeOnly(17, 20)),
                new DeclaredHoursEmployee(21, "Jesse", "van den Berg", "Medewerker", 12, new DateOnly(2025, 11, 17), new TimeOnly(9, 20), new TimeOnly(17, 20)),
                new DeclaredHoursEmployee(22, "Finn", "Smits", "Medewerker", 13, new DateOnly(2025, 11, 17), new TimeOnly(8, 20), new TimeOnly(16, 20)),
            ];
        }

        public DeclaredHours? Get(int id)
        {
            return declaredHoursList.FirstOrDefault(dh => dh.Id == id);
        }
        public List<DeclaredHours> GetByUserId(long userId)
        {
            return declaredHoursList.Where(dh => dh.UserId == userId).ToList();
        }
        public List<DeclaredHours> GetByState(DeclaredState state)
        {
            return declaredHoursList.Where(dh => dh.State == state).ToList();
        }
        public List<DeclaredHours> GetAll()
        {
            return declaredHoursList;
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

        public Task<DeclaredHours?> GetAsync(int id)
        {
            return Task.FromResult(Get(id));
        }
        public Task<List<DeclaredHours>> GetByUserIdAsync(long userId)
        {
            return Task.FromResult(GetByUserId(userId));
        }
        public Task<List<DeclaredHours>> GetByStateAsync(DeclaredState state)
        {
            return Task.FromResult(GetByState(state));
        }
        public Task<List<DeclaredHours>> GetAllAsync()
        {
            return Task.FromResult(GetAll());
        }
        public Task<DeclaredHours> AddAsync(DeclaredHours declaredHour)
        {
            return Task.FromResult(Add(declaredHour));
        }
        public Task<DeclaredHours> UpdateAsync(DeclaredHours declaredHour)
        {
            return Task.FromResult(Update(declaredHour));
        }
        public Task<DeclaredHours> DeleteAsync(int id)
        {
            return Task.FromResult(Delete(id));
        }
        public List<DeclaredHoursEmployee> GetAllEmployeeHours()
        {
            return declaredHoursList4;
        }
        public DeclaredHoursEmployee? GetEmployeeHour(int id)
        {
            return declaredHoursList4.FirstOrDefault(d => d.Id == id);
        }
        public DeclaredHoursEmployee AddEmployeeHour(DeclaredHoursEmployee hour)
        {
            declaredHoursList4.Add(hour);
            return hour;
        }
    }
}