namespace Cinema.Data.Database
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Comment")]
    public partial class Comment
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Comment()
        {
            Answers = new HashSet<Comment>();
        }

        public int Id { get; set; }

        public int? ReplyToCommentId { get; set; }
        
        [Required]
        public int MovieId { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime Datetime { get; set; }

        public int? CinemaId { get; set; }

        public int? CommentTypeId { get; set; }

        [Required]
        [StringLength(2500)]
        public string Text { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual CinemaEntity Cinema { get; set; }
        
        public virtual Movie Movie { get; set; }

        public virtual CommentType CommentType { get; set; }

        [ForeignKey("ReplyToCommentId")]
        public virtual Comment ReplyToComment { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Answers { get; set; }
    }
}
