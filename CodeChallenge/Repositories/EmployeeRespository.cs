using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CodeChallenge.Data;

namespace CodeChallenge.Repositories
{
    public class EmployeeRespository : IEmployeeRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<IEmployeeRepository> _logger;

        public EmployeeRespository(ILogger<IEmployeeRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Employee Add(Employee employee)
        {
            employee.EmployeeId = Guid.NewGuid().ToString();
            _employeeContext.Employees.Add(employee);
            return employee;
        }

        // NOTE: I could have modified this to include the direct report employees.
        // But I decided to use GetDirectReportIds for performance reasons listed below.
        // I also didn't know if there is a reason we are not lazy or eagerly loading the direct reports,
        // this is something I would ask in the interview. I would also clarify if we wanted the direct reports 
        // list in the ReportingStructure response.
        public Employee GetById(string id)
        {
            return _employeeContext.Employees.SingleOrDefault(e => e.EmployeeId == id);
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }

        public Employee Remove(Employee employee)
        {
            return _employeeContext.Remove(employee).Entity;
        }

        /// <summary>
        /// Returns the ids of the direct reports for the employee with the id passed in.
        /// NOTE: This only returns the ids instead of the full employee object because we have to query
        /// for the nested direct reports anyway and only care about the count of direct reports,
        /// so might as well make it as efficient as possible and not retrieve every column from the db.
        /// If this was a production system this would not scale because of how many trips we make to the db
        /// for the direct reports. If this was a production system I would create a view that uses a recursive CTE
        /// that retrieves the recursive direct reports. This is so we only have to make one trip to the database
        /// for all the recursive direct reports of a single employee. I know some people think repositories should
        /// only get the entity, create a new entity, and save the changes (DDD), but for this challenge I decided against that.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<List<string>> GetDirectReportIdsAsync(string id)
        {
            return _employeeContext.Employees
                .Where(x => x.EmployeeId == id)
                .SelectMany(x => x.DirectReports)
                .Select(x => x.EmployeeId)
                .ToListAsync();
        }
    }
}
