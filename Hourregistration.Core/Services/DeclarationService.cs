using Hourregistration.Core.Models;
using System.Threading.Tasks;
using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Interfaces.Services;
using System.Linq;

namespace Hourregistration.Core.Services
{
    public class DeclarationService
    {
        private readonly IDeclaredHoursService _declaredHoursService;
        private readonly IDraftDeclarationRepository _draftRepo;

        public DeclarationService(IDeclaredHoursService declaredHoursService, IDraftDeclarationRepository draftRepo)
        {
            _declaredHoursService = declaredHoursService;
            _draftRepo = draftRepo;
        }

        public async Task<(bool Success, string Message)> IndienenAsync(DeclaredHours declaratie)
        {
            if (declaratie.WorkedHours <= 0)
                return (false, "Aantal uren moet groter zijn dan 0.");

            if (declaratie.WorkedHours > 24)
                return (false, "Aantal uren kan niet meer dan 24 zijn.");

            if (declaratie.Date == DateOnly.MinValue)
                return (false, "Selecteer een geldige datum.");

            if (string.IsNullOrWhiteSpace(declaratie.Reason))
                return (false, "Selecteer een geldige reden.");

            // Prevent total declared hours for the same user on the same day from exceeding 8 hours.
            // Fetch existing declarations for the user and sum hours for the same Date.
            var existing = await _declaredHoursService.GetByUserIdAsync(declaratie.UserId);
            var sameDayTotal = existing
                .Where(d => d.Date == declaratie.Date)
                .Sum(d => d.WorkedHours);

            // If adding this declaration would exceed 8 hours, reject it.
            if (sameDayTotal + declaratie.WorkedHours > 12.0)
            {
                return (false, "Totaal aantal uren voor deze datum mag niet meer dan 8 uur bedragen (inclusief reeds ingediende uren).");
            }

            await _declaredHoursService.AddAsync(declaratie);
            return (true, "Declaratie is succesvol ingediend!");
        }

        public Task OpslaanAlsDraftAsync(DeclaredHours declaratie)
        {
            _draftRepo.AddDraft(declaratie);
            return Task.CompletedTask;
        }

        // Return true when deleted, false when no matching draft existed
        public Task<bool> VerwijderenDraftAsync(DeclaredHours declaratie)
        {
            var removed = _draftRepo.DeleteDraft(declaratie);
            return Task.FromResult(removed != null);
        }
    }
}
