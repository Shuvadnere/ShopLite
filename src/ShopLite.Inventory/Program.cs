//Console.WriteLine("Hello, World!");
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public class Program
{
    private static readonly string ProductsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "ShopLite",
        "products.json");

    public static void Main(string[] args)
    {
        List<Product> products;
        try
        {
            products = LoadProducts();
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            Console.WriteLine($"Could not load saved products: {ex.Message}");
            return;
        }

        int nextId = products.Count == 0 ? 1 : products.Max(product => product.Id) + 1;
        bool isRunning = true;

        while (isRunning)
        {
            Console.WriteLine("\n=== Product Inventory Menu ===");
            Console.WriteLine("1. Add Product");
            Console.WriteLine("2. List All Products");
            Console.WriteLine("3. Search Products");
            Console.WriteLine("4. Exit");
            Console.Write("Select an option: ");

            string? choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    AddProduct(products, ref nextId);
                    break;
                case "2":
                    ListProducts(products);
                    break;
                case "3":
                    SearchProducts(products);
                    break;
                case "4":
                    isRunning = false;
                    Console.WriteLine("Goodbye!");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please enter a number between 1 and 4.");
                    break;
            }
        }
    }

    private static List<Product> LoadProducts()
    {
        if (!File.Exists(ProductsFilePath))
        {
            // Use the sample inventory the first time the program runs.
            return new List<Product>
            {
                new Product(1, "Mechanical Keyboard", 89.99m, 15),
                new Product(2, "Wireless Mouse", 45.50m, 30),
                new Product(3, "USB-C Hub", 29.99m, 50),
                new Product(4, "27-inch Monitor", 199.99m, 10),
                new Product(5, "External SSD 1TB", 149.99m, 20),
                new Product(6, "Gaming Chair", 249.99m, 5),
                new Product(7, "Webcam 1080p", 59.99m, 25),
                new Product(8, "Bluetooth Headphones", 79.99m, 40),
                new Product(9, "Smartphone Stand", 19.99m, 100)
            };
        }

        return JsonSerializer.Deserialize<List<Product>>(File.ReadAllText(ProductsFilePath))
            ?? throw new JsonException("The saved product list is empty or invalid.");
    }

    private static void SaveProducts(List<Product> products)
    {
        string? directory = Path.GetDirectoryName(ProductsFilePath);
        if (directory is null)
        {
            throw new IOException("Could not determine where to save products.");
        }

        Directory.CreateDirectory(directory);
        File.WriteAllText(ProductsFilePath, JsonSerializer.Serialize(products, new JsonSerializerOptions
        {
            WriteIndented = true
        }));
    }

    private static void AddProduct(List<Product> products, ref int nextId)
    {
        Console.Write("Enter product name: ");
        string? name = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Product name cannot be empty.");
            return;
        }

        Console.Write("Enter price: ");
        if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price < 0)
        {
            Console.WriteLine("Invalid price.");
            return;
        }

        Console.Write("Enter stock quantity: ");
        if (!int.TryParse(Console.ReadLine(), out int stock) || stock < 0)
        {
            Console.WriteLine("Invalid stock quantity.");
            return;
        }

        Product newProduct = new(nextId, name, price, stock);
        products.Add(newProduct);

        try
        {
            SaveProducts(products);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            products.Remove(newProduct);
            Console.WriteLine($"Could not save product. It was not added: {ex.Message}");
            return;
        }

        nextId++;
        Console.WriteLine($"Added: {newProduct}");
    }

    private static void ListProducts(List<Product> products)
    {
        if (products.Count == 0)
        {
            Console.WriteLine("No products in inventory.");
            return;
        }

        Console.WriteLine("\nID  | Name                     | Price      | Stock");
        Console.WriteLine("----------------------------------------------------");
        foreach (Product product in products)
        {
            Console.WriteLine(product);
        }
    }

    private static void SearchProducts(List<Product> products)
    {
        Console.Write("Enter search term (name or ID): ");
        string? query = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(query))
        {
            Console.WriteLine("Search term cannot be empty.");
            return;
        }

        // Filter using LINQ and case-insensitive string comparison
        List<Product> results = products
            .Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                        p.Id.ToString() == query)
            .ToList();

        if (results.Count == 0)
        {
            Console.WriteLine("No matching products found.");
            return;
        }

        ListProducts(results);
    }
}

// Reference type (class) holding value types (int, decimal) and a string
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public Product(int id, string name, decimal price, int stock)
    {
        Id = id;
        Name = name;
        Price = price;
        Stock = stock;
    }

    public override string ToString() =>
        $"{Id,-3} | {Name,-24} | {Price,10:C} | {Stock,5}";
}