using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Common.Objects
{
    public class Account
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Username { get; set; }

        [Required]
        [MaxLength(320)]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public bool EmailVerified { get; set; }

        // bool 타입에서 어느부분이 틀렸을지 알려줄 목적으로 enum 타입으로 변경할 것
        public bool Validate()
        {
            if (!ValidateUsername(Username) || !ValidateEmail(Email) || !ValidatePassword(Password))
            {
                return false;
            }

            return true;
        }

        public bool ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return false;
            }

            return Regex.IsMatch(username, @"^[a-zA-Z0-9]{3,30}$");
        }

        public bool ValidateEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        public bool ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            // 최소 8자, 최소 하나의 대문자, 하나의 소문자, 하나의 숫자, 하나의 특수문자를 포함
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");
        }
    }
}
