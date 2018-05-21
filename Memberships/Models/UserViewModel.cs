using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Memberships.Models
{
    public class UserViewModel
    {
        [Display(Name = "User Id")]
        public string Id { get; set; }

        [Display(Name = "Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 2)]
        public string FirstName { get; set; }

        [Display(Name = "Password")]
        [Required]
        [StringLength(30, ErrorMessage = "The {0} must be at least {2} characters long and have 1 non letter, 1 digit, 1 upper case (\'A\'-\'Z\'). ",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}