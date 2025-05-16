using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.WebSockets;
using Microsoft.IdentityModel.Tokens;
using SensorApi.Services;
using ServerAPI.Services;
using System.Text;
using ServerAPI.Data;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using ServerAPI.Automapper;
using ServerAPI.Models;
using Amazon.S3;
using Amazon.Runtime;
using Microsoft.Extensions.Configuration;

namespace CompressAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    policy => policy
                        .WithOrigins("https://localhost:4200", "https://127.0.0.1:4200")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

            builder.Services.AddControllers();
            builder.Configuration.AddJsonFile("secrets.json", optional: true);

            builder.Services.AddAutoMapper(typeof(AppMappingProfile));


            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = "Your_Issuer",
                        ValidAudience = "Your_Audience",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("aVeryLongSecretKeyThatIsAtLeast32BytesLong")),
                        ClockSkew = TimeSpan.Zero
                    };
                });
            builder.Services.AddSingleton<DCTDecompressionService>();
            builder.Services.AddSingleton<WebSocketHandler>();
            builder.Services.AddSingleton<S3Service>();
            builder.Services.AddScoped<UserService>();
            builder.Services.AddTransient<IEmailSenderExtended, EmailSender>();
            builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);

            builder.Services.AddWebSockets(options =>
            {
                options.KeepAliveInterval = TimeSpan.FromSeconds(120);
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddGrpc();

            var app = builder.Build();
            app.UseHttpsRedirection();
            app.UseCors("CorsPolicy");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseWebSockets();

            app.MapGrpcService<DetectionGrpcService>();


            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
