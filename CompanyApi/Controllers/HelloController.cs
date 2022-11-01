using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public ActionResult<List<Company>> GetAllCompanys([FromQuery] int? size, [FromQuery] int? index)
        {
            if (size != null && index != null)
            {
                return companys.Skip((index.Value - 1) * size.Value)
                    .Take(size.Value).ToList();
            }

            return companys;
        }

        [HttpGet("companys/{companyID}")]
        public ActionResult<Company> GetCompanybyName(string companyID)
        {
            foreach (var company in companys)
            {
                if (company.CompanyID == companyID)
                {
                    return company;
                }
            }

            return NotFound();
        }

        [HttpPut("companys")]
        public ActionResult<Company> Updatecompany(Company company)
        {
            foreach (var existedcompany in companys)
            {
                if (existedcompany.CompanyID == company.CompanyID)
                {
                    existedcompany.Name = company.Name;
                    return company;
                }
            }

            return NotFound();
        }

        [HttpPost("companys/{companyid}/employees")]
        public ActionResult<Employee> AddNewEmployeetoCompany([FromRoute] string companyid, Employee employee)
        {
            employee.EmployeeID = Guid.NewGuid().ToString();
            foreach (var company in companys)
            {
            if (companyid == company.CompanyID)
            {
               company.AddEmployee(employee);
               return new CreatedResult("/companies/{companyid}/employees/{employee.EmployeeID}", employee);
            }
            }

            return NotFound();
        }

        [HttpGet("companys/{companyid}/employees")]
        public ActionResult<List<Employee>> GetAllEmployeefromCompany([FromRoute] string companyid)
        {
            foreach (var company in companys)
            {
                if (companyid == company.CompanyID)
                {
                    return company.Employees;
                }
            }

            return NotFound();
        }

        [HttpPut("companys/{companyid}/employees")]
        public ActionResult<Employee> Updateemployee([FromRoute] string companyid, Employee employee)
        {
            foreach (var existedcompany in companys)
            {
                if (existedcompany.CompanyID == companyid)
                {
                    foreach (var theemployee in existedcompany.Employees)
                    {
                        if (theemployee.EmployeeID == employee.EmployeeID)
                        {
                            theemployee.Name = employee.Name;
                            return theemployee;
                        }
                    }
                }
            }

            return NotFound();
        }

        [HttpDelete("companys/{companyid}/employees/{employeeid}")]
        public ActionResult<Employee> Deleteemployee([FromRoute] string companyid, [FromRoute] string employeeid)
        {
            foreach (var existedcompany in companys)
            {
                if (existedcompany.CompanyID == companyid)
                {
                    for (int i = 0; i < existedcompany.Employees.Count; i++)
                    {
                        if (existedcompany.Employees[i].EmployeeID == employeeid)
                        {
                            existedcompany.Employees.RemoveAt(i);
                            return NoContent();
                        }
                    }
                }
            }

            return NotFound();
        }
    }
}
