using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;

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
        public string PasswordHash { get; set; }

        public bool EmailVerified { get; set; }
    }
}
