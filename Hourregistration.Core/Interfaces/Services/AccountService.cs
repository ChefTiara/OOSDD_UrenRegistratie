using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Hourregistration.Core.Models;

namespace Hourregistration.Core.Services
{
    public class AccountService
    {
        private static readonly Lazy<AccountService> _instance = new(() => new AccountService());
        public static AccountService Instance => _instance.Value;

        private readonly ObservableCollection<Account> _accounts = new();
        private readonly SemaphoreSlim _mutex = new(1, 1);

        private AccountService()
        {
            // Seed beheer account (use same password you set in App if needed)
            _accounts.Add(new Account { Username = "beheer", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"), Role = "Beheer", IsActive = true });
        }

        public ObservableCollection<Account> GetAll() => _accounts;

        private void Validate(string username, string passwordOrHash, string role)
        {
            if (string.IsNullOrWhiteSpace(username)) throw new ArgumentException("Username is required");
            if (username.Length < 3) throw new ArgumentException("Username must be at least 3 chars");
            if (string.IsNullOrEmpty(passwordOrHash) || passwordOrHash.Length < 6) throw new ArgumentException("Password must be at least 6 chars");
            if (string.IsNullOrWhiteSpace(role)) throw new ArgumentException("Role is required");
        }

        public async Task<Account> CreateAsync(string username, string passwordHash, string role)
        {
            Validate(username, passwordHash, role);
            await _mutex.WaitAsync();
            try
            {
                if (_accounts.Any(a => a.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
                    throw new InvalidOperationException("Username exists");

                var acc = new Account { Username = username, PasswordHash = passwordHash, Role = role, IsActive = true };
                _accounts.Add(acc);
                await Task.Delay(150); // keep under 2s
                return acc;
            }
            finally { _mutex.Release(); }
        }

        public async Task UpdateAsync(Account updated)
        {
            if (updated == null) throw new ArgumentNullException(nameof(updated));
            Validate(updated.Username, updated.PasswordHash, updated.Role);
            await _mutex.WaitAsync();
            try
            {
                var existing = _accounts.FirstOrDefault(a => a.Id == updated.Id) ?? throw new InvalidOperationException("Not found");
                existing.Username = updated.Username;
                existing.PasswordHash = updated.PasswordHash;
                existing.Role = updated.Role;
                existing.IsActive = updated.IsActive;
                await Task.Delay(150);
            }
            finally { _mutex.Release(); }
        }

        public async Task DeactivateAsync(Guid id)
        {
            await _mutex.WaitAsync();
            try
            {
                var existing = _accounts.FirstOrDefault(a => a.Id == id) ?? throw new InvalidOperationException("Not found");
                existing.IsActive = false;
                await Task.Delay(100);
            }
            finally { _mutex.Release(); }
        }
    }
}
