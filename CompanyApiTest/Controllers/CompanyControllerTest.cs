﻿using System.Collections.Generic;
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
        public async Task Should_return_copany_successfully_given_company_Id()
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
    }
}
