
using FitMe.Contracts.Common;
using FitMe.Contracts.Product;

namespace FitMe.Controllers;

[Route("[controller]")]
[ApiController]
public class ProductController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;
    [HttpPost("")]
    public async Task<IActionResult>CreateAsync([FromForm] ProductRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _productService.CreateAsync(request, cancellationToken);
       return result.IsSuccess? Created() : result.ToProblem();
    }
    [HttpGet("")]
    public async Task<IActionResult> GetAllAsync([FromQuery] RequestFilters filters, CancellationToken cancellationToken = default)
    {
        var result = await _productService.GetAllAsync(filters, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await _productService.GetAsync(id, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateAsync(int id, [FromForm] ProductRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _productService.UpdateAsync(id, request, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var result = await _productService.DeleteAsync(id, cancellationToken);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }
}
