using Cinema.Data.Database;
using Cinema.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Cinema.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        [HttpPost]
        public ActionResult BuyTickets(List<SelectedSeatsViewModel> selected_seats, int? session_id)
        {
            if (session_id == null || !ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            return View();
        }

        private bool ValidateSelectedSeats()
        {
            return true;
        }
    }
}