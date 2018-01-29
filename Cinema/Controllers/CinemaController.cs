using Cinema.Data.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Cinema.Controllers
{
    [RoutePrefix("Cinema")]
    public class CinemaController : Controller
    {
        CinemaContext db = new CinemaContext();

        public ActionResult Index()
        {
            return View();
        }

        [Route("{name}")]
        public ActionResult Details(string name)
        {
            if (name == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cinema = db.Cinemas.FirstOrDefault(x => x.Name == name);
            if (cinema == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(cinema);
        }
    }
}