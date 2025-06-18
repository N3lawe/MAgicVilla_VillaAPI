using MagicVilla_Web.Service;
using MagicVilla_Web.Service.IService;
using MAgicVilla_Web;

namespace MagicVilla_Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();
            builder.Services.AddAutoMapper(typeof(MappingConfig));

            builder.Services.AddHttpClient<IVillaService, VillaService>();
            builder.Services.AddScoped<IVillaService, VillaService>();



            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();
            app.Run();
        }
    }
}
