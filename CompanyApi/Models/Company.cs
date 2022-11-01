using System;

namespace CompanyApi.Models;

public class Company
{
    public string? Id { get; set; }
    public string Name { get; set; }
    public Company(string name)
    {
        Name = name;
    }
}