using FitMe.Contracts.Brand;

namespace FitMe.Services;

public interface IBrandService
{
    Task<Result<IEnumerable<BrandResponse>>> GetAllAsync();
    Task<Result<BrandResponse>> GetByIdAsync(int id);
    Task<Result> CreateAsync(BrandRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(int id, BrandRequest request);
    Task<Result<bool>> DeleteAsync(int id);
}
