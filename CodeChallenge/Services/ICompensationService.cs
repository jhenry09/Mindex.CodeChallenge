using System.Collections.Generic;
using System.Threading.Tasks;
using CodeChallenge.Models;

namespace CodeChallenge.Services;

public interface ICompensationService
{
    /// <summary>
    /// Creates a <see cref="Compensation"/>
    /// </summary>
    /// <param name="compensation"> to create</param>
    /// <returns>A <see cref="Compensation"/> record for the employee</returns>
    Task<Compensation> CreateAsync(Compensation compensation);

    /// <summary>
    /// Gets the <see cref="Compensation"/> for the <see cref="Employee"/> with <param name="employeeId"/>
    /// </summary>
    /// <param name="employeeId">the id of the <see cref="Employee"/> to create the <see cref="Compensation"/> for</param>
    /// <returns>The <see cref="Compensation"/> records for the employee</returns>
    Task<List<Compensation>> GetByEmployeeId(string employeeId);
}