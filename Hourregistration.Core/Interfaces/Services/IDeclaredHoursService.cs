using Hourregistration.Core.Models;

namespace Hourregistration.Core.Interfaces.Services
{
    public interface IDeclaredHoursService
    {
        public DeclaredHours? Get(long id);
        public List<DeclaredHours> GetByUserId(long userId);
        public List<DeclaredHours> GetByState(DeclaredState state);
        public List<DeclaredHours> GetAll();
        public DeclaredHours GetLatestDeclarationFromUserId(long userId);
        public DeclaredHours Add(DeclaredHours declaredHour);
        public DeclaredHours Update(DeclaredHours declaredHour);
        public DeclaredHours Delete(long id);

        public Task<DeclaredHours?> GetAsync(long id);
        public Task<List<DeclaredHours>> GetByUserIdAsync(long userId);
        public Task<List<DeclaredHours>> GetByStateAsync(DeclaredState state);
        public Task<List<DeclaredHours>> GetAllAsync();
        public Task<DeclaredHours> GetLatestDeclarationFromUserIdAsync(long userId);
        public Task<DeclaredHours> AddAsync(DeclaredHours declaredHour);
        public Task<DeclaredHours> UpdateAsync(DeclaredHours declaredHour);
        public Task<DeclaredHours> DeleteAsync(long id);
    }
}