using MAgicVilla_VillaAPI.Data;
using MAgicVilla_VillaAPI.Repository;
using MAgicVilla_VillaAPI.Repository.IRepository;
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




        builder.Services.AddControllers().AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
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
