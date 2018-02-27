using Cinema.Data.Database;
using Cinema.Data.Identity;
using Cinema.Data.Repository;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
    public class TicketController : Controller
    {

        DbContext context;
        ApplicationUserManager userMgr;

        public TicketController(DbContext context)
        {
            this.context = context;
            userMgr = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
        }

        // GET: Ticket
        [Authorize]
        public ActionResult DrawTicket(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var user = userMgr.FindByName(User.Identity.Name);
            var ticket = user.Orders.SelectMany(x => x.OrderItems).Select(x => x.Ticket).Where(x=>x!=null).FirstOrDefault(x => x.Id == id);
            if (ticket==null||ticket.StatusId!=1)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            try
            {
                int width = 500;
                int height = 900;

                Bitmap image = new Bitmap(width, height);
                using (Graphics gfx = Graphics.FromImage(image))
                {
                    gfx.Clear(Color.FromArgb(10, 8, 19));

                    Font titleFont = new Font(FontFamily.GenericSansSerif, 15, FontStyle.Regular);
                    Font textFont = new Font(FontFamily.GenericSansSerif, 20, FontStyle.Regular);

                    //using (Pen pen = new Pen(Color.FromArgb(1, 255, 243, 36), 10))
                    //{
                    //    pen.Alignment = System.Drawing.Drawing2D.PenAlignment.Inset;
                    //    gfx.DrawRectangle(pen, 0, 0, width, height);
                    //}

                    Image logoImage = Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content", "WebsiteImage", "Logo", "full_logo.png"));
                    gfx.DrawImage(logoImage, (width - 360) / 2, 50, 360, 41);
                    using (SolidBrush brush = new SolidBrush(Color.FromArgb(85, 85, 85)))
                    {
                        int titleY = 125;
                        int titleYPlus = 100;

                        int textY = 150;
                        int textYPlus = 100;

                        int paddingLR = 40;
                        int margin = 10;

                        using (SolidBrush titleBrush = new SolidBrush(Color.FromArgb(255, 243, 36)))
                        {
                            gfx.DrawString("Фільм", titleFont, titleBrush, paddingLR, titleY);
                            titleY += titleYPlus;
                            gfx.DrawString("Дата", titleFont, titleBrush, paddingLR, titleY);
                            gfx.DrawString("Час", titleFont, titleBrush, width / 2 + 10, titleY);
                            titleY += titleYPlus;
                            gfx.DrawString("Зал", titleFont, titleBrush, paddingLR, titleY);
                            gfx.DrawString("Ряд", titleFont, titleBrush, width / 2 + 10, titleY);
                            gfx.DrawString("Місце", titleFont, titleBrush, width / 2 + width / 4 - 10, titleY);
                            titleY += titleYPlus;
                            gfx.DrawString("Тариф", titleFont, titleBrush, paddingLR, titleY);
                            titleY += titleYPlus;
                            gfx.DrawString("Ціна", titleFont, titleBrush, paddingLR, titleY);
                            gfx.DrawString("Квиток", titleFont, titleBrush, width / 2 + 10, titleY);

                        }

                        // назва фільму
                        gfx.FillRectangle(brush, paddingLR, textY, width - paddingLR * 2, 50);
                        gfx.DrawString(ticket.OrderItem.Movie.Name, textFont, Brushes.White, paddingLR, textY + 10);
                        textY += textYPlus;

                        // дата
                        gfx.FillRectangle(brush, paddingLR, textY, width / 2 - (paddingLR + margin), 50);
                        gfx.DrawString(ticket.SessionDateTime.ToShortDateString(), textFont, Brushes.White, paddingLR, textY + 10);
                        // час
                        gfx.FillRectangle(brush, width / 2 + margin, textY, width / 2 - (paddingLR + margin), 50);
                        gfx.DrawString(ticket.SessionDateTime.ToShortTimeString(), textFont, Brushes.White, width / 2 + margin, textY + 10);
                        textY += textYPlus;

                        // зал
                        gfx.FillRectangle(brush, paddingLR, textY, width / 2 - (paddingLR + margin), 50);
                        gfx.DrawString(ticket.Seat.CinemaHall.Name, textFont, Brushes.White, paddingLR, textY + 10);
                        // ряд
                        gfx.FillRectangle(brush, width / 2 + margin, textY, width / 4 - (paddingLR - margin), 50);
                        gfx.DrawString(ticket.Seat.Row.ToString(), textFont, Brushes.White, width / 2 + 10, textY + 10);
                        // місце
                        gfx.FillRectangle(brush, width / 2 + width / 4 - margin, textY, width / 4 - (paddingLR - margin), 50);
                        gfx.DrawString(ticket.Seat.Number.ToString(), textFont, Brushes.White, width / 2 + width / 4 - margin, textY + 10);
                        textY += textYPlus;

                        // тип
                        gfx.FillRectangle(brush, paddingLR, textY, width - paddingLR * 2, 50);
                        gfx.DrawString(ticket.Seat.SeatType.Name, textFont, Brushes.White, paddingLR, textY + 10);
                        textY += textYPlus;

                        // ціна
                        gfx.FillRectangle(brush, paddingLR, textY, width / 2 - (paddingLR + margin), 50);
                        gfx.DrawString((ticket.OrderItem.Price/100).ToString(), textFont, Brushes.White, paddingLR, textY + 10);
                        // Квиток
                        gfx.FillRectangle(brush, width / 2 + margin, textY, width / 2 - (paddingLR + margin), 50);
                        gfx.DrawString(ticket.Id.ToString(), textFont, Brushes.White, width / 2 + margin, textY + 10);
                        textY += textYPlus;
                    }
                    WebClient client = new WebClient();
                    Stream stream = client.OpenRead($"https://chart.googleapis.com/chart?cht=qr&chs=200x200&chld=L|0&chl=http://moviehouse.azurewebsites.net/api/Ticket/Check/{ticket.Id}");
                    Image qrImage = Image.FromStream(stream);
                    gfx.DrawImage(qrImage, (width - 200) / 2, height - 250, 200, 200);
                    stream.Flush();
                    stream.Close();
                    client.Dispose();
                }
                MemoryStream ms = new MemoryStream();
                image.Save(ms, ImageFormat.Png);
                ms.Position = 0;

                return new FileStreamResult(ms, "image/jpeg");

            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }
    }
}