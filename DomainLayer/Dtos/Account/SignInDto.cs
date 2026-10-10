using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Dtos.Account
{
    public class SignInDto
    {
        [EmailAddress]
        [Required(ErrorMessage = "Email is required.")]
        public string Email { get; set; }




        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Pasword is required.")]
        public string Password { get; set; }    
    }
}
