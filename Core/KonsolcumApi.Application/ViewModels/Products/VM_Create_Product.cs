using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.ViewModels.Products
{
    public class VM_Create_Product
    {
        [Required]
        public string name { get; set; }

        public string description { get; set; }

        public decimal price { get; set; }

        public int stockQuantity { get; set; }

        public bool isActive { get; set; } = true;
        public bool isFeatured { get; set; } = false;

        [Required]
        public string categoryId { get; set; }
    }
}
