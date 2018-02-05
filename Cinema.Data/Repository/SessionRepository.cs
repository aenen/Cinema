using Cinema.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Cinema.Data.Repository
{
    public class SessionRepository : Repository<Session>
    {
        public SessionRepository(DbContext context) : base(context)
        {
        }
    }
}
