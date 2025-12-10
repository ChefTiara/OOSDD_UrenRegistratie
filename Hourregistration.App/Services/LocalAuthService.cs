using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hourregistration.Core.Data.Repositories;
using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Models;

namespace Hourregistration.App.Services
{
    public class LocalAuthService
    {
        private readonly ILocalUserRepository _localUserRepository = ServiceHelper.GetService<ILocalUserRepository>()!;

        public (bool ok, string? role) Authenticate(string username, string password)
        {
            LocalUser? user = _localUserRepository.Authenticate(username, password);

            if (user == null)
                return (false, null);

            return (true, user.Role.ToString());
        }
    }
}