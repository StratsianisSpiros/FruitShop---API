using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    internal class ValidationHelpers
    {
        internal const string namePattern = "^[A-Z][A-Za-z]+$";
        internal const string nameError = "Name must contain only letters and first letter has to be capital and length no more than twenty";
    }
}
