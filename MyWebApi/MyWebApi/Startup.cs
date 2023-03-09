using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MyWebApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MyWebApi.Interfaces;
using MyWebApi.Repositories;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.Extensions.Options;
using MyWebApi.Services.Background;

namespace MyWebApi
{
    public class Startup
    {
        public IConfiguration _config { get; }
        
        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new List<CultureInfo>
                {
                    new CultureInfo("en"),
                    new CultureInfo("ru"),
                    new CultureInfo("uk"),
                };
                options.DefaultRequestCulture = new RequestCulture("ru");
                options.SupportedCultures = supportedCultures;
                //options.AddInitialRequestCultureProvider(new CustomRequestCultureProvider(async context =>
                //{
                //    Console.WriteLine((string)context.Request.Headers.AcceptLanguage);
                //    return await Task.FromResult(new ProviderCultureResult(context.Request.Headers.AcceptLanguage.ToString()));
                //}));
            });

            services.AddHostedService<BackgroundWorker>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyWebApi", Version = "v1" });
            });
            services.AddDbContext<UserContext>(options => options.UseNpgsql(_config.GetConnectionString("DefaultConnectionString")));
            services.AddScoped<IUserRepository, SystemUserRepository>();
            services.AddScoped<ITestRepository, TestRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<ISponsorRepository, SponsorRepository>();
            services.AddScoped<IBackgroundRepository, BackgroundRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles();

            var localizeOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizeOptions.Value);

            //app.UseRequestLocalization(new RequestLocalizationOptions 
            //{ 
            //    ApplyCurrentCultureToResponseHeaders = true
            //});

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MyWebApi v1"));
            }

            app.UseHttpsRedirection();


            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
