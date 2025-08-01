
using FitMe.Contracts.Category;

namespace FitMe.Controllers;

[Route("[controller]")]
[ApiController]
public class CategoryController(ICategoryService categoryService) : ControllerBase
{
    private readonly ICategoryService _categoryService = categoryService;
    [HttpPost("")]
    public async Task<IActionResult> CreateAsync(CategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.CreateAsync(request, cancellationToken);
        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpGet("")]
    public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetAllAsync(cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetAsync([FromRoute] int Id, CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetAsync(Id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPut("{Id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] int Id, [FromBody] CategoryRequest request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.UpdateAsync(Id, request, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpDelete("{Id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int Id, CancellationToken cancellationToken)
    {
        var result = await _categoryService.DeleteAsync(Id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
   
}
