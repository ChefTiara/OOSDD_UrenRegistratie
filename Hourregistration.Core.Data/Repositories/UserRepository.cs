using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Hourregistration.Core.Data.Repositories
{
    public class LocalUserRepository
    {
        private readonly List<LocalUser> _users = [
            new LocalUser(1, "Wuser", "1234", Role.Werknemer),
            new LocalUser(2, "OGuser", "1234", Role.Opdrachtgever),
            new LocalUser(3, "AMuser", "1234", Role.AdministratieMedewerker),
            new LocalUser(4, "Buser", "1234", Role.Beheer)
        ];

        public LocalUser? Authenticate(string username, string password)
        {
            return _users.FirstOrDefault(
                u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                && u.Password == password
            );
        }

        public Task<LocalUser?> Get(long userId)
        {             
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == userId)); 
        }

        public Task<List<LocalUser>> GetAll()
        {
            return Task.FromResult(_users);
        }

        public Task<List<LocalUser>> GetAllFromRole(Role role)
        {
            var usersFromRole = _users.Where(u => u.Role == role).ToList();
            return Task.FromResult(usersFromRole);
        }
    }
}