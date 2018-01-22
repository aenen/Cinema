using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Cinema.Data.Database;

namespace Cinema.Controllers
{
    public class AgeRatingsController : Controller
    {
        private CinemaContext db = new CinemaContext();

        // GET: AgeRatings
        public ActionResult Index()
        {
            return View(db.AgeRatings.ToList());
        }

        // GET: AgeRatings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AgeRating ageRating = db.AgeRatings.Find(id);
            if (ageRating == null)
            {
                return HttpNotFound();
            }
            return View(ageRating);
        }

        // GET: AgeRatings/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AgeRatings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] AgeRating ageRating)
        {
            if (ModelState.IsValid)
            {
                db.AgeRatings.Add(ageRating);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(ageRating);
        }

        // GET: AgeRatings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AgeRating ageRating = db.AgeRatings.Find(id);
            if (ageRating == null)
            {
                return HttpNotFound();
            }
            return View(ageRating);
        }

        // POST: AgeRatings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] AgeRating ageRating)
        {
            if (ModelState.IsValid)
            {
                db.Entry(ageRating).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ageRating);
        }

        // GET: AgeRatings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AgeRating ageRating = db.AgeRatings.Find(id);
            if (ageRating == null)
            {
                return HttpNotFound();
            }
            return View(ageRating);
        }

        // POST: AgeRatings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AgeRating ageRating = db.AgeRatings.Find(id);
            db.AgeRatings.Remove(ageRating);
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
