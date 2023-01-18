using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Codenames.Model
{
    public enum Team
    {
        RED = 1, BLUE=2
    }
    public class Player
    {
        
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public User? User { get; set; }
        public int? UserId { get; set; }
        public bool Spymaster { get; set; }
        public Team Team { get; set; }

    }
}
