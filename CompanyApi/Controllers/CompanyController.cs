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

        [HttpGet("{name}")]
        public IActionResult GetCompany([FromRoute] string name)
        {
            var company = _companyService.GetCompany(name);
            if (company == null)
            {
                return NotFound();
            }

            return Ok(company);
        }
    }
}
