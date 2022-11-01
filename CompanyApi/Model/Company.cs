using System;
using System.Collections.Generic;

namespace CompanyApi.Model
{
    public class Company
    {
        private string name;
        private string companyID;
        private List<Employee> employees;
        public Company(string name)
        {
            this.Name = name;
            this.CompanyID = string.Empty;
            this.Employees = new List<Employee>();
        }

        public string CompanyID { get => companyID; set => companyID = value; }
        public string Name { get => name; set => name = value; }
        public List<Employee> Employees { get => employees; set => employees = value; }

        public void AddNewEmployee(Employee employee)
        {
            employee.EmployeeID = Guid.NewGuid().ToString();
            employees.Add(employee);
            Console.WriteLine(employees);
        }
    }
}