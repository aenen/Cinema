using Cinema.Data.Database;
using Cinema.Data.Identity;
using Cinema.Data.Repository;
using Cinema.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Cinema.Controllers
{
    public class CommentController : Controller
    {
        CommentRepository commentRepository;
        CommentTypeRepository commentTypeRepository;
        MovieRepository movieRepository;
        CinemaRepository cinemaRepository;
        DbContext context;
        ApplicationUserManager userMgr;

        public CommentController(CommentRepository commentRepository, CommentTypeRepository commentTypeRepository, DbContext context, MovieRepository movieRepository, CinemaRepository cinemaRepository)
        {
            this.commentRepository = commentRepository;
            this.commentTypeRepository = commentTypeRepository;
            this.movieRepository = movieRepository;
            this.cinemaRepository = cinemaRepository;
            this.context = context;
            userMgr = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UserCommentCreate(CommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var user = await userMgr.FindByNameAsync(User.Identity.Name);
            user.Comments.Add(new Comment
            {
                Text = model.Text,
                CommentType = commentTypeRepository.Get(model.CommentTypeId),
                Cinema = cinemaRepository.Get(model.CinemaId),
                Movie = movieRepository.Get(model.MovieId)
            });
            var result = await userMgr.UpdateAsync(user);
            if (result.Succeeded)
                return Json("ok");

            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> UserCommentDelete(int id)
        {
            var user = await userMgr.FindByNameAsync(User.Identity.Name);
            var comment = user.Comments.FirstOrDefault(x => x.Id == id);
            if (comment == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            commentRepository.Delete(comment);
            commentRepository.Save();
            return Json("ok");
        }

        [HttpPost]
        public JsonResult GetCommentTypeJson()
        {
            return Json(commentTypeRepository.GetAll().Select(x => new { x.Id, x.Name, x.Description }));
        }
    }
}