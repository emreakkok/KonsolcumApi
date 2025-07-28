using KonsolcumApi.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KonsolcumApi.Domain.Entities
{
    public class Product : BaseEntity
    {
        
        // Satılacak ürünleri tutan ana entity
        public string Name { get; set; } = string.Empty;         // Ürün adı (örn: "FIFA 24", "PlayStation 5")
        public string Description { get; set; } = string.Empty;  // Ürün açıklaması
        public decimal Price { get; set; }                       // Ürün fiyatı
        public int StockQuantity { get; set; }                   // Stok miktarı
        public bool IsActive { get; set; } = true;               // Ürün aktif mi?
        public bool IsFeatured { get; set; } = false;            // Öne çıkan ürün mü?

        // Foreign Key
        public Guid CategoryId { get; set; }                     // Hangi kategoriye ait

        public string? ShowcaseImagePath { get; set; }

        // İlişkiler - Navigation Properties
        [JsonIgnore]
        public virtual Category? Category { get; set; }
        [JsonIgnore]
        public virtual ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();

        [JsonIgnore]
        public virtual ICollection<KonsolcumApi.Domain.Entities.File.File> Files { get; set; } = new List<KonsolcumApi.Domain.Entities.File.File>();
    }
}
