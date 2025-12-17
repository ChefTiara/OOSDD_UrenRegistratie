using Hourregistration.Core.Models;

namespace Hourregistration.Core.Interfaces.Repositories
{
    public interface IDeclaredHoursRepository
    {
        public DeclaredHours? Get(long id);
        public List<DeclaredHours> GetByUserId(long userId);
        public List<DeclaredHours> GetByState(DeclaredState state);
        public List<DeclaredHours> GetAll();
        public DeclaredHours GetLatestDeclarationFromUserId(long userId);
        public DeclaredHours Add(DeclaredHours declaredHour);
        public DeclaredHours Update(DeclaredHours declaredHour);
        public DeclaredHours Delete(long id);

        public Task<DeclaredHours?> GetAsync(long id, CancellationToken ct = default);
        public Task<List<DeclaredHours>> GetByUserIdAsync(long userId, CancellationToken ct = default);
        public Task<List<DeclaredHours>> GetByStateAsync(DeclaredState state, CancellationToken ct = default);
        public Task<List<DeclaredHours>> GetAllAsync(CancellationToken ct = default);
        public Task<DeclaredHours> GetLatestDeclarationFromUserIdAsync(long userId, CancellationToken ct = default);
        public Task<DeclaredHours> AddAsync(DateOnly date, int workedHours, string projectName, string description, long userId, CancellationToken ct = default);
        public Task<DeclaredHours> AddAsync(DeclaredHours declaredHour, CancellationToken ct = default);
        public Task<DeclaredHours> ReserveIdAsync(CancellationToken ct = default);
        public Task<DeclaredHours> UpdateAsync(DeclaredHours declaredHour, CancellationToken ct = default);
        public Task<DeclaredHours> DeleteAsync(long id, CancellationToken ct = default);
        public List<DeclaredHoursEmployee> GetAllEmployeeHours();
        public DeclaredHoursEmployee? GetEmployeeHour(long id);
        public DeclaredHoursEmployee AddEmployeeHour(DeclaredHoursEmployee hour);
    }
}