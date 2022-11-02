using System;

namespace CompanyApi.Controllers
{
    public class Employee
    {
        private string employeeID;
        private string name;
        private int salary;

        public Employee(string name)
        {
            this.employeeID = string.Empty;
            this.name = name;
            this.salary = 0;
        }

        public Employee(string name, string employeeid)
        {
            this.employeeID = employeeid;
            this.name = name;
            this.salary = 0;
        }

        public Employee()
        {
        }

        public string EmployeeID { get => employeeID; set => employeeID = value; }
        public string Name { get => name; set => name = value; }
        public int Salary { get => salary; set => salary = value; }
    }
}