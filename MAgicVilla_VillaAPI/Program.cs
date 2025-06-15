using MAgicVilla_VillaAPI.Data;
using MAgicVilla_VillaAPI.Repository;
using MAgicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MAgicVilla_VillaAPI;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<ApplicationDbContext>(option => { option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection")); });
        builder.Services.AddAutoMapper(typeof(MappingConfig));
        builder.Services.AddScoped<IVillaRepository, VillaRepository>();
        builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();




        builder.Services.AddControllers().AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
        builder.Services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ReportApiVersions = true;
        });
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();


        var app = builder.Build();
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
