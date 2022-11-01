using System;
using System.Collections.Generic;
using System.Linq;
using CompanyApi.Dto;
using CompanyApi.Models;

namespace CompanyApi.Services
{
    public class CompanyService : ICompanyService
    {
        public List<Company> Companies { get; set; }

        public CompanyService()
        {
            Companies = new List<Company>();
        }

        public Company? AddNewCompany(Company newCompany)
        {
            if (Companies.Any(_ => _.Name.Equals(newCompany.Name, StringComparison.CurrentCultureIgnoreCase)))
            {
                return null;
            }

            newCompany.Id = Guid.NewGuid().ToString();
            Companies.Add(newCompany);

            return newCompany;
        }

        public void DeleteAllCompany()
        {
            Companies.Clear();
        }

        public IList<Company> GetCompaniesByPage(int pageSize, int index)
        { 
            return Companies.Skip((pageSize - 1) * pageSize).Take(pageSize).ToList();
        }

        public Company? UpdateCompany(string id, CompanyUpdateDto updateInfo)
        {
            var updatingCompany = Companies.FirstOrDefault(_ => _.Id!.Equals(id));
            if (updatingCompany != null)
            {
                updatingCompany.Name = updateInfo.Name;
            }

            return updatingCompany;
        }

        public IList<Company> GetAllCompanies()
        {
            return Companies;
        }

        public Company? GetCompanyById(string id)
        {
            return Companies.FirstOrDefault(_ => _.Id!.Equals(id, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
