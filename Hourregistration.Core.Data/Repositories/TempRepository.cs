using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Hourregistration.Core.Data.Repositories
{
    public class LocalUserRepository
    {
        private readonly List<LocalUser> _users = new()
        {
            new LocalUser(1, "Wuser", "1234", Role.Werknemer),
            new LocalUser(2, "OGuser", "1234", Role.Opdrachtgever),
            new LocalUser(3, "AMuser", "1234", Role.AdministratieMedewerker),
            new LocalUser(4, "Buser", "1234", Role.Beheer)
        };

        public LocalUser? Authenticate(string username, string password)
        {
            return _users.FirstOrDefault(
                u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase)
                && u.Password == password
            );
        }
    }
}