using FitMe;
using FitMe.Contracts.Email;
using FitMe.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// إعدادات أساسية
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// إعداد الاتصال بقاعدة البيانات
var connectionString = builder.Configuration.GetConnectionString("con") ??
    throw new InvalidOperationException("Connection string not found!");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// إعداد هوية المستخدم
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// إعداد البريد الإلكتروني
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IEmailSender>(provider =>
{
    var emailSettings = provider.GetRequiredService<IOptions<MailSettings>>().Value;
    return new EmailSender(
        emailSettings.Email,
        emailSettings.AppPassword,
        emailSettings.Host,
        emailSettings.SSL,
        emailSettings.Port,
        emailSettings.IsBodyHtml
    );
});
builder.Services.AddHttpContextAccessor();

//builder.Services.AddAuthentication()
//               .AddGoogle(options =>
//               {
//                   IConfigurationSection googleAuthSection = builder.Configuration.GetSection("Authentication:Google");

//                   options.ClientId = googleAuthSection["ClientId"];
//                   options.ClientSecret = googleAuthSection["ClientSecret"];
//               });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
builder.Services.AddScoped<IBrandService, BrandService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads")),
    RequestPath = "/Uploads"
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");



app.UseAuthentication();
app.UseAuthorization();




app.MapControllers();
app.Run();
