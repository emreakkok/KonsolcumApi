using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Queries.Category.GetActiveCategories
{
    public class GetActiveCategoriesQueryResponse
    {
        public int TotalCategoryCount { get; set; }
        public object Categories { get; set; }
    }
}
