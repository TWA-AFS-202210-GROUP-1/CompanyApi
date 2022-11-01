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
    }
}
