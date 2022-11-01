using CompanyApi.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companies = new List<Company>();
        [HttpPost]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            foreach (Company everyCompany in companies)
            {
                if (everyCompany.Name == company.Name)
                {
                    return Conflict();
                }
            }

            company.CompanyID = Guid.NewGuid().ToString();
            companies.Add(company);
            return new CreatedResult($"/companies/{company.CompanyID}", company);
        }

        [HttpDelete("deleteAllCompnies")]
        public void DeleteAllCompanies()
        {
            companies.Clear();
        }
    }
}
