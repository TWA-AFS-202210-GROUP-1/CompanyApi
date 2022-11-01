using CompanyApi.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

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

        [HttpDelete("companies/employee")]
        public void ClearEmployeeList()
        {
            companiesList.ForEach(x => x.EmployeeList.Clear());
        }

        [HttpGet("companies")]
        public ActionResult<List<Company>> GetAllCompanyList([FromQuery] int? pageSize, [FromQuery] int? pageIndex)
        {
            if (pageSize != null && pageIndex != null)
            {
                return companiesList.GetRange((int)(pageSize * (pageIndex - 1)), (int)((pageSize * pageIndex) - 1));
            }

            return Ok(companiesList);
        }

        [HttpGet("companies/{companyId}")]
        public ActionResult<Company> GetOneCompanyByID([FromRoute] string companyId)
        {
            if (companiesList.Exists(x => x.CompanyId.Equals(companyId)))
            {
                return Ok(companiesList.Find(x => x.CompanyId.Equals(companyId)));
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [HttpPut("companies/{companyId}")]
        public ActionResult<Company> UpdateCompanyName([FromBody] Company company, [FromRoute] string companyId)
        {
            if (companiesList.Exists(x => x.CompanyId.Equals(companyId)))
            {
                companiesList.Find(x => x.CompanyId.Equals(companyId)).CompanyName = company.CompanyName;
                return Ok(companiesList.Find(x => x.CompanyId.Equals(companyId)));
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [HttpPost("companies/{companyId}/employee")]
        public ActionResult<Employee> AddEmployeeToCompany([FromBody] Employee employee, [FromRoute] string companyId)
        {
            if (companiesList.Exists(x => x.CompanyId.Equals(companyId)))
            {
                companiesList.Find(x => x.CompanyId.Equals(companyId)).EmployeeList.Add(employee);
                return Ok(employee);
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [HttpGet("companies/{companyId}/employee")]
        public ActionResult<List<Employee>> GetALlEmployeeForOneCompany([FromRoute] string companyId)
        {
            if (companiesList.Exists(x => x.CompanyId.Equals(companyId)))
            {
                return companiesList.Find(x => x.CompanyId.Equals(companyId)).EmployeeList;
            }
            else
            {
                return new NotFoundResult();
            }
        }

        [HttpPatch("companies/{companyId}/employee/{employeeId}")]
        public ActionResult<Employee> UpdateEmployeeInformationInOneCompany([FromRoute] string companyId, [FromRoute] string employeeId, [FromBody] Employee updateEmployee)
        {
            if (companiesList.Exists(x => x.CompanyId.Equals(companyId)) && companiesList.Find(x => x.CompanyId.Equals(companyId)).EmployeeList.Exists(e => e.EmployeeId.Equals(employeeId)))
            {
                companiesList.Find(x => x.CompanyId.Equals(companyId)).EmployeeList.Find(e => e.EmployeeId.Equals(employeeId)).Salary = updateEmployee.Salary;
                return companiesList.Find(x => x.CompanyId.Equals(companyId)).EmployeeList.Find(e => e.EmployeeId.Equals(employeeId));
            }
            else
            {
                return new NotFoundResult();
            }
        }
    }
}
