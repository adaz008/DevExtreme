using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Codenames.Model
{
    public class Word
    {
        public int Id { get; set; }
        public string WordItem { get; set; } = null!;
        public int GameId { get; set; }
    }
}
