using CompanyApi.Model;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace CompanyApiTest.Controllers
{
  public class CompanyControllerTest
  {
    [Fact]
    public async void Should_add_new_company_successfully()
    {
      // given
      var httpClient = await InitializeHttpClient();
      var company = new Company("SLB");
      var requestBody = GenerateRequestBody(company);
      // when
      var response = await PostRequestBody(httpClient, "/companies", requestBody);
      // then
      Assert.Equal(HttpStatusCode.Created, response.StatusCode);
      var returnedCompany = await DeserializeResponse<Company>(response);
      Assert.NotNull(returnedCompany);
      Assert.Equal("SLB", returnedCompany.Name);
      Assert.NotEmpty(returnedCompany.CompanyId);
    }

    [Fact]
    public async void Should_not_add_new_company_if_exists_in_the_system()
    {
      // given
      var httpClient = await InitializeHttpClient();
      var company = new Company("SLB");
      var requestBody = GenerateRequestBody(company);
      await PostRequestBody(httpClient, "/companies", requestBody);

      var newCompany = new Company("SLB");
      var newRequestBody = GenerateRequestBody(newCompany);
      // when
      var response = await PostRequestBody(httpClient, "/companies", newRequestBody);
      // then
      Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async void Should_get_all_companies_from_system()
    {
      // given
      var httpClient = await InitializeHttpClient();
      var companies = new List<Company>
      {
        new Company("SLB"),
        new Company("TW"),
      };
      var requestBodyList = GenerateRequestBodyList(companies);
      await PostRequestBodyList(httpClient, "/companies", requestBodyList);

      // when
      var response = await httpClient.GetAsync("/companies");
      // then
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
      var returnedCompanies = await DeserializeResponse<List<Company>>(response);
      Assert.NotNull(returnedCompanies);
      foreach (var company in returnedCompanies)
      {
        Assert.NotEmpty(company.CompanyId);
      }
    }

    [Fact]
    public async void Should_get_company_by_id_from_system()
    {
      // given
      var httpClient = await InitializeHttpClient();
      var companies = new List<Company>
      {
        new Company("SLB"),
        new Company("TW"),
      };
      var requestBodyList = GenerateRequestBodyList(companies);
      await PostRequestBodyList(httpClient, "/companies", requestBodyList);

      // when
      var response = await httpClient.GetAsync($"/companies/{companies[0].CompanyId}");
      // then
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
      var returnedCompany = await DeserializeResponse<Company>(response);
      Assert.NotNull(returnedCompany);
      Assert.Equal(companies[0].Name, returnedCompany.Name);
    }

    [Fact]
    public async void Should_get_2_companies_from_page_3()
    {
      // given
      var httpClient = await InitializeHttpClient();
      var companies = new List<Company>
      {
        new Company("SLB"),
        new Company("TW"),
        new Company("Baidu"),
        new Company("Tencent"),
        new Company("Microsoft"),
        new Company("MacroHard"),
        new Company("Vestas"),
        new Company("Siemens"),
      };
      var requestBodyList = GenerateRequestBodyList(companies);
      await PostRequestBodyList(httpClient, "/companies", requestBodyList);

      // when
      var response = await httpClient.GetAsync("/companies?pageSize=3&pageIndex=3");
      // then
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
      var returnedCompanies = await DeserializeResponse<List<Company>>(response);
      Assert.NotNull(returnedCompanies);
      Assert.Equal(2, returnedCompanies.Count);
    }

    [Fact]
    public async void Should_update_company_name_from_system()
    {
      // given
      var httpClient = await InitializeHttpClient();
      var company = new Company("Schlumberger");
      var requestBody = GenerateRequestBody(company);
      await PostRequestBody(httpClient, "/companies", requestBody);

      company.Name = "SLB";
      var newRequestBody = GenerateRequestBody(company);
      // when
      var response = await httpClient.PutAsync($"/companies/{company.CompanyId}", newRequestBody);
      // then
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
      var returnedCompany = await DeserializeResponse<Company>(response);
      Assert.NotNull(returnedCompany);
      Assert.Equal("SLB", returnedCompany.Name);
    }

    [Fact]
    public async void Should_add_an_employee_to_a_specific_company()
    {
      // given
      var httpClient = await InitializeHttpClient();
      var companies = new List<Company>
      {
        new Company("SLB"),
        new Company("TW"),
      };
      var requestBodyList = GenerateRequestBodyList(companies);
      await PostRequestBodyList(httpClient, "/companies", requestBodyList);

      var employee = new Employee("Bob", 1000);
      var newRequestBody = GenerateRequestBody(employee);
      string postUri = $"/companies/{companies[1].CompanyId}/employees";
      // when
      var response = await PostRequestBody(httpClient, postUri, newRequestBody);
      // then
      Assert.Equal(HttpStatusCode.Created, response.StatusCode);
      var newResponse = await httpClient.GetAsync($"/companies/{companies[1].CompanyId}");
      var returnedCompany = await DeserializeResponse<Company>(newResponse);
      Assert.NotNull(returnedCompany);
      Assert.Equal(employee, returnedCompany.Employees[0]);
    }

    [Fact]
    public async void Should_get_all_employees_of_a_specific_company()
    {
      // given
      var httpClient = await InitializeHttpClient();
      var companies = new List<Company>
      {
        new Company("SLB"),
        new Company("TW"),
      };
      var requestBodyList = GenerateRequestBodyList(companies);
      await PostRequestBodyList(httpClient, "/companies", requestBodyList);

      var employees = new List<Employee>
      {
        new Employee("Bob", 1000),
        new Employee("Mike", 1500),
      };
      var newRequestBodyList = GenerateRequestBodyList(employees);
      string postUri = $"/companies/{companies[1].CompanyId}/employees";
      await PostRequestBodyList(httpClient, postUri, newRequestBodyList);
      // when
      var response = await httpClient.GetAsync($"/companies/{companies[1].CompanyId}/employees");
      // then
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
      var returnedEmployees = await DeserializeResponse<List<Employee>>(response);
      Assert.NotNull(returnedEmployees);
      Assert.Equal(employees, returnedEmployees);
    }

    [Fact]
    public async void Should_update_employee_name_and_salary_under_a_company()
    {
      // given
      var httpClient = await InitializeHttpClient();
      var company = new Company("SLB");
      var requestBody = GenerateRequestBody(company);
      await PostRequestBody(httpClient, "/companies", requestBody);

      var employee = new Employee("Bob", 1000);
      var newRequestBody = GenerateRequestBody(employee);
      string postUri = $"/companies/{company.CompanyId}/employees";
      await PostRequestBody(httpClient, postUri, newRequestBody);

      employee.Name = "Mike";
      employee.Salary = 1500;
      var updatedRequestBody = GenerateRequestBody(employee);

      // when
      var response = await httpClient.PutAsync($"/companies/{company.CompanyId}/employees/{employee.EmployeeId}", updatedRequestBody);
      // then
      Assert.Equal(HttpStatusCode.OK, response.StatusCode);
      var returnedEmployee = await DeserializeResponse<Employee>(response);
      Assert.NotNull(returnedEmployee);
      Assert.Equal("Mike", returnedEmployee.Name);
      Assert.Equal(1500, returnedEmployee.Salary);
    }

    [Fact]
    public async void Should_remove_employee_by_id_under_a_company()
    {
      // given
      var httpClient = await InitializeHttpClient();
      var company = new Company("SLB");
      var requestBody = GenerateRequestBody(company);
      await PostRequestBody(httpClient, "/companies", requestBody);

      var employee = new Employee("Bob", 1000);
      var newRequestBody = GenerateRequestBody(employee);
      string postUri = $"/companies/{company.CompanyId}/employees";
      var postResponse = await PostRequestBody(httpClient, postUri, newRequestBody);
      // when
      var response = await httpClient.DeleteAsync($"/companies/{company.CompanyId}/employees/{employee.EmployeeId}");
      // then
      Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
      Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
      var getResponse = await httpClient.GetAsync($"/companies/{company.CompanyId}/employees");
      Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async void Should_remove_company_by_id_from_system()
    {
      // given
      var httpClient = await InitializeHttpClient();
      var companies = new List<Company>
      {
        new Company("SLB"),
        new Company("TW"),
      };
      var requestBodyList = GenerateRequestBodyList(companies);
      await PostRequestBodyList(httpClient, "/companies", requestBodyList);
      // when
      var response = await httpClient.DeleteAsync($"/companies/{companies[0].CompanyId}");
      // then
      Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
      var getResponse = await httpClient.GetAsync($"/companies/{companies[0].CompanyId}/employees");
      Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
      var getAllResponse = await httpClient.GetAsync($"/companies");
      var returnedCompanies = await DeserializeResponse<List<Company>>(getAllResponse);
      Assert.NotNull(returnedCompanies);
      Assert.Equal("TW", returnedCompanies[0].Name);
    }

    private static async Task<HttpClient> InitializeHttpClient()
    {
      var application = new WebApplicationFactory<Program>();
      var httpClient = application.CreateClient();
      await httpClient.DeleteAsync("/companies");

      return httpClient;
    }

    private static StringContent GenerateRequestBody<T>(T requestObject)
    {
      var serializedObject = JsonConvert.SerializeObject(requestObject);
      var requestBody = new StringContent(serializedObject, Encoding.UTF8, "application/json");

      return requestBody;
    }

    private static List<StringContent> GenerateRequestBodyList<T>(T requestObjects)
        where T : IEnumerable
    {
      var requestBodyList = new List<StringContent>();
      foreach (var requestObject in requestObjects)
      {
        var serializedObject = JsonConvert.SerializeObject(requestObject);
        var requestBody = new StringContent(serializedObject, Encoding.UTF8, "application/json");
        requestBodyList.Add(requestBody);
      }

      return requestBodyList;
    }

    private static async Task<HttpResponseMessage> PostRequestBody(HttpClient httpClient, string uri, StringContent requestBody)
    {
      return await httpClient.PostAsync(uri, requestBody);
    }

    private static async Task<List<HttpResponseMessage>> PostRequestBodyList(HttpClient httpClient, string uri, List<StringContent> requestBodyList)
    {
      var responseList = new List<HttpResponseMessage>();
      foreach (var requestBody in requestBodyList)
      {
        var response = await httpClient.PostAsync(uri, requestBody);
        responseList.Add(response);
      }

      return responseList;
    }

    private static async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
    {
      if (response.Content != null)
      {
        var responseBody = await response.Content.ReadAsStringAsync();
        var deserializeObject = JsonConvert.DeserializeObject<T>(responseBody);

        return deserializeObject;
      }
      else
      {
        return default;
      }
    }
  }
}
