using System;
using System.Collections.Generic;
using System.Linq;
using CompanyApi.Models;

namespace CompanyApi.Services
{
    public class CompanyService : ICompanyService
    {
        public IList<Company> Companies { get; set; }

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

        public IList<Company> GetAllCompanies()
        {
            return Companies;
        }

        public Company? GetCompany(string name)
        {
            return Companies.FirstOrDefault(_ => _.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
