using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Features.Queries.Product.GetActiveProducts
{
    public class GetActiveProductsQueryResponse
    {
        public int TotalProductCount { get; set; }
        public object Products { get; set; }
    }
}
