using System;

namespace CompanyApi.Model
{
  public class Company
  {
    public Company(string name)
    {
      Name = name;
      CompanyId = string.Empty;
    }

    public string Name { get; set; }
    public string CompanyId { get; set; }
  }
}
