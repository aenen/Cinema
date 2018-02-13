using Cinema.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Cinema.Data.Repository
{
    public class CommentRepository : Repository<Comment>
    {
        public CommentRepository(DbContext context) : base(context)
        {
        }
    }
}
