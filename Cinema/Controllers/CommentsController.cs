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
    public class CommentsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments
        public ActionResult Index()
        {
            var comments = db.Comments.Include(c => c.Cinema).Include(c => c.CommentType).Include(c => c.Movie).Include(c => c.ReplyToComment).Include(c => c.User);
            return View(comments.ToList());
        }

        // GET: Comments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        //// GET: Comments/Create
        //public ActionResult Create()
        //{
        //    ViewBag.CinemaId = new SelectList(db.Cinemas, "Id", "Name");
        //    ViewBag.CommentTypeId = new SelectList(db.CommentTypes, "Id", "Name");
        //    ViewBag.MovieId = new SelectList(db.Movies, "Id", "Name");
        //    ViewBag.ReplyToCommentId = new SelectList(db.Comments, "Id", "UserId");
        //    ViewBag.UserId = new SelectList(db.ApplicationUsers, "Id", "FirstName");
        //    return View();
        //}

        //// POST: Comments/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,ReplyToCommentId,UserId,MovieId,CinemaId,CommentTypeId,Text")] Comment comment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Comments.Add(comment);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.CinemaId = new SelectList(db.Cinemas, "Id", "Name", comment.CinemaId);
        //    ViewBag.CommentTypeId = new SelectList(db.CommentTypes, "Id", "Name", comment.CommentTypeId);
        //    ViewBag.MovieId = new SelectList(db.Movies, "Id", "Name", comment.MovieId);
        //    ViewBag.ReplyToCommentId = new SelectList(db.Comments, "Id", "UserId", comment.ReplyToCommentId);
        //    ViewBag.UserId = new SelectList(db.ApplicationUsers, "Id", "FirstName", comment.UserId);
        //    return View(comment);
        //}

        //// GET: Comments/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Comment comment = db.Comments.Find(id);
        //    if (comment == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.CinemaId = new SelectList(db.Cinemas, "Id", "Name", comment.CinemaId);
        //    ViewBag.CommentTypeId = new SelectList(db.CommentTypes, "Id", "Name", comment.CommentTypeId);
        //    ViewBag.MovieId = new SelectList(db.Movies, "Id", "Name", comment.MovieId);
        //    ViewBag.ReplyToCommentId = new SelectList(db.Comments, "Id", "UserId", comment.ReplyToCommentId);
        //    ViewBag.UserId = new SelectList(db.ApplicationUsers, "Id", "FirstName", comment.UserId);
        //    return View(comment);
        //}

        //// POST: Comments/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "Id,ReplyToCommentId,UserId,MovieId,CinemaId,CommentTypeId,Text")] Comment comment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(comment).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.CinemaId = new SelectList(db.Cinemas, "Id", "Name", comment.CinemaId);
        //    ViewBag.CommentTypeId = new SelectList(db.CommentTypes, "Id", "Name", comment.CommentTypeId);
        //    ViewBag.MovieId = new SelectList(db.Movies, "Id", "Name", comment.MovieId);
        //    ViewBag.ReplyToCommentId = new SelectList(db.Comments, "Id", "UserId", comment.ReplyToCommentId);
        //    ViewBag.UserId = new SelectList(db.ApplicationUsers, "Id", "FirstName", comment.UserId);
        //    return View(comment);
        //}

        // GET: Comments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
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
