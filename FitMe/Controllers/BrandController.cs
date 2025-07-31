
using FitMe.Contracts.Brand;

namespace FitMe.Controllers;

[Route("[controller]")]
[ApiController]
public class BrandController(IBrandService brandService) : ControllerBase
{
    private readonly IBrandService _brandService = brandService;

    [HttpPost("")]
    public async Task<IActionResult>CreateAsync (BrandRequest request ,CancellationToken cancellationToken)
    {
        var result = await _brandService.CreateAsync(request, cancellationToken);

        return result.IsSuccess ? Ok() : result.ToProblem();
    }
    [HttpGet("")]
    public async Task<IActionResult>GetAllAsync()
    {
        var result= await _brandService.GetAllAsync();
        return result.IsSuccess?Ok(result.Value):result.ToProblem();
    }
    [HttpGet("{Id}")]
    public async Task<IActionResult> GetAsync([FromRoute]int Id)
    {
        var result = await _brandService.GetByIdAsync(Id);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpDelete("{Id}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] int Id)
    {
        var result = await _brandService.DeleteAsync(Id);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
    [HttpPut("{Id}")]
    public async Task<IActionResult> UpdateAsync([FromRoute] int Id, [FromBody] BrandRequest request)
    {
        var result = await _brandService.UpdateAsync(Id, request);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}
