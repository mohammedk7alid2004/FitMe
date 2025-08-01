using FitMe.Contracts.Category;

namespace FitMe.Services;

public class CategoryService(ApplicationDbContext context) : ICategoryService
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Result> CreateAsync(CategoryRequest request, CancellationToken cancellationToken = default)
    {
        var CategoryExist = await _context.Categories.AnyAsync(x => x.Name == request.Name, cancellationToken);
        if(CategoryExist) 
            return Result.Failure(CategoryError.DuplicatedCategoryName);
        var category =  request.Adapt<Category>();
        var result = await _context.Categories.AddAsync(category, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
    public async Task<Result<IEnumerable<CategoryResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var categories = await _context.Categories.ToListAsync();
        if(categories is null)
            return Result.Failure<IEnumerable<CategoryResponse>>(CategoryError.CategoryNotFound);
        var response = categories.Adapt<IEnumerable<CategoryResponse>>();
        return Result.Success(response);
    }
    public async Task<Result<CategoryResponse>> GetAsync(int id, CancellationToken cancellationToken = default)
    {
       var category= await _context.Categories.SingleOrDefaultAsync(x=>x.CategoryId==id, cancellationToken);
        if (category is null)
            return Result.Failure<CategoryResponse>(CategoryError.CategoryNotFound);
        var response = category.Adapt<CategoryResponse>();
        return Result.Success(response);
    }

    public async Task<Result<bool>> UpdateAsync(int Id, CategoryRequest request, CancellationToken cancellationToken = default)
    {
        var OldCategory = await _context.Categories.SingleOrDefaultAsync(x => x.CategoryId == Id, cancellationToken);
        if (OldCategory is null)
            return Result.Failure<bool>(CategoryError.CategoryNotFound);

        var CategoryExist = await _context.Categories.AnyAsync(x => x.Name == request.Name && x.CategoryId != Id, cancellationToken);
        if (CategoryExist)
            return Result.Failure<bool>(CategoryError.DuplicatedCategoryName);

        OldCategory.Name = request.Name;

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(int Id, CancellationToken cancellationToken = default)
    {
        var OldCategory = await _context.Categories.SingleOrDefaultAsync(x => x.CategoryId == Id, cancellationToken);
        if (OldCategory is null)
            return Result.Failure<bool>(CategoryError.CategoryNotFound);
        _context.Categories.Remove(OldCategory);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success(true);

    }
}
