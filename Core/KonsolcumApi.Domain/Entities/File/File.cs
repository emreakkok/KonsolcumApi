using KonsolcumApi.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KonsolcumApi.Domain.Entities.File
{
    public class File : BaseEntity
    {
        public string FileName { get; set; }

        public string Path { get; set; }

        public bool Showcase { get; set; }

        public Guid? CategoryId { get; set; } // Category resmi ise doldurulur
        public Guid? ProductId { get; set; }  // Product resmi ise doldurulur

        [JsonIgnore] // JSON serileştirmesinde döngüsel referansları engellemek için
        public virtual Category? Category { get; set; }

        [JsonIgnore]

        public virtual Product? Product { get; set; }

    }
}
