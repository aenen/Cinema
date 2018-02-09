using Cinema.Data.Database;
using Cinema.Data.Identity;
using Cinema.Data.Repository;
using Cinema.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Cinema.Controllers
{
    public class OrderController : Controller
    {

        DbContext context;
        IRepository<Order> orderRepository;
        IRepository<Session> sessionRepository;
        public OrderController(IRepository<Session> sessionRepository, IRepository<Order> orderRepository, DbContext context)
        {
            this.sessionRepository = sessionRepository;
            this.orderRepository = orderRepository;
            this.context=context;
        }
        
        [HttpPost]
        [Authorize]
        public ActionResult BuyTickets(List<SelectedSeatsViewModel> selected_seats, int? session_id)
        {
            if (!IsValidSelectedSeats(selected_seats, session_id) || !ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Session session = sessionRepository.FindBy(x => x.Id == session_id).FirstOrDefault();
            List<OrderItem> orderItems = new List<OrderItem>();
            foreach (var item in selected_seats)
            {
                TicketPrice ticketPrice = session.TicketPrices.FirstOrDefault(x => x.Seat.Row == item.Row && x.Seat.Number == item.Number);
                orderItems.Add(new OrderItem
                {
                    Movie = session.Movie,
                    Price = ticketPrice.Price,
                    Ticket = new Ticket
                    {
                        TicketPrice = ticketPrice,
                        StatusId = 3
                    }
                });
            }

            ApplicationUserManager userMgr = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            Order order = new Order
            {
                TestIdForLiqpay = Guid.NewGuid().ToString(),
                OrderItems = orderItems,
                OrderStatusId=3,
                User= userMgr.FindByName(User.Identity.Name),
                PurchaseDate=DateTime.Now
            };

            orderRepository.AddOrUpdate(order);
            orderRepository.Save();


            return View();
        }

        /// <summary>
        /// Метод перевіряє правильність отриманих даних
        /// </summary>
        /// <param name="selected_seats"></param>
        /// <param name="session_id"></param>
        /// <returns>false - валідація не пройдена; true - все добре</returns>
        private bool IsValidSelectedSeats(List<SelectedSeatsViewModel> selected_seats, int? session_id)
        {
            if (selected_seats == null) return false;
            if (selected_seats.Count == 0) return false;
            if (session_id == null) return false;

            Session session = sessionRepository.FindBy(x => x.Id == session_id).FirstOrDefault();
            if (session == null) return false;
            if (session.DateTime < DateTime.Now.AddMinutes(60)) return false;

            foreach (var item in selected_seats)
            {
                TicketPrice ticketPrice = session.TicketPrices.FirstOrDefault(x => x.Seat.Row == item.Row && x.Seat.Number == item.Number);
                if (ticketPrice == null) return false;
                if (ticketPrice.Ticket != null) return false;
            }

            return true;
        }
    }
}