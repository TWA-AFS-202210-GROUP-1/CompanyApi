using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("api")]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companys = new List<Company>();
        [HttpPost("companys")]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            company.CompanyID = Guid.NewGuid().ToString();
            companys.Add(company);
            return new CreatedResult("/companies/{company.CompanyID}", company);
        }

        [HttpGet("companys")]
        public ActionResult<List<Company>> GetAllCompanys()
        {
            return companys;
        }

        [HttpGet("companys/{companyID}")]
        public ActionResult<Company> GetCompanybyName(string companyID)
        {
            foreach (var company in companys)
            {
                if (company.CompanyID == companyID)
                {
                    return company;
                }
            }

            return NotFound();
        }

        [HttpGet("companys/size/{size}/from/{aa}")]
        public ActionResult<Company> GetAllCompanys(string companyname)
        {
            foreach (var company in companys)
            {
                if (company.Name == companyname)
                {
                    return company;
                }
            }

            return NotFound();
        }
    }
}
