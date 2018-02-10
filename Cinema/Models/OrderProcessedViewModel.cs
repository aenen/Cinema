using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cinema.Models
{
    public class OrderProcessedViewModel
    {
        public OrderProcessedViewModel()
        {
            OrderItems = new List<OrderItemViewModel>();
        }

        public ICollection<OrderItemViewModel> OrderItems { get; set; }
        public int SumPrice => OrderItems.Sum(x => x.Price);

        public int SessionId { get; set; }
        public DateTime SessionDate { get; set; }

        public string CinemaName { get; set; }
        public string CinemaHallName { get; set; }

        public string MovieName { get; set; }
    }
    
    public class OrderItemViewModel
    {
        public int SeatRow { get; set; }
        public int SeatNumber { get; set; }
        public int Price { get; set; }
    }
}