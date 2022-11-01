using System.Collections.Generic;

namespace CompanyApi.Model
{
    public class Company
    {
        public Company(string companyName)
        {
            this.CompanyName = companyName;
            this.CompanyId = " ";
            EmployeeList = new List<Employee>();
        }

        public string CompanyName
        {
            get; set;
        }

        public string CompanyId
        {
            get; set;
        }

        public List<Employee> EmployeeList
        {
            get; set;
        }
    }
}