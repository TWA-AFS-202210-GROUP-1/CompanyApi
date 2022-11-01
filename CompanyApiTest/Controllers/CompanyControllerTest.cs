using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using CompanyApi.Dto;
using CompanyApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest: IDisposable
    {
        private readonly HttpClient _httpClient;
        public CompanyControllerTest()
        {
            var application = new WebApplicationFactory<Program>();
            _httpClient = application.CreateClient();
        }

        public void Dispose()
        {
            _httpClient.DeleteAsync("/companies");
        }

        [Fact]
        public async Task Should_return_created_company_with_when_add_successfully_given_a_not_exist_company_name()
        {
            // given
            var company = new Company("SLB");

            // when
            var response = await _httpClient.PostAsJsonAsync("/companies", company);
            var responseString = await response.Content.ReadAsStringAsync();

            var createdCompany = JsonConvert.DeserializeObject<Company>(responseString);

            // then
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotNull(createdCompany.Id);
        }

        [Fact]
        public async Task Should_return_conflict_with_when_add_company_given_a_exist_company_name()
        {
            // given
            var company = new Company("SLB");
            await _httpClient.PostAsJsonAsync("/companies", company);

            // when
            var response = await _httpClient.PostAsJsonAsync("/companies", company);

            // then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Should_return_ok_when_get_all_company()
        {
            // given
            var company01 = new Company("SLB");
            var company02 = new Company("TW");
            await _httpClient.PostAsJsonAsync("/companies", company01);
            await _httpClient.PostAsJsonAsync("/companies", company02);

            // when
            var response = await _httpClient.GetAsync("/companies");
            var responseString = await response.Content.ReadAsStringAsync();

            var allCompanies = JsonConvert.DeserializeObject<IList<Company>>(responseString);

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(2, allCompanies.Count);
        }

        [Fact]
        public async Task Should_return_ok_when_get_an_company_given_an_exist_company_id()
        {
            // given
            var company = new Company("SLB");
            var createdCompanyResponse = await _httpClient.PostAsJsonAsync("/companies", company);
            var createdCompanyString = await createdCompanyResponse.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(createdCompanyString);

            // when
            var response = await _httpClient.GetAsync($"/companies/{createdCompany.Id}");
            var responseString = await response.Content.ReadAsStringAsync();

            var slb = JsonConvert.DeserializeObject<Company>(responseString);

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("SLB", slb.Name);
        }

        [Fact]
        public async Task Should_return_ok_when_get_an_2_company_given_page_size_and_index_from()
        {
            // given
            var companies = new List<Company>()
            {
                new Company("SLB"),
                new Company("TW"),
                new Company("Facebook"),
                new Company("Google"),
                new Company("Microsoft"),
            };
            foreach (var company in companies)
            {
                await _httpClient.PostAsJsonAsync("/companies", company);
            }

            // when
            var response = await _httpClient.GetAsync("/companies?pageSize=2&index=2");
            var responseString = await response.Content.ReadAsStringAsync();

            var companiesResponse = JsonConvert.DeserializeObject<IList<Company>>(responseString);

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(2, companiesResponse.Count);
            Assert.Equal("Facebook", companiesResponse[0].Name);
        }

        [Fact]
        public async Task Should_return_updated_company_when_update_company_give_update_information()
        {
            // given
            var company = new Company("SLB");
            var createdCompanyResponse = await _httpClient.PostAsJsonAsync("/companies", company);
            var createdCompanyString = await createdCompanyResponse.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(createdCompanyString);
            var companyUpdate = new UpdateCompanyDto() { Name = "SLB-2" };

            // when
            var response = await _httpClient.PutAsJsonAsync($"/companies/{createdCompany.Id}", companyUpdate);
            var responseString = await response.Content.ReadAsStringAsync();

            var companyResponse = JsonConvert.DeserializeObject<Company>(responseString);

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("SLB-2", companyResponse.Name);
        }

        [Fact]
        public async Task Should_create_employee_for_company_when_create_successfully_given_a_employee_and_an_exist_company()
        {
            // given
            var company = new Company("SLB");
            var companyResponse = await _httpClient.PostAsJsonAsync("/companies", company);
            var companyResponseString = await companyResponse.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(companyResponseString);
            var companyId = createdCompany.Id;
            var employee = new Employee("Xu", 100);
            // when

            var employeeResponse = await _httpClient.PostAsJsonAsync($"/companies/{companyId}/employees", employee);
            var employeeResponseString = await companyResponse.Content.ReadAsStringAsync();
            var createdEmployee = JsonConvert.DeserializeObject<Employee>(employeeResponseString);

            // then
            employeeResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, employeeResponse.StatusCode);
            Assert.NotNull(createdEmployee.Id);
        }

        [Fact]
        public async Task Should_get_all_employees_for_company_when_create_successfully_given_an_exist_company()
        {
            // given
            var company = new Company("SLB");
            var companyResponse = await _httpClient.PostAsJsonAsync("/companies", company);
            var companyResponseString = await companyResponse.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(companyResponseString);
            var companyId = createdCompany.Id;
            var employee = new Employee("Xu", 100);
            await _httpClient.PostAsJsonAsync($"/companies/{companyId}/employees", employee);
            // when

            var allEmployeesResponse = await _httpClient.GetAsync($"/companies/{companyId}/employees");
            var allEmployeesResponseString = await allEmployeesResponse.Content.ReadAsStringAsync();
            var allEmployees = JsonConvert.DeserializeObject<IList<Employee>>(allEmployeesResponseString);

            // then
            allEmployeesResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, allEmployeesResponse.StatusCode);
            Assert.Equal(1, allEmployees.Count);
            Assert.Equal("Xu", allEmployees[0].Name);
        }

        [Fact]
        public async Task Should_return_updated_employee_for_company_when_update_successfully_given_an_update_info()
        {
            // given
            var company = new Company("SLB");
            var companyResponse = await _httpClient.PostAsJsonAsync("/companies", company);
            var companyResponseString = await companyResponse.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(companyResponseString);
            var companyId = createdCompany.Id;
            var employee = new Employee("Xu", 100);
            var createdEmployeeResponse = await _httpClient.PostAsJsonAsync($"/companies/{companyId}/employees", employee);
            var createdEmployeeResponseString = await createdEmployeeResponse.Content.ReadAsStringAsync();
            var createdEmployee = JsonConvert.DeserializeObject<Employee>(createdEmployeeResponseString);
            var updateEmployeeInfo = new UpdateEmployeeDto() { Name = "Du", Salary = 50 };
            // when

            var updateEmployeeResponse = await _httpClient.PutAsJsonAsync($"/companies/{companyId}/employees/{createdEmployee.Id}", updateEmployeeInfo);
            var updateEmployeeResponseString = await updateEmployeeResponse.Content.ReadAsStringAsync();
            var updateEmployee = JsonConvert.DeserializeObject<Employee>(updateEmployeeResponseString);

            // then
            updateEmployeeResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, updateEmployeeResponse.StatusCode);
            Assert.Equal("Du", updateEmployee.Name);
            Assert.Equal(50, updateEmployee.Salary);
        }

        [Fact]
        public async Task Should_return_no_content_when_delete_employee_successfully_given_an_existed_ids()
        {
            // given
            var company = new Company("SLB");
            var companyResponse = await _httpClient.PostAsJsonAsync("/companies", company);
            var companyResponseString = await companyResponse.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(companyResponseString);
            var companyId = createdCompany.Id;
            var employee = new Employee("Xu", 100);
            var createdEmployeeResponse = await _httpClient.PostAsJsonAsync($"/companies/{companyId}/employees", employee);
            var createdEmployeeResponseString = await createdEmployeeResponse.Content.ReadAsStringAsync();
            var createdEmployee = JsonConvert.DeserializeObject<Employee>(createdEmployeeResponseString);
            // when

            var deleteEmployeeResponse = await _httpClient.DeleteAsync($"/companies/{companyId}/employees/{createdEmployee.Id}");
            var reGetDeletedEmployeeResponse = await _httpClient.GetAsync($"/companies/{companyId}/employees/{createdEmployee.Id}");

            // then
            deleteEmployeeResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, deleteEmployeeResponse.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, reGetDeletedEmployeeResponse.StatusCode);
        }

        [Fact]
        public async Task Should_return_no_content_when_delete_company_successfully_given_an_existed_id()
        {
            // given
            var company = new Company("SLB");
            var companyResponse = await _httpClient.PostAsJsonAsync("/companies", company);
            var companyResponseString = await companyResponse.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(companyResponseString);
            var companyId = createdCompany.Id;
            var employee = new Employee("Xu", 100);
            var createdEmployeeResponse = await _httpClient.PostAsJsonAsync($"/companies/{companyId}/employees", employee);
            var createdEmployeeResponseString = await createdEmployeeResponse.Content.ReadAsStringAsync();
            var createdEmployee = JsonConvert.DeserializeObject<Employee>(createdEmployeeResponseString);
            // when

            var deleteEmployeeResponse = await _httpClient.DeleteAsync($"/companies/{companyId}");
            var reGetDeletedCompanyResponse = await _httpClient.GetAsync($"/companies/{companyId}");
            var reGetEmployeeInDeletedCompanyResponse = await _httpClient.GetAsync($"/companies/{companyId}/employees/{createdEmployee.Id}");

            // then
            deleteEmployeeResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.NoContent, deleteEmployeeResponse.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, reGetDeletedCompanyResponse.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, reGetEmployeeInDeletedCompanyResponse.StatusCode);
        }
    }
}
