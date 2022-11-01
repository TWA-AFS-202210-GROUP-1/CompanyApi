namespace CompanyApi.Model
{
    public class Company
    {
        private string name;
        private string companyID;
        public Company(string name)
        {
            this.Name = name;
            this.CompanyID = string.Empty;
        }

        public string CompanyID { get => companyID; set => companyID = value; }
        public string Name { get => name; set => name = value; }
    }
}