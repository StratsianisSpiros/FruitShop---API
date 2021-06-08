using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class RegisterDto
    {
        [Required]
        [StringLength(20)]
        [RegularExpression(ValidationHelpers.namePattern, ErrorMessage = ValidationHelpers.nameError)]
        public string DisplayName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression(@"(?=^.{6,10}$)(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!@#$%^&amp;*()_+}{&quot;:;'?/&gt;.&lt;,])(?!.*\s).*$", 
                ErrorMessage = "Password must contain one uppercase, one number and one special character and be at least six characters long")]
        public string Password { get; set; }
    }
}
