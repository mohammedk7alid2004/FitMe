using FitMe.Contracts.Common;
using FitMe.Contracts.Product;

namespace FitMe.Services; 

public interface IProductService
{
  Task<Result<PaginatedList<ProductResponse>>>GetAllAsync(RequestFilters filters, CancellationToken cancellationToken = default);
    Task<Result<ProductResponse>> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<Result>CreateAsync (ProductRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateAsync(int id, ProductRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
