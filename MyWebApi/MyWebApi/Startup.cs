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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyWebApi", Version = "v1" });
            });
            services.AddDbContext<StoreContext>(options => options.UseNpgsql(_config.GetConnectionString("DefaultConnectionString")));
            services.AddDbContext<UserContext>(options => options.UseNpgsql(_config.GetConnectionString("DefaultConnectionString")));
            services.AddDbContext<TestContext>(options => options.UseNpgsql(_config.GetConnectionString("DefaultConnectionString")));
            services.AddDbContext<AdminContext>(options => options.UseNpgsql(_config.GetConnectionString("DefaultConnectionString")));
            services.AddDbContext<SponsorContext>(options => options.UseNpgsql(_config.GetConnectionString("DefaultConnectionString")));
            services.AddScoped<IUserRepository, SystemUserRepository>();
            services.AddScoped<ITestRepository, TestRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<ISponsorRepository, SponsorRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
