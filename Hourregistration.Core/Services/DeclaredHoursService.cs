using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Models;

namespace Hourregistration.Core.Services
{
    public class DeclaredHoursService : IDeclaredHoursService
    {
        private readonly IDeclaredHoursRepository _declaredHoursRepository;
        
        public DeclaredHoursService(IDeclaredHoursRepository declaredHoursRepository)
        {
            _declaredHoursRepository = declaredHoursRepository;
        }

        public DeclaredHours? Get(long id)
        {
            return _declaredHoursRepository.Get(id);
        }
        public List<DeclaredHours> GetByUserId(long userId)
        {
            return _declaredHoursRepository.GetByUserId(userId);
        }
        public List<DeclaredHours> GetByState(DeclaredState state)
        {
            return _declaredHoursRepository.GetByState(state);
        }
        public List<DeclaredHours> GetAll()
        {
            return _declaredHoursRepository.GetAll();
        }
        public DeclaredHours GetLatestDeclarationFromUserId(long userId)
        {
            return _declaredHoursRepository.GetLatestDeclarationFromUserId(userId);
        }
        public DeclaredHours Add(DeclaredHours declaredHour)
        {
            return _declaredHoursRepository.Add(declaredHour);
        }
        public DeclaredHours Update(DeclaredHours declaredHour)
        {
            return _declaredHoursRepository.Update(declaredHour);
        }
        public DeclaredHours Delete(long id)
        {
            return _declaredHoursRepository.Delete(id);
        }

        public Task<DeclaredHours?> GetAsync(long id)
        {
            return _declaredHoursRepository.GetAsync(id);
        }
        public Task<List<DeclaredHours>> GetByUserIdAsync(long userId)
        {
            return _declaredHoursRepository.GetByUserIdAsync(userId);
        }
        public Task<List<DeclaredHours>> GetByStateAsync(DeclaredState state)
        {
            return _declaredHoursRepository.GetByStateAsync(state);
        }
        public Task<List<DeclaredHours>> GetAllAsync()
        {
            return _declaredHoursRepository.GetAllAsync();
        }
        public Task<DeclaredHours> GetLatestDeclarationFromUserIdAsync(long userId)
        {
            return _declaredHoursRepository.GetLatestDeclarationFromUserIdAsync(userId);
        }
        public Task<DeclaredHours> AddAsync(DeclaredHours declaredHour)
        {
            return _declaredHoursRepository.AddAsync(declaredHour);
        }
        public Task<DeclaredHours> UpdateAsync(DeclaredHours declaredHour)
        {
            return _declaredHoursRepository.UpdateAsync(declaredHour);
        }
        public Task<DeclaredHours> DeleteAsync(long id)
        {
            return _declaredHoursRepository.DeleteAsync(id);
        }
    }
}