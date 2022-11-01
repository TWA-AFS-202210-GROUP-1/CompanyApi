using System;
using System.Collections.Generic;

namespace CompanyApi.Models;

public class Company
{
    public string? Id { get; set; }
    public string Name { get; set; }
    public IList<Employee> Employees { get; set; }
    public Company(string name)
    {
        Name = name;
        Employees = new List<Employee>();
    }
}