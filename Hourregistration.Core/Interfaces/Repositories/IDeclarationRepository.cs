using Hourregistration.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hourregistration.Core.Interfaces.Repositories
{
    public interface IDeclarationRepository
    {
        Declaration Add(Declaration declaration);
        List<Declaration> GetAll();
        Declaration Delete(Declaration declaration);
    }
}
