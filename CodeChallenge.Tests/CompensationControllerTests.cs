using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CodeChallenge.Models;
using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration;

[TestClass]
public class CompensationControllerTests
{
    private static HttpClient _httpClient;
    private static TestServer _testServer;

    [ClassInitialize]
    // Attribute ClassInitialize requires this signature
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
    public static void InitializeClass(TestContext context)
    {
        _testServer = new TestServer();
        _httpClient = _testServer.NewClient();
    } 
    
    [ClassCleanup]
    public static void CleanUpTest()
    {
        _httpClient.Dispose();
        _testServer.Dispose();
    }

    [TestMethod]
    public async Task CreateCompensation_Returns_Created_1()
    {
        // Arrange
        var employee = new Employee()
        {
            EmployeeId = "62c1084e-6e34-4630-93fd-9153afb65309",
            Department = "Engineering",
            FirstName = "Pete",
            LastName = "Best",
            Position = "Developer II",
        };

        var compensation = new Compensation
        {
            EffectiveDate = DateTime.Parse("2024-04-12T00:00:01"),
            Salary = 200000,
            Employee = employee
        };

        var requestContent = new JsonSerialization().ToJson(compensation);

        // Execute
        var postResult = await _httpClient.PostAsync("api/compensation",
            new StringContent(requestContent, Encoding.UTF8, "application/json"));

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, postResult.StatusCode);

        var newCompensation = postResult.DeserializeContent<Compensation>();
        Assert.IsNotNull(newCompensation);
        Assert.AreEqual(employee.FirstName, newCompensation.Employee.FirstName);
        Assert.AreEqual(employee.LastName, newCompensation.Employee.LastName);
        Assert.AreEqual(employee.Department, newCompensation.Employee.Department);
        Assert.AreEqual(employee.Position, newCompensation.Employee.Position);
        
        Assert.AreEqual(compensation.Salary, newCompensation.Salary);
        Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
    }
    
    [TestMethod]
    public async Task CreateCompensation_Returns_Created_2()
    {
        // Arrange
        var employee = new Employee()
        {
            EmployeeId = "62c1084e-6e34-4630-93fd-9153afb65309",
            Department = "Engineering",
            FirstName = "Pete",
            LastName = "Best",
            Position = "Developer II",
        };

        var compensation = new Compensation
        {
            EffectiveDate = DateTime.Parse("2024-04-12T00:00:02"),
            Salary = 300000,
            Employee = employee
        };

        var requestContent = new JsonSerialization().ToJson(compensation);

        // Execute
        var postResult = await _httpClient.PostAsync("api/compensation",
            new StringContent(requestContent, Encoding.UTF8, "application/json"));

        // Assert
        Assert.AreEqual(HttpStatusCode.Created, postResult.StatusCode);

        var newCompensation = postResult.DeserializeContent<Compensation>();
        Assert.IsNotNull(newCompensation);
        Assert.AreEqual(employee.FirstName, newCompensation.Employee.FirstName);
        Assert.AreEqual(employee.LastName, newCompensation.Employee.LastName);
        Assert.AreEqual(employee.Department, newCompensation.Employee.Department);
        Assert.AreEqual(employee.Position, newCompensation.Employee.Position);
        
        Assert.AreEqual(compensation.Salary, newCompensation.Salary);
        Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate);
    }

    [TestMethod]
    public async Task CreateCompensation_Returns_BadRequest()
    {
        // Execute
        var postResult = await _httpClient.PostAsync("api/compensation",
            new StringContent(string.Empty, Encoding.UTF8, "application/json"));

        // Assert
        Assert.AreEqual(HttpStatusCode.BadRequest, postResult.StatusCode);
    }
    
    [TestMethod]
    public async Task GetCompensation_Returns_Ok_1()
    {
        // Arrange
        var employeeId = "62c1084e-6e34-4630-93fd-9153afb65309";

        // Execute
        var getResult = await _httpClient.GetAsync($"api/compensation/{employeeId}");
            
        // Assert
        Assert.AreEqual(HttpStatusCode.OK, getResult.StatusCode);
        var compensation = getResult.DeserializeContent<List<Compensation>>()
            .OrderBy(x => x.Salary)
            .ToList();
        
        // There should be 2 compensations after the above tests
        Assert.AreEqual(employeeId, compensation[0].Employee.EmployeeId);
        Assert.AreEqual(compensation[0].Salary, 200000);
        Assert.AreEqual(compensation[0].EffectiveDate, DateTime.Parse("2024-04-12T00:00:01"));
        Assert.AreEqual(employeeId, compensation[1].Employee.EmployeeId);
        Assert.AreEqual(compensation[1].Salary, 300000);
        Assert.AreEqual(compensation[1].EffectiveDate, DateTime.Parse("2024-04-12T00:00:02"));
    }
}