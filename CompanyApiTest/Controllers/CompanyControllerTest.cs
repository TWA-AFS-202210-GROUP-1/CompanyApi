using CompanyApi.Model;
using DeepEqual.Syntax;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        [Fact]
        public async void Should_return_companiesList_when_call_get_all_api()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            _ = httpClient.DeleteAsync("/companies");

            var companyNameList = new List<string>()
            {
                "Umbrella",
                "Tencent",
                "Sony",
            };
            List<Company> companyList = await AddMultiCompaniesToBackend(httpClient, companyNameList);

            //when
            var response = await httpClient.GetAsync("/companies");

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var savedCompanyList = JsonConvert.DeserializeObject<List<Company>>(responseBody);
            savedCompanyList.Select(c => c.CompanyName).ToList().ShouldDeepEqual(companyNameList);
        }

        [Fact]
        public async void Should_return_an_existing_company_when_get_one_company()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            _ = httpClient.DeleteAsync("/companies");

            var companyNameList = new List<string>()
            {
                "Umbrella",
                "Tencent",
                "Sony",
            };
            List<Company> companyList = await AddMultiCompaniesToBackend(httpClient, companyNameList);
            string needToFindCompanyId = companyList[0].CompanyId;

            //when
            var response = await httpClient.GetAsync($"/companies/{needToFindCompanyId}");

            // then
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseBody = await response.Content.ReadAsStringAsync();
            var savedCompany = JsonConvert.DeserializeObject<Company>(responseBody);
            savedCompany.ShouldDeepEqual(companyList[0]);
        }

        [Fact]
        public async void Should_return_NotFound_when_get_one_company_with_invalid_id()
        {
            //given
            var application = new WebApplicationFactory<Program>();
            var httpClient = application.CreateClient();
            _ = httpClient.DeleteAsync("/companies");

            var companyNameList = new List<string>()
            {
                "Umbrella",
                "Tencent",
                "Sony",
            };
            await AddMultiCompaniesToBackend(httpClient, companyNameList);
            string needToFindCompanyId = "invalid Id";

            //when
            var response = await httpClient.GetAsync($"/companies/{needToFindCompanyId}");

            // then
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        private static async Task<List<Company>> AddMultiCompaniesToBackend(HttpClient httpClient, List<string> companyNameList)
        {
            List<Company> companyList = new List<Company>();
            foreach (string singleCompanyName in companyNameList)
            {
                Company singleCompany = new Company(companyName: singleCompanyName);
                var singleCompanyJson = JsonConvert.SerializeObject(singleCompany);
                var postBody = new StringContent(singleCompanyJson, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("/companies", postBody);
                var responseBody = await response.Content.ReadAsStringAsync();
                var savedCompany = JsonConvert.DeserializeObject<Company>(responseBody);
                companyList.Add(savedCompany);
            }

            return companyList;
        }
    }
}
