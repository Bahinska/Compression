using Microsoft.AspNetCore.WebSockets;
using ServerAPI.Services;

namespace CompressAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .WithOrigins("http://localhost:5039")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            builder.Services.AddSingleton<DCTDecompressionService>();
            builder.Services.AddWebSockets(options =>
            {
                options.KeepAliveInterval = TimeSpan.FromSeconds(120);
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddGrpc();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseCors("CorsPolicy");

            app.MapGrpcService<DetectionGrpcService>();

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseWebSockets();

            app.MapControllers();

            app.Run();
        }
    }
}
