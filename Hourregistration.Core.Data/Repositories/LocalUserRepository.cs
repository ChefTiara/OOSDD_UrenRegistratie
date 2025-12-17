using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Hourregistration.Core.Data.Repositories
{
    public class LocalUserRepository : ILocalUserRepository
    {
        private readonly List<LocalUser> _users;
        private long _lastId;

        public LocalUserRepository()
        {
            // seed users with hashed passwords (use BCrypt for consistency)
            _users = new List<LocalUser>
            {
                new LocalUser(1, "Wuser", BCrypt.Net.BCrypt.HashPassword("1234"), Role.Werknemer),
                new LocalUser(2, "OGuser", BCrypt.Net.BCrypt.HashPassword("1234"), Role.Opdrachtgever),
                new LocalUser(3, "AMuser", BCrypt.Net.BCrypt.HashPassword("1234"), Role.Administratiemedewerker),
                new LocalUser(4, "Buser", BCrypt.Net.BCrypt.HashPassword("1234"), Role.Beheer)
            };

            _lastId = _users.Any() ? _users.Max(u => u.Id) : 0;
        }

        public LocalUser? Authenticate(string username, string password)
        {
            var user = _users.FirstOrDefault(u => u.Username.Equals(username, System.StringComparison.OrdinalIgnoreCase));
            if (user == null) return null;

            var stored = user.Password ?? string.Empty;

            if (!user.IsActive)
            {
                return null;
            }

            // If stored password looks like a bcrypt hash, verify; otherwise fall back to plain compare
            if (stored.StartsWith("$2a$") || stored.StartsWith("$2b$") || stored.StartsWith("$2y$"))
            {
                return BCrypt.Net.BCrypt.Verify(password, stored) ? user : null;
            }

            return stored == password ? user : null;
        }

        public Task<LocalUser?> Get(long userId, CancellationToken ct = default)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == userId));
        }

        public Task<List<LocalUser>> GetAll(CancellationToken ct = default)
        {
            // return a shallow copy to avoid callers mutating internal list directly
            return Task.FromResult(_users.ToList());
        }

        public Task<List<LocalUser>> GetAllFromRole(Role role, CancellationToken ct = default)
        {
            var usersFromRole = _users.Where(u => u.Role == role).ToList();
            return Task.FromResult(usersFromRole);
        }

        public Task<LocalUser> AddAsync(string username, string password, Role role, CancellationToken ct = default)
        {
            var id = Interlocked.Increment(ref _lastId);
            var user = new LocalUser(id, username, password, role);
            _users.Add(user);
            return Task.FromResult(user);
        }

        public Task UpdateAsync(LocalUser updated, CancellationToken ct = default)
        {
            var existing = _users.FirstOrDefault(u => u.Id == updated.Id)
                ?? throw new InvalidOperationException("User not found");
            existing.Username = updated.Username;
            existing.Password = updated.Password;
            existing.Role = updated.Role;
            existing.IsActive = updated.IsActive;
            existing.LatestDeclaration = updated.LatestDeclaration;
            return Task.CompletedTask;
        }
    }
}