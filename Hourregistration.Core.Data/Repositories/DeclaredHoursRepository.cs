using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Models;

namespace Hourregistration.Core.Data.Repositories
{
    public class DeclaredHoursRepository : IDeclaredHoursRepository
    {
        private readonly List<DeclaredHours> declaredHoursList;
        private readonly List<DeclaredHoursEmployee> declaredHoursList4;
        private readonly ILocalUserRepository _localUserRepository;

        private long _lastId;
        public DeclaredHoursRepository(ILocalUserRepository localUserRepository)
        {
            _localUserRepository = localUserRepository;
            declaredHoursList = [
                new DeclaredHours(1, new DateOnly(2025, 11, 3), 7, "Boodschappenapp", "Boodschappen", 0) { User = _localUserRepository.Get(0).Result! },
                new DeclaredHours(2, new DateOnly(2025, 11, 4), 3, "Boodschappenapp", "Het is etenstijd waar ben je >:(", 1) { User = _localUserRepository.Get(1).Result! },
                new DeclaredHours(3, new DateOnly(2025, 11, 5), 6, "Boodschappenapp", "", 0) { User = _localUserRepository.Get(0).Result! },
                new DeclaredHours(4, new DateOnly(2025, 11, 6), 2, "Boodschappenapp", "", 2) { User = _localUserRepository.Get(2).Result! },
                new DeclaredHours(5, new DateOnly(2025, 11, 7), 3, "Boodschappenapp", "Werk jij op vrijdag??", 1) { User = _localUserRepository.Get(1).Result! },

                new DeclaredHours(6, new DateOnly(2025, 11, 10), 4, "Urenregistratie", "", 3) { State = DeclaredState.Akkoord, User = _localUserRepository.Get(3).Result! },
                new DeclaredHours(7, new DateOnly(2025, 11, 11), 6, "Boodschappenapp", "", 0) { User = _localUserRepository.Get(0).Result! },
                new DeclaredHours(8, new DateOnly(2025, 11, 12), 3, "Urenregistratie", "", 2) { State = DeclaredState.Geweigerd, User = _localUserRepository.Get(2).Result! },
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

            _lastId = declaredHoursList.Any() ? declaredHoursList.Max(u => u.Id) : 0;
        }

        public DeclaredHours? Get(long id)
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
        public DeclaredHours GetLatestDeclarationFromUserId(long userId)
        {
            return declaredHoursList
                .Where(dh => dh.UserId == userId)
                .OrderByDescending(dh => dh.Date)
                .ThenByDescending(dh => dh.CreatedAt)
                .First();
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
        public DeclaredHours Delete(long id)
        {
            DeclaredHours? existingDeclaredHour = Get(id) ?? throw new ArgumentException("Declared hour not found");
            declaredHoursList.Remove(existingDeclaredHour);
            return existingDeclaredHour;
        }

        public Task<DeclaredHours?> GetAsync(long id, CancellationToken ct = default)
        {
            return Task.FromResult(Get(id));
        }
        public Task<List<DeclaredHours>> GetByUserIdAsync(long userId, CancellationToken ct = default)
        {
            return Task.FromResult(GetByUserId(userId));
        }
        public Task<List<DeclaredHours>> GetByStateAsync(DeclaredState state, CancellationToken ct = default)
        {
            return Task.FromResult(GetByState(state));
        }
        public Task<List<DeclaredHours>> GetAllAsync(CancellationToken ct = default)
        {
            return Task.FromResult(GetAll());
        }
        public Task<DeclaredHours> GetLatestDeclarationFromUserIdAsync(long userId, CancellationToken ct = default)
        {
            return Task.FromResult(GetLatestDeclarationFromUserId(userId));
        }
        public Task<DeclaredHours> AddAsync(DateOnly date, int workedHours, string projectName, string description, long userId, CancellationToken ct = default)
        {
            var id = Interlocked.Increment(ref _lastId);
            var declaredHour = new DeclaredHours(id, date, workedHours, projectName, description, userId);
            return Task.FromResult(Add(declaredHour));
        }
        public Task<DeclaredHours> UpdateAsync(DeclaredHours declaredHour, CancellationToken ct = default)
        {
            return Task.FromResult(Update(declaredHour));
        }
        public Task<DeclaredHours> DeleteAsync(long id, CancellationToken ct = default)
        {
            return Task.FromResult(Delete(id));
        }
        public List<DeclaredHoursEmployee> GetAllEmployeeHours()
        {
            return declaredHoursList4;
        }
        public DeclaredHoursEmployee? GetEmployeeHour(long id)
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