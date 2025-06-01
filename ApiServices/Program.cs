
using BusinessObjects;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Repositories.UserRepository;
using Services.UserServices;

namespace ApiServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string connectionString = builder.Configuration.GetConnectionString("MyConnection");

            builder.Services.AddDbContext<WebChatContext>(options =>
                options.UseSqlServer(connectionString));

            // Add data access object to the container.
            builder.Services.AddScoped<UserDAO>();

            // Add repositories to the container.
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            // Add services to the container.
            builder.Services.AddScoped<IUserService, UserService>();


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
