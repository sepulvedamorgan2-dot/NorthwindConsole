using NLog;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using NorthwindConsole.Model;
using System.ComponentModel.DataAnnotations;
using Microsoft.IdentityModel.Tokens;

public class CategoryFunctionality
{
    public static void AllCategoryRelatedProducts(Logger logger)
    {
        var db = new DataContext();
        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
        foreach (var item in query)
        {
            Console.WriteLine($"{item.CategoryName}");
            foreach (Product p in item.Products)
            {
                if (p.DateDeleted is null)
                {
                    Console.WriteLine($"\t{p.ProductName}");
                }
            }
        }
    }
    public static void SingleCategoryRelatedProducts(Logger logger)
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
        logger.Info("User selected category with ID {id} to display related products", id);
        Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id)!;
        Console.WriteLine($"{category.CategoryName} - {category.Description}");
        foreach (Product p in category.Products)
        {
            if (p.DateDeleted is null)
            {
                Console.WriteLine($"\t{p.ProductName}");
            }
        }
    }
    public static void AddCategory(Logger logger)
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
    public static void DisplayCategories(Logger logger)
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

    public static void EditCategory(Logger logger)
    {

        var db = new DataContext();
        var query = db.Categories.OrderBy(p => p.CategoryId);
        string functionString = "edit";
        Category category = GetTable.GetCategory(db, functionString)!;
        //color change to red for the user input
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Choosen Category: ");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"{category.CategoryId}. {category.CategoryName}\n{category.Description}");
        Console.WriteLine("Do you want to change the Category Name (y) or (n):");
        if (Console.ReadLine() == "y")
        {
            string input;
            Console.WriteLine($"Current Name: {category.CategoryName}");
            while (true)
            {

                Console.WriteLine("Enter new category name: ");
                category.CategoryName = Console.ReadLine();
                if (!query.Any(q => q.CategoryName == category.CategoryName))
                {
                    break;
                }
                logger.Error("Error: Category Name Already Exists");
            }

        }
        Console.WriteLine("Do you want to change the Category Description (y) or (n):");
        if (Console.ReadLine() == "y")
        {
            Console.WriteLine($"Current Description: {category.Description}");
            while (true)
            {
                Console.WriteLine("Enter new category name: ");
                category.CategoryName = Console.ReadLine();
                if (!category.CategoryName.IsNullOrEmpty())
                {
                    break;
                }
                Console.WriteLine("Must Enter Value");
            }
        }
        db.EditCategory(category);
    }




}


