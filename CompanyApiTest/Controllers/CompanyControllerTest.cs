﻿using System.Collections;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using CompanyApi.Model;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;
using Xunit.Sdk;
using System.Reflection;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest
    {
        [Fact]
        public async void Should_add_new_company_successfully()
        {
            /*
             * 1. Create Application
             * 2. Create HttpClient
             * 3. Prepare request body (serializeToJson, SerializeToHttpContent)
             * 4. Call API
             * 5. Verify status code
             * 6. Verify response body (DeSerializeToObject)
             */
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company(name: "SLB");

            var postBody = SerializeToStringContent(company);

            // when
            var response = await httpClient.PostAsync("/companies", postBody);

            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var createdCompany = await DeserializeObject<Company>(response);
            Assert.Equal("SLB", createdCompany.Name);
            Assert.NotNull(createdCompany.CompanyId);
        }

        [Fact]
        public async void Should_return_409_when_add_exist_company()
        {
            // given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company(name: "SLB");

            var postBody = SerializeToStringContent(company);
            await httpClient.PostAsync("/companies", postBody);

            // when
            var response = await httpClient.PostAsync("/companies", postBody);

            // then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async void Should_get_all_companies_successfully()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var companyList = new List<Company>()
            {
                new Company(name: "SLB"),
                new Company(name: "TW"),
            };

            foreach (var company in companyList)
            {
                var postBody = SerializeToStringContent(company);
                await httpClient.PostAsync("/companies", postBody);
            }

            //when
            var responseList = await httpClient.GetAsync("/companies");

            // then
            responseList.EnsureSuccessStatusCode();
            var allCompanies = DeserializeObject<List<Company>>(responseList).Result;
            Assert.Equal(companyList, allCompanies);
            Assert.Equal(HttpStatusCode.OK, responseList.StatusCode);
        }

        [Fact]
        public async void Should_get_company_by_id_of_system_successfully()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company(name: "SLB");

            var postBody = SerializeToStringContent(company);
            var addCompany = await httpClient.PostAsync("/companies", postBody);

            var companyId = DeserializeObject<Company>(addCompany).Result.CompanyId;

            //when
            var response = await httpClient.GetAsync($"companies/{companyId}");

            // then
            response.EnsureSuccessStatusCode();
            var getCompany = DeserializeObject<Company>(response).Result;
            Assert.Equal(company.Name, getCompany.Name);
        }

        [Fact]
        public async void Should_return_not_found_when_get_no_exist_company_by_id()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var company = new Company(name: "SLB");

            var postBody = SerializeToStringContent(company);
            await httpClient.PostAsync("/companies", postBody);

            //when
            var response = await httpClient.GetAsync("companies/OtherCompany_ID");

            // then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async void Should_obtain_X_page_size_companies_from_index_of_Y()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");
            var companyList = new List<Company>()
            {
                new Company(name: "SLB"),
                new Company(name: "TW"),
                new Company(name: "APPLE"),
                new Company(name: "MICROSOFT"),
            };

            foreach (var company in companyList)
            {
                var postBody = SerializeToStringContent(company);
                await httpClient.PostAsync("/companies", postBody);
            }

            //when
            var responseList = await httpClient.GetAsync("/companies?pageSize=2&&pageIndex=2");

            // then
            var getCompanies = new List<Company>()
            {
                new Company(name: "APPLE"),
                new Company(name: "MICROSOFT"),
            };
            responseList.EnsureSuccessStatusCode();
            var allCompanies = DeserializeObject<List<Company>>(responseList).Result;
            Assert.Equal(getCompanies, allCompanies);
            Assert.Equal(HttpStatusCode.OK, responseList.StatusCode);
        }

        [Fact]
        public async void Should_update_basic_information_of_an_existing_company()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company(name: "SLB");
            var postBody = SerializeToStringContent(company);
            var response = await httpClient.PostAsync("/companies", postBody);
            var originCompany = DeserializeObject<Company>(response).Result;

            company.Name = "slb";
            var modifyPostBody = SerializeToStringContent(company);

            //when
            var companyId = originCompany.CompanyId;
            var modifyResponse = await httpClient.PutAsync($"/companies/{companyId}", modifyPostBody);

            // then
            modifyResponse.EnsureSuccessStatusCode();
            var resultCompany = DeserializeObject<Company>(modifyResponse).Result;
            Assert.Equal(company, resultCompany);
            Assert.Equal(HttpStatusCode.OK, modifyResponse.StatusCode);
        }

        [Fact]
        public async void Should_return_not_found_of_not_existing_company_when_modify()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company(name: "SLB");
            var postBody = SerializeToStringContent(company);
            await httpClient.PostAsync("/companies", postBody);

            //when
            var companyId = "otherCompanyId";
            var modifyResponse = await httpClient.PutAsync($"/companies/{companyId}", postBody);

            // then
            Assert.Equal(HttpStatusCode.NotFound, modifyResponse.StatusCode);
        }

        [Fact]
        public async void Add_an_employee_to_a_specific_company()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company(name: "SLB");
            var response = await httpClient.PostAsync("/companies", SerializeToStringContent(company));
            var companyId = DeserializeObject<Company>(response).Result.CompanyId;

            var employee = new List<Employee>() { new Employee(name: "YZJ", salary: 10), };
            var employeePostBody = SerializeToStringContent(employee);

            //when
            var addEmployeeResponse = await httpClient.PostAsync($"/companies/{companyId}/employees", employeePostBody);

            // then
            addEmployeeResponse.EnsureSuccessStatusCode();
            var addEmployee = DeserializeObject<List<Employee>>(addEmployeeResponse).Result;
            Assert.Equal(HttpStatusCode.OK, addEmployeeResponse.StatusCode);
            Assert.Equal(employee, addEmployee);

            var modifyCompanyMessage = await httpClient.GetAsync($"companies/{companyId}");
            var modifyCompany = DeserializeObject<Company>(modifyCompanyMessage).Result;

            Assert.Single(modifyCompany.Employee);
            Assert.Equal(addEmployee, modifyCompany.Employee);
        }

        [Fact]
        public async void Obtain_employee_list_of_specific_company()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company(name: "SLB");
            var response = await httpClient.PostAsync("/companies", SerializeToStringContent(company));
            var companyId = DeserializeObject<Company>(response).Result.CompanyId;

            var employeeList = new List<Employee>()
            {
                new Employee(name: "YZJ", salary: 10),
                new Employee(name: "LJ", salary: 11),
                new Employee(name: "LWR", salary: 12),
            };
            var employeePostBody = SerializeToStringContent(employeeList);
            await httpClient.PostAsync($"/companies/{companyId}/employees", employeePostBody);

            //when
            var addEmployeeListResponse = await httpClient.GetAsync($"/companies/{companyId}/employees");

            // then
            addEmployeeListResponse.EnsureSuccessStatusCode();

            var addEmployeeList = DeserializeObject<List<Employee>>(addEmployeeListResponse).Result;
            Assert.Equal(HttpStatusCode.OK, addEmployeeListResponse.StatusCode);
            Assert.Equal(employeeList, addEmployeeList);

            var modifyCompanyMessage = await httpClient.GetAsync($"companies/{companyId}");
            var modifyCompany = DeserializeObject<Company>(modifyCompanyMessage).Result;

            Assert.Equal(employeeList, modifyCompany.Employee);
        }

        [Fact]
        public async void Update_basic_information_of_specific_employee_under_specific_company()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company(name: "SLB");
            var response = await httpClient.PostAsync("/companies", SerializeToStringContent(company));
            var companyId = DeserializeObject<Company>(response).Result.CompanyId;

            var employeeList = new List<Employee>()
            {
                new Employee(name: "YZJ", salary: 10),
                new Employee(name: "LJ", salary: 11),
                new Employee(name: "LWR", salary: 12),
            };
            var employeePostBody = SerializeToStringContent(employeeList);
            await httpClient.PostAsync($"/companies/{companyId}/employees", employeePostBody);

            var allEmployeeResponse = await httpClient.GetAsync($"/companies/{companyId}/employees");
            var employees = DeserializeObject<List<Employee>>(allEmployeeResponse).Result;

            employees[0].Name = "OutMan";
            employees[0].Salary = 5;

            var modifyEmployee = SerializeToStringContent(employees[0]);
            var modifyEmployeeMessage = httpClient.PatchAsync(
                $"/companies/{companyId}/employees/{employees[0].EmployeeId}", modifyEmployee).Result;

            var deserializeObjectToEmployee = DeserializeObject<Employee>(modifyEmployeeMessage).Result;

            Assert.Equal(employees[0], deserializeObjectToEmployee);

            var modifyCompanyMessage = await httpClient.GetAsync($"companies/{companyId}");
            var modifyCompany = DeserializeObject<Company>(modifyCompanyMessage).Result;

            Assert.Equal(employees[0], modifyCompany.Employee[0]);
        }

        [Fact]
        public async void Delete_an_employee_of_company()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company(name: "SLB");
            var response = await httpClient.PostAsync("/companies", SerializeToStringContent(company));
            var companyId = DeserializeObject<Company>(response).Result.CompanyId;

            var employeeList = new List<Employee>()
            {
                new Employee(name: "YZJ", salary: 10),
                new Employee(name: "LJ", salary: 11),
                new Employee(name: "LWR", salary: 12),
            };
            var employeePostBody = SerializeToStringContent(employeeList);
            await httpClient.PostAsync($"/companies/{companyId}/employees", employeePostBody);

            var allEmployeeResponse = await httpClient.GetAsync($"/companies/{companyId}/employees");
            var employees = DeserializeObject<List<Employee>>(allEmployeeResponse).Result;

            var modifyEmployee = SerializeToStringContent(employees[0]);
            var deleteEmployeeMessage = httpClient.DeleteAsync(
                $"/companies/{companyId}/employees/{employees[0].EmployeeId}").Result;

            var deserializeObjectToEmployee = DeserializeObject<Employee>(deleteEmployeeMessage).Result;

            Assert.Equal(employees[0], deserializeObjectToEmployee);
        }

        [Fact]
        public async void Should_Delete_company_by_id()
        {
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            await httpClient.DeleteAsync("/companies");

            var company = new Company(name: "SLB");
            var companyResponse = await httpClient.PostAsync("/companies", SerializeToStringContent(company));
            var companyId = DeserializeObject<Company>(companyResponse).Result.CompanyId;

            var employeeList = new List<Employee>()
            {
                new Employee(name: "YZJ", salary: 10),
                new Employee(name: "LJ", salary: 11),
                new Employee(name: "LWR", salary: 12),
            };
            var employeePostBody = SerializeToStringContent(employeeList);
            await httpClient.PostAsync($"/companies/{companyId}", employeePostBody);

            //when
            var response = await httpClient.DeleteAsync($"companies/{companyId}");

            // then
            response.EnsureSuccessStatusCode();
            var getCompany = DeserializeObject<Company>(response).Result;
            Assert.Equal(company, getCompany);
        }

        private static StringContent SerializeToStringContent<T>(T input)
        {
            var inputJson = JsonConvert.SerializeObject(input);
            var postBody = new StringContent(inputJson, Encoding.UTF8, "application/json");
            return postBody;
        }

        private static async Task<T?> DeserializeObject<T>(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            var deserializeObject = JsonConvert.DeserializeObject<T>(responseBody);
            return deserializeObject;
        }
    }
}
