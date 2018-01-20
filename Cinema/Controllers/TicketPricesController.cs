using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cinema.DbLayer;

namespace Cinema.Controllers
{
    public class TicketPricesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TicketPrices
        public ActionResult Index()
        {
            var ticketPrices = db.TicketPrices.Include(t => t.Seat).Include(t => t.Session).Include(t => t.Ticket);
            return View(ticketPrices.ToList());
        }

        // GET: TicketPrices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketPrice ticketPrice = db.TicketPrices.Find(id);
            if (ticketPrice == null)
            {
                return HttpNotFound();
            }
            return View(ticketPrice);
        }

        // GET: TicketPrices/Create
        public ActionResult Create()
        {
            ViewBag.SeatId = new SelectList(db.Seats, "Id", "Id");
            ViewBag.SessionId = new SelectList(db.Sessions, "Id", "Id");
            ViewBag.Id = new SelectList(db.Tickets, "Id", "Id");
            return View();
        }

        // POST: TicketPrices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,SeatId,SessionId,Price")] TicketPrice ticketPrice)
        {
            if (ModelState.IsValid)
            {
                db.TicketPrices.Add(ticketPrice);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.SeatId = new SelectList(db.Seats, "Id", "Id", ticketPrice.SeatId);
            ViewBag.SessionId = new SelectList(db.Sessions, "Id", "Id", ticketPrice.SessionId);
            ViewBag.Id = new SelectList(db.Tickets, "Id", "Id", ticketPrice.Id);
            return View(ticketPrice);
        }

        // GET: TicketPrices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketPrice ticketPrice = db.TicketPrices.Find(id);
            if (ticketPrice == null)
            {
                return HttpNotFound();
            }
            ViewBag.SeatId = new SelectList(db.Seats, "Id", "Id", ticketPrice.SeatId);
            ViewBag.SessionId = new SelectList(db.Sessions, "Id", "Id", ticketPrice.SessionId);
            ViewBag.Id = new SelectList(db.Tickets, "Id", "Id", ticketPrice.Id);
            return View(ticketPrice);
        }

        // POST: TicketPrices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,SeatId,SessionId,Price")] TicketPrice ticketPrice)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ticketPrice).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SeatId = new SelectList(db.Seats, "Id", "Id", ticketPrice.SeatId);
            ViewBag.SessionId = new SelectList(db.Sessions, "Id", "Id", ticketPrice.SessionId);
            ViewBag.Id = new SelectList(db.Tickets, "Id", "Id", ticketPrice.Id);
            return View(ticketPrice);
        }

        // GET: TicketPrices/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TicketPrice ticketPrice = db.TicketPrices.Find(id);
            if (ticketPrice == null)
            {
                return HttpNotFound();
            }
            return View(ticketPrice);
        }

        // POST: TicketPrices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TicketPrice ticketPrice = db.TicketPrices.Find(id);
            db.TicketPrices.Remove(ticketPrice);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
