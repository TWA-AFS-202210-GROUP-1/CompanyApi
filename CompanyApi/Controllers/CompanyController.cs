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
        public IActionResult UpdateCompanyInfo([FromRoute] string id, [FromBody] UpdateCompanyDto info)
        {
            var updatedCompany = _companyService.UpdateCompany(id, info);
            if (updatedCompany == null)
            {
                return NotFound();
            }

            return Ok(updatedCompany);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteCompany([FromRoute] string id)
        {
            var isDeleteCompany = _companyService.DeleteCompany(id);
            if (isDeleteCompany)
            {
                return NoContent();
            }

            return NotFound();
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

        [HttpGet("{companyId}/employees")]
        public IActionResult GetAllEmployeesInCompany([FromRoute] string companyId)
        {
            var employees = _companyService.GetAllEmployeesInCompany(companyId);
            if (employees == null)
            {
                return NotFound();
            }

            return Ok(employees);
        }

        [HttpGet("{companyId}/employees/{employeeId}")]
        public IActionResult GetEmployeeInCompanyById([FromRoute] string companyId, [FromRoute] string employeeId)
        {
            var employee = _companyService.GetEmployeeById(companyId, employeeId);
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpPut("{companyId}/employees/{employeeId}")]
        public IActionResult UpdateEmployeeInCompany([FromRoute] string companyId, [FromRoute] string employeeId, [FromBody] UpdateEmployeeDto info)
        {
            var employee = _companyService.UpdateEmployee(companyId, employeeId, info);
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpDelete("{companyId}/employees/{employeeId}")]
        public IActionResult DeleteEmployeeInCompany([FromRoute] string companyId, [FromRoute] string employeeId)
        {
            var isDelete = _companyService.DeleteEmployee(companyId, employeeId);
            if (isDelete)
            {
                return NoContent();
            }

            return NotFound();
        }
    }
}
