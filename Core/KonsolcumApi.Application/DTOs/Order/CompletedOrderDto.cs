using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.DTOs.Order
{
    public class CompletedOrderDto
    {
        public string OrderCode { get; set; }
        public DateTime OrderDate { get; set; }
        [JsonPropertyName("userName")]
        public string Username { get; set; }

        public string Email { get; set; }

    }
}
