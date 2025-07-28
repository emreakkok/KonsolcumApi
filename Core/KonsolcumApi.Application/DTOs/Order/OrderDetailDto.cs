using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.DTOs.Order
{
    public class OrderDetailDto
    {
        public Guid OrderId { get; set; }
        public string OrderCode { get; set; }
        public string ShippingAddress { get; set; }
        [JsonPropertyName("userName")]
        public string Username { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; }

        public List<OrderDetailProductDto> Products { get; set; }

        public bool IsCompleted { get; set; }
    }

}
