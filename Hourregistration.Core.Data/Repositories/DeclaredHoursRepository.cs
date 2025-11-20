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
                new DeclaredHours(1, new DateOnly(2025, 11, 3), new TimeOnly(8, 20), new TimeOnly(16, 20), "Boodschappenapp"),
                new DeclaredHours(2, new DateOnly(2025, 11, 4), new TimeOnly(7, 20), new TimeOnly(18, 20), "Boodschappenapp"),
                new DeclaredHours(3, new DateOnly(2025, 11, 5), new TimeOnly(8, 20), new TimeOnly(17, 20), "Boodschappenapp"),
                new DeclaredHours(4, new DateOnly(2025, 11, 6), new TimeOnly(8, 20), new TimeOnly(16, 20), "Boodschappenapp"),
                new DeclaredHours(5, new DateOnly(2025, 11, 7), new TimeOnly(9, 20), new TimeOnly(17, 20), "Boodschappenapp"),
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