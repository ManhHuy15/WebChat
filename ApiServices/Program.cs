
using ApiServices.Chat;
using BusinessObjects;
using CloudinaryDotNet;
using DataAccess;
using DTOs.EmailDTOs;
using DTOs.FriendDTOs;
using DTOs.UserDTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using Microsoft.OpenApi.Models;
using Repositories.FriendRepository;
using Repositories.GroupRepository;
using Repositories.MessageRepository;
using Repositories.UserRepository;
using Services.AuthenServices;
using Services.AuthenServices.InterfaceAuthen;
using Services.ClouldinaryServices;
using Services.EmailServices;
using Services.FriendServices;
using Services.GroupServices;
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
            var configuration = builder.Configuration;
            string connectionString = configuration.GetConnectionString("MyConnection");
            var cloudName = configuration.GetValue<string>("CloudinaryAccount:CloudName");
            var apiKey = configuration.GetValue<string>("CloudinaryAccount:ApiKey");
            var apiSecret = configuration.GetValue<string>("CloudinaryAccount:ApiSecret");
            var gmailSetting = configuration.GetSection(GmailSettings.GmailSettingsKey).Get<GmailSettings>();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
                    ValidateAudience = false,
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
                    },
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/hubs"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            }); ;


            builder.Services.AddDbContext<WebChatContext>(options =>
                options.UseSqlServer(connectionString));
            builder.Services.AddSingleton(new Cloudinary(new Account(cloudName, apiKey, apiSecret)));

            // Add data access object to the container.
            builder.Services.AddScoped<BaseDAO>();
            builder.Services.AddScoped<UserDAO>();
            builder.Services.AddScoped<MessageDAO>();
            builder.Services.AddScoped<GroupDAO>();
            builder.Services.AddScoped<FriendDAO>();

            // Add repositories to the container.
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IGroupRepository, GroupRepository>();
            builder.Services.AddScoped<IFriendRepository, FriendRepository>();

            // Add services to the container.
            builder.Services.AddScoped<IJWTService, JWTService>();
            builder.Services.AddSingleton<GmailSettings>(gmailSetting);
            builder.Services.AddScoped<IEmailSenderService, EmailSenderService>();
            builder.Services.AddScoped<IPasswordHashingService, PasswordHashingService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IMessageService, MessageService>();
            builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();
            builder.Services.AddScoped<IGroupService, GroupService>();
            builder.Services.AddScoped<IFriendService, FriendService>();
            
            builder.Services.AddSignalR();

            builder.Services.AddControllers().AddOData(options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100));
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
                              .AllowCredentials()
                              .SetIsOriginAllowed(origin => true);
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

            app.MapHub<ChatHub>("/hubs/chat");
            app.Run();
        }
    }
}
