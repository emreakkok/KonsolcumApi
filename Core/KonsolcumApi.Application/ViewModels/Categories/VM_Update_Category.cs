using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.ViewModels.Categories
{
    public class VM_Update_Category
    {
        public string id { get; set; }

        public string name { get; set; }

        public string description { get; set; }

        public string? showcaseImagePath { get; set; }
        public bool isActive { get; set; }
    }
}
