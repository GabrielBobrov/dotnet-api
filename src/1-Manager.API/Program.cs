using System.Text;
using System.Text.Json.Serialization;
using AutoMapper;
using EscNet.IoC.Cryptography;
using Manager.API.Token;
using Manager.API.ViewModels;
using Manager.Core.Communication.Handlers;
using Manager.Core.Communication.Mediator;
using Manager.Core.Communication.Mediator.Interfaces;
using Manager.Core.Communication.Messages.Notifications;
using Manager.Domain.Entities;
using Manager.Infra.Context;
using Manager.Infra.Interfaces;
using Manager.Infra.Repositories;
using Manager.Services.DTO;
using Manager.Services.Interfaces;
using Manager.Services.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

#region Jwt

    var secretKey = builder.Configuration["Jwt:Key"];

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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

#endregion

#region AutoMapper

    var autoMapperConfig = new MapperConfiguration(cfg =>
    {
        cfg.CreateMap<User, UserDto>().ReverseMap();
        cfg.CreateMap<CreateUserViewModel, UserDto>().ReverseMap();
        cfg.CreateMap<UpdateUserViewModel, UserDto>().ReverseMap();
    });

            builder.Services.AddSingleton(autoMapperConfig.CreateMapper());

#endregion

builder.Services.AddSingleton(d => builder.Configuration);
#region Services
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<IUserRepository, UserRepository>();
#endregion

#region Database
    builder.Services.AddDbContext<ManagerContext>(options => options.UseMySql(builder.Configuration["ConnectionStrings:ManagerAPIMySql"],Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.27-mysql")));
#endregion

#region Mediator

    builder.Services.AddMediatR(typeof(Program));
    builder.Services.AddScoped<INotificationHandler<DomainNotification>, DomainNotificationHandler>();
    builder.Services.AddScoped<IMediatorHandler, MediatorHandler>();

#endregion

#region Cryptography
    builder.Services.AddRijndaelCryptography(builder.Configuration["Cryptography:Key"]);
#endregion

#region Token
    builder.Services.AddScoped<ITokenGenerator, TokenGenerator>();
#endregion

#region Swagger

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Manager API",
                    Version = "v1",
                    Description = "API construída na serie de vídeos no canal Lucas Eschechola.",
                    Contact = new OpenApiContact
                    {
                        Name = "Lucas Eschechola",
                        Email = "lucas.gabriel@eu.com",
                        Url = new Uri("https://eschechola.com.br")
                    },
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Por favor utilize Bearer <TOKEN>",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
                });
            });

#endregion


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
