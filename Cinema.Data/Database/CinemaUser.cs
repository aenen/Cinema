using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Data.Database
{
    public class CinemaUser
    {
        [Key]
        public string UserName { get; set; }

        public virtual Cinema Cinema { get; set; }
        public virtual City City { get; set; }
    }
}
