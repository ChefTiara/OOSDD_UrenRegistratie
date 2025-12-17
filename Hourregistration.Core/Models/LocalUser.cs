using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hourregistration.Core.Models
{
    public partial class LocalUser : Model
    {
        public string Username { get; set; }
        public string Password { get; set; } // plain text for demo
        public Role Role { get; set; } = Role.Werknemer;     // store as string, parse later
        public bool IsActive { get; set; } = true;
        public DateTime LatestDeclaration { get; set; }

        public LocalUser(long id, string username, string password, Role role): base(id)
        {
            Username = username;
            Password = password;
            Role = role;

        }
    }
}
