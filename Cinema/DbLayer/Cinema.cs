namespace Cinema.DbLayer
{
    using global::Cinema.DbLayer;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Cinema")]
    public partial class Cinema
    {
        public Cinema()
        {
            CinemaHalls = new HashSet<CinemaHall>();
            Comments = new HashSet<Comment>();
            Users = new HashSet<ApplicationUser>();
        }
        
        public int Id { get; set; }

        [Required]
        public int CityId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }

        [StringLength(25)]
        public string PhoneNumber { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        public virtual City City { get; set; }

        public virtual ICollection<CinemaHall> CinemaHalls { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}
