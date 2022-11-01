using System;

namespace CompanyApi.Model
{
  public class Employee
  {
    public Employee(string name, double salary)
    {
      Name = name;
      Salary = salary;
      EmployeeId = Guid.NewGuid().ToString();
    }

    public string Name { get; set; }
    public double Salary { get; set; }
    public string EmployeeId { get; set; }

    public override bool Equals(object? obj)
    {
      return obj is Employee employee
             && Name.Equals(employee.Name)
             && Salary.Equals(employee.Salary);
    }

    public override int GetHashCode()
    {
      throw new NotImplementedException();
    }
  }
}
