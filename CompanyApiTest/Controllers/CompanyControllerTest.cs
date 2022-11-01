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
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, mediaType: "application/json");
            // when
            var response = await httpClient.PostAsync("/companies", postBody);
            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            // then
            Assert.Equal("SLB", createdCompany.Name);
            Assert.NotEmpty(createdCompany.CompanyID);
        }

        [Fact]
        public async Task Should_return_conflict_when_add_new_company_given_exist_same_name_company()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company(name: "SLB");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, mediaType: "application/json");
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
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            foreach (var company in companies)
            {
                var companyJson = JsonConvert.SerializeObject(company);
                var postBody = new StringContent(companyJson, Encoding.UTF8, mediaType: "application/json");
                _ = await httpClient.PostAsync("/companies", postBody);
            }

            // when
            var response = await httpClient.GetAsync("/companies");
            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var allCompanies = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            Assert.Equal(2, allCompanies.Count);
        }

        [Fact]
        public async Task Should_return_company_successfully_given_company_Id()
        {
            // given
            List<Company> companies = new List<Company>() { new Company(name: "SLB"), new Company(name: "Apple") };
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var targetId = string.Empty;
            foreach (var company in companies)
            {
                var companyJson = JsonConvert.SerializeObject(company);
                var postBody = new StringContent(companyJson, Encoding.UTF8, mediaType: "application/json");
                var response = await httpClient.PostAsync("/companies", postBody);
                var responseBody = await response.Content.ReadAsStringAsync();
                var responseCompany = JsonConvert.DeserializeObject<Company>(responseBody);
                targetId = responseCompany.CompanyID;
            }

            // when
            var targetResponse = await httpClient.GetAsync($"/companies/{targetId}");
            // then
            Assert.Equal(HttpStatusCode.OK, targetResponse.StatusCode);
            var targetResponseBody = await targetResponse.Content.ReadAsStringAsync();
            var targetCompany = JsonConvert.DeserializeObject<Company>(targetResponseBody);
            Assert.Equal("Apple", targetCompany.Name);
        }

        [Fact]
        public async Task Should_return_companies_successfully_given_page_size_and_index()
        {
            // given
            Company company = new Company(name: "SLB");
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, mediaType: "application/json");
            _ = await httpClient.PostAsync("/companies", postBody);

            // when
            var targetResponse = await httpClient.GetAsync($"/companies?pageSize=2&&pageIndex=1");
            // then
            Assert.Equal(HttpStatusCode.OK, targetResponse.StatusCode);
            var targetResponseBody = await targetResponse.Content.ReadAsStringAsync();
            var targetCompanies = JsonConvert.DeserializeObject<List<Company>>(targetResponseBody);
            Assert.Equal("SLB", targetCompanies[0].Name);
        }

        [Fact]
        public async Task Should_return_updated_company_successfully()
        {
            // given
            Company company = new Company(name: "SLB");
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, mediaType: "application/json");
            var response = await httpClient.PostAsync("/companies", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseCompany = JsonConvert.DeserializeObject<Company>(responseBody).CompanyID;
            company.CompanyID = responseCompany;
            company.Name = "slb";
            var editedCompanyJson = JsonConvert.SerializeObject(company);
            var editedPostBody = new StringContent(editedCompanyJson, Encoding.UTF8, mediaType: "application/json");

            // when
            var targetResponse = await httpClient.PutAsync($"/companies/{responseCompany}", editedPostBody);
            // then
            Assert.Equal(HttpStatusCode.OK, targetResponse.StatusCode);
            var targetResponseBody = await targetResponse.Content.ReadAsStringAsync();
            var targetCompany = JsonConvert.DeserializeObject<Company>(targetResponseBody);
            Assert.Equal("slb", targetCompany.Name);
        }

        [Fact]
        public async Task Should_return_added_employee_to_company_successfully()
        {
            //given
            Company company = new Company(name: "SLB");
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, mediaType: "application/json");
            var response = await httpClient.PostAsync("/companies", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Employee employee = new Employee(name: "Winnie");
            var employeeJson = JsonConvert.SerializeObject(employee);
            var employeePostBody = new StringContent(employeeJson, Encoding.UTF8, mediaType: "application/json");

           // when
            var targetResponse = await httpClient.PostAsync($"/companies/{responseCompany.CompanyID}/employees", employeePostBody);
            //then
            Assert.Equal(HttpStatusCode.OK, targetResponse.StatusCode);
            var targetResponseBody = await targetResponse.Content.ReadAsStringAsync();
            var targetEmployee = JsonConvert.DeserializeObject<Employee>(targetResponseBody);
            Assert.Equal("Winnie", targetEmployee.Name);
           // Assert.Equal(1, responseCompany.Employees.Count);
        }

        [Fact]
        public async Task Should_return_employees_of_company_successfully()
        {
            // given
            Company company = new Company(name: "SLB");
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, mediaType: "application/json");
            var response = await httpClient.PostAsync("/companies", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            List<Employee> employees = new List<Employee>() { new Employee(name: "Winnie"), new Employee(name: "tony") };
            foreach (var employee in employees)
            {
                var employeeJson = JsonConvert.SerializeObject(employee);
                var employeePostBody = new StringContent(employeeJson, Encoding.UTF8, mediaType: "application/json");
                _ = await httpClient.PostAsync($"/companies/{responseCompany.CompanyID}/employees", employeePostBody);
            }

            // when
            var targetResponse = await httpClient.GetAsync($"/companies/{responseCompany.CompanyID}/employees");
            // then
            Assert.Equal(HttpStatusCode.OK, targetResponse.StatusCode);
            var targetResponseBody = await targetResponse.Content.ReadAsStringAsync();
            var targetEmployees = JsonConvert.DeserializeObject<List<Employee>>(targetResponseBody);
            Assert.Equal(2, targetEmployees.Count);
        }

        [Fact]
        public async Task Should_return_updated_employee_of_a_company_successfully()
        {
            // given
            Company company = new Company(name: "SLB");
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, mediaType: "application/json");
            var response = await httpClient.PostAsync("/companies", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Employee employee = new Employee(name: "Winnie");
            var employeeJson = JsonConvert.SerializeObject(employee);
            var employeePostBody = new StringContent(employeeJson, Encoding.UTF8, mediaType: "application/json");
            var addEmployeeResponse = await httpClient.PostAsync($"/companies/{responseCompany.CompanyID}/employees", employeePostBody);
            var employeeResponseBody = await addEmployeeResponse.Content.ReadAsStringAsync();
            var responseEmployee = JsonConvert.DeserializeObject<Employee>(employeeResponseBody);
            employee.Salary = 1000;
            var editedEmployeeJson = JsonConvert.SerializeObject(employee);
            var editedPostBody = new StringContent(editedEmployeeJson, Encoding.UTF8, mediaType: "application/json");
            // when
            var targetResponse = await httpClient.PutAsync($"/companies/{responseCompany.CompanyID}/employees/{responseEmployee.EmployeeID}", editedPostBody);
            // then
            Assert.Equal(HttpStatusCode.OK, targetResponse.StatusCode);
            var targetResponseBody = await targetResponse.Content.ReadAsStringAsync();
            var targetEmployee = JsonConvert.DeserializeObject<Employee>(targetResponseBody);
            Assert.Equal(1000, targetEmployee.Salary);
        }

        [Fact]
        public async Task Should_delete_employee_of_a_company_successfully()
        {
            // given
            Company company = new Company(name: "SLB");
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, mediaType: "application/json");
            var response = await httpClient.PostAsync("/companies", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Employee employee = new Employee(name: "Winnie");
            var employeeJson = JsonConvert.SerializeObject(employee);
            var employeePostBody = new StringContent(employeeJson, Encoding.UTF8, mediaType: "application/json");
            var addEmployeeResponse = await httpClient.PostAsync($"/companies/{responseCompany.CompanyID}/employees", employeePostBody);
            var employeeResponseBody = await addEmployeeResponse.Content.ReadAsStringAsync();
            var responseEmployee = JsonConvert.DeserializeObject<Employee>(employeeResponseBody);
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
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, mediaType: "application/json");
            var response = await httpClient.PostAsync("/companies", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var responseCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Employee employee = new Employee(name: "Winnie");
            var employeeJson = JsonConvert.SerializeObject(employee);
            var employeePostBody = new StringContent(employeeJson, Encoding.UTF8, mediaType: "application/json");
            _ = await httpClient.PostAsync($"/companies/{responseCompany.CompanyID}/employees", employeePostBody);
            // when
            var targetResponse = await httpClient.DeleteAsync($"/companies/{responseCompany.CompanyID}");
            // then
            Assert.Equal(HttpStatusCode.NoContent, targetResponse.StatusCode);
        }
    }
}
