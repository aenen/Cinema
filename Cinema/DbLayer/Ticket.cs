namespace Cinema.DbLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Ticket")]
    public partial class Ticket
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Ticket()
        {

        }
        
        public int Id { get; set; }

        public int? OrderItemId { get; set; }

        public int TicketPriceId { get; set; }

        public bool IsUsed { get; set; }

        public virtual OrderItem OrderItem { get; set; }

        [Required]
        public virtual TicketPrice TicketPrice { get; set; }
    }
}
