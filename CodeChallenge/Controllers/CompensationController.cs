using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using CodeChallenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CodeChallenge.Controllers;

[ApiController]
[Route("api/compensation")]
public class CompensationController : ControllerBase
{
    private readonly ILogger<CompensationController> _logger;
    private readonly ICompensationService _compensationService;
    
    public CompensationController(ILogger<CompensationController> logger,
        ICompensationService compensationService)
    {
        _logger = logger;
        _compensationService = compensationService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateCompensation([FromBody] Compensation compensation)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Compensation received does not have a valid model.");
            return UnprocessableEntity(compensation);
        }
        
        _logger.LogDebug($"Received compensation create request for employee {compensation.Employee.FirstName} {compensation.Employee.LastName}");
        
        var createdCompensation = await _compensationService.CreateAsync(compensation);

        if (createdCompensation is null)
        {
            _logger.LogError("Error creating compensation during ICompensationService call.");
            return BadRequest();
        }
        
        return CreatedAtRoute("getCompensationByEmployeeId", new { id = createdCompensation.Employee.EmployeeId }, createdCompensation);
    }
    
    [HttpGet("{id}", Name = "getCompensationByEmployeeId")]
    public async Task<IActionResult> GetEmployeeById(string id)
    {
        _logger.LogDebug($"Received compensation get request for employee id '{id}'");

        var compensations = await _compensationService.GetByEmployeeId(id);

        if (compensations is null || !compensations.Any())
            return NotFound();

        return Ok(compensations);
    }
}