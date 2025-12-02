using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hourregistration.Core.Models
{
    public class DeclarationRowModel
    {
        public DateTime Datum { get; set; } = DateTime.Now;
        public double? AantalUren { get; set; }
        public string Reden { get; set; }
        public string Beschrijving { get; set; }
    }
}
