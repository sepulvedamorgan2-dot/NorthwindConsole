using NLog;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using NorthwindConsole.Model;
using System.ComponentModel.DataAnnotations;
public class GetTable
{

string path = Directory.GetCurrentDirectory() + "//nlog.config";


public static Category? GetCategory(DataContext db, string prompt)
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
public static Product? GetProduct(DataContext db, string prompt)
{
    var products = db.Products
    .Include(p => p.Category)
    .Include(p => p.Supplier)
    .OrderBy(p => p.ProductId);
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

public static Supplier? GetSupplier(DataContext db, string prompt)
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
}