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

        // Returns the authenticated LocalUser or null
        public LocalUser? Authenticate(string username, string password)
        {
            LocalUser? user = _localUserRepository.Authenticate(username, password);

            // repository already verifies hash/plaintext; just return user
            return user;
        }
    }
}