using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.DTOs.Order
{
    public class OrderDetailProductDto
    {
        public Guid BasketItemId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice => UnitPrice * Quantity;
        public string? ShowcaseImagePath { get; set; }
        public string Description { get; set; }
    }

}
