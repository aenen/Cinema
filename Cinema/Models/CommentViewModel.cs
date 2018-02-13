using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cinema.Models
{
    public class CommentViewModel
    {
        [Required]
        public int MovieId { get; set; }
        [Required]
        [StringLength(1000)]
        public string Text { get; set; }

        public int CommentTypeId { get; set; }
        public int CinemaId { get; set; }
    }
}