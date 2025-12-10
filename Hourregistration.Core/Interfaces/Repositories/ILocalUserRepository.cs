using Hourregistration.Core.Models;

namespace Hourregistration.Core.Interfaces.Repositories
{
    public interface ILocalUserRepository
    {
        public LocalUser? Authenticate(string username, string password);
        public Task<LocalUser?> Get(long userId);
        public Task<List<LocalUser>> GetAll();
        public Task<List<LocalUser>> GetAllFromRole(Role role);
    }
}