using System;
using System.Collections.Generic;

namespace CompanyApi.Model
{
  public class Company
  {
    public Company(string name)
    {
      Name = name;
      CompanyId = Guid.NewGuid().ToString();
      Employees = new List<Employee>();
    }

    public string Name { get; set; }
    public string CompanyId { get; set; }
    public List<Employee> Employees { get; set; }

    public override bool Equals(object? obj)
    {
      return obj is Company company
             && Name.Equals(company.Name)
             && Employees.Equals(company.Employees);
    }

    public override int GetHashCode()
    {
      throw new NotImplementedException();
    }
  }
}
