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

        public DeclaredHours? Get(int id)
        {
            return _declaredHoursRepository.Get(id);
        }
        public List<DeclaredHours> GetByClient(Client client)
        {
            return _declaredHoursRepository.GetByClient(client);        
        }
        public List<DeclaredHours> GetByState(DeclaredState state)
        {
            return _declaredHoursRepository.GetByState(state);
        }
        public List<DeclaredHours> GetAll()
        {
            return _declaredHoursRepository.GetAll();
        }
        public DeclaredHours Add(DeclaredHours declaredHour)
        {
            return _declaredHoursRepository.Add(declaredHour);
        }
        public DeclaredHours Update(DeclaredHours declaredHour)
        {
            return _declaredHoursRepository.Update(declaredHour);
        }
        public DeclaredHours Delete(int id)
        {
            return _declaredHoursRepository.Delete(id);
        }

        public Task<DeclaredHours?> GetAsync(int id)
        {
            return _declaredHoursRepository.GetAsync(id);
        }
        public Task<List<DeclaredHours>> GetByClientAsync(Client client)
        {
            return _declaredHoursRepository.GetByClientAsync(client);
        }
        public Task<List<DeclaredHours>> GetByStateAsync(DeclaredState state)
        {
            return _declaredHoursRepository.GetByStateAsync(state);
        }
        public Task<List<DeclaredHours>> GetAllAsync()
        {
            return _declaredHoursRepository.GetAllAsync();
        }
        public Task<DeclaredHours> AddAsync(DeclaredHours declaredHour)
        {
            return _declaredHoursRepository.AddAsync(declaredHour);
        }
        public Task<DeclaredHours> UpdateAsync(DeclaredHours declaredHour)
        {
            return _declaredHoursRepository.UpdateAsync(declaredHour);
        }
        public Task<DeclaredHours> DeleteAsync(int id)
        {
            return _declaredHoursRepository.DeleteAsync(id);
        }
    }
}