using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Dtos.Account
{
    public class SignUpDto
    {
        [Display(Name = "Full name")]
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Full name must be between 3 to 20 characters.")]
        public string FullName { get; set; }



        [EmailAddress]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }



        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Pasword is required.")]
        public string Password { get; set; }



        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Required(ErrorMessage = "Confirm your password.")]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } 
    }
}
