using System;

namespace CompanyApi.Model
{
    public class Employee
    {
        public Employee()
        {
            EmployeeId = Guid.NewGuid().ToString();
        }

        public string EmployeeId
        {
            get; set;
        }
    }
}
