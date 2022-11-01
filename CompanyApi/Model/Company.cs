namespace CompanyApi.Model
{
    public class Company
    {
        public Company(string companyName)
        {
            this.CompanyName = companyName;
            this.CompanyId = " ";
        }

        public string CompanyName
        {
            get; set;
        }

        public string CompanyId
        {
            get; set;
        }
    }
}