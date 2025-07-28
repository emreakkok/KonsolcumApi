using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.DTOs.Order
{
    public class CreateOrder
    {
        public string ShippingAddress { get; set; }
        public string? BasketId { get; set; }
    }
}
