using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Data;
using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CodeChallenge.Repositories;

public class CompensationRepository : ICompensationRepository
{
    private readonly EmployeeContext _employeeContext;
    
    public CompensationRepository(EmployeeContext employeeContext)
    {
        _employeeContext = employeeContext;
    }
    
    public ValueTask<EntityEntry<Compensation>> AddAsync(Compensation compensation)
    {
        compensation.CompensationId = Guid.NewGuid().ToString();
        return _employeeContext.Compensations.AddAsync(compensation);
    }

    // There was no specification on the relation between an employee and compensation.
    // I took the path of one employee to many compensations because the employee 
    // could have had multiple different compensations throughout their career
    public Task<List<Compensation>> GetByEmployeeIdAsync(string employeeId)
    {
        return _employeeContext.Compensations
            .Where(x => x.Employee.EmployeeId == employeeId)
            .Include(x => x.Employee)
            .ToListAsync();
    }

    public Task SaveAsync()
    {
        return _employeeContext.SaveChangesAsync();
    }
}