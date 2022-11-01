using CompanyApi.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest
    {
        [Fact]
        public async void Should_add_a_company_to_compantList()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            _ = httpClient.DeleteAsync("/companies");

            Company company = new Company(companyName: "Umbrella");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");

            //when
            var response = await httpClient.PostAsync("/companies", postBody);
            // then
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var savedCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            Assert.Equal(company.CompanyName, savedCompany.CompanyName);
            Assert.NotEmpty(savedCompany.CompanyId);
        }

        [Fact]
        public async void Should_return_conflict_code_when_add_a_exist_company_to_compantList()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            _ = httpClient.DeleteAsync("/companies");

            Company company = new Company(companyName: "Umbrella");
            var companyJson = JsonConvert.SerializeObject(company);
            var postBody = new StringContent(companyJson, Encoding.UTF8, "application/json");

            //when
            await httpClient.PostAsync("/companies", postBody);
            var response = await httpClient.PostAsync("/companies", postBody);

            // then
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
    }
}
