using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using YABA.API.Middlewares;
using YABA.API.Settings;
using YABA.API.Settings.Swashbuckle;
using YABA.Data.Configuration;
using YABA.Data.Context;
using YABA.Service.Configuration;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
var auth0Section = configuration.GetSection("Authentication").GetSection("Auth0");
var auth0Settings = auth0Section.Get<Auth0Settings>();
var domain = $"https://{auth0Settings.Domain}/";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.Authority = domain;
    options.Audience = auth0Settings.Identifier;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        NameClaimType = ClaimTypes.NameIdentifier
    };
});

builder.Services.AddApiVersioning(setup =>
{
    setup.DefaultApiVersion = new ApiVersion(1, 0);
    setup.AssumeDefaultVersionWhenUnspecified = true;
    setup.ReportApiVersions = true;
    setup.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// Add services to the container.
builder.Services.AddServiceProjectDependencyInjectionConfiguration(configuration);
builder.Services.AddDataProjectDependencyInjectionConfiguration(configuration);
builder.Services.AddControllers().AddNewtonsoftJson();

// Add AutoMapper profiles
var mapperConfiguration = new MapperConfiguration(mapperConfiguration =>
{
    mapperConfiguration.AddProfile(new YABA.API.Settings.AutoMapperProfile());
    mapperConfiguration.AddProfile(new YABA.Service.Configuration.AutoMapperProfile());
});

IMapper mapper = mapperConfiguration.CreateMapper();
builder.Services.AddSingleton(mapper);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
         c =>
         {
             c.SwaggerDoc(
                 "v1",
                 new OpenApiInfo
                 {
                     Title = "YABA.API",
                     Version = "v1"
                 });
             c.OperationFilter<RemoveVersionParameterFilter>();
             c.DocumentFilter<ReplaceVersionWithExactValueInPathFilter>();
             c.ResolveConflictingActions(apiDescription => apiDescription.First());
         }
);

var app = builder.Build();

// Run database migrations
using (var scope = app.Services.CreateScope())
{
    var yabaDbContext = scope.ServiceProvider.GetRequiredService<YABAReadWriteContext>();
    yabaDbContext.Database.Migrate();
}

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

// Add custom middlewares
app.UseMiddleware<AddCustomClaimsMiddleware>();

app.Run();
