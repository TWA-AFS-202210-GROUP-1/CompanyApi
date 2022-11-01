using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public ActionResult<List<Company>> GetAllCompanys([FromQuery] int? size, [FromQuery] int? index)
        {
            if (size != null && index != null)
            {
                return companys.Skip((index.Value - 1) * size.Value)
                    .Take(size.Value).ToList();
            }

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

        [HttpPut("companys")]
        public ActionResult<Company> Updatecompany(Company company)
        {
            foreach (var existedcompany in companys)
            {
                if (existedcompany.CompanyID == company.CompanyID)
                {
                    existedcompany.Name = company.Name;
                    return company;
                }
            }

            return NotFound();
        }
    }
}
