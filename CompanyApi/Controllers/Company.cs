using System;
using System.Collections.Generic;

namespace CompanyApi.Controllers
{
    public class Company
    {
        private string companyID;
        private string name;
        private List<Employee> employees;

        public Company(string name)
        {
            this.CompanyID = string.Empty;
            this.Name = name;
            this.Employees = new List<Employee>();
        }

        public Company(string name, string companyid)
        {
            this.CompanyID = companyid;
            this.Name = name;
            this.Employees = new List<Employee>();
        }

        public Company()
        {
        }

        public string CompanyID { get => companyID; set => companyID = value; }
        public string Name { get => name; set => name = value; }
        public List<Employee> Employees { get => employees; set => employees = value; }

        public void AddEmployee(Employee employee)
        {
            employees.Add(employee);
        }
    }
}