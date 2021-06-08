using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Dtos
{
    public class AddressDto
    {
        [Required]
        [StringLength(20)]
        [RegularExpression(ValidationHelpers.namePattern, ErrorMessage = ValidationHelpers.nameError)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(20)]
        [RegularExpression(ValidationHelpers.namePattern, ErrorMessage = ValidationHelpers.nameError)]
        public string LastName { get; set; }
        [Required]
        [StringLength(40, ErrorMessage ="Street can be of maximum forty characters")]
        public string Street { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Street can be of maximum twenty characters")]
        public string City { get; set; }
        [Required]
        [StringLength(20, ErrorMessage = "Street can be of maximum twenty characters")]
        public string State { get; set; }
        [Required]
        [StringLength(10, ErrorMessage = "Street can be of maximum ten characters")]
        public string ZipCode { get; set; }
    }
}
