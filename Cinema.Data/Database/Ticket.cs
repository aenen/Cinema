namespace Cinema.Data.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ticket")]
    public partial class Ticket
    {
        public int Id { get; set; }

        public int StatusId { get; set; }
        
        public int TicketPriceId { get; set; }

        public bool IsUsed { get; set; }
        
        public virtual TicketStatus Status { get; set; }
        
        public virtual OrderItem OrderItem { get; set; }

        [Required]
        public virtual TicketPrice TicketPrice { get; set; }
    }
}
