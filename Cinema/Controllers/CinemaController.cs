using Cinema.Data.Database;
using Cinema.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Cinema.Controllers
{
    [RoutePrefix("Cinema")]
    public class CinemaController : Controller
    {
        IRepository<CinemaEntity> repository;

        public CinemaController(IRepository<CinemaEntity> repository)
        {
            this.repository = repository;
        }

        public ActionResult Index()
        {
            return View(repository.GetAll().ToList());
        }

        [Route("{name}")]
        public ActionResult Details(string name)
        {
            if (name == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cinema = repository.GetAll().FirstOrDefault(x => x.Name == name);
            if (cinema == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(cinema);
        }
    }
}