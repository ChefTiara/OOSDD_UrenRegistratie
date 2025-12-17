using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hourregistration.Core.Models;

namespace Hourregistration.Core.Interfaces.Services
{
    public interface IAccountService
    {
        public Task<LocalUser> CreateAsync(string username, string passwordHash, string role);
        public Task UpdateAsync(LocalUser updated);
        public Task DeactivateAsync(long userId);
    }
}
