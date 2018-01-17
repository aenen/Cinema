namespace Cinema.DbLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OrderItem")]
    public partial class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int TicketId { get; set; }

        public int Price => Ticket.TicketPrice.Price;
        
        public virtual Order Order { get; set; }

        [Required]
        public virtual Ticket Ticket { get; set; }
    }
}
