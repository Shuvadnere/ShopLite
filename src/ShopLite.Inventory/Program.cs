//Console.WriteLine("Hello, World!");
using System;
using System.Collections.Generic;
using System.Linq;

// In-memory collection initialized with sample products
List<Product> products = new()
{
    new Product(1, "Mechanical Keyboard", 89.99m, 15),
    new Product(2, "Wireless Mouse", 45.50m, 30),
    new Product(3, "USB-C Hub", 29.99m, 50)
};

int nextId = 4;
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

static void AddProduct(List<Product> products, ref int nextId)
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

    Product newProduct = new(nextId++, name, price, stock);
    products.Add(newProduct);
    Console.WriteLine($"Added: {newProduct}");
}

static void ListProducts(List<Product> products)
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

static void SearchProducts(List<Product> products)
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