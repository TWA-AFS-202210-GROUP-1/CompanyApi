using System.ComponentModel.DataAnnotations;
using CompanyApi.Models;
using CompanyApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace CompanyApi.Controllers
{
    [ApiController]
    [Route("companies")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;
        public CompanyController(ICompanyService service)
        {
            _companyService = service;
        }

        [HttpPost]
        public IActionResult AddNewCompany([FromBody] Company company)
        {
            var newCompany = _companyService.AddNewCompany(company);
            if (newCompany == null)
            {
                return Conflict();
            }

            return Created($"companies/{newCompany.Id}", newCompany);
        }

        [HttpGet]
        public IActionResult GetAllCompanies()
        {
            var companies = _companyService.GetAllCompanies();
            return Ok(companies);
        }

        [HttpDelete]
        public IActionResult DeleteAllCompanies()
        {
            _companyService.DeleteAllCompany();
            return NoContent();
        }

        [HttpGet("{id}")]
        public IActionResult GetCompanyById([FromRoute] string id)
        {
            var company = _companyService.GetCompanyById(id);
            if (company == null)
            {
                return NotFound();
            }

            return Ok(company);
        }
    }
}
