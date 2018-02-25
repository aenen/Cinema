using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Cinema.Data.Database;
using System.Data.Entity.Migrations;

namespace Cinema.Jobs
{
    public class DatabaseCleaner : IJob
    {
        //CinemaContext db = new CinemaContext();

        public void Execute(IJobExecutionContext context)
        {
            using (CinemaContext db = new CinemaContext())
            {
                // видаляю зарезервовані на 15 хв квитики, які по якимось причинам не були викуплені
                DateTime dtNow = DateTime.Now.AddMinutes(-15);
                var deleteTickest = db.Tickets.Where(x => dtNow > x.CreationDateTime && x.StatusId == 3).ToList();
                db.Tickets.RemoveRange(deleteTickest);
                db.SaveChanges();

                // видаляю квитки, які були заброньовані, але не викуплені за 30хв до початку сеансу (+ змінюю статус замовлення на "відхилено")
                DateTime dtNow2 = DateTime.Now.AddMinutes(30);
                var reservedDidntPayed = db.Sessions.Where(x => dtNow2 > x.DateTime).SelectMany(x => x.TicketPrices.Where(y => y.Ticket.StatusId == 2).Select(xx => xx.Ticket).ToList()).ToList();
                foreach (var item in reservedDidntPayed)
                {
                    if (item.OrderItem != null)
                    {
                        var order = item.OrderItem.Order;
                        order.OrderStatusId = 3;
                        db.Orders.AddOrUpdate(order);
                    }
                }
                db.Tickets.RemoveRange(reservedDidntPayed);
                db.SaveChanges();

                // видаляю застарілі сеанси (які почались 12 годин тому)
                DateTime dtNow3 = DateTime.Now.AddHours(-12);
                //dtNow = DateTime.Now.AddMinutes(-10);
                var oldSessions=db.Sessions.Where(x => dtNow3 > x.DateTime).ToList();
                db.Sessions.RemoveRange(oldSessions);
                db.SaveChanges();
            }
        }
    }
}