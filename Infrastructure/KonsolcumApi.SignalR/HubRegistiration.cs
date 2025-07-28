using KonsolcumApi.SignalR.Hubs;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.SignalR
{
    public static class HubRegistiration
    {
        public static void MapHubs(this WebApplication webApplication)
        {
            webApplication.MapHub<CategoryHub>("/categories-hub");
            webApplication.MapHub<ProductHub>("/products-hub");
            webApplication.MapHub<OrderHub>("/orders-hub");
        }
    }
}
