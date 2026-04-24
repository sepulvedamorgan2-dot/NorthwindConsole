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
    Console.WriteLine("6) Edit a Product");
    Console.WriteLine("7) Display all Product + discontinued filter");
    Console.WriteLine("8) Display specific product details");
    Console.WriteLine("Enter to quit");
    string? choice = Console.ReadLine();
    Console.Clear();
    logger.Info("Option {choice} selected", choice);

    if (choice == "1")
    {
        CategoryFunctionality.DisplayCategories(logger);
    }
    else if (choice == "2")
    {
        CategoryFunctionality.AddCategory(logger);
    }
    else if (choice == "3")
    {
        CategoryFunctionality.SingleCategoryRelatedProducts(logger);
    }
    else if (choice == "4")
    {
        CategoryFunctionality.AllCategoryRelatedProducts(logger);
    }
    else if (choice == "5")
    {
        ProductFunctionality.AddProduct(logger);
    }
    else if (choice == "6")
    {
        ProductFunctionality.EditProduct(logger);
    }
    else if (choice =="7")
    {
        ProductFunctionality.CustomProductDisplay(logger);
    }
    else if (choice =="8")
    {
        ProductFunctionality.FullProductInfo(logger);
    }
    else if (String.IsNullOrEmpty(choice))
    {
        break;
    }
    Console.WriteLine();
} while (true);

logger.Info("Program ended");