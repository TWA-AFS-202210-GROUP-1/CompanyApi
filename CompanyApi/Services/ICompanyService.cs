using System.Collections.Generic;
using CompanyApi.Dto;
using CompanyApi.Models;

namespace CompanyApi.Services;

public interface ICompanyService
{
    public Company? AddNewCompany(Company newCompany);
    public IList<Company> GetAllCompanies();
    public Company? GetCompanyById(string id);
    public void DeleteAllCompany();
    public IList<Company> GetCompaniesByPage(int pageSize, int index);
    public Company? UpdateCompany(string id, UpdateCompanyDto info);
    public bool DeleteCompany(string id);
    public Employee AddEmployeeToCompany(string companyId, Employee employee);
    public IList<Employee>? GetAllEmployeesInCompany(string companyId);
    public Employee? UpdateEmployee(string companyId, string employeeId, UpdateEmployeeDto info);
    public bool DeleteEmployee(string companyId, string employeeId);
    public Employee? GetEmployeeById(string companyId, string employeeId);
}