namespace Cinema.Data.Database
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
        
        public int MovieId { get; set; }

        public int Price { get; set; }
        
        public virtual Order Order { get; set; }
        
        public virtual Ticket Ticket { get; set; }

        public virtual Movie Movie { get; set; }
    }
}
