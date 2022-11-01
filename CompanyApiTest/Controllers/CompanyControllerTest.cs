using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace CompanyApi.Controllers
{
    public class CompanyControllerTest
    {
        [Fact]
        public async Task Should_add_new_company_successfully()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            /*
             * Method: POST
             * URI: /opi/addNewPet
             * Body:
             * {
             *  "name: "Kitty",
             *  "type":"cat",
             *  "color":"white",
             *  "price": 1000
             * }
             */
            var company = new Company(name: "ABC");
            var companyJSON = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJSON, Encoding.UTF8, "application/json");

            // when
            var response = await httpClient.PostAsync("/api/companys", postBody);

            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);

            Assert.Equal("ABC", createdCompany.Name);
            Assert.NotEmpty(createdCompany.CompanyID);
        }

        [Fact]
        public async Task Should_get_all_companys()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            /*
             * Method: POST
             * URI: /opi/addNewPet
             * Body:
             * {
             *  "name: "Kitty",
             *  "type":"cat",
             *  "color":"white",
             *  "price": 1000
             * }
             */
            var company = new Company(name: "ABC");
            var companyJSON = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJSON, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/api/companys", postBody);
            // when
            var response = await httpClient.GetAsync("/api/companys");

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<List<Company>>(responseBody);

            Assert.Equal("ABC", createdCompany[0].Name);
            Assert.NotEmpty(createdCompany[0].CompanyID);
        }

        [Fact]
        public async Task Should_get_an_existed_company()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            /*
             * Method: POST
             * URI: /opi/addNewPet
             * Body:
             * {
             *  "name: "Kitty",
             *  "type":"cat",
             *  "color":"white",
             *  "price": 1000
             * }
             */
            var company = new Company(name: "ABC");
            var companyJSON = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJSON, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/api/companys", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            var id = createdCompany.CompanyID;
            // when
            response = await httpClient.GetAsync("/api/companys/" + id);

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            responseBody = await response.Content.ReadAsStringAsync();
            createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);

            Assert.Equal("ABC", createdCompany.Name);
            Assert.NotEmpty(createdCompany.CompanyID);
        }

        [Fact]
        public async Task Should_get_companys_from_2_to_4()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            /*
             * Method: POST
             * URI: /opi/addNewPet
             * Body:
             * {
             *  "name: "Kitty",
             *  "type":"cat",
             *  "color":"white",
             *  "price": 1000
             * }
             */
            var company = new Company(name: "ABC");
            var companyJSON = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJSON, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/api/companys", postBody);
            company = new Company(name: "CBA");
            companyJSON = JsonConvert.SerializeObject(company);
            postBody = new StringContent(companyJSON, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/api/companys", postBody);
            company = new Company(name: "NBA");
            companyJSON = JsonConvert.SerializeObject(company);
            postBody = new StringContent(companyJSON, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/api/companys", postBody);
            company = new Company(name: "NBL");
            companyJSON = JsonConvert.SerializeObject(company);
            postBody = new StringContent(companyJSON, Encoding.UTF8, "application/json");
            await httpClient.PostAsync("/api/companys", postBody);
            // when
            var response = await httpClient.GetAsync("/api/companys/?size=2&index=2");

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<List<Company>>(responseBody);

            Assert.Equal(2, createdCompany.Count);
            Assert.Equal("NBA", createdCompany[0].Name);
            Assert.Equal("NBL", createdCompany[1].Name);
        }

        [Fact]
        public async Task Should_update_an_existed_company()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            /*
             * Method: POST
             * URI: /opi/addNewPet
             * Body:
             * {
             *  "name: "Kitty",
             *  "type":"cat",
             *  "color":"white",
             *  "price": 1000
             * }
             */
            var company = new Company(name: "ABC");
            var companyJSON = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJSON, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/api/companys", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            var id = createdCompany.CompanyID;
            // when
            company = new Company(name: "CBA", companyid: id);
            companyJSON = JsonConvert.SerializeObject(company);
            postBody = new StringContent(companyJSON, Encoding.UTF8, "application/json");
            response = await httpClient.PutAsync("/api/companys/", postBody);

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            responseBody = await response.Content.ReadAsStringAsync();
            createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);

            Assert.Equal("CBA", createdCompany.Name);
            Assert.NotEmpty(createdCompany.CompanyID);
        }

        [Fact]
        public async Task Should_add_an_employee_to_an_existed_company()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            /*
             * Method: POST
             * URI: /opi/addNewPet
             * Body:
             * {
             *  "name: "Kitty",
             *  "type":"cat",
             *  "color":"white",
             *  "price": 1000
             * }
             */
            var company = new Company(name: "ABC");
            var companyJSON = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJSON, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/api/companys", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            var id = createdCompany.CompanyID;
            // when
            var employee = new Employee(name: "Tom");
            var employeeJSON = JsonConvert.SerializeObject(employee);
            postBody = new StringContent(employeeJSON, Encoding.UTF8, "application/json");
            response = await httpClient.PostAsync($"/api/companys/{id}/employees", postBody);

            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            responseBody = await response.Content.ReadAsStringAsync();
            var createdEmployee = JsonConvert.DeserializeObject<Employee>(responseBody);

            Assert.Equal("Tom", createdEmployee.Name);
            Assert.NotEmpty(createdEmployee.EmployeeID);
        }

        [Fact]
        public async Task Should_get_all_employees_from_a_company()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            /*
             * Method: POST
             * URI: /opi/addNewPet
             * Body:
             * {
             *  "name: "Kitty",
             *  "type":"cat",
             *  "color":"white",
             *  "price": 1000
             * }
             */
            var company = new Company(name: "ABC");
            var companyJSON = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJSON, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/api/companys", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            var id = createdCompany.CompanyID;
            // when
            var employee = new Employee(name: "Tom");
            var employeeJSON = JsonConvert.SerializeObject(employee);
            postBody = new StringContent(employeeJSON, Encoding.UTF8, "application/json");
            response = await httpClient.PostAsync($"/api/companys/{id}/employees", postBody);
            employee = new Employee(name: "Jerry");
            employeeJSON = JsonConvert.SerializeObject(employee);
            postBody = new StringContent(employeeJSON, Encoding.UTF8, "application/json");
            response = await httpClient.PostAsync($"/api/companys/{id}/employees", postBody);
            response = await httpClient.GetAsync($"/api/companys/{id}/employees");

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            responseBody = await response.Content.ReadAsStringAsync();
            var createdEmployee = JsonConvert.DeserializeObject<List<Employee>>(responseBody);

            Assert.Equal(2, createdEmployee.Count);
            Assert.Equal("Tom", createdEmployee[0].Name);
            Assert.Equal("Jerry", createdEmployee[1].Name);
        }

        [Fact]
        public async Task Should_update_a_employee_from_a_company()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            /*
             * Method: POST
             * URI: /opi/addNewPet
             * Body:
             * {
             *  "name: "Kitty",
             *  "type":"cat",
             *  "color":"white",
             *  "price": 1000
             * }
             */
            var company = new Company(name: "ABC");
            var companyJSON = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJSON, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/api/companys", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            var id = createdCompany.CompanyID;
            // when
            var employee = new Employee(name: "Tom");
            var employeeJSON = JsonConvert.SerializeObject(employee);
            postBody = new StringContent(employeeJSON, Encoding.UTF8, "application/json");
            response = await httpClient.PostAsync($"/api/companys/{id}/employees", postBody);
            responseBody = await response.Content.ReadAsStringAsync();
            var createdEmployee = JsonConvert.DeserializeObject<Employee>(responseBody);
            var employeeid = createdEmployee.EmployeeID;
            employee = new Employee(name: "Jerry", employeeid: employeeid);
            employeeJSON = JsonConvert.SerializeObject(employee);
            postBody = new StringContent(employeeJSON, Encoding.UTF8, "application/json");
            response = await httpClient.PutAsync($"/api/companys/{id}/employees", postBody);

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            responseBody = await response.Content.ReadAsStringAsync();
            createdEmployee = JsonConvert.DeserializeObject<Employee>(responseBody);

            Assert.Equal("Jerry", createdEmployee.Name);
        }

        [Fact]
        public async Task Should_delete_a_employee_from_a_company()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            /*
             * Method: POST
             * URI: /opi/addNewPet
             * Body:
             * {
             *  "name: "Kitty",
             *  "type":"cat",
             *  "color":"white",
             *  "price": 1000
             * }
             */
            var company = new Company(name: "ABC");
            var companyJSON = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJSON, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync("/api/companys", postBody);
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            var id = createdCompany.CompanyID;
            // when
            var employee = new Employee(name: "Tom");
            var employeeJSON = JsonConvert.SerializeObject(employee);
            postBody = new StringContent(employeeJSON, Encoding.UTF8, "application/json");
            response = await httpClient.PostAsync($"/api/companys/{id}/employees", postBody);
            responseBody = await response.Content.ReadAsStringAsync();
            var createdEmployee = JsonConvert.DeserializeObject<Employee>(responseBody);
            var employeeid = createdEmployee.EmployeeID;
            response = await httpClient.DeleteAsync($"/api/companys/{id}/employees/{employeeid}");

            // then
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}
