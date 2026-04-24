using NLog;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using NorthwindConsole.Model;
using System.ComponentModel.DataAnnotations;
string path = Directory.GetCurrentDirectory() + "//nlog.config";

// create instance of Logger
var logger = LogManager.Setup().LoadConfigurationFromFile(path).GetCurrentClassLogger();

logger.Info("Program started");

do
{
    Console.WriteLine("1) Display categories");
    Console.WriteLine("2) Add category");
    Console.WriteLine("3) Display Category and related products");
    Console.WriteLine("4) Display all Categories and their related products");
    Console.WriteLine("5) Add a Product");
    Console.WriteLine("Enter to quit");
    string? choice = Console.ReadLine();
    Console.Clear();
    logger.Info("Option {choice} selected", choice);

    if (choice == "1")
    {
        // display categories
        var configuration = new ConfigurationBuilder()
                .AddJsonFile($"appsettings.json");
        var config = configuration.Build();

        var db = new DataContext();
        var query = db.Categories.OrderBy(p => p.CategoryName);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{query.Count()} records returned");
        Console.ForegroundColor = ConsoleColor.Magenta;
        foreach (var item in query)
        {
            Console.WriteLine($"{item.CategoryName} - {item.Description}");
        }
        Console.ForegroundColor = ConsoleColor.White;
    }
    else if (choice == "2")
    {
        // Add category
        Category category = new();
        Console.WriteLine("Enter Category Name:");
        category.CategoryName = Console.ReadLine()!;
        Console.WriteLine("Enter the Category Description:");
        category.Description = Console.ReadLine();
        ValidationContext context = new ValidationContext(category, null, null);
        List<ValidationResult> results = new List<ValidationResult>();

        var isValid = Validator.TryValidateObject(category, context, results, true);
        if (isValid)
        {
            var db = new DataContext();
            // check for unique name
            if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
            {
                // generate validation error
                isValid = false;
                results.Add(new ValidationResult("Name exists", ["CategoryName"]));
            }
            else
            {
                logger.Info("Validation passed");
                db.AddCategory(category);
            }
        }
        if (!isValid)
        {
            foreach (var result in results)
            {
                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
            }
        }
    }
    else if (choice == "3")
    {
        var db = new DataContext();
        var query = db.Categories.OrderBy(p => p.CategoryId);

        Console.WriteLine("Select the category whose products you want to display:");
        Console.ForegroundColor = ConsoleColor.DarkRed;
        foreach (var item in query)
        {
            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
        }
        Console.ForegroundColor = ConsoleColor.White;
        int id = int.Parse(Console.ReadLine()!);
        Console.Clear();
        logger.Info($"CategoryId {id} selected");
        Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id)!;
        Console.WriteLine($"{category.CategoryName} - {category.Description}");
        foreach (Product p in category.Products)
        {
            Console.WriteLine($"\t{p.ProductName}");
        }
    }
    else if (choice == "4")
    {
        var db = new DataContext();
        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
        foreach (var item in query)
        {
            Console.WriteLine($"{item.CategoryName}");
            foreach (Product p in item.Products)
            {
                Console.WriteLine($"\t{p.ProductName}");
            }
        }
    }
    else if (choice == "5")
    {
        AddProduct(logger);
        

    }
    else if (choice == "6")
    {
        // var db = new DataContext();
        // Console.WriteLine(GetCategory(db, "test").CategoryName);
        // Console.WriteLine(GetProduct(db).ProductName);
        // Console.WriteLine(GetSupplier(db).CompanyName);
    }
    else if (String.IsNullOrEmpty(choice))
    {
        break;
    }
    Console.WriteLine();
} while (true);

static Category? GetCategory(DataContext db, string prompt)
{
    // display all blogs
    var categories = db.Categories.OrderBy(b => b.CategoryId);//takes the datacontext(basically the database information), takes the blogs in order of their blodID and assigns them to the blogs variable
                                                              //need to use .Include(b => b.Posts) somewhere if I wanted posts due to lazy loading
    foreach (Category c in categories) //loops throught the list?(array maybe?) and writes out the id and name of each blog(b is the current blog in the loop)
    {//correction: blogs is the query typed out earlier, not the actual results of the query. the query is ran whenever blogs is called
        Console.WriteLine($"{c.CategoryId}: {c.CategoryName}");
    }
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"What category will the {prompt} be a part of?");
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("Selection:");
    if (int.TryParse(Console.ReadLine(), out int categoryID)) //.int converts to int. tryparse returns two values, a boolean true if no error is thrown, false if error is thrown, if successful
                                                              //the value of the console readline is assigned to the new variable int BlogID
    {// the ! is Bang opperator: tells cs to shut up about maybe returning a null value
        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == categoryID)!; //finds the blog with matching blog id, done this way so user doesnt have to enter a blog name and can instead enter the blog # printed before
        return category; //returns whatever blog OBJECT matches the blogid
    }
    return null;
}
static Product? GetProduct(DataContext db, string prompt)
{
    var products = db.Products.OrderBy(p => p.ProductId);
    foreach (Product p in products)
    {
        Console.WriteLine($"{p.ProductId}: {p.ProductName}");
    }
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"What product will the {prompt} be?");
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("Selection:");
    if (int.TryParse(Console.ReadLine(), out int productID))
    {
        Product product = db.Products.FirstOrDefault(p => p.ProductId == productID)!;
        return product;
    }
    return null;
}

static Supplier? GetSupplier(DataContext db, string prompt)
{
    var suppliers = db.Suppliers.OrderBy(s => s.SupplierId);
    foreach (Supplier s in suppliers)
    {
        Console.WriteLine($"{s.SupplierId}: {s.CompanyName}");
    }
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine($"What supplier will the {prompt} be?");
    Console.ForegroundColor = ConsoleColor.White;
    Console.Write("Selection:");
    if (int.TryParse(Console.ReadLine(), out int supplierID))
    {
        Supplier supplier = db.Suppliers.FirstOrDefault(s => s.SupplierId == supplierID)!;
        return supplier;
    }
    return null;
}
static void AddProduct(Logger logger)
{
    var db = new DataContext();
        var query = db.Products.OrderBy(p => p.ProductId);
        string functionString = "product";
        Product productToAdd = new();

        while (true)
        {
            do
            {
                Console.WriteLine("Enter the name of the product:");
                productToAdd.ProductName = Console.ReadLine();
            } while (string.IsNullOrEmpty(productToAdd.ProductName));
            if (!query.Any(q => q.ProductName.ToLower() == productToAdd.ProductName.ToLower())) //bug if product is ever null
            {
                break;
            }
            logger.Info($"{productToAdd.ProductName} already exists");
        }
        productToAdd.Category = GetCategory(db, functionString);
        if (productToAdd.Category == null)
        {
            logger.Error("User failed to select a valid category. Aborting add.");
            return;
        }
        productToAdd.Supplier = GetSupplier(db, functionString);
        if (productToAdd.Supplier == null)
        {
            logger.Error("User failed to select a valid supplier. Aborting add.");
            return;
        }
        try
        {
            Console.WriteLine("Enter the Unit Price of the product:");
            productToAdd.UnitPrice = Math.Round(Convert.ToDecimal(Console.ReadLine().Replace("$", "")), 2);
        }
        catch (FormatException)
        {
            logger.Error("User entered an invalid format for the unit price.");
            return;
        }
        
        Console.WriteLine("Enter the quantity of the product in stock:");
        productToAdd.UnitsInStock = short.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter the quantity of the product on order:");
        productToAdd.UnitsOnOrder = short.Parse(Console.ReadLine()!);
        Console.WriteLine("Enter the quantity of the product needed to trigger a reorder:");
        productToAdd.ReorderLevel = short.Parse(Console.ReadLine()!);
        Console.WriteLine("Quantity per unit (ex: 24 - 12 oz bottles):");
        productToAdd.QuantityPerUnit = Console.ReadLine();
        Console.WriteLine("Is the product discontinued? (y/n)");
        string discontinued = Console.ReadLine()!;
        if (discontinued.ToLower() == "y")
        {
            productToAdd.Discontinued = true;
        }
        else
        {
            productToAdd.Discontinued = false;
        }
        db.AddProduct(productToAdd);
}
logger.Info("Program ended");