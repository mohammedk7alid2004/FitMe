using FitMe;
using FitMe.Contracts.Email;
using FitMe.Settings;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add basic services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database and dependencies
builder.Services.AddDependencies(builder.Configuration);

// Configure EmailSender
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
app.UseDeveloperExceptionPage(); // قبل أي middleware زي routing

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
