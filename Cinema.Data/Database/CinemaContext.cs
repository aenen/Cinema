using Cinema.Data.Identity;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Data.Database
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Orders = new HashSet<Order>();
            Comments = new HashSet<Comment>();
        }

        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        [Column(TypeName = "Date")]
        public DateTime Birthday { get; set; }
        [StringLength(500)]
        public string PicturePath { get; set; }

        public virtual City City { get; set; }

        public virtual CinemaEntity FavotiteCinema { get; set; }

        public virtual ICollection<Order> Orders { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
    public class CinemaContext : IdentityDbContext<ApplicationUser>
    {
        public CinemaContext() : base("Cinema", throwIfV1Schema: false)
        {
            System.Data.Entity.Database.SetInitializer(new DataBaseInitializer());
        }

        public static CinemaContext Create()
        {
            return new CinemaContext();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>()
               .HasOptional(x => x.Ticket)
               .WithOptionalPrincipal(x => x.OrderItem)
               .Map(a => a.MapKey("OrderItemId"));

            modelBuilder.Entity<TicketPrice>()
               .HasOptional(x => x.Ticket)
               .WithOptionalPrincipal(x => x.TicketPrice)
               .Map(a => a.MapKey("TicketPriceId"));
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<CommentType> CommentTypes { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<CinemaEntity> Cinemas { get; set; }
        public virtual DbSet<CinemaHall> CinemaHalls { get; set; }
        public virtual DbSet<SeatType> SeatTypes { get; set; }
        public virtual DbSet<Seat> Seats { get; set; }
        public virtual DbSet<Session> Sessions { get; set; }
        public virtual DbSet<Ticket> Tickets { get; set; }
        public virtual DbSet<TicketPrice> TicketPrices { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Movie> Movies { get; set; }
        public virtual DbSet<AgeRating> AgeRatings { get; set; }
        public virtual DbSet<TicketStatus> TicketStatus { get; set; }
        public virtual DbSet<OrderStatus> OrderStatus { get; set; }
        public virtual DbSet<SeatStyle> SeatStyles { get; set; }
    }

    public class DataBaseInitializer : DropCreateDatabaseAlways<CinemaContext>
    {
        protected override void Seed(CinemaContext context)
        {
            base.Seed(context);

            PerformInitialSetup(context);
            PerformAdditionalSetup(context);
        }

        public void PerformInitialSetup(CinemaContext context)
        {
            ApplicationUserManager userMgr = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            ApplicationRoleManager roleMgr = new ApplicationRoleManager(new RoleStore<ApplicationRole>(context));

            // додаю ролі
            roleMgr.Create(new ApplicationRole("Administrator"));
            roleMgr.Create(new ApplicationRole("Moderator"));
            //roleMgr.Create(new ApplicationRole("User"));

            // додаю адміністратора
            string userName = "admin@gmail.com";
            string password = "12345Admin.";
            string email = "admin@gmail.com";
            userMgr.Create(new ApplicationUser { UserName = userName, Email = email, Birthday = new DateTime(1997, 1, 23) }, password);
            userMgr.AddToRole(userMgr.FindByName(userName).Id, "Administrator");
            var userAdmin = userMgr.FindByName(userName);
            userAdmin.EmailConfirmed = true;
            userMgr.Update(userAdmin);

            //...

            context.TicketStatus.Add(new TicketStatus { Name = "Сплачено" });
            context.TicketStatus.Add(new TicketStatus { Name = "Заброньовано", Description = "За 30 хвилин до початку сеансу підійдіть до каси кінотеатру та назвіть номер квитка або номер замовлення." });
            context.TicketStatus.Add(new TicketStatus { Name = "Зарезервовано", Description = "Квиток зарезервовано на 10 хвилин." });

            context.OrderStatus.Add(new OrderStatus { Name = "Сплачено" });
            context.OrderStatus.Add(new OrderStatus { Name = "Заброньовано", Description = "Викупіть квитки через сайт, або на касі кінотеатру за 30 хвилин до початку сеансу." });
            context.OrderStatus.Add(new OrderStatus { Name = "Відхилено" });

            context.CommentTypes.Add(new CommentType { Name = "Відгук" });
            context.CommentTypes.Add(new CommentType { Name = "Скарга" });
            context.CommentTypes.Add(new CommentType { Name = "Пропозиція" });

            context.AgeRatings.Add(new AgeRating { Name = "0+", Description = "Без вікових обмежень." });
            context.AgeRatings.Add(new AgeRating { Name = "12+", Description = "Старше 12 років, або в супроводі батьків чи опікунів." });
            context.AgeRatings.Add(new AgeRating { Name = "16+", Description = "Старше 16 років." });
            context.AgeRatings.Add(new AgeRating { Name = "18+", Description = "Старше 18 років." });

            context.Cities.Add(new City { Name = "Київ" });
            context.Cities.Add(new City { Name = "Чернігів" });
            context.Cities.Add(new City { Name = "Одеса" });
            context.Cities.Add(new City { Name = "Львів" });

            context.SeatTypes.Add(new SeatType { Name = "Звичайне", Keyword = "seat-common", DefaultPrice = 5000 });
            context.SeatTypes.Add(new SeatType { Name = "Люкс", Keyword = "seat-lux", DefaultPrice = 6000, Description = "М'ягкі крісла з високою спинкою та фіксованим сидінням." });
            context.SeatTypes.Add(new SeatType { Name = "Супер Люкс", Keyword = "seat-super-lux", DefaultPrice = 10000, Description = "Крісла-реклайнери, що забезпечують підвищений комфорт перегляду, для гурманів кіно." });
            context.SeatTypes.Add(new SeatType { Name = "Диван для двох", Keyword = "seat-sofa", DefaultPrice = 20000, Description = "Комфортний диван для двох." });

            context.SaveChanges();
        }

        public void PerformAdditionalSetup(CinemaContext context)
        {
            context.Cinemas.Add(new CinemaEntity { Name = "Панорама", Keyword = "Panorama", CityId = 1, PhoneNumber = "+380 (97) 345 67 89", Address = "Оболонський проспект, 10", BackgroundPath= "/Content/CinemaBackground/342736.png" });
            context.Cinemas.Add(new CinemaEntity { Name = "Vision", Keyword = "Vision", CityId = 3, PhoneNumber = "+380 (68) 987 65 43", Address = "Штильова вулиця, 134", BackgroundPath = "/Content/CinemaBackground/12342412.jpg" });

            context.CinemaHalls.Add(new CinemaHall { CinemaId = 1, Name = "1" });

            context.CinemaHalls.Add(new CinemaHall { CinemaId = 2, Name = "Light" });
            //context.CinemaHalls.Add(new CinemaHall { CinemaId = 2, Name = "Dark" });

            #region кінотеатр #1 --- зал #1
            // row #1
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 1, Number = 1, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 0.5, PositionY = 0.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 1, Number = 2, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 3.5, PositionY = 0.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 1, Number = 3, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 6.5, PositionY = 0.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 1, Number = 4, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 20.5, PositionY = 0.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 1, Number = 5, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 23.5, PositionY = 0.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 1, Number = 6, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 26.5, PositionY = 0.5 } });
            // row #2
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 2, Number = 1, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 0.5, PositionY = 4 } });
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 2, Number = 2, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 3.5, PositionY = 4 } });
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 2, Number = 3, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 6.5, PositionY = 4 } });
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 2, Number = 4, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 10.5, PositionY = 4 } });
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 2, Number = 5, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 13.5, PositionY = 4 } });
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 2, Number = 6, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 16.5, PositionY = 4 } });
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 2, Number = 7, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 20.5, PositionY = 4 } });
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 2, Number = 8, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 23.5, PositionY = 4 } });
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 2, Number = 9, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 26.5, PositionY = 4 } });
            // row #3
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 3, Number = 1, SeatTypeId = 4, SeatStyle = new SeatStyle { PositionX = 4, PositionY = 9 } });
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 3, Number = 2, SeatTypeId = 4, SeatStyle = new SeatStyle { PositionX = 10, PositionY = 9 } });
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 3, Number = 3, SeatTypeId = 4, SeatStyle = new SeatStyle { PositionX = 15, PositionY = 9 } });
            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 3, Number = 4, SeatTypeId = 4, SeatStyle = new SeatStyle { PositionX = 21, PositionY = 9 } });
            #endregion

            #region кінотеатр #2 --- зал #1
            // row #1
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 1, Number = 1, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 4.4, PositionY = 0.4 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 1, Number = 2, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 6.6, PositionY = 0.4 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 1, Number = 3, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 8.8, PositionY = 0.4 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 1, Number = 4, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 11, PositionY = 0.4 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 1, Number = 5, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 13.2, PositionY = 0.4 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 1, Number = 6, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 15.4, PositionY = 0.4 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 1, Number = 7, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 17.6, PositionY = 0.4 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 1, Number = 8, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 19.8, PositionY = 0.4 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 1, Number = 9, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 22, PositionY = 0.4 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 1, Number = 10, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 24.2, PositionY = 0.4 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 1, Number = 11, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 26.4, PositionY = 0.4 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 1, Number = 12, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 28.6, PositionY = 0.4 } });
            // row #2
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 2, Number = 1, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 2.2, PositionY = 3.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 2, Number = 2, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 4.4, PositionY = 3.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 2, Number = 3, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 6.6, PositionY = 3.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 2, Number = 4, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 8.8, PositionY = 3.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 2, Number = 5, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 11, PositionY = 3.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 2, Number = 6, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 13.2, PositionY = 3.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 2, Number = 7, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 15.4, PositionY = 3.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 2, Number = 8, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 17.6, PositionY = 3.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 2, Number = 9, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 19.8, PositionY = 3.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 2, Number = 10, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 22, PositionY = 3.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 2, Number = 11, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 24.2, PositionY = 3.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 2, Number = 12, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 26.4, PositionY = 3.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 2, Number = 13, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 28.6, PositionY = 3.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 2, Number = 14, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 33, PositionY = 3.5 } });
            // row #3
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 3, Number = 1, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 0, PositionY = 6.6 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 3, Number = 2, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 2.2, PositionY = 6.6 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 3, Number = 3, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 4.4, PositionY = 6.6 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 3, Number = 4, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 6.6, PositionY = 6.6 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 3, Number = 5, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 8.8, PositionY = 6.6 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 3, Number = 6, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 12, PositionY = 6.6 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 3, Number = 7, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 14.2, PositionY = 6.6 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 3, Number = 8, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 16.4, PositionY = 6.6 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 3, Number = 9, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 18.6, PositionY = 6.6 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 3, Number = 10, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 20.8, PositionY = 6.6 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 3, Number = 12, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 24.2, PositionY = 6.6 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 3, Number = 13, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 26.4, PositionY = 6.6 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 3, Number = 14, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 28.6, PositionY = 6.6 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 3, Number = 15, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 33, PositionY = 6.6 } });
            // row #4
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 4, Number = 1, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 0, PositionY = 9.7 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 4, Number = 2, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 2.2, PositionY = 9.7 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 4, Number = 3, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 4.4, PositionY = 9.7 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 4, Number = 4, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 6.6, PositionY = 9.7 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 4, Number = 5, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 8.8, PositionY = 9.7 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 4, Number = 6, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 12, PositionY = 9.7 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 4, Number = 7, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 14.2, PositionY = 9.7 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 4, Number = 8, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 16.4, PositionY = 9.7 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 4, Number = 9, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 18.6, PositionY = 9.7 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 4, Number = 10, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 20.8, PositionY = 9.7 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 4, Number = 12, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 24.2, PositionY = 9.7 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 4, Number = 13, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 26.4, PositionY = 9.7 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 4, Number = 14, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 28.6, PositionY = 9.7 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 4, Number = 15, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 33, PositionY = 9.7 } });
            // row #5
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 5, Number = 1, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 0, PositionY = 12.8 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 5, Number = 2, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 2.2, PositionY = 12.8 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 5, Number = 3, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 4.4, PositionY = 12.8 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 5, Number = 4, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 6.6, PositionY = 12.8 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 5, Number = 5, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 8.8, PositionY = 12.8 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 5, Number = 6, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 11, PositionY = 12.8 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 5, Number = 7, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 13.2, PositionY = 12.8 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 5, Number = 8, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 15.4, PositionY = 12.8 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 5, Number = 9, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 17.6, PositionY = 12.8 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 5, Number = 10, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 19.8, PositionY = 12.8 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 5, Number = 11, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 22, PositionY = 12.8 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 5, Number = 12, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 24.2, PositionY = 12.8 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 5, Number = 13, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 26.4, PositionY = 12.8 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 5, Number = 14, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 28.6, PositionY = 12.8 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 5, Number = 15, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 30.8, PositionY = 12.8 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 5, Number = 16, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 33, PositionY = 12.8 } });
            // row #6
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 6, Number = 1, SeatTypeId = 3, SeatStyle = new SeatStyle { PositionX = 1, PositionY = 16.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 6, Number = 2, SeatTypeId = 3, SeatStyle = new SeatStyle { PositionX = 4, PositionY = 16.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 6, Number = 3, SeatTypeId = 3, SeatStyle = new SeatStyle { PositionX = 9.8, PositionY = 16.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 6, Number = 4, SeatTypeId = 3, SeatStyle = new SeatStyle { PositionX = 12.8, PositionY = 16.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 6, Number = 5, SeatTypeId = 3, SeatStyle = new SeatStyle { PositionX = 19.6, PositionY = 16.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 6, Number = 6, SeatTypeId = 3, SeatStyle = new SeatStyle { PositionX = 22.6, PositionY = 16.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 6, Number = 7, SeatTypeId = 3, SeatStyle = new SeatStyle { PositionX = 28.4, PositionY = 16.5 } });
            context.Seats.Add(new Seat { CinemaHallId = 2, Row = 6, Number = 8, SeatTypeId = 3, SeatStyle = new SeatStyle { PositionX = 31.4, PositionY = 16.5 } });
            #endregion

            #region фільми
            context.Movies.Add(new Movie
            {
                Name = "Частка Бога",
                OriginalName = "God Particle",
                AgeRatingId = 3,
                Country = "США",
                Description = "Після неочікованого результату експеременту з прискоренням заряджених часток група астронавтів опиняється в повній ізоляції. Після їх жахливого відкриття екіпаж космічної станції повинен боротись за виживання.",
                Director = "Джуліус Онах",
                Script = "Орен Візель, Дуг Юнг",
                Duration = new TimeSpan(1, 30, 0),
                Genres = "Жахи, детектив, фантастика, триллер",
                Language = "Українська",
                ShowStart = new DateTime(2018, 4, 20),
                ShowEnd = new DateTime(2018, 5, 17),
                Starring = "Гугу Мбата‑Роу, Девід Оєлоуо, Елізабет Дебікі, Чжан Цзи",
                PosterPath = "/Content/Poster/4Ho2vcwcNK1iqPqxpJdUeskeSqq.jpg",
                BackgroundPath = "/Content/Background/2cnUCcNmmj79hGmimvWlUo5P2K8.jpg",
            });

            context.Movies.Add(new Movie
            {
                Name = "Месники: Війна нескінченності",
                OriginalName = "Avengers: Infinity War",
                AgeRatingId = 2,
                Country = "США",
                Description = "Всесвіт MARVEL ще не бачив битви такого розмаху. Усі супергерої об’єднаються, щоб протистояти наймогутнішому ворогу. Люди тут безсилі, залишається тільки спостерігати.",
                Director = "Ентоні Русо, Джо Русо",
                Script = "Крістофер Маркус, Стівен Мак-Філі",
                Duration = new TimeSpan(2, 18, 0),
                Genres = "Бойовик, пригоди, фантастика",
                Language = "Українська",
                ShowStart = new DateTime(2018, 5, 3),
                ShowEnd = new DateTime(2018, 6, 30),
                Starring = "Роберт Дауні-молодший, Кріс Претт, Кріс Еванс, Кріс Хемсворт",
                PosterPath = "/Content/Poster/tgQV4jyb9BgcV6db8uKPjTgSfpD.jpg",
                BackgroundPath = "/Content/Background/j3OjLcOpDsGF3j6f8Myyra5Bq62.jpg",
                TrailerLink = "https://www.youtube.com/watch?v=tdR-lS2UD_M"
            });

            context.Movies.Add(new Movie
            {
                Name = "Суперсімейка 2",
                OriginalName = "Incredibles 2",
                AgeRatingId = 1,
                Country = "США",
                Description = @"Повернення Суперсімейки від Disney\Pixar! Які карколомні пригоди чекають родину супершпигунів цього разу?",
                Director = "Бред Бйорд",
                Duration = new TimeSpan(1, 32, 0),
                Genres = "Мультфільм, сімейний, пригоди, бойовик, фантастика",
                Language = "Українська",
                ShowStart = new DateTime(2018, 6, 14),
                ShowEnd = new DateTime(2018, 8, 5),
                Starring = "Бред Бйорд, Сем'юель Лерой Джексон, Холлі Хантер, Сара Воуел",
                PosterPath = "/Content/Poster/bLBsSGIHzkFR3we2JjFzggnOAUO.jpg",
                BackgroundPath = "/Content/Background/kqoBtMmiycbbhGLXGkKhL8SdaWB.jpg",
                TrailerLink = "https://www.youtube.com/watch?v=BZJ5ilvAyjo"
            });

            context.Movies.Add(new Movie
            {
                Name = "СУПЕР ПОЛІЦЕЙСЬКІ 2",
                OriginalName = "SUPER TROOPERS 2",
                AgeRatingId = 2,
                Country = "США",
                Director = "Джей Чандраскехар",
                Script = "Джей Чандраскехар, Кевін Хефернан, Стів Лемм, Ерік Столанске, Пол Сотер",
                Duration = new TimeSpan(1, 30, 0),
                Genres = "Детектив, кримінал, комедія",
                Language = "Українська",
                ShowStart = new DateTime(2018, 4, 20),
                ShowEnd = new DateTime(2018, 5, 25),
                Starring = "Джей Чандраскехар, Кевін Хефернан, Стів Лемм, Ерік Столанске, Пол Сотер",
                PosterPath = "/Content/Poster/fcdtVnNpFkMxHvcacJCZthGWS2E.jpg",
                BackgroundPath = "/Content/Background/k4EOiyltF2zzfRckCUbsG1FDFzP.jpg"
            });

            context.Movies.Add(new Movie
            {
                Name = "Дикі предки",
                OriginalName = "Early man",
                AgeRatingId = 2,
                Country = "Великобританія, Франція",
                Director = "Нік Парк",
                Script = "Марк Бертон, Джеймс Хіггінсон",
                Description = "Кам'яне століття і навіть пізніші цивілізації - все одно дикі предки, але як же вони були схожі на нас! У них теж було надто багато родичів, а чоловіки боролися за улюблену жінку. Вони теж обожнювали коштовності, грали в шкіряний м'яч, і кожен прагнув стати першим.",
                Duration = new TimeSpan(1, 30, 0),
                Genres = "Мультфільм, комедія",
                Language = "Українська",
                ShowStart = new DateTime(2018, 3, 29),
                ShowEnd = new DateTime(2018, 4, 25),
                Starring = "Том Хіддлстон, Мейсиі Уільямс, Едді Редмейн, Річард Айоейд, Марк Вільямс, Тімоті Сполл, Джонні Вегас",
                PosterPath = "/Content/Poster/ugw07fJIZMVrrIGeN1MO7Xecj5h.jpg",
                BackgroundPath = "/Content/Background/z30NXJEY4YDBWEL0ICHGtv4Mt26.jpg",
                TrailerLink = "https://www.youtube.com/watch?v=KMwKWztllkE"
            });
            #endregion

            context.SaveChanges();

            #region сеанси
            context.Sessions.Add(new Session { MovieId = 1, DateTime = DateTime.Now.AddHours(5), CinemaHallId = 1 });
            context.Sessions.Add(new Session { MovieId = 1, DateTime = DateTime.Now.AddHours(2), CinemaHallId = 1 });
            context.Sessions.Add(new Session { MovieId = 1, DateTime = DateTime.Now.AddDays(1), CinemaHallId = 1 });
            context.Sessions.Add(new Session { MovieId = 2, DateTime = DateTime.Now.AddDays(1), CinemaHallId = 1 });
            foreach (var item in context.CinemaHalls.Find(1).Seats)
            {
                context.TicketPrices.Add(new TicketPrice { Seat = item, Price = Convert.ToInt32(item.SeatType.DefaultPrice), SessionId = 1 });
                context.TicketPrices.Add(new TicketPrice { Seat = item, Price = Convert.ToInt32(item.SeatType.DefaultPrice), SessionId = 2 });
                context.TicketPrices.Add(new TicketPrice { Seat = item, Price = Convert.ToInt32(item.SeatType.DefaultPrice), SessionId = 3 });
                context.TicketPrices.Add(new TicketPrice { Seat = item, Price = Convert.ToInt32(item.SeatType.DefaultPrice), SessionId = 4 });
            }

            context.Sessions.Add(new Session { MovieId = 1, DateTime = DateTime.Now.AddDays(2), CinemaHallId = 2 });
            context.Sessions.Add(new Session { MovieId = 2, DateTime = DateTime.Now.AddHours(1), CinemaHallId = 2 });
            context.Sessions.Add(new Session { MovieId = 2, DateTime = DateTime.Now.AddHours(2.5), CinemaHallId = 2 });
            context.Sessions.Add(new Session { MovieId = 2, DateTime = DateTime.Now.AddDays(4), CinemaHallId = 2 });
            context.Sessions.Add(new Session { MovieId = 3, DateTime = DateTime.Now.AddHours(1), CinemaHallId = 2 });
            context.Sessions.Add(new Session { MovieId = 3, DateTime = DateTime.Now.AddDays(3), CinemaHallId = 2 });
            context.Sessions.Add(new Session { MovieId = 1, DateTime = DateTime.Now.AddMinutes(60), CinemaHallId = 2 });

            context.Sessions.Add(new Session { MovieId = 1, DateTime = DateTime.Now.AddDays(5), CinemaHallId = 2 });
            foreach (var item in context.CinemaHalls.Find(2).Seats)
            {
                context.TicketPrices.Add(new TicketPrice { Seat = item, Price = Convert.ToInt32(item.SeatType.DefaultPrice), SessionId = 5 });
                context.TicketPrices.Add(new TicketPrice { Seat = item, Price = Convert.ToInt32(item.SeatType.DefaultPrice), SessionId = 6 });
                context.TicketPrices.Add(new TicketPrice { Seat = item, Price = Convert.ToInt32(item.SeatType.DefaultPrice), SessionId = 7 });
                context.TicketPrices.Add(new TicketPrice { Seat = item, Price = Convert.ToInt32(item.SeatType.DefaultPrice), SessionId = 8 });
                context.TicketPrices.Add(new TicketPrice { Seat = item, Price = Convert.ToInt32(item.SeatType.DefaultPrice), SessionId = 9 });
                context.TicketPrices.Add(new TicketPrice { Seat = item, Price = Convert.ToInt32(item.SeatType.DefaultPrice), SessionId = 10 });
                context.TicketPrices.Add(new TicketPrice { Seat = item, Price = Convert.ToInt32(item.SeatType.DefaultPrice), SessionId = 11 });
                context.TicketPrices.Add(new TicketPrice { Seat = item, Price = Convert.ToInt32(item.SeatType.DefaultPrice), SessionId = 12 });
            }
            context.Sessions.Add(new Session { MovieId = 2, DateTime = DateTime.Now.AddHours(1), CinemaHallId = 1 });
            context.Sessions.Add(new Session { MovieId = 2, DateTime = DateTime.Now.AddHours(2.5), CinemaHallId = 1 });
            context.Sessions.Add(new Session { MovieId = 2, DateTime = DateTime.Now.AddDays(4), CinemaHallId = 1 });
            context.Sessions.Add(new Session { MovieId = 3, DateTime = DateTime.Now.AddHours(1), CinemaHallId = 1 });
            context.Sessions.Add(new Session { MovieId = 3, DateTime = DateTime.Now.AddDays(3), CinemaHallId = 1 });
            context.Sessions.Add(new Session { MovieId = 1, DateTime = DateTime.Now.AddMinutes(60), CinemaHallId = 1 });
            foreach (var item in context.CinemaHalls.Find(1).Seats)
            {
                context.TicketPrices.Add(new TicketPrice { Seat = item, Price = Convert.ToInt32(item.SeatType.DefaultPrice), SessionId = 13 });
                context.TicketPrices.Add(new TicketPrice { Seat = item, Price = Convert.ToInt32(item.SeatType.DefaultPrice), SessionId = 14 });
                context.TicketPrices.Add(new TicketPrice { Seat = item, Price = Convert.ToInt32(item.SeatType.DefaultPrice), SessionId = 15 });
                context.TicketPrices.Add(new TicketPrice { Seat = item, Price = Convert.ToInt32(item.SeatType.DefaultPrice), SessionId = 16 });
                context.TicketPrices.Add(new TicketPrice { Seat = item, Price = Convert.ToInt32(item.SeatType.DefaultPrice), SessionId = 17 });
                context.TicketPrices.Add(new TicketPrice { Seat = item, Price = Convert.ToInt32(item.SeatType.DefaultPrice), SessionId = 18 });

            }
            #endregion

            context.SaveChanges();
        }
    }
}