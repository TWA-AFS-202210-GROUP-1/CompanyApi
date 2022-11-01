using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CompanyApi.Dto;
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
        public IActionResult GetAllCompanies([FromQuery] int? pageSize, [FromQuery] int? index)
        {
            if (pageSize == null ^ index == null)
            {
                return BadRequest();
            }

            IList<Company> companies;
            if (pageSize != null && index != null)
            {
                companies = _companyService.GetCompaniesByPage(pageSize.Value, index.Value);
                return Ok(companies);
            }   

            companies = _companyService.GetAllCompanies();
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

        [HttpPut("{id}")]
        public IActionResult UpdateCompanyInfo([FromRoute] string id, [FromBody] CompanyUpdateDto updateInfo)
        {
            var updatedCompany = _companyService.UpdateCompany(id, updateInfo);
            if (updatedCompany == null)
            {
                return NoContent();
            }

            return Ok(updatedCompany);
        }

        [HttpPost("{companyId}/employees")]
        public IActionResult AddEmployeeToCompany([FromRoute] string companyId, [FromBody] Employee employee)
        {
            var newEmployee = _companyService.AddEmployeeToCompany(companyId, employee);
            if (string.IsNullOrEmpty(newEmployee.Id))
            {
                return NotFound();
            }

            return Created($"companies/{companyId}/employees/{newEmployee.Id}", newEmployee);
        }
    }
}
