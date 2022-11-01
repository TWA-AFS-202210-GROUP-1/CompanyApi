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
    }
}
