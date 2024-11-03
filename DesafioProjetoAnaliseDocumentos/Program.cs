namespace DesafioProjetoAnaliseDocumentos
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Serilog.Formatting.Json;
    using Serilog;
    using System;
    using DesafioProjetoAnaliseDocumentos.Services;
    using Microsoft.Extensions.Configuration;
    using DesafioProjetoAnaliseDocumentos.Context;
    using System.IO;
    using System.Globalization;
    using Serilog.Formatting.Compact;
    using System.Text;

    static class Program
    {
        public static void Main(String[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());
            builder.Configuration.AddJsonFile("config.json", false, true);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            Log.Logger = new LoggerConfiguration()
                            .Enrich.FromLogContext()
                            .WriteTo.Console(formatProvider: CultureInfo.CurrentCulture)
                            .CreateBootstrapLogger();

            builder.Services.AddSerilog((services, lc) => lc
               .ReadFrom.Services(services)
               .Enrich.FromLogContext()
               .WriteTo.Console(formatProvider: CultureInfo.CurrentCulture)
               .WriteTo.File(formatter: new CompactJsonFormatter(),
                             path: "./logs/log-.txt",
                             rollingInterval: RollingInterval.Day,
                             rollOnFileSizeLimit: true,
                             retainedFileCountLimit: 10,
                             encoding: Encoding.UTF8
               )
           );

            builder.Services.AddScoped<IAzureStorageContext, AzureStorageContext>();
            builder.Services.AddScoped<IAzureStorageService, AzureStorageService>();


            var app = builder.Build();

            app.UseSerilogRequestLogging();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
