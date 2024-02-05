using System.Globalization;
namespace lab2
{
    public class Employee
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public long Sin { get; set; }
        public DateTime Date { get; set; }
        public string Occupation { get; set; }

        public Employee() { }

        public Employee(string Id, string Name, string Address, string Phone, long Sin, DateTime Date, string Occupation)
        {
            this.Id = Id;
            this.Name = Name;
            this.Address = Address;
            this.Phone = Phone;
            this.Sin = Sin;
            this.Date = Date;
            this.Occupation = Occupation;
        }
    }

    public class Salaried : Employee
    {
        public double Salary { get; set; }

        public Salaried() { }

        public Salaried(string Id, string Name, string Address, string Phone, long Sin, DateTime Date, string Occupation, double Salary)
            : base(Id, Name, Address, Phone, Sin, Date, Occupation)
        {
            this.Salary = Salary;
        }
    }

    public class PartTime : Employee
    {
        public double Rate { get; set; }
        public int HoursWorked { get; set; }

        public PartTime() { }

        public PartTime(string Id, string Name, string Address, string Phone, long Sin, DateTime Date, string Occupation, double Rate, int HoursWorked)
            : base(Id, Name, Address, Phone, Sin, Date, Occupation)
        {
            this.Rate = Rate;
            this.HoursWorked = HoursWorked;
        }
    }

    public class Wages : Employee
    {
        public double Rate { get; set; }
        public int HoursWorked { get; set; }

        public Wages() { }

        public Wages(string Id, string Name, string Address, string Phone, long Sin, DateTime Date, string Occupation, double Rate, int HoursWorked)
            : base(Id, Name, Address, Phone, Sin, Date, Occupation)
        {
            this.Rate = Rate;
            this.HoursWorked = HoursWorked;
        }
    }

    class Program
    {
        static void Main()
        {
            List<Employee> employeeList = new List<Employee>();
            FillEmployeeList(employeeList);

            double averageWeeklyPay = CalculateAverageWeeklyPay(employeeList);
            Console.WriteLine("OUTPUT");
            Console.WriteLine($"\nAverage Weekly Pay of all employees: {averageWeeklyPay}.00");

            var highestWeeklyPayWages = CalculateHighestWeeklyPayWages(employeeList);
            Console.WriteLine($"Employee who got Highest Weekly Pay: {highestWeeklyPayWages.Name}, {highestWeeklyPayWages.WeeklyPay}.00");

            var lowestSalarySalaried = CalculateLowestSalarySalaried(employeeList);
            Console.WriteLine($"Lowest Salary of Salaried employee: {lowestSalarySalaried.Name}, {lowestSalarySalaried.Salary}.00");

            var categoryPercentages = CalculateEmployeeCategoryPercentage(employeeList);
            Console.WriteLine($"\nEmployee Category Percentages:\nSalaried: {categoryPercentages.Item1}%,\nPartTime: {categoryPercentages.Item2}%,\nWages: {categoryPercentages.Item3}%");
        }

        static void FillEmployeeList(List<Employee> employeeList)
        {
            // Read data from employees.txt file
            string[] lines = File.ReadAllLines("respond/employees.txt");

            foreach (string line in lines)
            {
                string[] data = line.Split(':');
                string id = data[0].Trim();
                string name = data[1].Trim();
                string address = data[2].Trim();
                string phone = data[3].Trim();
                long sin = long.Parse(data[4].Trim());

                DateTime? ParseEmployeeDate(string dateString)
                {
                    DateTime date;

                    if (DateTime.TryParseExact(dateString.Trim(), "MMM dd, yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date) ||
                        DateTime.TryParseExact(dateString.Trim(), "MMM d, yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    {
                        return date;
                    }
                    else
                    {
                        Console.WriteLine($"Error parsing date: {dateString}");
                        return null;
                    }
                }

                string occupation = data[6].Trim();

                DateTime? date = ParseEmployeeDate(data[5].Trim());

                if (date.HasValue)
                {
                    if (id[0] >= '0' && id[0] <= '4')
                    {
                        // Salaried
                        double salary = double.Parse(data[7].Trim());
                        employeeList.Add(new Salaried(id, name, address, phone, sin, date.Value, occupation, salary));
                    }
                    else if (id[0] >= '5' && id[0] <= '7')
                    {
                        // Wages
                        double rate = double.Parse(data[7].Trim());
                        int hoursWorked = int.Parse(data[8].Trim());
                        employeeList.Add(new Wages(id, name, address, phone, sin, date.Value, occupation, rate, hoursWorked));
                    }
                    else if (id[0] >= '8' && id[0] <= '9')
                    {
                        // PartTime
                        double rate = double.Parse(data[7].Trim());
                        int hoursWorked = int.Parse(data[8].Trim());
                        employeeList.Add(new PartTime(id, name, address, phone, sin, date.Value, occupation, rate, hoursWorked));
                    }
                    else
                    {
                        Console.WriteLine($"Invalid employee ID for employee with ID {id}");
                    }
                }
                else
                {
                    // Handle parsing error
                    Console.WriteLine($"Error parsing date for employee with ID {id}");
                }
            }
        }

        static double CalculateAverageWeeklyPay(List<Employee> employeeList)
        {
            double totalWeeklyPay = 0;

            foreach (var employee in employeeList)
            {
                if (employee is Salaried salaried)
                {
                    totalWeeklyPay += salaried.Salary;
                }
                else if (employee is Wages wages)
                {
                    totalWeeklyPay += CalculateWagesWeeklyPay(wages);
                }
                else if (employee is PartTime partTime)
                {
                    totalWeeklyPay += CalculatePartTimeWeeklyPay(partTime);
                }
            }

            return totalWeeklyPay / employeeList.Count;
        }

        static (string Name, double WeeklyPay) CalculateHighestWeeklyPayWages(List<Employee> employeeList)
        {
            Wages highestPayWages = null;
            double highestWeeklyPay = 0;

            foreach (var employee in employeeList.Where(e => e is Wages))
            {
                var wages = (Wages)employee;
                double weeklyPay = CalculateWagesWeeklyPay(wages);

                if (weeklyPay > highestWeeklyPay)
                {
                    highestWeeklyPay = weeklyPay;
                    highestPayWages = wages;
                }
            }

            return (highestPayWages?.Name, highestWeeklyPay);
        }

        static (string Name, double Salary) CalculateLowestSalarySalaried(List<Employee> employeeList)
        {
            Salaried lowestSalarySalaried = null;
            double lowestSalary = double.MaxValue;

            foreach (var employee in employeeList.Where(e => e is Salaried))
            {
                var salaried = (Salaried)employee;

                if (salaried.Salary < lowestSalary)
                {
                    lowestSalary = salaried.Salary;
                    lowestSalarySalaried = salaried;
                }
            }

            return (lowestSalarySalaried?.Name, lowestSalary);
        }

        static (double, double, double) CalculateEmployeeCategoryPercentage(List<Employee> employeeList)
        {
            double salariedPercentage = 0;
            double partTimePercentage = 0;
            double wagesPercentage = 0;

            foreach (var employee in employeeList)
            {
                if (employee is Salaried)
                {
                    salariedPercentage++;
                }
                else if (employee is PartTime)
                {
                    partTimePercentage++;
                }
                else if (employee is Wages)
                {
                    wagesPercentage++;
                }
            }

            int totalEmployees = employeeList.Count;

            if (totalEmployees > 0)
            {
                salariedPercentage = (salariedPercentage / totalEmployees) * 100;
                partTimePercentage = (partTimePercentage / totalEmployees) * 100;
                wagesPercentage = (wagesPercentage / totalEmployees) * 100;
            }

            return (salariedPercentage, partTimePercentage, wagesPercentage);
        }

        static double CalculatePartTimeWeeklyPay(PartTime partTime)
        {
            return partTime.Rate * partTime.HoursWorked;
        }

        static double CalculateWagesWeeklyPay(Wages wages)
        {
            double regularPay = Math.Min(wages.Rate * wages.HoursWorked, 40 * wages.Rate);
            double overtimePay = Math.Max(0, wages.Rate * (wages.HoursWorked - 40) * 1.5);

            return regularPay + overtimePay;
        }
    }
}
