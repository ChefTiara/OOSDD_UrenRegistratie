using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hourregistration.App.Models
{
    public class LocalUser
    {
        public string Username { get; set; }
        public string Password { get; set; } // plain text for demo
        public string Role { get; set; }     // store as string, parse later
    }
}
