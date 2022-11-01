using System;

namespace CompanyApi.Model
{
    public class Employee
    {
        public Employee(string employeeName, int salary)
        {
            EmployeeId = Guid.NewGuid().ToString();
            Salary = salary;
            EmployeeName = employeeName;
        }

        public string EmployeeId
        {
            get; set;
        }

        public string EmployeeName
        {
            get; set;
        }

        public int Salary
        {
            get; set;
        }

    }
}
