using System;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using CodeChallenge.Repositories;
using Microsoft.Extensions.Logging;

namespace CodeChallenge.Services;

public class ReportingStructureService : IReportingStructureService
{
    private readonly IEmployeeService _employeeService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<ReportingStructureService> _logger;
    
    public ReportingStructureService(IEmployeeService employeeService, 
        IEmployeeRepository employeeRepository,
        ILogger<ReportingStructureService> logger)
    {
        _employeeService = employeeService;
        _employeeRepository = employeeRepository;
        _logger = logger;
    }
    
    public async Task<ReportingStructure> GetByEmployeeIdAsync(string id)
    {
        var employee = _employeeService.GetById(id);

        if (employee is null)
        {
            _logger.LogError($"Employee with id {id} does not exist. Unable to build reporting structure.");
            return null;
        }
        
        // One thing to think about is what if an employee reports to multiple people? 
        // The references aren't setup to handle that so I didn't bother messing with it.
        // But if I had to deal with this, we would need to keep track of the employee ids already
        // accounted for in the direct report count.
        return new ReportingStructure
        {
            Employee = employee,
            NumberOfReports = await GetNumberOfDirectReportsAsync(id: employee.EmployeeId)
        };
    }

    /// <summary>
    /// This uses recursion to traverse the direct report tree. This could also be done with BFS or DFS,
    /// but I find the recursive approach simple enough.
    /// </summary>
    /// <param name="id"></param>
    /// <returns>Count of direct reports</returns>
    private async Task<int> GetNumberOfDirectReportsAsync(string id)
    {
        // NOTE: please look at the summary above this repository method for an explanation on why this 
        // is not efficient in a production system and what I would do in that situation.
        var directReports = await _employeeRepository.GetDirectReportIdsAsync(id);
        
        var numberOfDirectReports = directReports.Count;
        
        _logger.LogTrace($"Employee with id {id} has {numberOfDirectReports} direct reports.");
        
        foreach (var directReport in directReports)
        {
            numberOfDirectReports += await GetNumberOfDirectReportsAsync(directReport);
        }

        return numberOfDirectReports;
    }
}