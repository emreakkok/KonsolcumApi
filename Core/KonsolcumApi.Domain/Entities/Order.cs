using KonsolcumApi.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KonsolcumApi.Domain.Entities
{
    public class Order : BaseEntity
    {
        // Müşteri siparişlerini tutan entity
       
        public string ShippingAddress { get; set; } = string.Empty; // Teslimat adresi
        public string OrderCode { get; set; } = string.Empty;  // Sipariş numarası (örn: "ORD-2024-001")
        public Guid BasketId { get; set; }

        // İlişkiler - Navigation Properties
        [JsonIgnore]
        public virtual Basket? Basket { get; set; }

        [JsonIgnore]
        public virtual CompletedOrder? CompletedOrder { get; set; }


    }
}
