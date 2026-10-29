namespace HospitalAppointmentEFCore.Helper;

public static class ConsoleHelper
{
    public static string ReadRequired(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            var value = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(value)) return value.Trim();
            Console.WriteLine("Input cannot be empty.");
        }
    }

    public static int ReadPositiveInt(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            if (int.TryParse(Console.ReadLine(), out int value) && value > 0) return value;
            Console.WriteLine("Enter a positive whole number.");
        }
    }

    public static int ReadAge()
    {
        while (true)
        {
            Console.Write("Enter patient age: ");
            if (int.TryParse(Console.ReadLine(), out int age) && age >= 0 && age <= 120)
                return age;
            Console.WriteLine("Age must be between 0 and 120.");
        }
    }

    public static DateTime ReadFutureDate()
    {
        while (true)
        {
            Console.Write("Appointment date and time (yyyy-MM-dd HH:mm): ");
            if (DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd HH:mm",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out var date)
                && date > DateTime.Now)
                return date;
            Console.WriteLine("Enter a future date in yyyy-MM-dd HH:mm format.");
        }
    }
}
