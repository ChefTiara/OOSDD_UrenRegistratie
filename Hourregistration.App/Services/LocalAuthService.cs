using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hourregistration.Core.Data.Repositories;
using Hourregistration.Core.Models;

namespace Hourregistration.App.Services
{
    public class LocalAuthService
    {
        private readonly LocalUserRepository _repo = new();

        public (bool ok, string role) Authenticate(string username, string password)
        {
            LocalUser user = _repo.Authenticate(username, password);

            if (user == null)
                return (false, null);

            return (true, user.Role.ToString());
        }
    }
}