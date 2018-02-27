using Cinema.Data.Database;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Cinema.Controllers
{
    public class HomeController : Controller
    {
        CinemaContext db = new CinemaContext();

        public ActionResult Index()
        {

            return View("~/Views/Shared/_HomeLayout.cshtml");
            //return View();
        }
    }
}