using System.Collections.Generic;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CodeChallenge.Repositories;

public interface ICompensationRepository
{
    /// <summary>
    /// Adds the <see cref="Compensation"/> to the db context
    /// </summary>
    /// <param name="compensation">The compensation to add</param>
    /// <returns>A <see cref="Compensation"/> record</returns>
    ValueTask<EntityEntry<Compensation>> AddAsync(Compensation compensation);
    
    /// <summary>
    /// Gets the <see cref="Compensation"/> for the <see cref="Employee"/> with <param name="employeeId"/>
    /// </summary>
    /// <param name="employeeId">the id of the <see cref="Employee"/> to create the <see cref="Compensation"/> for</param>
    /// <returns>The <see cref="Compensation"/> records for the employee</returns>
    Task<List<Compensation>>GetByEmployeeIdAsync(string employeeId);
    
    /// <summary>
    /// Persists the changes
    /// </summary>
    /// <returns></returns>
    Task SaveAsync();
}