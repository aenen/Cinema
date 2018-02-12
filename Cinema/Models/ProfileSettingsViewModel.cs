using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Cinema.Models
{
    public class ProfileSettingsViewModel
    {
        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        public DateTime Birthday { get; set; }
        public int CityId { get; set; }
        public int FavoriteCinemaId { get; set; }
    }
}