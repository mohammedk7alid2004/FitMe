using FitMe.Contracts.Users;
using FitMe.Extensions;
using Microsoft.AspNetCore.Identity;

namespace FitMe.Services;

public class UserService(UserManager<ApplicationUser>user , IWebHostEnvironment  env, IHttpContextAccessor httpContextAccessor) : IUserService
{
    private readonly UserManager<ApplicationUser> _user = user;
    private readonly IWebHostEnvironment _env = env;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public async  Task<Result> ChangePasswordAsync(string userId, ChangePasswordRequest request)
    {
        var user = await _user.FindByIdAsync(userId);
        var result= await _user.ChangePasswordAsync(user!,request.currentPassword,request.newPassword);
        if (result.Succeeded)
         return   Result.Success();
        var error = result.Errors.First();
        return Result.Failure(new Error(error.Code,error.Description,StatusCodes.Status400BadRequest));
    }

    public async Task<Result<UserProfileResponse>> GetProfileAsync(string userId)
    {
        var request = _httpContextAccessor.HttpContext?.Request;

        var user = await _user.Users
            .Where(x => x.Id == userId)
            .Select(u => new UserProfileResponse(
                u.Email,
                u.FullName,
                !string.IsNullOrEmpty(u.Photo)
                    ? $"{request.Scheme}://{request.Host}{u.Photo}"
                    : null
            ))
            .SingleAsync();

        return Result.Success(user);
    }
    public async Task<Result> UpdateProfileAsync(string userId, UpdateProfileRequest request)
    {
        var user = await _user.FindByIdAsync(userId);
        if (user == null)
        {
            return Result.Failure(new Error("user.not_found", "User not found", StatusCodes.Status404NotFound));
        }

        request.Adapt(user);

        if (request.Photo != null)
        {
            var photoUpdated = await user.UploadPhotoAsync(request.Photo, _env, _user);
            if (!photoUpdated)
            {
                return Result.Failure(new Error("photo.upload_failed", "Failed to upload photo", StatusCodes.Status500InternalServerError));
            }
        }

        var result = await _user.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var error = result.Errors.First();
            return Result.Failure(new Error(error.Code, error.Description, StatusCodes.Status400BadRequest));
        }

        return Result.Success();
    }
}
