
using KE03_INTDEV_SE_1_Base.DAL;

namespace KE03_INTDEV_SE_1_Base
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // We gebruiken nu een Dapper-gebaseerde DAL-laag die verbinding maakt met
            // een Azure SQL Server. De DAL-services worden in de DI container geregistreerd.
            builder.Services.AddScoped<SQLCustomer>();
            builder.Services.AddScoped<SQLOrder>();
            builder.Services.AddScoped<SQLProducts>();
            builder.Services.AddScoped<SQLCustomerService>();
            builder.Services.AddScoped<SQLReview>();

            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
