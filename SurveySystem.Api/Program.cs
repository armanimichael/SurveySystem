using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SurveySystem.Data;
using SurveySystem.Data.Models;
using SurveySystem.Services.JWTService;
using SurveySystem.Services.MailService;
using SurveySystem.Services.SurveyService;
using SurveySystem.Services.UserService;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// CORS
var allowedOrigins = builder.Configuration.GetValue<string>("CORS:Origins").Split(',');
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policyBuilder =>
        {
            policyBuilder
                .WithOrigins(allowedOrigins)
                .AllowCredentials()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(opts => opts.IncludeScopes = true);

// Add services to the container.
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddTransient<IMailService, MailService>();

builder.Services.AddControllers()
    // Custom validation error
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            IEnumerable<string> errors =
                context.ModelState.Values
                    .Select(v => v.Errors.Select(err => err.ErrorMessage))
                    .Where(v => v.Any())
                    .SelectMany(v => v)
                    .Distinct();

            var response = new ApiResponse(false, errors, (int)HttpStatusCode.BadRequest);
            return new BadRequestObjectResult(response);
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Survey API"
    });
    // Enable Swagger Auth
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer [token]'",
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Db context
string connectionString = builder.Configuration["DB:SqlServerConnectionString"];
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));

// Dals
builder.Services.AddScoped<ISurveyService, SurveyService>();

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedEmail = true)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Auth
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidAudience = builder.Configuration["JWT:ValidAudience"],
            ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:SecretKey"])),
            ClockSkew = TimeSpan.Zero
        };
    });

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Cookies Policies
app.UseCookiePolicy(new CookiePolicyOptions
{
    HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.Always,
    MinimumSameSitePolicy = SameSiteMode.None
});

app.UseCors();

app.UseHttpsRedirection();

app.UseExceptionHandler("/error");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();