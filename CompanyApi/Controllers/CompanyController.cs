﻿using CompanyApi.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;

namespace CompanyApi.Controllers
{
  [ApiController]
  [Route("companies")]
  public class CompanyController : ControllerBase
  {
    private static readonly List<Company> companies = new ();

    [HttpPost]
    public ActionResult<Company> AddNewCompany(Company company)
    {
      if (!HasCompany(company))
      {
        companies.Add(company);

        return new CreatedResult($"/companies/{company.CompanyId}", company);
      }
      else
      {
        return new ConflictResult();
      }
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

    [HttpDelete]
    public IActionResult DeleteAllCompanies()
    {
      companies.Clear();

      return NoContent();
    }

    private static bool HasCompany(Company newCompany)
    {
      var hasCompany = false;
      foreach (var company in companies)
      {
        if (company.Name.Equals(newCompany.Name))
        {
          hasCompany = true;
          break;
        }
      }

      return hasCompany;
    }
  }
}
