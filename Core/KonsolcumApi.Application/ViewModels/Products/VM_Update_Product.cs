using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.ViewModels.Products
{
    public class VM_Update_Product
    {
        public string id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public decimal price { get; set; }
        public int stockQuantity { get; set; }
        public bool isActive { get; set; }
        public bool isFeatured { get; set; }
        public string categoryId { get; set; }
        public string categoryName { get; set; }
        public DateTime createdDate { get; set; }
        public DateTime? updatedDate { get; set; }

        public string? showcaseImagePath { get; set; }
    }
}
