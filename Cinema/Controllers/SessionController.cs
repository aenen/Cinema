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
    [RoutePrefix("Session")]
    public class SessionController : Controller
    {
        IRepository<Session> repository;
        public SessionController(IRepository<Session> repository)
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

            var session = repository.GetAll().FirstOrDefault(x => x.Id == id);
            if (session == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            return View(session);
        }
    }
}