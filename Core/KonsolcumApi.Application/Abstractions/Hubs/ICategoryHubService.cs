using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Application.Abstractions.Hubs
{
    public interface ICategoryHubService
    {
        Task CategoryAddedMessageAsync(string message);
    }
}
