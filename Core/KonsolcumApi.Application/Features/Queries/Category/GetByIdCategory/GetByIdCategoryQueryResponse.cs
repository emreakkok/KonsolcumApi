using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Queries.Category.GetByIdCategory
{
    public class GetByIdCategoryQueryResponse
    {
        public string Name { get; set; } = string.Empty;         // Kategori adı (örn: "PlayStation", "Xbox")
        public string Description { get; set; } = string.Empty;  // Kategori açıklaması
        public bool IsActive { get; set; } = true;

    }
}
