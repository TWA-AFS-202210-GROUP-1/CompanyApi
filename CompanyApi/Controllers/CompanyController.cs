using CompanyApi.Model;
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
    public IActionResult GetAllCompanies()
    {
      return Ok(companies);
    }

    [HttpGet("{id}")]
    public IActionResult GetAllCompanies([FromRoute] string id)
    {
      try
      {
        var returnedCompany = companies.First(company => company.CompanyId.Equals(id));

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
