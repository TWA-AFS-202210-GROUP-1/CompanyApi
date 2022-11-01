namespace CompanyApi.Model
{
    public class Employee
    {
        private string name;
        private string employeeID;
        private int salary;
        public Employee(string name)
        {
            this.Name = name;
            this.employeeID = string.Empty;
            this.salary = salary;
        }

        public string EmployeeID { get => employeeID; set => employeeID = value; }
        public string Name { get => name; set => name = value; }
    }
}