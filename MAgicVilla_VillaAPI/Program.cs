using MAgicVilla_VillaAPI;
using MAgicVilla_VillaAPI.Data;
using MAgicVilla_VillaAPI.Models;
using MAgicVilla_VillaAPI.Repository;
using MAgicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        #region Configure Database
        builder.Services.AddDbContext<ApplicationDbContext>(option =>
        {
            option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"));
        });
        #endregion

        #region Configure Dependency Injection
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        builder.Services.AddScoped<IVillaRepository, VillaRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
        builder.Services.AddAutoMapper(typeof(MappingConfig));
        #endregion

        #region Configure Authentication (JWT)
        var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");
        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });
        #endregion

        #region Configure Controllers
        builder.Services.AddControllers(options =>
        {
            options.CacheProfiles.Add("Default30", new CacheProfile() { Duration = 30 });
        })
        .AddNewtonsoftJson()
        .AddXmlDataContractSerializerFormatters();
        #endregion

        #region Configure Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Enter 'Bearer' [space] and your token.",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
                }
            });

            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1.0",
                Title = "Magic Villa API",
                Description = "API to manage Villa"
            });
        });
        #endregion

        var app = builder.Build();

        #region Configure Middleware
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Magic_Villa API V1");
            });
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
        #endregion
    }
}
