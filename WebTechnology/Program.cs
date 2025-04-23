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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hien v Tam API", Version = "v1" });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});
builder.Services.AddDbContext<WebTech>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("Connection"),
        new MySqlServerVersion(new Version(8, 0, 41))
    )
);

builder.Services.AddCors(option =>
{
    option.AddDefaultPolicy(policy => policy.AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowed(_ => true));
}
);
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ITrendRepository, TrendRepostiory>();
builder.Services.AddScoped<ITrendService, TrendService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductTrendsRepository, ProductTrendsRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IDimensionRepository, DimensionRepository>();
builder.Services.AddScoped<IProductPriceRepository, ProductPriceRepository>();
builder.Services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IProductStatusRepository, ProductStatusRespository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<ICartItemService, CartItemService>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddHostedService<UserAuthCleanupService>();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
    
    if (jwtSettings == null)
    {
        throw new InvalidOperationException("JWT settings not found in configuration");
    }

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings.AccessTokenKey)),
        ClockSkew = jwtSettings.ClockSkew
    };
});
builder.Services.AddAuthorization(options =>
{

    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    // Policy cho Customer
    options.AddPolicy("CustomerOnly", policy =>
        policy.RequireRole("Customer"));

    options.AddPolicy("AdminOrCustomer", policy =>
        policy.RequireRole("Admin", "Customer"));
});
builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSettings"));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
