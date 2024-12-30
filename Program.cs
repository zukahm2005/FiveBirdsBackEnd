using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using five_birds_be.Data;
using five_birds_be.Jwt;
using five_birds_be.Response;
using five_birds_be.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using five_birds_be.Servi;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 30)))
);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });



builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>

        {
            if (string.IsNullOrEmpty(context.Token) && context.Request.Cookies.ContainsKey("token"))
            {
                context.Token = context.Request.Cookies["token"];
            }
            return Task.CompletedTask;
        },

            OnTokenValidated = context =>
            {
                var claimsIdentity = context.Principal.Identity as ClaimsIdentity;

                var roleClaim = claimsIdentity?.FindFirst("Role");
                if (roleClaim != null)
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roleClaim.Value));
                }

                return Task.CompletedTask;
            }
        };

    });


builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
                             .SelectMany(v => v.Errors)
                             .Select(e => e.ErrorMessage)
                             .ToList();
        var errorResponse = ApiResponse<string>.Failure(400, string.Join("; ", errors));
        return new BadRequestObjectResult(errorResponse);
    };
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.WithOrigins("http://localhost:5173", "http://46.202.178.139:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
});



builder.WebHost.UseUrls("http://0.0.0.0:5005");
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ExamService>();
builder.Services.AddScoped<QuestionService>();
builder.Services.AddScoped<AnswerService>();
builder.Services.AddScoped<ResultService>();
builder.Services.AddScoped<CandidateTestService>();


builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<CloudinaryService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<ICandidateService, CandidateService>();
builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Bearer token"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


var app = builder.Build();

app.UseCors("AllowAll");
app.UseCookiePolicy(new CookiePolicyOptions
{
    MinimumSameSitePolicy = SameSiteMode.None, 
    HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always,
    Secure = CookieSecurePolicy.SameAsRequest , 
});
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API Documentation V1");
    c.RoutePrefix = string.Empty;
});

app.MapControllers();
app.Run();
