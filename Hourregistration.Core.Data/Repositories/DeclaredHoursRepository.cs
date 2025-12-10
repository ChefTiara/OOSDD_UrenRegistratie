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
                // Pending examples
                new DeclaredHours(1, new DateOnly(2025, 11, 3), new TimeOnly(8, 20), new TimeOnly(16, 20), "Boodschappenapp", "Angela de Wit"),
                new DeclaredHours(2, new DateOnly(2025, 11, 4), new TimeOnly(7, 20), new TimeOnly(18, 20), "Boodschappenapp", "Boris Hout"),
                new DeclaredHours(3, new DateOnly(2025, 11, 5), new TimeOnly(8, 20), new TimeOnly(17, 20), "Boodschappenapp", "Abel van Rutje"),

                // Reviewed examples
                new DeclaredHours(
                    4,
                    new DateOnly(2025, 11, 6),
                    new TimeOnly(8, 20),
                    new TimeOnly(16, 20),
                    "Boodschappenapp",
                    "Angela de Wit",
                    submittedOn: new DateOnly(2025, 11, 6),
                    reviewedOn: new DateOnly(2025, 11, 7),
                    state: DeclaredState.Approved),
                new DeclaredHours(
                    5,
                    new DateOnly(2025, 11, 7),
                    new TimeOnly(9, 20),
                    new TimeOnly(17, 20),
                    "Boodschappenapp",
                    "Boris Hout",
                    submittedOn: new DateOnly(2025, 11, 7),
                    reviewedOn: new DateOnly(2025, 11, 8),
                    state: DeclaredState.Denied),

                new DeclaredHours(
                    6,
                    new DateOnly(2025, 11, 10),
                    new TimeOnly(9, 20),
                    new TimeOnly(17, 20),
                    "Urenregistratie",
                    "Jannet M.",
                    submittedOn: new DateOnly(2025, 11, 10),
                    reviewedOn: new DateOnly(2025, 11, 11),
                    state: DeclaredState.Approved),
                new DeclaredHours(
                    7,
                    new DateOnly(2025, 11, 11),
                    new TimeOnly(8, 20),
                    new TimeOnly(17, 20),
                    "Boodschappenapp",
                    "Jens K.",
                    submittedOn: new DateOnly(2025, 11, 11),
                    reviewedOn: new DateOnly(2025, 11, 12),
                    state: DeclaredState.Denied),
                new DeclaredHours(
                    8,
                    new DateOnly(2025, 11, 12),
                    new TimeOnly(9, 20),
                    new TimeOnly(17, 20),
                    "Urenregistratie",
                    "Maaike V.",
                    submittedOn: new DateOnly(2025, 11, 12))
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