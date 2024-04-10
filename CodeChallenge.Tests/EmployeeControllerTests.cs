
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
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
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
        }
        
        // NOTE: I would normally break up these unit tests to test the controller and services separate.
        // So the service tests really test the different logical scenarios and the controller tests are for the 
        // correct responses. To keep consistent and since TestServer is already there for me, I stuck with just this 
        // test class for the purpose of the challenge and since these are in a project name that mentions integration
        // tests.
        [TestMethod]
        public async Task GetReportingStructure_Returns_Ok_1()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";

            // Execute
            var getResult = await _httpClient.GetAsync($"api/employee/{employeeId}/reporting-structure");
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, getResult.StatusCode);
            var reportingStructure = getResult.DeserializeContent<ReportingStructure>();

            Assert.AreEqual(employeeId, reportingStructure.Employee.EmployeeId);
            Assert.AreEqual(reportingStructure.NumberOfReports, 4);
        }
        
        [TestMethod]
        public async Task GetReportingStructure_Returns_Ok_2()
        {
            // Arrange
            var employeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f";

            // Execute
            var getResult = await _httpClient.GetAsync($"api/employee/{employeeId}/reporting-structure");
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, getResult.StatusCode);
            var reportingStructure = getResult.DeserializeContent<ReportingStructure>();

            Assert.AreEqual(employeeId, reportingStructure.Employee.EmployeeId);
            Assert.AreEqual(reportingStructure.NumberOfReports, 2);
        }
        
        [TestMethod]
        public async Task GetReportingStructure_Returns_Ok_3()
        {
            // Arrange
            var employeeId = "62c1084e-6e34-4630-93fd-9153afb65309";

            // Execute
            var getResult = await _httpClient.GetAsync($"api/employee/{employeeId}/reporting-structure");
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, getResult.StatusCode);
            var reportingStructure = getResult.DeserializeContent<ReportingStructure>();

            Assert.AreEqual(employeeId, reportingStructure.Employee.EmployeeId);
            Assert.AreEqual(reportingStructure.NumberOfReports, 0);
        }
        
        [TestMethod]
        public async Task GetReportingStructure_Returns_NotFound_BadId()
        {
            // Arrange
            var employeeId = "badId:(";

            // Execute
            var getResult = await _httpClient.GetAsync($"api/employee/{employeeId}/reporting-structure");
            
            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, getResult.StatusCode);
        }
        
        [TestMethod]
        public async Task GetReportingStructure_Returns_NotFound_EmptyString()
        {
            // Arrange
            var employeeId = string.Empty;

            // Execute
            var getResult = await _httpClient.GetAsync($"api/employee/{employeeId}/reporting-structure");
            
            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, getResult.StatusCode);
        }
        
        [TestMethod]
        public async Task GetReportingStructure_Returns_NotFound_NullString()
        {
            // Arrange
            string employeeId = null;

            // Execute
            var getResult = await _httpClient.GetAsync($"api/employee/{employeeId}/reporting-structure");
            
            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, getResult.StatusCode);
        }
        
        // Moving these update tests below my tests so I don't have to update Pete Best back to having 2 direct reports
        
        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }
        
        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
