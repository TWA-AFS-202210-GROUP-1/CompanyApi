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
        private static List<Company> companies = new List<Company>();
        [HttpPost]
        public ActionResult<Company> AddNewCompany(Company company)
        {
            foreach (Company everyCompany in companies)
            {
                if (everyCompany.Name == company.Name)
                {
                    return Conflict();
                }
            }

            company.CompanyID = Guid.NewGuid().ToString();
            companies.Add(company);
            return new CreatedResult($"/companies/{company.CompanyID}", company);
        }

        [HttpDelete]
        public void DeleteAllCompanies()
        {
            companies.Clear();
        }

        [HttpGet]
        public ActionResult<List<Company>> GetAllCompanies([FromQuery] int? pageSize, [FromQuery] int? pageIndex)
        {
            //int maxIndex = companies.Count % pageSize.Value;
            if (pageSize != null && pageIndex != null)
            {
                return Ok(companies.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value).ToList());              
            }

            return Ok(companies);
        }

        [HttpGet("{companyID}")]
        public ActionResult<Company> GetCompanyByID([FromRoute] string companyID)
        {
            Company company = companies.Find(x => x.CompanyID == companyID);
            if (company == null)
            {
                return NotFound();
            }

            return Ok(company);
        }

        [HttpPost("{companyID}")]
        public ActionResult<Company> UpdateCompanyByID([FromRoute] string companyID, [FromBody] Company editedCompany)
        {
            Company company = companies.Find(x => x.CompanyID == companyID);
            if (company == null)
            {
                return NotFound();
            }

            company.Name = editedCompany.Name;
            return Ok(company);
        }

        [HttpPost("{companyID}/employees")]
        public ActionResult<Employee> AddEmployeeToCompany([FromRoute] string companyID, [FromBody] Employee employee)
        {
            Company company = companies.Find(x => x.CompanyID == companyID);
            company.AddNewEmployee(employee);
            return Ok(employee);
        }

        [HttpGet("{companyID}/employees")]
        public ActionResult<List<Employee>> GetEmployeesOfCompany([FromRoute] string companyID)
        {
            Company company = companies.Find(x => x.CompanyID == companyID);
            return Ok(company.Employees);
        }
    }
}
