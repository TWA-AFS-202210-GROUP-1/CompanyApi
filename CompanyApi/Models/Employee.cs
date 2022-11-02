using System;

namespace CompanyApi.Models;

public class Employee
{
    public string? Id { get; set; }
    public string Name { get; set; }
    public double Salary { get; set; }

    public Employee(string name, double salary)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Salary = salary;
    }
}