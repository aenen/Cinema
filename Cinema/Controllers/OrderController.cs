using Cinema.Data.Database;
using Cinema.Data.Identity;
using Cinema.Data.Repository;
using Cinema.Models;
using Cinema.Payment.LiqPay;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Cinema.Controllers
{
    public class OrderController : Controller
    {

        DbContext context;
        IRepository<Order> orderRepository;
        IRepository<Session> sessionRepository;
        IRepository<Ticket> ticketRepository;
        public OrderController(IRepository<Session> sessionRepository, IRepository<Order> orderRepository, IRepository<Ticket> ticketRepository, DbContext context)
        {
            this.sessionRepository = sessionRepository;
            this.orderRepository = orderRepository;
            this.ticketRepository = ticketRepository;
            this.context = context;
        }

        [HttpPost]
        [Authorize]
        public ActionResult Processed(List<SelectedSeatsViewModel> selected_seats, int? session_id)
        {
            if (!IsValidSelectedSeats(selected_seats, session_id) || !ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Session session = sessionRepository.FindBy(x => x.Id == session_id).FirstOrDefault();
            List<OrderItemViewModel> orderItems = new List<OrderItemViewModel>();
            foreach (var item in selected_seats)
            {
                TicketPrice ticketPrice = session.TicketPrices.FirstOrDefault(x => x.Seat.Row == item.Row && x.Seat.Number == item.Number);
                orderItems.Add(new OrderItemViewModel { Price = ticketPrice.Price, SeatRow = item.Row, SeatNumber = item.Number });
            }

            OrderProcessedViewModel model = new OrderProcessedViewModel
            {
                OrderItems = orderItems,
                SessionId = session.Id,
                SessionDate = session.DateTime,
                CinemaHallName = session.CinemaHall.Name,
                CinemaName = session.CinemaHall.Cinema.Name,
                MovieName = session.Movie.Name
            };

            return View(model);
        }


        [HttpPost]
        [Authorize]
        public ActionResult BuyTickets(List<SelectedSeatsViewModel> selected_seats, int? session_id)
        {
            if (!IsValidSelectedSeats(selected_seats, session_id) || !ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // створюю список позицій замовлення (квитків)
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
                        StatusId = 3, // зарезервовано на 15хв
                    }
                });
            }

            // створюю замовлення та зберігаю його в бд
            ApplicationUserManager userMgr = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            Order order = new Order
            {
                TestIdForLiqpay = Guid.NewGuid().ToString(), // ця властивість необхідна для тестування "лікпею"
                OrderItems = orderItems,
                OrderStatusId = 3, // статус "відхилено" поки користувач не заплатить
                User = userMgr.FindByName(User.Identity.Name),
                PurchaseDate = DateTime.Now
            };
            orderRepository.AddOrUpdate(order);
            orderRepository.Save();

            // на основі створенго замовлення збираю необхідні для liqpay api дані та відправляю їх на в'ю
            LiqPayHelper liqPayHelper = new LiqPayHelper(ConfigurationManager.AppSettings["LiqPayPrivateKey"], ConfigurationManager.AppSettings["LiqPayPublicKey"]);
            string redirect_url = Request.Url.Scheme + System.Uri.SchemeDelimiter + Request.Url.Host +
                (Request.Url.IsDefaultPort ? "" : ":" + Request.Url.Port) + "/Order/LiqPayCallback";
            var model = liqPayHelper.GetLiqPayModel(order, redirect_url);
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult ReserveTickets(List<SelectedSeatsViewModel> selected_seats, int? session_id)
        {
            if (!IsValidSelectedSeats(selected_seats, session_id) || !ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // створюю список позицій замовлення (квитків)
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
                        StatusId = 2, // заброньовано (видалиться за 30хв до початку сеансу)
                    }
                });
            }

            // створюю замовлення та зберігаю його в бд
            ApplicationUserManager userMgr = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            Order order = new Order
            {
                OrderItems = orderItems,
                OrderStatusId = 2, // не сплачено (заброньовано)
                User = userMgr.FindByName(User.Identity.Name),
                PurchaseDate = DateTime.Now
            };
            orderRepository.AddOrUpdate(order);
            orderRepository.Save();

            return View();
        }

        [HttpPost]
        public ActionResult LiqPayCallback()
        {
            LiqPayHelper liqPayHelper = new LiqPayHelper(ConfigurationManager.AppSettings["LiqPayPrivateKey"], ConfigurationManager.AppSettings["LiqPayPublicKey"]);

            // --- Перетворюю відповідь LiqPay в Dictionary<string, string> для зручності:
            var request_dictionary = Request.Form.AllKeys.ToDictionary(key => key, key => Request.Form[key]);

            // --- Розшифровую параметр data відповіді LiqPay та перетворюю в Dictionary<string, string> для зручності:
            byte[] request_data = Convert.FromBase64String(request_dictionary["data"]);
            string decodedString = Encoding.UTF8.GetString(request_data);
            var request_data_dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(decodedString);

            // --- Отримую сигнатуру для перевірки
            var mySignature = liqPayHelper.GetLiqPaySignature(request_dictionary["data"]);

            // --- Якщо сигнатура серевера не співпадає з сигнатурою відповіді LiqPay - щось пішло не так
            if (mySignature != request_dictionary["signature"])
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            string order_id = request_data_dictionary["order_id"];
            Order order = orderRepository.FindBy(x => x.TestIdForLiqpay == order_id).FirstOrDefault();
            if (order == null) new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            // --- Якщо статус відповіді "Тест" або "Успіх" - все добре
            if (request_data_dictionary["status"] == "sandbox" || request_data_dictionary["status"] == "success")
            {
                // Тут можна оновити статус замовлення та зробити всі необхідні речі. Id замовлення можна взяти тут: request_data_dictionary[order_id]
                foreach (var item in order.OrderItems)
                {
                    item.Ticket.StatusId = 1;
                }
                order.OrderStatusId = 1;
                order.PurchaseDate = DateTime.Now;
                orderRepository.AddOrUpdate(order);
                orderRepository.Save();

                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }

            // delete tickets
            foreach (var item in order.OrderItems)
            {
                if (item.Ticket != null)
                    ticketRepository.Delete(item.Ticket);
            }
            ticketRepository.Save();

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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