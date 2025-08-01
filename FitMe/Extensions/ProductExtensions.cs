namespace FitMe.Extensions
{
    public static class ProductExtensions
    {
        public static async Task<bool> UploadPhotoAsync2(
     this Product product,
     IFormFile photo,
     IWebHostEnvironment env,
     ApplicationDbContext _context)
        {
            if (photo == null || photo.Length == 0)
                return false;

            try
            {
                var uploadsFolder = Path.Combine(env.WebRootPath, "uploads", "product");
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

                if (!string.IsNullOrEmpty(product.ImageUrl))
                {
                    var oldFilePath = Path.Combine(env.WebRootPath, product.ImageUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (File.Exists(oldFilePath))
                    {
                        File.Delete(oldFilePath);
                    }
                }

                product.ImageUrl = $"/uploads/product/{uniqueFileName}";

                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
