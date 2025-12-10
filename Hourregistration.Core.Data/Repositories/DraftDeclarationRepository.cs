using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Models;

namespace Hourregistration.Core.Data.Repositories
{
    public class DraftDeclarationRepository : IDraftDeclarationRepository
    {
        private List<Declaration> draftDeclarations = new();

        public Declaration AddDraft(Declaration declaration)
        {
            draftDeclarations.Add(declaration);
            return declaration;
        }

        public List<Declaration> GetAllDrafts()
        {
            return draftDeclarations;
        }

        public Declaration DeleteDraft(Declaration declaration)
        {
            draftDeclarations.Remove(declaration);
            return declaration;
        }
    }
}
