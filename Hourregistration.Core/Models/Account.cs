using System;

namespace Hourregistration.Core.Models
{
    public class Account
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = "Medewerker";
        public bool IsActive { get; set; } = true;
    }
}
