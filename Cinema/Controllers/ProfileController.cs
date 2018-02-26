using Cinema.Data.Database;
using Cinema.Data.Identity;
using Cinema.Data.Repository;
using Cinema.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Cinema.Controllers
{
    public class ProfileController : Controller
    {
        DbContext context;
        CinemaRepository cinemaRepository;
        CityRepository cityRepository;
        ApplicationUserManager userMgr;

        public ProfileController(DbContext context, CinemaRepository cinemaRepository, CityRepository cityRepository)
        {
            this.context = context;
            this.cinemaRepository = cinemaRepository;
            this.cityRepository = cityRepository;
            userMgr = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
        }

        // GET: Profile
        [Authorize]
        public async Task<ActionResult> Index()
        {
            var user = await userMgr.FindByNameAsync(User.Identity.Name);
            return View(user);
        }

        // GET: Profile/Settings
        [Authorize]
        public async Task<ActionResult> Settings()
        {
            var user = await userMgr.FindByNameAsync(User.Identity.Name);
            var model = new ProfileSettingsViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Birthday = user.Birthday
            };

            List<CinemaEntity> listCinema = new List<CinemaEntity> { new CinemaEntity { Id = 0, Name = "Не вибрано" } };
            listCinema.AddRange(cinemaRepository.GetAll());
            List<City> listCity = new List<City> { new City { Id = 0, Name = "Не вибрано" } };
            listCity.AddRange(cityRepository.GetAll());
            int userFavoriteCinema = (user.FavotiteCinema == null) ? 0 : user.FavotiteCinema.Id;
            int userCity = (user.City == null) ? 0 : user.City.Id;
            ViewBag.FavoriteCinemaId = new SelectList(listCinema, "id", "name", userFavoriteCinema);
            ViewBag.CityId = new SelectList(listCity, "id", "name", userCity);

            return View(model);
        }

        [Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Settings(ProfileSettingsViewModel model, HttpPostedFileBase file)
        {
            var user = await userMgr.FindByNameAsync(User.Identity.Name);
            List<CinemaEntity> listCinema = new List<CinemaEntity> { new CinemaEntity { Id = 0, Name = "Не вибрано" } };
            listCinema.AddRange(cinemaRepository.GetAll());
            List<City> listCity = new List<City> { new City { Id = 0, Name = "Не вибрано" } };
            listCity.AddRange(cityRepository.GetAll());
            int userFavoriteCinema = (user.FavotiteCinema == null) ? 0 : user.FavotiteCinema.Id;
            int userCity = (user.City == null) ? 0 : user.City.Id;
            ViewBag.FavoriteCinemaId = new SelectList(listCinema, "id", "name", userFavoriteCinema);
            ViewBag.CityId = new SelectList(listCity, "id", "name", userCity);

            if ((file != null && !IsImageOk(file)) || !ModelState.IsValid)
            {
                return View(model);
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Birthday = model.Birthday;
            user.FavotiteCinema = cinemaRepository.Get(model.FavoriteCinemaId);
            user.City = cityRepository.Get(model.CityId);

            // зберігаю зображення
            if (file != null && IsImageOk(file))
            {
                if (user.PicturePath != null && System.IO.File.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, user.PicturePath)))
                {
                    System.IO.File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, user.PicturePath));
                }

                var pictureSm = MakeThumbnail(Image.FromStream(file.InputStream), 100, 100);

                string picture_folder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "UserData", "Avatar");
                string picture_name = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string picture_folder_name = Path.Combine(picture_folder, picture_name);
                pictureSm.Save(picture_folder_name);
                //file.SaveAs(picture_folder_name);
                user.PicturePath = Path.Combine("Content", "UserData", "Avatar", picture_name);
            }

            var result = await userMgr.UpdateAsync(user);
            if (result.Succeeded)
                return RedirectToAction("Index", "Profile");

            return View(model);
        }

        private bool IsImageOk(HttpPostedFileBase file)
        {
            bool imageOk = true;
            string file_extention = Path.GetExtension(file.FileName).ToLower();
            if (file_extention != ".png" && file_extention != ".jpg" && file_extention != ".jpeg")
            {
                ModelState.AddModelError("File", "Зображення повинне мати формати \".png\", \".jpg\", або \".jpeg\"");
                return false;
            }

            var img = System.Drawing.Image.FromStream(file.InputStream, true, true);
            int width = img.Width;
            int height = img.Height;
            if (width < 100 || height < 100)
            {
                ModelState.AddModelError("File", "Зображення повинне бути розширенням як мінімум 100х100px");
                imageOk = false;
            }

            double fileSizeMb = file.ContentLength / 1024 / 1024;
            if (fileSizeMb > 5)
            {
                ModelState.AddModelError("File", "Зображення повинне бути розміром не більше 5 мегабайт");
                imageOk = false;
            }

            return imageOk;
        }

        public Image MakeThumbnail(Image image, int maxWidth, int maxHeight)
        {
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);

            using (var graphics = Graphics.FromImage(newImage))
            {
                //graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            //return image.GetThumbnailImage(newWidth, newHeight, () => false, IntPtr.Zero);
            return newImage;
        }
    }
}