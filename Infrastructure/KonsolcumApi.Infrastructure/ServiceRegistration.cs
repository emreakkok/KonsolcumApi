using KonsolcumApi.Application.Abstractions.Services;
using KonsolcumApi.Application.Abstractions.Services.Configurations;
using KonsolcumApi.Application.Abstractions.Token;
using KonsolcumApi.Application.Services;
using KonsolcumApi.Infrastructure.Services;
using KonsolcumApi.Infrastructure.Services.Configurations;
using KonsolcumApi.Infrastructure.Services.Token;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IFileService, FileService>();
            serviceCollection.AddScoped<ITokenHandler, TokenHandler>();
            serviceCollection.AddScoped<IMailService, MailService>();
            serviceCollection.AddScoped<IApplicationService, ApplicationService>();

        }
    }
}
