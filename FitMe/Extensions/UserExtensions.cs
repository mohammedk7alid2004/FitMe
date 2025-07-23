using System.Security.Claims;

namespace FitMe.Extensions;

public static class UserExtensions
{
    public static string? GetUserId(this ClaimsPrincipal user) =>
                user.FindFirstValue(ClaimTypes.NameIdentifier);
    public static async Task<bool> UploadPhotoAsync(
     this ApplicationUser user,
     IFormFile photo,
     IWebHostEnvironment env,
     UserManager<ApplicationUser> userManager)
    {
        if (photo == null || photo.Length == 0)
            return false;

        try
        {
            var uploadsFolder = Path.Combine(env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(photo.FileName)}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(fileStream);
            }

            if (!string.IsNullOrEmpty(user.Photo))
            {
                var oldFilePath = Path.Combine(env.WebRootPath, user.Photo.TrimStart('/'));
                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            user.Photo = $"/uploads/{uniqueFileName}";
            var result = await userManager.UpdateAsync(user);

            return result.Succeeded;
        }
        catch (Exception ex)
        {
            return false;
        }
    }
}
