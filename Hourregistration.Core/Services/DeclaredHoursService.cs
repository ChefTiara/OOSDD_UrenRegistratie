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
        public List<DeclaredHours> GetAll()
        {
            return _declaredHoursRepository.GetAll();
        }
        public List<DeclaredHours> GetByState(DeclaredState state)
        {
            return _declaredHoursRepository.GetByState(state);
        }
        public double GetTotalWorkedHours()
        {
            var allDeclaredHours = _declaredHoursRepository.GetAll();
            return allDeclaredHours.Sum(dh => dh.WorkedHours);
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

    }
}