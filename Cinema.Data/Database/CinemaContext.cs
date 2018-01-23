using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Data.Database
{
    public class CinemaContext : DbContext
    {
        public CinemaContext() : base("Cinema")
        {
            System.Data.Entity.Database.SetInitializer(new DataBaseInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<OrderItem>()
               .HasOptional(x => x.Ticket)
               .WithOptionalPrincipal(x => x.OrderItem)
               .Map(a => a.MapKey("OrderItemId"));
        }

        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<CommentType> CommentTypes { get; set; }
        public virtual DbSet<City> Cities { get; set; }
        public virtual DbSet<Cinema> Cinemas { get; set; }
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
    }

    public class DataBaseInitializer : DropCreateDatabaseAlways<CinemaContext>
    {
        protected override void Seed(CinemaContext context)
        {
            base.Seed(context);
            PerformInitialSetup(context);
        }

        public void PerformInitialSetup(CinemaContext context)
        {
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
    }
}
