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
    public class CinemaController : Controller
    {
        IRepository<CinemaEntity> cinemaRepository;

        public CinemaController(IRepository<CinemaEntity> cinemaRepository)
        {
            this.cinemaRepository = cinemaRepository;
        }

        public ActionResult Index()
        {
            return View(cinemaRepository.GetAll().ToList());
        }

        [Route("Cinema/Details/{name}")]
        public ActionResult Details(string name)
        {
            if (name == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var cinema = cinemaRepository.GetAll().FirstOrDefault(x => x.Keyword.ToLower() == name.ToLower());
            if (cinema == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(cinema);
        }

        [HttpPost]
        //[Route("Cinema/GetCinemaJson")]
        public JsonResult GetCinemaJson()
        {
            return Json(cinemaRepository.GetAll().Select(x => new { x.Id, x.Name, x.Address, x.Keyword, /*CityName = x.City.Name,*/ x.PhoneNumber }));
        }

        [HttpPost]
        public ActionResult ChangeCinemaCookie(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Response.Cookies.Remove("SeletedCinemaId");
            HttpCookie cookieCinema = new HttpCookie("SeletedCinemaId");
            cookieCinema.Value = id.ToString();
            Response.SetCookie(cookieCinema);

            return Json("ok");
        }
    }
}