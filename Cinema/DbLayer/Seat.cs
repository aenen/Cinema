namespace Cinema.DbLayer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Seat")]
    public partial class Seat
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Seat()
        {
            Tickets = new HashSet<Ticket>();
            TicketPrices = new HashSet<TicketPrice>();
        }
        
        public int Id { get; set; }

        public int Row { get; set; }

        public int Number { get; set; }

        [Required]
        public int CinemaHallId { get; set; }
        
        [Required]
        public int SeatTypeId { get; set; }

        public virtual CinemaHall CinemaHall { get; set; }

        public virtual SeatType SeatType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ticket> Tickets { get; set; }

        public virtual ICollection<TicketPrice> TicketPrices { get; set; }
    }
}
