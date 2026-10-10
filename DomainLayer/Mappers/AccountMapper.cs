using DomainLayer.Dtos.Account;
using DomainLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Mappers
{
    public static class AccountMapper
    {
        public static AppUser ToUser(this SignUpDto signUpDto)
        {
            return new AppUser
            {
                FullName = signUpDto.FullName,
                UserName = signUpDto.Email,
                Email = signUpDto.Email,

                RoleName = "User",
                CreatedOn = DateTimeOffset.UtcNow,
                ModifiedOn = null
            };
        }
    }
}
