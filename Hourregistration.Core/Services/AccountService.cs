using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Interfaces.Services;
using Hourregistration.Core.Models;

namespace Hourregistration.Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly SemaphoreSlim _mutex = new(1, 1);

        private readonly ILocalUserRepository _userRepository;

        public AccountService(ILocalUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<LocalUser>> GetAllAsync()
        {
            return await _userRepository.GetAll();
        }

        private void Validate(string username, string passwordOrHash, string role)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username is required");
            if (username.Length < 3) throw new ArgumentException("Username must be at least 3 chars");
            if (string.IsNullOrEmpty(passwordOrHash) || passwordOrHash.Length < 6) throw new ArgumentException("Password must be at least 6 chars");
            if (string.IsNullOrWhiteSpace(role)) throw new ArgumentException("Role is required");
        }

        public async Task<LocalUser> CreateAsync(string username, string passwordHash, string role)
        {
            // validate inputs before acquiring mutex
            Validate(username, passwordHash, role);

            await _mutex.WaitAsync();
            try
            {
                var accounts = await _userRepository.GetAll();
                if (accounts.Any(a => a.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException("Username exists");

                var acc = await _userRepository.AddAsync(username, passwordHash, Enum.Parse<Role>(role));
                await Task.Delay(150); // keep under 2s (simulated)
                return acc;
            }
            finally { _mutex.Release(); }
        }

        public async Task UpdateAsync(LocalUser updated)
        {
            if (updated == null) throw new ArgumentNullException(nameof(updated));
            Validate(updated.Username, updated.Password, updated.Role.ToString());
            await _mutex.WaitAsync();
            try
            {
                var accounts = await _userRepository.GetAll();
                var existing = accounts.FirstOrDefault(a => a.Id == updated.Id) ?? throw new InvalidOperationException("Not found");
                existing.Username = updated.Username;
                existing.Password = updated.Password;
                existing.Role = updated.Role;
                existing.IsActive = updated.IsActive;
                await _userRepository.UpdateAsync(existing);
                await Task.Delay(150);
            }
            finally { _mutex.Release(); }
        }

        public async Task DeactivateAsync(long id)
        {
            await _mutex.WaitAsync();
            try
            {
                var accounts = await _userRepository.GetAll();
                var existing = accounts.FirstOrDefault(a => a.Id == id) ?? throw new InvalidOperationException("Not found");
                existing.IsActive = false;
                await _userRepository.UpdateAsync(existing);
                await Task.Delay(100);
            }
            finally { _mutex.Release(); }
        }
    }
}
