using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Codenames.Model
{
    public class User : IdentityUser
    {
        //public int Id { get; set; }
        public string? Name { get; set; }
        //public string Email { get; set; } = null!;
        //public string Password { get; set; } = null!;
        public int Games { get; set; }
        public int Wins { get; set; }
        public int GamesSm { get; set; }
        public int WinsSm { get; set; }

    }
}
