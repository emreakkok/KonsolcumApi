using KonsolcumApi.Application.Abstractions.Services;
using KonsolcumApi.Application.Repositories;
using KonsolcumApi.Domain.Entities.Identity;
using KonsolcumApi.Persistence.Contexts.DapperContext;
using KonsolcumApi.Persistence.Contexts.EfContext;
using KonsolcumApi.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolcumApi.Persistence.Services
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Tablo Oluşturmak için
            services.AddDbContext<EfDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<EfDbContext>().AddDefaultTokenProviders();
                                                       // GeneratePasswordResetTokenAsync

            // Diğer Veritabanı işleri için
            services.AddScoped<Context>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<IBasketItemRepository, BasketItemRepository>();
            services.AddScoped<ICompletedOrderRepository, CompletedOrderRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IEndpointRepository, EndpointRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();

            services.AddScoped<IFileRepository, FileRepository>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IBasketService, BasketService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAuthorizationEndpointService, AuthorizationEndpointService>();



        }

    }
}
