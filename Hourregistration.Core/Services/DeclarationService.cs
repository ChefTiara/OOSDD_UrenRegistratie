using Hourregistration.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hourregistration.Core.Services
{
    public class DeclarationService
    {
        public (bool Success, string Message) Indienen(Declaration declaratie)
        {
            if (declaratie.AantalUren <= 0)
                return (false, "Aantal uren moet groter zijn dan 0.");

            if (declaratie.AantalUren > 24)
                return (false, "Aantal uren kan niet meer dan 24 zijn.");

            if (declaratie.Datum == default)
                return (false, "Selecteer een geldige datum.");

            return (true, "Declaratie is succesvol ingediend!");
        }
    }
}
