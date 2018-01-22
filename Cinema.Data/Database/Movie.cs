namespace Cinema.Data.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Movie")]
    public partial class Movie
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Movie()
        {
            Comments = new HashSet<Comment>();
            Sessions = new HashSet<Session>();
        }

        public int Id { get; set; }

        public int? AgeRatingId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ShowStart { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ShowEnd { get; set; }

        public TimeSpan? Duration { get; set; }

        [StringLength(100)]
        public string OriginalName { get; set; }

        [StringLength(2500)]
        public string Description { get; set; }

        [StringLength(250)]
        public string Director { get; set; }

        [StringLength(100)]
        public string Language { get; set; }

        [StringLength(250)]
        public string Script { get; set; }

        [StringLength(100)]
        public string Country { get; set; }

        [StringLength(500)]
        public string Starring { get; set; }

        [StringLength(250)]
        public string Genres { get; set; }

        [StringLength(250)]
        public string TrailerLink { get; set; }

        [StringLength(250)]
        public string PosterPath { get; set; }

        public virtual AgeRating AgeRating { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Session> Sessions { get; set; }
    }
}
