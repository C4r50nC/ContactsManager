using ContactsManager.Core.Domain.IdentityEntities;
using ContactsManager.Core.Domain.RepositoryContracts;
using ContactsManager.Core.ServiceContracts;
using ContactsManager.Core.Services;
using ContactsManager.Infrastructure.DbContext;
using ContactsManager.Infrastructure.Repositories;
using ContactsManager.Ui.Filters.ActionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ContactsManager.Ui.StartupExtensions
{
    public static class ServiceConfigurationExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ResponseHeaderActionFilter>(); // Add to services so it can be used in ResponseHeaderFilterFactoryAttribute.CreateInstance()

            services.AddControllersWithViews(options =>
            {
                // options.Filters.Add<ResponseHeaderActionFilter>(); // Add global filter without arguments
                var logger = services.BuildServiceProvider().GetRequiredService<ILogger<ResponseHeaderActionFilter>>();
                options.Filters.Add(new ResponseHeaderActionFilter(logger)
                {
                    ResponseHeaderKey = "X-Key-From-Global",
                    ResponseHeaderValue = "Value-From-Global",
                    Order = 2,
                });

                // Prevents XSRF attack globally
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });

            services.AddScoped<ICountriesRepository, CountriesRepository>();
            services.AddScoped<IPersonsRepository, PersonsRepository>();

            services.AddScoped<ICountriesService, CountriesService>();
            services.AddScoped<IPersonsGetterService, PersonsGetterServiceWithFewerExcelFields>();
            services.AddScoped<PersonsGetterService, PersonsGetterService>();
            services.AddScoped<IPersonsAdderService, PersonsAdderService>();
            services.AddScoped<IPersonsSorterService, PersonsSorterService>();
            services.AddScoped<IPersonsDeleterService, PersonsDeleterService>();
            services.AddScoped<IPersonsUpdaterService, PersonsUpdaterService>();

            services.AddTransient<PersonsListActionFilter>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequiredLength = 4;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireDigit = false;
                    options.Password.RequiredUniqueChars = 2;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();

            services.AddAuthorization(options =>
            {
                // Enforce authorization policy for all action methods without [AllowAnonymous] tag
                options.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();

                options.AddPolicy("NotAuthenticated", policy =>
                {
                    policy.RequireAssertion(context =>
                    {
                        return context.User.Identity == null || !context.User.Identity.IsAuthenticated;
                    });
                });
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Accounts/Login";
            });

            services.AddHttpLogging(httpLoggingOptions =>
            {
                httpLoggingOptions.LoggingFields = Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.RequestProperties
                | Microsoft.AspNetCore.HttpLogging.HttpLoggingFields.ResponsePropertiesAndHeaders;
            });

            return services;
        }
    }
}
