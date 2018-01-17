namespace Cinema.DbLayer
{
    using global::Cinema.DbLayer;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("City")]
    public partial class City
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public City()
        {
            Cinemas = new HashSet<Cinema>();
            Users = new HashSet<ApplicationUser>();
        }
        
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public virtual ICollection<Cinema> Cinemas { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}
