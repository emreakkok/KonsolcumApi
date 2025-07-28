using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.DTOs.Order
{
    public class ListOrder
    {
        public string Id { get; set; }
        public string OrderCode { get; set; }
        [JsonPropertyName("userName")]
        public string Username { get; set; }

        public string ShippingAddress { get; set; }

        public decimal TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; }

        public bool IsCompleted { get; set; }
    }
}
