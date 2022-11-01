using System.Collections.Generic;
using CompanyApi.Models;

namespace CompanyApi.Services;

public interface ICompanyService
{
    public Company? AddNewCompany(Company newCompany);
    public IList<Company> GetAllCompanies();
    public Company? GetCompanyById(string id);
    public void DeleteAllCompany();
}