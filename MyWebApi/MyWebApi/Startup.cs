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
using WebApi.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Interfaces;
using WebApi.Repositories;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.Extensions.Options;
using WebApi.Services.Background;

namespace WebApi
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
            // Configure supported cultures
            var defaultCulture = _config["Globalization:DefaultCulture"];
            var supportedCultures = _config.GetSection("Globalization:SupportedCultures")
                .Get<string[]>();

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture(_config["Globalization:DefaultCulture"]);
                options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
                options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
            });

            services.AddHostedService<BackgroundWorker>();

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });
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
            app.UseRequestLocalization();

            app.UseStaticFiles();

            var localizeOptions = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(localizeOptions.Value);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));
            }

            app.UseHttpsRedirection();


            app.UseRouting();

            app.UseAuthorization();

            //Actualize Database
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetRequiredService<UserContext>();
                context.Database.Migrate();
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
