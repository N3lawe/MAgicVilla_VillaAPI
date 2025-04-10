
using MAgicVilla_VillaAPI.Logging;
using Serilog;

namespace MAgicVilla_VillaAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // add services to the container

            //Log.Logger =new LoggerConfiguration().MinimumLevel.Debug()
            //    .WriteTo.File("Log/villaLogs.txt", rollingInterval:RollingInterval.Day).CreateLogger();

            //builder.Host.UseSerilog();

            builder.Services.AddControllers(option =>
            {
               // option.ReturnHttpNotAcceptable = true;
            }
            ).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<ILogging, LoggingV2>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
