using Hourregistration.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hourregistration.Core.Interfaces.Repositories;

namespace Hourregistration.Core.Services
{
    public class DeclarationService
    {
        private readonly IDeclarationRepository _declarationRepo;
        private readonly IDraftDeclarationRepository _draftRepo;

        public DeclarationService(
            IDeclarationRepository declarationRepo,
            IDraftDeclarationRepository draftRepo)
        {
            _declarationRepo = declarationRepo;
            _draftRepo = draftRepo;
        }


        public (bool Success, string Message) Indienen(Declaration declaratie)
        {
            if (declaratie.AantalUren <= 0)
                return (false, "Aantal uren moet groter zijn dan 0.");

            if (declaratie.AantalUren > 24)
                return (false, "Aantal uren kan niet meer dan 24 zijn.");

            if (declaratie.Datum == default)
                return (false, "Selecteer een geldige datum.");

            if (string.IsNullOrWhiteSpace(declaratie.Reden))
                return (false, "Selecteer een geldige reden.");

            _declarationRepo.Add(declaratie);

            return (true, "Declaratie is succesvol ingediend!");
        }


        public void OpslaanAlsDraft(Declaration declaratie)
        {
            _draftRepo.AddDraft(declaratie);
        }


        public void VerwijderenDraft(Declaration declaratie)
        {
            _draftRepo.DeleteDraft(declaratie);
        }
    }
}
