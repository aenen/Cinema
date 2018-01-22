namespace Cinema.Data.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web;

    [Table("TicketPrice")]
    public partial class TicketPrice
    {
        public TicketPrice()
        {

        }

        public int Id { get; set; }

        [Required]
        public int SeatId { get; set; }

        [Required]
        public int SessionId { get; set; }

        public int Price { get; set; }

        public virtual Seat Seat { get; set; }

        public virtual Session Session { get; set; }
        
        public virtual Ticket Ticket { get; set; }
    }
}