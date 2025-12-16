using Hourregistration.Core.Models;

namespace Hourregistration.Core.Interfaces.Repositories
{
    public interface ILocalUserRepository
    {
        public LocalUser? Authenticate(string username, string password);
        public Task<LocalUser?> Get(long userId, CancellationToken ct = default);
        public Task<List<LocalUser>> GetAll(CancellationToken ct = default);
        public Task<List<LocalUser>> GetAllFromRole(Role role, CancellationToken ct = default);
        public Task<LocalUser> AddAsync(string username, string password, Role role, CancellationToken ct = default);
    }
}