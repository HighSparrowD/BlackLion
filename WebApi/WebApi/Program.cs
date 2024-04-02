using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using WebApi.Data;
using WebApi.Interfaces;
using WebApi.Interfaces.Services;
using WebApi.Models.Models.Identity;
using WebApi.Repositories;
using WebApi.Services.Admin;
using WebApi.Services.Authentication;
using WebApi.Services.Background;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

var defaultCulture = configuration["Globalization:DefaultCulture"];
var supportedCultures = configuration.GetSection("Globalization:SupportedCultures")
	.Get<string[]>();

// Add configuration for RequestLocalizationOptions
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
	options.DefaultRequestCulture = new RequestCulture(configuration["Globalization:DefaultCulture"]);
	options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
	options.SupportedUICultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
});

builder.Services.AddControllers().AddJsonOptions(opts =>
{
	var enumConverter = new JsonStringEnumConverter();
	opts.JsonSerializerOptions.Converters.Add(enumConverter);
});

builder.Host.UseSerilog();

Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
	.WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
	.WriteTo.File(path: "../.local/log/log-.txt", restrictedToMinimumLevel: LogEventLevel.Information, 
		rollingInterval: RollingInterval.Day, 
		flushToDiskInterval: TimeSpan.FromDays(31) )
	.CreateLogger();

builder.Services.AddHostedService<BackgroundWorker>();

var secret = configuration["Jwt:Key"];
var key = Encoding.ASCII.GetBytes(secret);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
	options => options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
	{
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidateIssuer = false,
		ValidateAudience = false,
		IssuerSigningKey = new SymmetricSecurityKey(key)
	}
);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
    c.CustomSchemaIds(type => type.ToString());
	
	// Authentication
	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
	{
		Name = "Authorization",
		Scheme = "Bearer",
		BearerFormat = "JWT",
		Type = SecuritySchemeType.ApiKey,
		In = ParameterLocation.Header
	});
	c.AddSecurityRequirement(new OpenApiSecurityRequirement()
	{
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
	
});

// Custom authorization is utilized
builder.Services.AddAuthorization();

builder.Services.AddDbContext<UserContext>(options => options.UseNpgsql(configuration.GetConnectionString("DefaultConnectionString")));
builder.Services.AddScoped<IUserRepository, SystemUserRepository>();
builder.Services.AddScoped<ITestRepository, TestRepository>();
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
builder.Services.AddScoped<ISponsorRepository, SponsorRepository>();
builder.Services.AddScoped<IBackgroundRepository, BackgroundRepository>();
builder.Services.AddScoped<IRegistrationRepository, RegistrationRepository>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAdminService, AdminService>();

// Configuration
var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseRequestLocalization();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
	app.UseSwagger();
	app.UseSwaggerUI(options => {
		options.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1");

	});
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Update Database
using (var serviceScope = app.Services.GetService<IServiceScopeFactory>().CreateScope())
{
	var context = serviceScope.ServiceProvider.GetRequiredService<UserContext>();
	context.Database.Migrate();
}

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

await app.RunAsync();
