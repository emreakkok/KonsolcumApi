using KonsolcumApi.Application.Abstractions.Hubs;
using KonsolcumApi.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.SignalR.HubServices
{
    public class CategoryHubService : ICategoryHubService
    {
        readonly IHubContext<CategoryHub> _hubContext;

        public CategoryHubService(IHubContext<CategoryHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task CategoryAddedMessageAsync(string message)
        {
            await _hubContext.Clients.All.SendAsync(ReceiveFunctionNames.CategoryAddedMessage, message);
        }
    }
}
