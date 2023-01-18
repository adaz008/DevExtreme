using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace Codenames.Model
{
    public enum Color
    {
        RED=1, BLUE=2, WHITE=3, BLACK=4
    }
    public class Field
    {
        public int Id { get; set; }
        public Word Word { get; set; } = null!;
        public int WordId { get; set; }
        public Color Color { get; set; }
        public bool Picked { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
