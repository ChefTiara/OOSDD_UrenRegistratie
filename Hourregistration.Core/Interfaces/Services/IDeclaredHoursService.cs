using Hourregistration.Core.Models;

namespace Hourregistration.Core.Interfaces.Services
{
    public interface IDeclaredHoursService
    {
        public DeclaredHours? Get(int id);
        public List<DeclaredHours> GetAll();
        public List<DeclaredHours> GetByState(DeclaredState state);
        public DeclaredHours Add(DeclaredHours declaredHour);
        public DeclaredHours Update(DeclaredHours declaredHour);
        public DeclaredHours Delete(int id);
    }
}