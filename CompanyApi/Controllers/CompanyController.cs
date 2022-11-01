using CompanyApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace CompanyApi.Controllers
{
    [ApiController]
    //[Route("api")]
    public class CompanyController : ControllerBase
    {
        private static List<Company> companiesList = new List<Company>();

        [HttpPost("companies")]
        //[ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Company))]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            if (companiesList.Exists(x => x.CompanyName.Equals(company.CompanyName)))
            {
                return Conflict();
            }

            company.CompanyId = Guid.NewGuid().ToString();
            companiesList.Add(company);
            return Created($"companies/{company.CompanyId}", company);
        }

        [HttpDelete("companies")]
        public void ClearCompaniesList()
        {
            companiesList.Clear();
        }
    }
}
