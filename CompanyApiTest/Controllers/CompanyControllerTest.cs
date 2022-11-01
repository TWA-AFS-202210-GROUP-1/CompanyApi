using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CompanyApi.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace CompanyApiTest.Controllers
{
    public class CompanyControllerTest
    {
        private readonly HttpClient _httpClient;
        public CompanyControllerTest()
        {
            var application = new WebApplicationFactory<Program>();
            _httpClient = application.CreateClient();

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
    }
}
