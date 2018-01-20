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
    public class CommentTypesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: CommentTypes
        public ActionResult Index()
        {
            return View(db.CommentTypes.ToList());
        }

        // GET: CommentTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentType commentType = db.CommentTypes.Find(id);
            if (commentType == null)
            {
                return HttpNotFound();
            }
            return View(commentType);
        }

        // GET: CommentTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CommentTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Description")] CommentType commentType)
        {
            if (ModelState.IsValid)
            {
                db.CommentTypes.Add(commentType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(commentType);
        }

        // GET: CommentTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentType commentType = db.CommentTypes.Find(id);
            if (commentType == null)
            {
                return HttpNotFound();
            }
            return View(commentType);
        }

        // POST: CommentTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] CommentType commentType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(commentType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(commentType);
        }

        // GET: CommentTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommentType commentType = db.CommentTypes.Find(id);
            if (commentType == null)
            {
                return HttpNotFound();
            }
            return View(commentType);
        }

        // POST: CommentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CommentType commentType = db.CommentTypes.Find(id);
            db.CommentTypes.Remove(commentType);
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
