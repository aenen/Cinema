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
            Database.CommandTimeout = 0;
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

    public class DataBaseInitializer : CreateDatabaseIfNotExists<CinemaContext>
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
            context.Cinemas.Add(new CinemaEntity
            {
                Name = "Панорама",
                Keyword = "Panorama",
                CityId = 1,
                PhoneNumber = "+380 (97) 345 67 89",
                Address = "Кловський узвіз, 9/1",
                Description = "Кінотеатр працює з 8:00 до 03:00",
                BackgroundPath = "/Content/CinemaBackground/342736.png"
            });
            context.Cinemas.Add(new CinemaEntity
            {
                Name = "Vision",
                Keyword = "Vision",
                CityId = 3,
                PhoneNumber = "+380 (68) 987 65 43",
                Address = "16 ул. Дерибасовская",
                Description = "Кінотеатр працює цілодобово (24 години)",
                BackgroundPath = "/Content/CinemaBackground/12342412.jpg"
            });

            context.CinemaHalls.Add(new CinemaHall { CinemaId = 1, Name = "1" });

            context.CinemaHalls.Add(new CinemaHall { CinemaId = 2, Name = "Light" });
            context.CinemaHalls.Add(new CinemaHall { CinemaId = 2, Name = "Dark" });
            context.CinemaHalls.Add(new CinemaHall { CinemaId = 2, Name = "IMAX" });
            context.CinemaHalls.Add(new CinemaHall { CinemaId = 2, Name = "4DX" });

            context.CinemaHalls.Add(new CinemaHall { CinemaId = 1, Name = "2" });

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

            #region кінотеатр #2 --- зал #2
            double hallPosY = 0.4; //3.1
            for (int row = 1; row < 7; row++)
            {
                context.Seats.Add(new Seat { CinemaHallId = 3, Row = row, Number = 1, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 0, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 3, Row = row, Number = 2, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 2.2, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 3, Row = row, Number = 3, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 4.4, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 3, Row = row, Number = 4, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 6.6, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 3, Row = row, Number = 5, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 8.8, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 3, Row = row, Number = 6, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 11, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 3, Row = row, Number = 7, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 13.2, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 3, Row = row, Number = 8, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 15.4, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 3, Row = row, Number = 9, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 17.6, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 3, Row = row, Number = 10, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 19.8, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 3, Row = row, Number = 11, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 22, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 3, Row = row, Number = 12, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 24.2, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 3, Row = row, Number = 13, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 26.4, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 3, Row = row, Number = 14, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 28.6, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 3, Row = row, Number = 15, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 30.8, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 3, Row = row, Number = 16, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 33, PositionY = hallPosY } });

                hallPosY += 3.1;
            }
            #endregion

            #region кінотеатр #2 --- зал #3
            hallPosY = 1; //3.5
            for (int row = 1; row < 5; row++)
            {
                context.Seats.Add(new Seat { CinemaHallId = 4, Row = row, Number = 1, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 0, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 4, Row = row, Number = 2, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 2.2, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 4, Row = row, Number = 3, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 4.4, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 4, Row = row, Number = 4, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 6.6, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 4, Row = row, Number = 5, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 8.8, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 4, Row = row, Number = 6, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 11, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 4, Row = row, Number = 7, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 13.2, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 4, Row = row, Number = 8, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 15.4, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 4, Row = row, Number = 9, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 17.6, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 4, Row = row, Number = 10, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 19.8, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 4, Row = row, Number = 11, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 22, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 4, Row = row, Number = 12, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 24.2, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 4, Row = row, Number = 13, SeatTypeId = 2, SeatStyle = new SeatStyle { PositionX = 26.4, PositionY = hallPosY } });

                hallPosY += 3.5;
            }
            #endregion

            #region кінотеатр #2 --- зал #4
            hallPosY = 1; //4
            for (int row = 1; row < 4; row++)
            {
                double hallPosX = 0; //3
                for (int number = 1; number < 6; number++)
                {
                    context.Seats.Add(new Seat { CinemaHallId = 5, Row = row, Number = number, SeatTypeId = 3, SeatStyle = new SeatStyle { PositionX = hallPosX, PositionY = hallPosY } });
                    hallPosX += 3;
                }

                hallPosY += 4;
            }
            #endregion

            #region кінотеатр #1 --- зал #2
            hallPosY = 0.5; //3.2
            for (int row = 1; row < 6; row++)
            {
                context.Seats.Add(new Seat { CinemaHallId = 6, Row = row, Number = 1, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 0, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 6, Row = row, Number = 2, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 2.2, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 6, Row = row, Number = 3, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 4.4, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 6, Row = row, Number = 4, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 6.6, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 6, Row = row, Number = 5, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 8.8, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 6, Row = row, Number = 6, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 11, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 6, Row = row, Number = 7, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 13.2, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 6, Row = row, Number = 8, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 15.4, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 6, Row = row, Number = 9, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 17.6, PositionY = hallPosY } });
                context.Seats.Add(new Seat { CinemaHallId = 6, Row = row, Number = 10, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 19.8, PositionY = hallPosY } });

                hallPosY += 3.2;
            }
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

            context.Movies.Add(new Movie
            {
                Name = "Червоний горобець",
                OriginalName = "Red Sparrow",
                AgeRatingId = 3,
                Country = "США",
                Director = "Френсіс Лоуренс",
                Script = "Джастін Хейс, Ерік Уоррен Сінгер",
                Description = "Домініку Єгорову (Дженніфер Лоуренс) проти її волі вербують до російської секретної служби у якості «горобця» - професйної звабниці. Домініка вчиться використовувати своє тіло в якості зброї, але прагне зберегти почуття власної гідності під час проходження цього принизливого вишколу. Перетворившись на знаряддя несправедливої системи, вона стає одним з найцінніших агентів секретної програми. Її першою ціллю є Нат Неш (Джоел Едгертон), офіцер ЦРУ, відповідальний за проникнення американських «кротів» до російської розвідки. Між двома молодими людьми зав’язуються складні стосунки - переплетіння взаємного потягу та омани, які ставлять під загрозу їхню кар’єру, світогляд та безпеку обох держав.",
                Duration = new TimeSpan(2, 21, 0),
                Genres = "Tрилер",
                Language = "Українська",
                ShowStart = new DateTime(2018, 2, 1),
                ShowEnd = new DateTime(2018, 3, 25),
                Starring = "Дженніфер Лоуренс, Джоел Едгертон, Маттіас Шонартс, Шарлотта Ремплінг, Кіран Хайндс",
                PosterPath = "/Content/Poster/g1B3YQJqWVWheE7Jg6iOYjv8zre.jpg",
                BackgroundPath = "/Content/Background/tdyFAf8rAVJs3QzjRkJ5lUc41fb.jpg",
                TrailerLink = "https://www.youtube.com/watch?v=3sdis4oIJ3I"
            });

            context.Movies.Add(new Movie
            {
                Name = "Розкрадачка гробниць: Лара Крофт",
                OriginalName = "Tomb Raider",
                AgeRatingId = 2,
                Country = "США",
                Director = "Роар Утхауг",
                Script = "Женева Робертсон-Дуорет",
                Description = "Лара Крофт відправляється у свою першу експедицію, щоб завершити розпочате батьком археологічне дослідження та розкрити античні секрети, які в свою чергу допоможуть очистити її опороченное ім'я. Їй належить боротися за виживання на схибленому від культу острові, застосовуючи всю свою вправність, силу і зброю.",
                Duration = new TimeSpan(1, 42, 0),
                Genres = "Бойовик, пригоди, фентезі",
                Language = "Українська",
                ShowStart = new DateTime(2018, 3, 15),
                ShowEnd = new DateTime(2018, 5, 30),
                Starring = "Алісія Вікандер, Уолтон Гоггінс, Домінік Уест, Деніел Ву, Олександр Віллауме",
                PosterPath = "/Content/Poster/o7LWmTUsme0mhEC5YVE1kZO7JLx.jpg",
                BackgroundPath = "/Content/Background/gVtxvD7DBQJojua6J0I9t7p48i.jpg",
                TrailerLink = "https://www.youtube.com/watch?v=VfWo-fdXvoo"
            });

            context.Movies.Add(new Movie
            {
                Name = "Гноми вдома",
                OriginalName = "Gnome Alone",
                AgeRatingId = 1,
                Country = "Канада",
                Director = "Пітер Лепеніотіс",
                Script = "Джаред Міка Херман, Роберт Морленд",
                Description = "Хлоя, переїхавши в старовинний маєток зі своєю родиною, і не здогадувалася, що фігурки садових гномів у дворі насправді живі, і що вони, весь цей час, захищають світ від чудиськ з інших вимірів. Попереду очікується нове вторгнення троггів і Хлоя з гномами готова дати їм відсіч.",
                Duration = new TimeSpan(1, 29, 0),
                Genres = "Мультфільм",
                Language = "Українська",
                ShowStart = new DateTime(2018, 2, 22),
                ShowEnd = new DateTime(2018, 3, 29),
                Starring = "Беккі Джи, Джош Пек, Тара Стронг, Девід Кокнер, Джим Каммінгс",
                PosterPath = "/Content/Poster/rkg0QzAHCkkCE8UZ3faeYVXpjke.jpg",
                BackgroundPath = "/Content/Background/nd1Acuc6UN32PyK9VIaSZDIvrr.jpg",
                TrailerLink = "https://www.youtube.com/watch?v=NIbQpl85vl8"
            });

            context.Movies.Add(new Movie
            {
                Name = "Складки часу",
                OriginalName = "A Wrinkle in Time",
                AgeRatingId = 1,
                Country = "США",
                Director = "Ава ДюВерней",
                Script = "Дженніфер Лі, Мадлен Л’Енгл",
                Description = "У центрі сюжету молода Мег Муррі, батько якої безслідно зник під час наукового експерименту. Після низки таємничих подій вона відправляється на його пошуки разом з однокласником і молодшим кузеном. Їх чекають подорожі в часі і знайомство з дивними світами.",
                Duration = new TimeSpan(1, 49, 0),
                Genres = "Фантастика, пригоди, фентезі, сімейний",
                Language = "Українська",
                ShowStart = new DateTime(2018, 3, 8),
                ShowEnd = new DateTime(2018, 5, 12),
                Starring = "Різ Візерспун, Гугу Эмбата-Ро, Кріс Пайн, Майкл Пенья, Зак Галіфіанакіс",
                PosterPath = "/Content/Poster/fkP0lCLM9S11E8IKyKUdEJLjqJi.jpg",
                BackgroundPath = "/Content/Background/mx6zzr2H34fXMsBmL2it8NsmKF1.jpg",
                TrailerLink = "https://www.youtube.com/watch?v=mQZ-2jFRd-4"
            });

            context.Movies.Add(new Movie
            {
                Name = "Острів для собак",
                OriginalName = "Isle of Dogs",
                AgeRatingId = 1,
                Country = "США",
                Director = "Вес Андерсон",
                Script = "Вес Андерсон",
                Description = "Дія відбувається в Японії. Фільм розкаже про пригодницьку подорож хлопчика, що вирушив на пошуки своєї собаки.",
                Duration = new TimeSpan(1, 24, 0),
                Genres = "Комедія, мультфільм, пригоди",
                Language = "Українська",
                ShowStart = new DateTime(2018, 3, 23),
                ShowEnd = new DateTime(2018, 5, 7),
                Starring = "Скарлетт Йоханссон, Тільда Суінтон, Браян Кренстон, Лів Шрайбер, Едвард Нортон",
                PosterPath = "/Content/Poster/ntxstZ7AXiWgqFjrQnq7GuN9XHU.jpg",
                BackgroundPath = "/Content/Background/5YtXsLG9ncjjFyGZjoeV31CGf01.jpg",
            });

            context.Movies.Add(new Movie
            {
                Name = "Погоня за ураганом",
                OriginalName = "The Hurricane Heist",
                AgeRatingId = 2,
                Country = "США",
                Director = "Роб Коен",
                Script = "Карлос Девіс, Ентоні Фінглтон",
                Description = "Злочинці намагаються зробити пограбування століття - обчистити Монетний двір США, поки на регіон обрушується ураган п'ятої категорії.",
                Duration = new TimeSpan(1, 42, 0),
                Genres = "Бойовик",
                Language = "Українська",
                ShowStart = new DateTime(2018, 3, 1),
                ShowEnd = new DateTime(2018, 4, 19),
                Starring = "Тобі Кеббелл, Меггі Грейс, Райан Квантен, Ралф Айнесон, Мелісса Болона",
                PosterPath = "/Content/Poster/wh1f7peigW0qUXXwynwVAt7axZd.jpg",
                BackgroundPath = "/Content/Background/22Q1GGmw1vt1OiTTt2ue9q4k6MW.jpg",
            });

            #endregion

            context.SaveChanges();

            #region генерую сеанси
            Random random = new Random();
            int moviesCount = context.Movies.Count() + 1;
            // сеанси на 7 днів // ні, на 7 не буду. генерація 1 дня зайняла 15хв (30-72 сеанси)
            for (int i = 2; i < 4; i++)
            {
                // у всіх залах
                foreach (var itemCinemaHall in context.CinemaHalls)
                {
                    double hour = 2; //+=2
                    // від 5 до 12 сеансів на день
                    int sessionPerDay = 3;// random.Next(5, 10);
                    for (int sessionNumber = 0; sessionNumber < sessionPerDay; sessionNumber++)
                    {
                        int movieId = random.Next(1, moviesCount);
                        double randMin = random.Next(-10, 30);
                        var currentSession = new Session
                        {
                            MovieId = movieId,
                            DateTime = DateTime.Now.AddDays(i).AddHours(hour).AddMinutes(randMin),
                            CinemaHall = itemCinemaHall
                        };
                        context.Sessions.Add(currentSession);
                        // додаю ціни за місця на сеанс
                        foreach (var itemSeat in itemCinemaHall.Seats)
                        {
                            context.TicketPrices.Add(new TicketPrice { Seat = itemSeat, Price = Convert.ToInt32(itemSeat.SeatType.DefaultPrice), Session = currentSession });
                        }
                        hour += 2 + randMin / 60;
                    }
                }
                context.SaveChanges();
            }
            #endregion

            context.SaveChanges();
        }
    }
}