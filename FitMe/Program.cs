using FitMe;
using FitMe.Contracts.Email;
using FitMe.Settings;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();


var connectionString = builder.Configuration.GetConnectionString("con")??
    throw new InvalidOperationException("Connection String Cannot Found !");
builder.Services.AddDbContext<ApplicationDbContext>(option => option.UseSqlServer(connectionString));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDependencies(builder.Configuration);
builder.Services.AddSwaggerGen();
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
app.UseStaticFiles(); // This enables serving files from wwwroot
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads")),
    RequestPath = "/Uploads"
}); 
app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
