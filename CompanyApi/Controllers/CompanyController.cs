﻿using CompanyApi.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CompanyApi.Controllers
{
  [ApiController]
  [Route("companies")]
  public class CompanyController : ControllerBase
  {
    private static readonly List<Company> companies = new ();

    [HttpPost]
    public IActionResult AddNewCompany(Company newCompany)
    {
      if (companies.Any(company => company.Name.Equals(newCompany.Name)))
      {
        return Conflict();
      }
      else
      {
        companies.Add(newCompany);

        return Created($"/companies/{newCompany.CompanyId}", newCompany);
      }
    }

    [HttpPost("{companyId}/employees")]
    public IActionResult AddNewEmployee([FromRoute] string companyId, Employee employee)
    {
      var targetCompany = companies.First(company => company.CompanyId.Equals(companyId));
      targetCompany.Employees.Add(employee);

      return Created($"/companies/{targetCompany.CompanyId}/employees", targetCompany);
    }

    [HttpGet]
    public IActionResult GetCompanies([FromQuery] int? pageSize, [FromQuery] int? pageIndex)
    {
      if (companies.Count.Equals(0))
      {
        return NotFound();
      }

      if (pageSize != null && pageIndex != null)
      {
        var queriedCompanies = companies
          .Skip((pageIndex.Value - 1) * pageSize.Value)
          .Take(pageSize.Value)
          .ToList();

        return Ok(queriedCompanies);
      }
      else
      {
        return Ok(companies);
      }
    }

    [HttpGet("{companyId}")]
    public IActionResult GetCompanyById([FromRoute] string companyId)
    {
      try
      {
        var returnedCompany = companies.First(company => company.CompanyId.Equals(companyId));

        return Ok(returnedCompany);
      }
      catch (Exception e)
      {
        return NotFound(e.Message);
      }
    }

    [HttpGet("{companyId}/employees")]
    public IActionResult GetEmployees([FromRoute] string companyId)
    {
      try
      {
        var returnedCompany = companies.First(company => company.CompanyId.Equals(companyId));
        if (returnedCompany.Employees.Count > 0)
        {
          return Ok(returnedCompany.Employees);
        }
        else
        {
          return NotFound();
        }
      }
      catch (Exception e)
      {
        return NotFound(e.Message);
      }
    }

    [HttpPut("{companyId}")]
    public IActionResult UpdateCompanyInformation([FromRoute] string companyId, Company updatedCompany)
    {
      try
      {
        var returnedCompany = companies.First(company => company.CompanyId.Equals(companyId));
        returnedCompany.Name = updatedCompany.Name;

        return Ok(returnedCompany);
      }
      catch (Exception e)
      {
        return NotFound(e.Message);
      }
    }

    [HttpPut("{companyId}/employees/{employeeId}")]
    public IActionResult UpdateEmployeeInformation([FromRoute] string companyId, [FromRoute] string employeeId, Employee updatedEmployee)
    {
      try
      {
        var returnedCompany = companies.First(company => company.CompanyId.Equals(companyId));
        var returnedEmployee = returnedCompany.Employees.First(employee => employee.EmployeeId.Equals(employeeId));
        returnedEmployee.Name = updatedEmployee.Name;
        returnedEmployee.Salary = updatedEmployee.Salary;

        return Ok(returnedEmployee);
      }
      catch (Exception e)
      {
        return NotFound(e.Message);
      }
    }

    [HttpDelete("{companyId}")]
    public IActionResult DeleteCompanyById([FromRoute] string companyId)
    {
      var returnedCompany = companies.First(company => company.CompanyId.Equals(companyId));
      companies.Remove(returnedCompany);

      return NoContent();
    }

    [HttpDelete("{companyId}/employees/{employeeId}")]
    public IActionResult DeleteEmployeeById([FromRoute] string companyId, [FromRoute] string employeeId)
    {
      var returnedCompany = companies.First(company => company.CompanyId.Equals(companyId));
      var returnedEmployee = returnedCompany.Employees.First(employee => employee.EmployeeId.Equals(employeeId));
      returnedCompany.Employees.Remove(returnedEmployee);

      return NoContent();
    }

    [HttpDelete]
    public IActionResult DeleteAllCompanies()
    {
      companies.Clear();

      return NoContent();
    }
  }
}
