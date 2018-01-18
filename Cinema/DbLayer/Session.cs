namespace Cinema.DbLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Session")]
    public partial class Session
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Session()
        {
            Tickets = new HashSet<Ticket>();
            TicketPrices = new HashSet<TicketPrice>();
        }
        
        public int Id { get; set; }

        public DateTime DateTime { get; set; }

        [Required]
        public int MovieId { get; set; }

        //[Required]
        public int? CinemaHallId { get; set; }

        [ForeignKey("CinemaHallId")]
        public virtual CinemaHall CinemaHall { get; set; }

        public virtual Movie Movie { get; set; }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ticket> Tickets { get; set; }

        public virtual ICollection<TicketPrice> TicketPrices { get; set; }
    }
}
