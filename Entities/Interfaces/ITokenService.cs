using Entities.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
