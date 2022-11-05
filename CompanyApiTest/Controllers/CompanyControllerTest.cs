using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CompanyApi.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest
    {
        /*
         * 1.create application
         * 2. create client
         * 3.prepare request body
         * 4. call api
         * 5. verify status code
         * 6. verify response body
         */
        [Fact]
        public async Task Should_add_new_company_successfully()
        {
            // given
            HttpClient httpClient = await CreateApp();
            var company = new Company(name: "SLB");
            StringContent postBody = Serialize(company);
            // when
            var response = await httpClient.PostAsync("/companies", postBody);
            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Company createdCompany = await Deserialize<Company>(response);
            // then
            Assert.Equal("SLB", createdCompany.Name);
            Assert.NotEmpty(createdCompany.CompanyID);
        }

        [Fact]
        public async Task Should_return_conflict_when_add_new_company_given_exist_same_name_company()
        {
            // given
            HttpClient httpClient = await CreateApp();
            var company = new Company(name: "SLB");
            StringContent postBody = Serialize(company);
            _ = await httpClient.PostAsync("/companies", postBody);
            // when
            var response = await httpClient.PostAsync("/companies", postBody);
            // then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Should_return_all_copanies_successfully()
        {
            // given
            List<Company> companies = new List<Company>() { new Company(name: "SLB"), new Company(name: "Apple") };
            HttpClient httpClient = await CreateApp();
            foreach (var company in companies)
            {
                StringContent postBody = Serialize(company);
                _ = await httpClient.PostAsync("/companies", postBody);
            }

            // when
            var response = await httpClient.GetAsync("/companies");
            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var allCompanies = await Deserialize<List<Company>>(response);
            Assert.Equal(2, allCompanies.Count);
        }

        [Fact]
        public async Task Should_return_company_successfully_given_company_Id()
        {
            // given
            List<Company> companies = new List<Company>() { new Company(name: "SLB"), new Company(name: "Apple") };
            HttpClient httpClient = await CreateApp();
            var targetId = string.Empty;
            foreach (var company in companies)
            {
                StringContent postBody = Serialize(company);
                var response = await httpClient.PostAsync("/companies", postBody);
                var responseCompany = await Deserialize<Company>(response);
                targetId = responseCompany.CompanyID;
            }

            // when
            var targetResponse = await httpClient.GetAsync($"/companies/{targetId}");
            // then
            Assert.Equal(HttpStatusCode.OK, targetResponse.StatusCode);
            var targetCompany = await Deserialize<Company>(targetResponse);
            Assert.Equal("Apple", targetCompany.Name);
        }

        [Fact]
        public async Task Should_return_companies_successfully_given_page_size_and_index()
        {
            // given
            Company company = new Company(name: "SLB");
            HttpClient httpClient = await CreateApp();
            StringContent postBody = Serialize(company);
            _ = await httpClient.PostAsync("/companies", postBody);

            // when
            var targetResponse = await httpClient.GetAsync($"/companies?pageSize=2&&pageIndex=1");
            // then
            Assert.Equal(HttpStatusCode.OK, targetResponse.StatusCode);
            var targetCompanies = await Deserialize<List<Company>>(targetResponse);
            Assert.Equal("SLB", targetCompanies[0].Name);
        }

        [Fact]
        public async Task Should_return_updated_company_successfully()
        {
            // given
            Company company = new Company(name: "SLB");
            HttpClient httpClient = await CreateApp();
            StringContent postBody = Serialize(company);
            var response = await httpClient.PostAsync("/companies", postBody);
            var responseCompany = await Deserialize<Company>(response);
            company.CompanyID = responseCompany.CompanyID;
            company.Name = "slb";
            var editedPostBody = Serialize(company);

            // when
            var targetResponse = await httpClient.PutAsync($"/companies/{responseCompany.CompanyID}", editedPostBody);
            // then
            Assert.Equal(HttpStatusCode.OK, targetResponse.StatusCode);
            var targetCompany = await Deserialize<Company>(targetResponse);
            Assert.Equal("slb", targetCompany.Name);
        }

        [Fact]
        public async Task Should_return_added_employee_to_company_successfully()
        {
            //given
            Company company = new Company(name: "SLB");
            HttpClient httpClient = await CreateApp();
            StringContent postBody = Serialize(company);
            var response = await httpClient.PostAsync("/companies", postBody);
            var responseCompany = await Deserialize<Company>(response);
            Employee employee = new Employee(name: "Winnie");
            StringContent employeePostBody = Serialize(employee);

            // when
            var targetResponse = await httpClient.PostAsync($"/companies/{responseCompany.CompanyID}/employees", employeePostBody);
            //then
            Assert.Equal(HttpStatusCode.OK, targetResponse.StatusCode);
            var targetEmployee = await Deserialize<Employee>(targetResponse);
            Assert.Equal("Winnie", targetEmployee.Name);
            var targetCompany = await httpClient.GetAsync($"/companies/{responseCompany.CompanyID}");
            var targetCompanyR = await Deserialize<Company>(targetCompany);
            Assert.Equal(1, targetCompanyR.Employees.Count);
        }

        [Fact]
        public async Task Should_return_employees_of_company_successfully()
        {
            // given
            Company company = new Company(name: "SLB");
            HttpClient httpClient = await CreateApp();
            StringContent postBody = Serialize(company);
            var response = await httpClient.PostAsync("/companies", postBody);
            var responseCompany = await Deserialize<Company>(response);
            List<Employee> employees = new List<Employee>() { new Employee(name: "Winnie"), new Employee(name: "tony") };
            foreach (var employee in employees)
            {
                StringContent employeePostBody = Serialize(employee);
                _ = await httpClient.PostAsync($"/companies/{responseCompany.CompanyID}/employees", employeePostBody);
            }

            // when
            var targetResponse = await httpClient.GetAsync($"/companies/{responseCompany.CompanyID}/employees");
            // then
            Assert.Equal(HttpStatusCode.OK, targetResponse.StatusCode);
            var targetEmployees = await Deserialize<List<Employee>>(targetResponse);
            Assert.Equal(2, targetEmployees.Count);
        }

        [Fact]
        public async Task Should_return_updated_employee_of_a_company_successfully()
        {
            // given
            Company company = new Company(name: "SLB");
            HttpClient httpClient = await CreateApp();
            var postBody = Serialize(company);
            var response = await httpClient.PostAsync("/companies", postBody);
            var responseCompany = await Deserialize<Company>(response);
            Employee employee = new Employee(name: "Winnie");
            var employeePostBody = Serialize(employee);
            var addEmployeeResponse = await httpClient.PostAsync($"/companies/{responseCompany.CompanyID}/employees", employeePostBody);
            var responseEmployee = await Deserialize<Employee>(addEmployeeResponse);
            employee.Salary = 1000;
            var editedPostBody = Serialize(employee);
            // when
            var targetResponse = await httpClient.PutAsync($"/companies/{responseCompany.CompanyID}/employees/{responseEmployee.EmployeeID}", editedPostBody);
            // then
            Assert.Equal(HttpStatusCode.OK, targetResponse.StatusCode);
            var targetEmployee = await Deserialize<Employee>(targetResponse);
            Assert.Equal(1000, targetEmployee.Salary);
        }

        [Fact]
        public async Task Should_delete_employee_of_a_company_successfully()
        {
            // given
            Company company = new Company(name: "SLB");
            HttpClient httpClient = await CreateApp();
            var postBody = Serialize(company);
            var response = await httpClient.PostAsync("/companies", postBody);
            var responseCompany = await Deserialize<Company>(response);
            Employee employee = new Employee(name: "Winnie");
            var employeePostBody = Serialize(employee);
            var addEmployeeResponse = await httpClient.PostAsync($"/companies/{responseCompany.CompanyID}/employees", employeePostBody);
            var responseEmployee = await Deserialize<Employee>(addEmployeeResponse);
            // when
            var targetResponse = await httpClient.DeleteAsync($"/companies/{responseCompany.CompanyID}/employees/{responseEmployee.EmployeeID}");
            // then
            Assert.Equal(HttpStatusCode.NoContent, targetResponse.StatusCode);
            Assert.Equal(0, company.Employees.Count);
        }

        [Fact]
        public async Task Should_delete_a_company_successfully()
        {
            // given
            Company company = new Company(name: "SLB");
            HttpClient httpClient = await CreateApp();
            var postBody = Serialize(company);
            var response = await httpClient.PostAsync("/companies", postBody);
            var responseCompany = await Deserialize<Company>(response);
            Employee employee = new Employee(name: "Winnie");
            var employeePostBody = Serialize(employee);
            _ = await httpClient.PostAsync($"/companies/{responseCompany.CompanyID}/employees", employeePostBody);
            // when
            var targetResponse = await httpClient.DeleteAsync($"/companies/{responseCompany.CompanyID}");
            // then
            Assert.Equal(HttpStatusCode.NoContent, targetResponse.StatusCode);
        }

        private static async Task<T> Deserialize<T>(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<T>(responseBody);
            return createdCompany;
        }

        private static StringContent Serialize<T>(T target)
        {
            var companyJson = JsonConvert.SerializeObject(target);
            var postBody = new StringContent(companyJson, Encoding.UTF8, mediaType: "application/json");
            return postBody;
        }

        private static async Task<HttpClient> CreateApp()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            return httpClient;
        }
    }
}
