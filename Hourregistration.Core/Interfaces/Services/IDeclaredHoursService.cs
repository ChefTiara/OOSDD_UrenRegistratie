using Hourregistration.Core.Models;

namespace Hourregistration.Core.Interfaces.Services
{
    public interface IDeclaredHoursService
    {
        public DeclaredHours? Get(int id);
        public List<DeclaredHours> GetByClient(Client client);
        public List<DeclaredHours> GetByState(DeclaredState state);
        public List<DeclaredHours> GetAll();
        public DeclaredHours Add(DeclaredHours declaredHour);
        public DeclaredHours Update(DeclaredHours declaredHour);
        public DeclaredHours Delete(int id);

        public Task<DeclaredHours?> GetAsync(int id);
        public Task<List<DeclaredHours>> GetByClientAsync(Client client);
        public Task<List<DeclaredHours>> GetByStateAsync(DeclaredState state);
        public Task<List<DeclaredHours>> GetAllAsync();
        public Task<DeclaredHours> AddAsync(DeclaredHours declaredHour);
        public Task<DeclaredHours> UpdateAsync(DeclaredHours declaredHour);
        public Task<DeclaredHours> DeleteAsync(int id);
    }
}