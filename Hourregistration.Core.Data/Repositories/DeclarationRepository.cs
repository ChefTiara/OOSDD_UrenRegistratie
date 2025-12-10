using Hourregistration.Core.Interfaces.Repositories;
using Hourregistration.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hourregistration.Core.Data.Repositories
{
    public class DeclarationRepository : IDeclarationRepository
    {
        private List<Declaration> declarations = new();

        public Declaration Add(Declaration declaration)
        {
            declarations.Add(declaration);
            return declaration;
        }

        public List<Declaration> GetAll()
        {
            return declarations;
        }

        public Declaration Delete(Declaration declaration)
        {
            declarations.Remove(declaration);
            return declaration;
        }
    }
}
