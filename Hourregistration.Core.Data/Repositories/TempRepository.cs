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
            new LocalUser { Username = "Wuser",  Password = "1234", Role = "Werknemer" },
            new LocalUser { Username = "OGuser",      Password = "1234", Role = "Opdrachtgever" },
            new LocalUser { Username = "AMuser",   Password = "1234", Role = "AdministratieMedewerker" },
            new LocalUser { Username = "Buser",  Password = "1234", Role = "Beheer" }
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