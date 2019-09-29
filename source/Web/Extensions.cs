using DotNetCore.AspNetCore;
using DotNetCore.IoC;
using DotNetCoreArchitecture.Application;
using DotNetCoreArchitecture.Database;
using DotNetCoreArchitecture.Infra;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DotNetCoreArchitecture.Web
{
    public static class Extensions
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddMatchingInterface(typeof(IUserApplicationService).Assembly);
        }

        public static void AddContext(this IServiceCollection services)
        {
            var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();

            var connectionString = configuration.GetConnectionString(nameof(Context));

            services.AddDbContextMigrate<Context>(options => options.UseSqlServer(connectionString));
        }

        public static void AddDatabaseServices(this IServiceCollection services)
        {
            services.AddMatchingInterface(typeof(IUnitOfWork).Assembly);
        }

        public static void AddInfraServices(this IServiceCollection services)
        {
            services.AddMatchingInterface(typeof(ISignInService).Assembly);
        }

        public static void AddSecurity(this IServiceCollection services)
        {
            services.AddHash(10000, 128);
            services.AddJsonWebToken(Guid.NewGuid().ToString(), TimeSpan.FromHours(12));
            services.AddAuthenticationJwtBearer();
        }

        public static void AddSpa(this IServiceCollection services)
        {
            services.AddSpaStaticFiles("Frontend/dist");
        }

        public static void UseSpa(this IApplicationBuilder application)
        {
            application.UseSpaAngularServer("Frontend", "development");
        }
    }
}
