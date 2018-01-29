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

            string roleName = "Administrators";
            string userName = "admin@gmail.com";
            string password = "12345Admin.";
            string email = "admin@gmail.com";

            if (!roleMgr.RoleExists(roleName))
            {
                roleMgr.Create(new ApplicationRole(roleName));
            }

            ApplicationUser user = userMgr.FindByName(userName);
            if (user == null)
            {
                userMgr.Create(new ApplicationUser { UserName = userName, Email = email }, password);
                user = userMgr.FindByName(userName);
            }

            if (!userMgr.IsInRole(user.Id, roleName))
            {
                userMgr.AddToRole(user.Id, roleName);
            }

            //...

            context.TicketStatus.Add(new TicketStatus { Name = "Сплачено" });
            context.TicketStatus.Add(new TicketStatus { Name = "Заброньовано", Description = "За 30 хвилин до початку сеансу підійдіть до каси кінотеатру та назвіть номер квитка або номер замовлення." });

            context.OrderStatus.Add(new OrderStatus { Name = "Сплачено" });
            context.OrderStatus.Add(new OrderStatus { Name = "Не сплачено", Description = "Викупіть квитки через сайт, або на касі кінотеатру за 30 хвилин до початку сеансу." });
            context.OrderStatus.Add(new OrderStatus { Name = "Відхилено" });

            context.CommentTypes.Add(new CommentType { Name = "Не вибрано" });
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

            context.SeatTypes.Add(new SeatType { Name = "Звичайне", Keyword = "seat-common" });
            context.SeatTypes.Add(new SeatType { Name = "Люкс", Keyword = "seat-lux", Description = "М'ягкі крісла з високою спинкою та фіксованим сидінням." });
            context.SeatTypes.Add(new SeatType { Name = "Супер Люкс", Keyword = "seat-super-lux", Description = "Крісла-реклайнери, що забезпечують підвищений комфорт перегляду, для гурманів кіно." });
            context.SeatTypes.Add(new SeatType { Name = "Диван для двох", Keyword = "seat-sofa", Description = "Комфортний диван для двох." });

            context.SaveChanges();
        }

        public void PerformAdditionalSetup(CinemaContext context)
        {
            context.Cinemas.Add(new CinemaEntity { Name = "Панорама", Keyword="Panorama", CityId = 1, PhoneNumber = "+380 (97) 345 67 89", Address = "Оболонський проспект, 10" });
            context.Cinemas.Add(new CinemaEntity { Name = "Vision", Keyword="Vision", CityId = 3, PhoneNumber = "+380 (68) 987 65 43", Address = "Штильова вулиця, 134" });

            context.CinemaHalls.Add(new CinemaHall { CinemaId = 1, Name = "1" });

            context.CinemaHalls.Add(new CinemaHall { CinemaId = 2, Name = "Light" });
            context.CinemaHalls.Add(new CinemaHall { CinemaId = 2, Name = "Dark" });

            context.Seats.Add(new Seat { CinemaHallId = 1, Row = 1, Number = 1, SeatTypeId = 1, SeatStyle = new SeatStyle { PositionX = 0, PositionY = 0, SizeHeight = 50, SizeWidth = 40 } });

            context.SaveChanges();
        }
    }
}
