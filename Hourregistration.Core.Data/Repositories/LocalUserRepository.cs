using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Hourregistration.Core.Data.Repositories
{
    public class LocalUserRepository : ILocalUserRepository
    {
        private readonly List<LocalUser> _users = [
            new LocalUser(1, "Wuser", "1234", Role.Werknemer),
            new LocalUser(2, "OGuser", "1234", Role.Opdrachtgever),
            new LocalUser(3, "AMuser", "1234", Role.Administratiemedewerker),
            new LocalUser(4, "Buser", "1234", Role.Beheer)
        ];

        // last assigned id (not next). Initialized from existing data.
        private long _lastId;

        public LocalUserRepository()
        {
            _lastId = _users.Any() ? _users.Max(u => u.Id) : 0;
        }

        public LocalUser? Authenticate(string username, string password)
        {
            return _users.FirstOrDefault(
                u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                && u.Password == password
            );
        }

        public Task<LocalUser?> Get(long userId, CancellationToken ct = default)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == userId));
        }

        public Task<List<LocalUser>> GetAll(CancellationToken ct = default)
        {
            return Task.FromResult(_users);
        }

        public Task<List<LocalUser>> GetAllFromRole(Role role, CancellationToken ct = default)
        {
            var usersFromRole = _users.Where(u => u.Role == role).ToList();
            return Task.FromResult(usersFromRole);
        }

        public Task<LocalUser> AddAsync(string username, string password, Role role, CancellationToken ct = default)
        {
            var id = Interlocked.Increment(ref _lastId); // first id will be max(existing)+1
            var user = new LocalUser(id, username, password, role);
            _users.Add(user);
            return Task.FromResult(user);
        }
    }
}