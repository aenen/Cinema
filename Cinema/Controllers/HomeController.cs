using Cinema.Data.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cinema.Controllers
{
    public class HomeController : Controller
    {
        CinemaContext db = new CinemaContext();
        
        public ActionResult Index()
        {
            return View(db);
        }
    }
}