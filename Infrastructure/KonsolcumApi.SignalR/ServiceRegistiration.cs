using KonsolcumApi.Application.Abstractions.Hubs;
using KonsolcumApi.SignalR.HubServices;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.SignalR
{
    public static class ServiceRegistiration
    {
        public static void AddSignalRServices(this IServiceCollection collection)
        {
            collection.AddTransient<ICategoryHubService, CategoryHubService>();
            collection.AddTransient<IProductHubService, ProductHubService>();
            collection.AddTransient<IOrderHubService, OrderHubService>();
            collection.AddSignalR();
        }
    }
}
