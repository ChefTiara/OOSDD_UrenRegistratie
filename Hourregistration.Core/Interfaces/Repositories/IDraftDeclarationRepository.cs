using Hourregistration.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hourregistration.Core.Interfaces.Repositories
{
    public interface IDraftDeclarationRepository
    {
        Declaration AddDraft(Declaration declaration);
        List<Declaration> GetAllDrafts();
        Declaration DeleteDraft(Declaration declaration);
    }
}
