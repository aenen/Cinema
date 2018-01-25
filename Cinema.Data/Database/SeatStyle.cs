using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Data.Database
{
    public class SeatStyle
    {
        [Key, ForeignKey("Seat")]
        public int SeatId { get; set; }

        public double PositionX { get; set; }

        public double PositionY { get; set; }

        public double SizeWidth { get; set; }

        public double SizeHeight { get; set; }
        
        public virtual Seat Seat { get; set; }
    }
}
