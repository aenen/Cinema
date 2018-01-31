using Cinema.Data.Database;
using Cinema.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Cinema.Controllers
{
    [RoutePrefix("Movie")]
    public class MovieController : Controller
    {
        IRepository<Movie> repository;
        public MovieController(IRepository<Movie> repository)
        {
            this.repository = repository;
        }

        [Route("{id:int}")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var movie = repository.GetAll().FirstOrDefault(x => x.Id==id);
            if (movie == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(movie);
        }
    }
}