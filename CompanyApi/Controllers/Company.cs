using System;

namespace CompanyApi.Controllers
{
    public class Company
    {
        private string companyID;
        private string name;

        public Company(string name)
        {
            this.CompanyID = string.Empty;
            this.Name = name;
        }

        public Company(string name, string companyid)
        {
            this.CompanyID = companyid;
            this.Name = name;
        }

        public Company()
        {
        }

        public string CompanyID { get => companyID; set => companyID = value; }
        public string Name { get => name; set => name = value; }
    }
}