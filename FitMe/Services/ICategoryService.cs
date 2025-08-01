using FitMe.Contracts.Category;

namespace FitMe.Services;

public interface ICategoryService
{
    Task<Result<IEnumerable<CategoryResponse>>> GetAllAsync(CancellationToken cancellationToken=default);
    Task<Result<CategoryResponse>>GetAsync(int id, CancellationToken cancellationToken = default);
    Task<Result>CreateAsync(CategoryRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(int Id, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(int Id , CategoryRequest request, CancellationToken cancellationToken = default);
}
