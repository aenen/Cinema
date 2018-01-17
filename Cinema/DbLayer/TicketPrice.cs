namespace Cinema.DbLayer
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

        public int SeatId { get; set; }

        public int SessionId { get; set; }

        public int Price { get; set; }

        [Required]
        public virtual Seat Seat { get; set; }

        [Required]
        public virtual Session Session { get; set; }
        
        public virtual Ticket Ticket { get; set; }
    }
}