
using BusinessObjects;
using DataAccess;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Repositories.MessageRepository;
using Repositories.UserRepository;
using Services.AuthenServices;
using Services.AuthenServices.InterfaceAuthen;
using Services.MessageServices;
using Services.UserServices;
using System.Text;

namespace ApiServices
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string connectionString = builder.Configuration.GetConnectionString("MyConnection");


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
            .AddCookie()
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options => {
                options.ClientId = builder.Configuration.GetSection("GoogleKeys:ClientId").Value;
                options.ClientSecret = builder.Configuration.GetSection("GoogleKeys:ClientSecret").Value;
                options.Scope.Add("profile");
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
                    ValidAudience = builder.Configuration["JwtConfig:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"])),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    }
                };
            }); ;


            builder.Services.AddDbContext<WebChatContext>(options =>
                options.UseSqlServer(connectionString));

            // Add data access object to the container.
            builder.Services.AddScoped<UserDAO>();
            builder.Services.AddScoped<MessageDAO>();

            // Add repositories to the container.
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();

            // Add services to the container.
            builder.Services.AddScoped<IJWTService, JWTService>();
            builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IMessageService, MessageService>();


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    BearerFormat = "JWT",
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    Name = "JWT Authentication",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                options.AddSecurityDefinition("Bearer", jwtSecurityScheme);

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          jwtSecurityScheme,
                          Array.Empty<string>()
                    }
                });
            });

         
            builder.Services.AddAuthorization();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowSpecificOrigins",
                    policy =>
                    {
                        policy.WithOrigins(
                                    "https://localhost:50501", 
                                    "http://localhost:5108"
                                )
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
            });
            var app = builder.Build();
            app.UseCors("AllowSpecificOrigins");
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
