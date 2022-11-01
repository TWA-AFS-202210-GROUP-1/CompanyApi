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
    public Company? UpdateCompany(string id, CompanyUpdateDto updateInfo);
    public Employee AddEmployeeToCompany(string companyId, Employee employee);
}