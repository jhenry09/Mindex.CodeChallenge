using System.Collections.Generic;
using System.Threading.Tasks;
using CodeChallenge.Models;
using CodeChallenge.Repositories;
using Microsoft.Extensions.Logging;

namespace CodeChallenge.Services;

public class CompensationService : ICompensationService
{
    private readonly ILogger<CompensationService> _logger;
    private readonly ICompensationRepository _compensationRepository;
    private readonly IEmployeeRepository _employeeRepository;

    public CompensationService(ILogger<CompensationService> logger,
        ICompensationRepository compensationRepository,
        IEmployeeRepository employeeRepository)
    {
        _logger = logger;
        _compensationRepository = compensationRepository;
        _employeeRepository = employeeRepository;
    }
    
    // <inheritdoc>
    public async Task<Compensation> CreateAsync(Compensation compensation)
    {
        if (compensation is null)
        {   
            _logger.LogError("Error creating compensation, compensation is null.");
            return null;
        }

        var employee = _employeeRepository.GetById(compensation.Employee.EmployeeId);

        if (employee is null)
        {
            _logger.LogError($"Error creating compensation, employee {compensation.Employee.EmployeeId} does not exist.");
            return null;
        }

        compensation.Employee = employee;
        
        await _compensationRepository.AddAsync(compensation);
        await _compensationRepository.SaveAsync();

        return compensation;
    }
    
    // <inheritdoc>
    public async Task<List<Compensation>> GetByEmployeeId(string employeeId)
    {
        if (string.IsNullOrEmpty(employeeId))
        {
            _logger.LogError("The employee id is empty, unable to get compensations.");
            return null;
        }
        
        return await _compensationRepository.GetByEmployeeIdAsync(employeeId);
    }
}