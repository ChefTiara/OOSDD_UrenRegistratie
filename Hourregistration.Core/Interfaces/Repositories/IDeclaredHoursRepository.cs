using Hourregistration.Core.Models;

namespace Hourregistration.Core.Interfaces.Repositories
{
    public interface IDeclaredHoursRepository
    {
        public DeclaredHours? Get(int id);
        public List<DeclaredHours> GetByClientId(long clientId);
        public List<DeclaredHours> GetByState(DeclaredState state);
        public List<DeclaredHours> GetAll();
        public DeclaredHours Add(DeclaredHours declaredHour);
        public DeclaredHours Update(DeclaredHours declaredHour);
        public DeclaredHours Delete(int id);

        public Task<DeclaredHours?> GetAsync(int id);
        public Task<List<DeclaredHours>> GetByClientIdAsync(long clientId);
        public Task<List<DeclaredHours>> GetByStateAsync(DeclaredState state);
        public Task<List<DeclaredHours>> GetAllAsync();
        public Task<DeclaredHours> AddAsync(DeclaredHours declaredHour);
        public Task<DeclaredHours> UpdateAsync(DeclaredHours declaredHour);
        public Task<DeclaredHours> DeleteAsync(int id);
        public List<DeclaredHoursEmployee> GetAllEmployeeHours();
        public DeclaredHoursEmployee? GetEmployeeHour(int id);
        public DeclaredHoursEmployee AddEmployeeHour(DeclaredHoursEmployee hour);
    }
}