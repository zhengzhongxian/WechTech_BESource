using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using WebTechnology.API;
using WebTechnology.Repository.CoreHelpers.Crud;
using WebTechnology.Repository.Repositories.Implementations;
using WebTechnology.Repository.Repositories.Interfaces;
using WebTechnology.Repository.UnitOfWork;
using WebTechnology.Service.Models;
using WebTechnology.Service.Services.BackgroundServices;
using WebTechnology.Service.Services.Implementationns;
using WebTechnology.Service.Services.Interfaces;
using WebTechnology.Service.CoreHelpers;
using WebTechnology.Service.CoreHelpers.Multimedia;
using System.Security.Claims;
using WebTechnology.Configurations;
using WebTechnology.Service.Services.Implementations;
using WebTechnology.Repository.Mappings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Learn more about configuring Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
SwaggerConfiguration.ConfigureSwagger(builder.Services);

// Configure database
builder.Services.AddDbContext<WebTech>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("Connection"),
        new MySqlServerVersion(new Version(8, 0, 41))
    )
);

// Configure CORS
builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(policy => policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true));
});

// Configure AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Configure services and repositories
ServiceConfiguration.ConfigureServices(builder.Services);

// Configure authentication
AuthenticationConfiguration.ConfigureAuthentication(builder.Services, builder.Configuration);

// Configure email settings
builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSettings"));

// Configure Cloudinary
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("Cloudinary"));

// Add background services
builder.Services.AddHostedService<UserAuthCleanupService>();

// Register SeedService
builder.Services.AddScoped<ISeedService, SeedService>();


// dcmm
builder.Services.AddAutoMapper(typeof(UserStatusMappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebTechnology API V1");
        c.OAuthClientId("swagger-ui");
        c.OAuthAppName("Swagger UI");
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseMiddleware<CustomUnauthorizedMiddleware>();
app.UseAuthorization();

app.UseStaticFiles(); // For custom CSS/JS
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hien v Tam API v1");
    c.RoutePrefix = "api-docs";
    c.InjectStylesheet("/swagger-ui/custom.css");
    c.InjectJavascript("/swagger-ui/custom.js");
});

// Cấu hình ReDoc với giao diện màu sáng
app.UseReDoc(c =>
{
    c.DocumentTitle = "Hien v Tam API Documentation";
    c.RoutePrefix = "redoc";
    c.SpecUrl = "/swagger/v1/swagger.json";
    c.RequiredPropsFirst(); // Hiển thị các thuộc tính bắt buộc lên đầu
    c.SortPropsAlphabetically(); // Sắp xếp thuộc tính theo bảng chữ cái
    c.HideDownloadButton(); // Ẩn nút tải xuống
    c.HideHostname(); // Ẩn hostname
    c.ExpandResponses("200,201"); // Mở rộng các response thành công
    c.InjectStylesheet("/redoc/custom-redoc.css"); // CSS tùy chỉnh cho ReDoc
});

app.MapControllers();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var seedService = scope.ServiceProvider.GetRequiredService<ISeedService>();
    await seedService.SeedDataAsync();
}

app.Run();
