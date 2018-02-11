using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Cinema.Data.Database;

namespace Cinema.Jobs
{
    public class DatabaseCleaner : IJob
    {
        //CinemaContext db = new CinemaContext();

        public void Execute(IJobExecutionContext context)
        {
            using (CinemaContext db = new CinemaContext())
            {
                DateTime dtNow = DateTime.Now.AddMinutes(-15);
                var deleteTickest = db.Tickets.Where(x => x.CreationDateTime < dtNow&& x.StatusId == 3).ToList();
                //deleteTickest.AddRange(db.Sessions.Where(x => DateTime.Now > x.DateTime.AddMinutes(-30)).SelectMany(x => x.TicketPrices.Where(y=>y.Ticket.StatusId == 2).Select(xx => xx.Ticket).ToList()).ToList());
                db.Tickets.RemoveRange(deleteTickest);
                db.SaveChanges();
            }
        }
    }
}