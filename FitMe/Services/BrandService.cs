using FitMe.Abstractions;
using FitMe.Contracts.Brand;
using System.Collections.Generic;

namespace FitMe.Services;

public class BrandService (ApplicationDbContext context): IBrandService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result> CreateAsync(BrandRequest request, CancellationToken cancellationToken = default)
    {
        bool brandExists = await _context.Brands
          .AnyAsync(x => x.Name == request.Name, cancellationToken);

        if (brandExists)
        
            return Result.Failure(BrandError.DuplicatedBrandName);
        var brand = request.Adapt<Brand>();

        var result = await _context.Brands.AddAsync(brand,cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();


    }
    public async Task<Result<IEnumerable<BrandResponse>>> GetAllAsync()
    {
        var brands = await _context.Brands.ToListAsync();
        if (brands is null)
            return Result.Failure<IEnumerable<BrandResponse>>(BrandError.BrandNotFound);
        var response = brands.Adapt<IEnumerable<BrandResponse>>();
        return Result.Success(response);
    }
    public async Task<Result<BrandResponse>> GetByIdAsync(int id)
    {
       var result = await _context.Brands.SingleOrDefaultAsync(z=>z.Id == id);
        if (result is null)
            return Result.Failure<BrandResponse>(BrandError.BrandNotFound);
        var response = result.Adapt<BrandResponse>();
        return Result.Success(response);
    }
    public async Task<Result<bool>> DeleteAsync(int id)
    {
        var brand = await _context.Brands.FindAsync(id);

        if (brand is null)
            return Result.Failure<bool>(BrandError.BrandNotFound);

        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();

        return Result.Success(true);
    }




    public async  Task<Result<bool>> UpdateAsync(int id, BrandRequest request)
    {
        var brand = await _context.Brands.FindAsync(id);

        if (brand is null)
            return Result.Failure<bool>(BrandError.BrandNotFound);
        var result= request.Adapt(brand);
        _context.Brands.Update(result);
        await _context.SaveChangesAsync();
        return Result.Success(true);
    }
}
