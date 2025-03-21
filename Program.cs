using DraganaMakeup.Context;
using DraganaMakeup.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = $"Server={Environment.GetEnvironmentVariable("MYSQLHOST")};" +
                       $"Port={Environment.GetEnvironmentVariable("MYSQLPORT")};" +
                       $"Database={Environment.GetEnvironmentVariable("MYSQLDATABASE")};" +
                       $"User={Environment.GetEnvironmentVariable("MYSQLUSER")};" +
                       $"Password={Environment.GetEnvironmentVariable("MYSQLPASSWORD")};" +
                       "SslMode=Preferred;";

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString,
        new MySqlServerVersion(new Version(8, 0, 39)),
        mysqlOptions => mysqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)
    ));

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AppointmentService>();
builder.Services.AddScoped<EmailService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins($"{Environment.GetEnvironmentVariable("ORIGIN")}")
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.None; 
    options.OnAppendCookie = context =>
    {
        if (context.CookieOptions.Secure)
        {
            context.CookieOptions.Extensions.Add("partitioned");
        }
    };
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("AllowAngularApp");

app.UseSwagger();
app.UseSwaggerUI();

app.UseCookiePolicy();

app.MapControllers();

var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://*:{port}"); 
app.Run();