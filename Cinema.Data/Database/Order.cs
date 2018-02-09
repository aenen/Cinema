namespace Cinema.Data.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Linq;

    [Table("Order")]
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }

        public string TestIdForLiqpay { get; set; }

        public int OrderStatusId { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime PurchaseDate { get; set; }

        public int TotalPrice => OrderItems.Sum(x => x.Price);
        
        public virtual ApplicationUser User { get; set; }

        public virtual OrderStatus OrderStatus { get; set; }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
