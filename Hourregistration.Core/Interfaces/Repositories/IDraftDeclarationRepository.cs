using Hourregistration.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hourregistration.Core.Interfaces.Repositories
{
    public interface IDraftDeclarationRepository
    {
        DeclaredHours AddDraft(DeclaredHours declaration);
        List<DeclaredHours> GetAllDrafts();

        // Return the removed draft, or null when not found.
        DeclaredHours? DeleteDraft(DeclaredHours declaration);
    }
}
