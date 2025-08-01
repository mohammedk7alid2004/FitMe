using FitMe.Contracts.Common;
using FitMe.Contracts.Product;
using FitMe.Extensions;

namespace FitMe.Services;

public class ProductService(ApplicationDbContext context, IWebHostEnvironment env) : IProductService
{
    private readonly ApplicationDbContext _context = context;
    private readonly IWebHostEnvironment _env = env;

    public async Task<Result> CreateAsync(ProductRequest request, CancellationToken cancellationToken = default)
    {
        var ProductExists = await _context.Products.AnyAsync(p => p.Name == request.Name,cancellationToken);
        if (ProductExists)
            return Result.Failure(ProductError.ProductAlreadyExists);
        var product = request.Adapt<Product>();
        var uploaded = await product.UploadPhotoAsync2(request.ImageUrl, _env, _context);
        if (!uploaded)
            return Result.Failure(ProductError.ProductPhotoNotUploaded);
        var result = await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<PaginatedList<ProductResponse>>> GetAllAsync(RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var productExist= await _context.Products.AnyAsync(cancellationToken);
        if (!productExist)
            return Result.Failure<PaginatedList<ProductResponse>>(ProductError.ProductNotFound);
        var products =  _context.Products.ProjectToType<ProductResponse>().AsNoTracking();
        var response = await PaginatedList<ProductResponse>.CreateAsync(products, filters.PageNumber, filters.PageSize, cancellationToken);
        return Result.Success(response);
    }

    public async Task<Result<ProductResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var productExist = await _context.Products.AnyAsync(p => p.Id == id, cancellationToken);
        if (!productExist)
            return Result.Failure<ProductResponse>(ProductError.ProductNotFound);
        var product = await _context.Products
            .Where(p => p.Id == id)
            .ProjectToType<ProductResponse>()
            .AsNoTracking()
            .SingleOrDefaultAsync(cancellationToken);
        return Result.Success(product!);
    }

    public async Task<Result<bool>> UpdateAsync(int id, ProductRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (product is null)
            return Result.Failure<bool>(ProductError.ProductNotFound);

        var productName = await _context.Products
            .AnyAsync(p => p.Name == request.Name && p.Id != id, cancellationToken);

        if (productName)
            return Result.Failure<bool>(ProductError.ProductAlreadyExists);

        request.Adapt(product);

        var uploaded = await product.UploadPhotoAsync2(request.ImageUrl, _env, _context);
        if (!uploaded)
            return Result.Failure<bool>(ProductError.ProductPhotoNotUploaded);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(true);
    }


    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var productExist = await _context.Products.FindAsync(id);
        if (productExist is null)
            return Result.Failure<bool>(ProductError.ProductNotFound);
        _context.Products.Remove(productExist);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(true);

    }

}
