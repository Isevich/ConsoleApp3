using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

class Program
{
    // ======= ШЛЯХИ ДО ФАЙЛІВ =======
    static string productPath = "products.csv";
    static string userPath = "users.csv";

    static void Main()
    {
        InitFiles();

        Console.WriteLine("1 - Реєстрація");
        Console.WriteLine("2 - Вхід");
        Console.Write("Ваш вибір: ");
        string choice = Console.ReadLine();

        if (choice == "1")
            Register();

        if (choice == "2")
        {
            if (!Login())
            {
                Console.WriteLine("❌ Невірний email або пароль");
                return;
            }
        }

        // ======= МЕНЮ ТОВАРІВ =======
        while (true)
        {
            Console.WriteLine("\n1 - Додати товар");
            Console.WriteLine("2 - Показати всі товари");
            Console.WriteLine("3 - Статистика");
            Console.WriteLine("0 - Вихід");
            Console.Write("Вибір: ");

            string cmd = Console.ReadLine();

            if (cmd == "1") AddProduct();
            if (cmd == "2") ShowProducts();
            if (cmd == "3") ShowStatistics();
            if (cmd == "0") break;
        }
    }

    // ======= ІНІЦІАЛІЗАЦІЯ ФАЙЛІВ =======
    static void InitFiles()
    {
        if (!File.Exists(productPath))
            File.WriteAllText(productPath, "Id,Name,Price,Category\n");

        if (!File.Exists(userPath))
            File.WriteAllText(userPath, "Id,Email,Password\n");
    }

    // ======= ГЕНЕРАЦІЯ ID =======
    static int GenerateId(string path)
    {
        if (!File.Exists(path)) return 1;

        var lines = File.ReadAllLines(path).Skip(1);
        int max = 0;

        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length == 0) continue;

            if (int.TryParse(parts[0], out int id))
                if (id > max) max = id;
        }

        return max + 1;
    }

    // ======= РЕЄСТРАЦІЯ =======
    static void Register()
    {
        Console.Write("Email: ");
        string email = Console.ReadLine();

        Console.Write("Пароль: ");
        string password = Console.ReadLine();

        var lines = File.ReadAllLines(userPath).Skip(1);

        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length < 2) continue;

            if (parts[1] == email)
            {
                Console.WriteLine("❌ Такий email вже існує");
                return;
            }
        }

        int id = GenerateId(userPath);
        File.AppendAllText(userPath, $"{id},{email},{Hash(password)}\n");

        Console.WriteLine("✅ Реєстрація успішна");
    }

    // ======= ВХІД =======
    static bool Login()
    {
        Console.Write("Email: ");
        string email = Console.ReadLine();

        Console.Write("Пароль: ");
        string password = Console.ReadLine();

        string hash = Hash(password);
        var lines = File.ReadAllLines(userPath).Skip(1);

        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length != 3) continue;

            if (parts[1] == email && parts[2] == hash)
                return true;
        }

        return false;
    }

    // ======= ХЕШУВАННЯ ПАРОЛЯ =======
    static string Hash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToBase64String(bytes);
    }

    // ======= ДОДАТИ ТОВАР =======
    static void AddProduct()
    {
        Console.Write("Назва: ");
        string name = Console.ReadLine();

        Console.Write("Ціна: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal price))
        {
            Console.WriteLine("❌ Некоректна ціна");
            return;
        }

        Console.Write("Категорія: ");
        string category = Console.ReadLine();

        int id = GenerateId(productPath);
        File.AppendAllText(productPath, $"{id},{name},{price},{category}\n");

        Console.WriteLine("✅ Товар додано");
    }

    // ======= ПОКАЗАТИ ТОВАРИ =======
    static void ShowProducts()
    {
        var lines = File.ReadAllLines(productPath).Skip(1);

        Console.WriteLine("\nID | Name | Price | Category");
        Console.WriteLine("--------------------------------");

        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length != 4) continue;

            if (!int.TryParse(parts[0], out int id)) continue;
            if (!decimal.TryParse(parts[2], out decimal price)) continue;

            Console.WriteLine($"{id} | {parts[1]} | {price} | {parts[3]}");
        }
    }

    // ======= СТАТИСТИКА =======
    static void ShowStatistics()
    {
        var prices = new List<decimal>();
        var lines = File.ReadAllLines(productPath).Skip(1);

        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length != 4) continue;

            if (decimal.TryParse(parts[2], out decimal price))
                prices.Add(price);
        }

        if (prices.Count == 0)
        {
            Console.WriteLine("Немає даних");
            return;
        }

        Console.WriteLine($"\nКількість: {prices.Count}");
        Console.WriteLine($"Мінімум: {prices.Min()}");
        Console.WriteLine($"Максимум: {prices.Max()}");
        Console.WriteLine($"Сума: {prices.Sum()}");
        Console.WriteLine($"Середнє: {prices.Average()}");
    }
}
