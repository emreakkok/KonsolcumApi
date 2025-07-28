using KonsolcumApi.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KonsolcumApi.Domain.Entities
{
    public class Category : BaseEntity
    {
        // Ürün kategorilerini tutan entity (Konsol, Oyun, Aksesuar vs.)
        public string Name { get; set; } = string.Empty;         // Kategori adı (örn: "PlayStation", "Xbox")
        public string Description { get; set; } = string.Empty;  // Kategori açıklaması
        public bool IsActive { get; set; } = true;               // Kategori aktif mi?

        public string? ShowcaseImagePath { get; set; }

        // İlişkiler - Navigation Properties
        [JsonIgnore]
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();

        [JsonIgnore]
        public virtual ICollection<KonsolcumApi.Domain.Entities.File.File> Files { get; set; } = new List<KonsolcumApi.Domain.Entities.File.File>();
    }
}
