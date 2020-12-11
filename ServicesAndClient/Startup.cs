using AutoMapper;
using GloboTicket.Services.EventCatalog.DbContexts;
using GloboTicket.Services.ShoppingBasket.DbContexts;
using GloboTicket.Services.ShoppingBasket.Services;
using GloboTicket.Web.Models;
using GloboTicket.Web.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;

namespace ServicesAndClient
{
    public class Startup
    {
        private readonly IHostEnvironment environment;
        private readonly IConfiguration config;
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            config = configuration;
            this.environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var builder = services.AddControllersWithViews();
            services.AddHttpContextAccessor();

            if (environment.IsDevelopment())
                builder.AddRazorRuntimeCompilation();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddDbContext<EventCatalogDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("EventCatalogConnection")));
            services.AddScoped<GloboTicket.Services.EventCatalog.Repositories.ICategoryRepository, GloboTicket.Services.EventCatalog.Repositories.CategoryRepository>();
            services.AddScoped<GloboTicket.Services.EventCatalog.Repositories.IEventRepository, GloboTicket.Services.EventCatalog.Repositories.EventRepository>();


            services.AddDbContext<ShoppingBasketDbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("ShoppingBasketConnection"));
            });
            services.AddScoped<GloboTicket.Services.ShoppingBasket.Repositories.IBasketRepository, GloboTicket.Services.ShoppingBasket.Repositories.BasketRepository>();
            services.AddScoped<GloboTicket.Services.ShoppingBasket.Repositories.IBasketLinesRepository, GloboTicket.Services.ShoppingBasket.Repositories.BasketLinesRepository>();
            services.AddScoped<GloboTicket.Services.ShoppingBasket.Repositories.IEventRepository, GloboTicket.Services.ShoppingBasket.Repositories.EventRepository>();
            //services.AddHttpClient<IEventCatalogServiceForShoppingBasket, EventCatalogServiceForShoppingBasket>(c => c.BaseAddress = new Uri(config["ApiConfigs:EventCatalog:Uri"]));
            services.AddHttpClient<IEventCatalogServiceForShoppingBasket, EventCatalogServiceForShoppingBasket>();


            //services.AddHttpClient<IEventCatalogServiceForWebClient, EventCatalogServiceForWebClient>(c => c.BaseAddress = new Uri(config["ApiConfigs:EventCatalog:Uri"]));
            services.AddHttpClient<IEventCatalogServiceForWebClient, EventCatalogServiceForWebClient>();
            //services.AddHttpClient<IShoppingBasketServiceForWebClient, ShoppingBasketServiceForWebClient>(c => c.BaseAddress = new Uri(config["ApiConfigs:ShoppingBasket:Uri"]));
            services.AddHttpClient<IShoppingBasketServiceForWebClient, ShoppingBasketServiceForWebClient>();

            services.AddSingleton<Settings>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EventDto Catalog API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "EventDto Catalog API V1");

            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
