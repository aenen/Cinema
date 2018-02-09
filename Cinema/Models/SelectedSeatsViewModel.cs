using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cinema.Models
{
    public class SelectedSeatsViewModel
    {
        [Required]
        public int Row { get; set; }
        [Required]
        public int Number { get; set; }
        //public int Price { get; set; }
    }
}