using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Cinema.Data.Database;
using System.Data.Entity.Migrations;
using Cinema.Models;

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
                if (deleteTickest != null && deleteTickest.Count > 0)
                    TicketHub.NotifyToAllClients();

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
                if (reservedDidntPayed != null && reservedDidntPayed.Count > 0)
                    TicketHub.NotifyToAllClients();

                // видаляю застарілі сеанси (які почались 12 годин тому)
                DateTime dtNow3 = DateTime.Now.AddHours(-12);
                //dtNow = DateTime.Now.AddMinutes(-10);
                var oldSessions = db.Sessions.Where(x => dtNow3 > x.DateTime).ToList();
                db.Sessions.RemoveRange(oldSessions);
                db.SaveChanges();


                // якщо сеансів меньше 50 - додаю 1
                if (db.Sessions.Count() < 50)
                {
                    Random random = new Random();
                    int movieId = random.Next(1, db.Movies.Count() + 1);
                    int cinemaHallId = random.Next(1, db.CinemaHalls.Count() + 1);
                    var itemCinemaHall = db.CinemaHalls.Find(cinemaHallId);

                    double randMin = random.Next(-10, 30);
                    int randHour = random.Next(1, 120);

                    var currentSession = new Session
                    {
                        MovieId = movieId,
                        DateTime = DateTime.Now.AddHours(randHour).AddMinutes(randMin),
                        CinemaHall = itemCinemaHall
                    };
                    db.Sessions.Add(currentSession);
                    // додаю ціни за місця на сеанс
                    foreach (var itemSeat in itemCinemaHall.Seats)
                    {
                        db.TicketPrices.Add(new TicketPrice { Seat = itemSeat, Price = Convert.ToInt32(itemSeat.SeatType.DefaultPrice), Session = currentSession });
                    }

                    db.SaveChanges();
                }
            }
        }
    }
}