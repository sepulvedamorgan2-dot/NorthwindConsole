using NLog;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using NorthwindConsole.Model;
using System.ComponentModel.DataAnnotations;

public class ProductFunctionality
{

    public static void AddProduct(Logger logger)
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
        productToAdd.Category = GetTable.GetCategory(db, functionString);
        if (productToAdd.Category == null)
        {
            logger.Error("User failed to select a valid category. Aborting add.");
            return;
        }
        productToAdd.Supplier = GetTable.GetSupplier(db, functionString);
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
    public static void EditProduct(Logger logger)
    {
        var db = new DataContext();
        var query = db.Products.OrderBy(p => p.ProductId);
        string functionString = "edited record";
        Product productToEdit = GetTable.GetProduct(db, functionString);
        Console.WriteLine("Do you want to edit the product name? (y/n)");
        string editName = Console.ReadLine()!;
        if (editName.ToLower() == "y")
        {
            while (true)
            {
                do
                {
                    Console.WriteLine("Enter the name of the product:");
                    productToEdit.ProductName = Console.ReadLine();
                } while (string.IsNullOrEmpty(productToEdit.ProductName));
                if (!query.Any(q => q.ProductName.ToLower() == productToEdit.ProductName.ToLower())) //bug if product is ever null
                {
                    break;
                }
                logger.Info($"{productToEdit.ProductName} already exists");
            }
        }
        Console.WriteLine("Do you want to edit the category? (y/n)");
        string editCategory = Console.ReadLine()!;
        if (editCategory.ToLower() == "y")
        {
            productToEdit.Category = GetTable.GetCategory(db, functionString);
            if (productToEdit.Category == null)
            {
                logger.Error("User failed to select a valid category. Aborting add.");
                return;
            }
        }
        Console.WriteLine("Do you want to edit the supplier? (y/n)");
        string editSupplier = Console.ReadLine()!;
        if (editSupplier.ToLower() == "y")
        {
            productToEdit.Supplier = GetTable.GetSupplier(db, functionString);
            if (productToEdit.Supplier == null)
            {
                logger.Error("User failed to select a valid supplier. Aborting add.");
                return;
            }
        }
        Console.WriteLine("Do you want to edit the unit price? (y/n)");
        string editUnitPrice = Console.ReadLine()!;
        if (editUnitPrice.ToLower() == "y")
        {
            try
            {
                Console.WriteLine("Enter the Unit Price of the product:");
                productToEdit.UnitPrice = Math.Round(Convert.ToDecimal(Console.ReadLine().Replace("$", "")), 2);
            }
            catch (FormatException)
            {
                logger.Error("User entered an invalid format for the unit price.");
                return;
            }
        }
        Console.WriteLine("Do you want to edit the quantity in stock? (y/n)");
        string editStock = Console.ReadLine()!;
        if (editStock.ToLower() == "y")
        {
            Console.WriteLine("Enter the quantity of the product in stock:");
            productToEdit.UnitsInStock = short.Parse(Console.ReadLine()!);
        }
        Console.WriteLine("Do you want to edit the quantity on order? (y/n)");
        string editOnOrder = Console.ReadLine()!;
        if (editOnOrder.ToLower() == "y")
        {
            Console.WriteLine("Enter the quantity of the product on order:");
            productToEdit.UnitsOnOrder = short.Parse(Console.ReadLine()!);
        }
        Console.WriteLine("Do you want to edit the reorder level? (y/n)");
        string editReorder = Console.ReadLine()!;
        if (editReorder.ToLower() == "y")
        {
            Console.WriteLine("Enter the quantity of the product needed to trigger a reorder:");
            productToEdit.ReorderLevel = short.Parse(Console.ReadLine()!);
        }
        Console.WriteLine("Do you want to edit the quantity per unit? (y/n)");
        string editQuantityPerUnit = Console.ReadLine()!;
        if (editQuantityPerUnit.ToLower() == "y")
        {
            Console.WriteLine("Quantity per unit (ex: 24 - 12 oz bottles):");
            productToEdit.QuantityPerUnit = Console.ReadLine();
        }
        Console.WriteLine("Do you want to edit whether the product is discontinued? (y/n)");
        string editDiscontinued = Console.ReadLine()!;
        if (editDiscontinued.ToLower() == "y")
        {
            Console.WriteLine("Is the product discontinued? (y/n)");
            string discontinued = Console.ReadLine()!;
            if (discontinued.ToLower() == "y")
            {
                productToEdit.Discontinued = true;
            }
            else
            {
                productToEdit.Discontinued = false;
            }
        }

        // db.SaveChanges();

        db.EditProduct(productToEdit);
    }
    public static void CustomProductDisplay(Logger logger)
    {
        var db = new DataContext();
        var query = db.Products.OrderBy(p => p.ProductId);
        string functionString = "edited record";
        while (true)
        {
            Console.WriteLine("1) Display all products\n2) Display discontinued products only\n3) Display non-discontinued products only\nEnter to quit");
            string choice = Console.ReadLine()!;
            if (choice == "1")
            {
                foreach (var product in query)
                {
                    if (product.Discontinued == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{product.ProductId} - {product.ProductName} // DISCONTINUED");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{product.ProductId} - {product.ProductName}");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            else if (choice == "2")
            {
                foreach (var product in query)
                {
                    if (product.Discontinued == true)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{product.ProductId} - {product.ProductName} // DISCONTINUED");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            else if (choice == "3")
            {
                foreach (var product in query)
                {
                    if (product.Discontinued == false)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{product.ProductId} - {product.ProductName} // STILL CARRIED");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }


            }

            else
            {
                break;
            }
            Console.WriteLine("Enter to continue");
            Console.ReadLine();
        }
    }
    public static void FullProductInfo(Logger logger)
    {
        var db = new DataContext();
        var query = db.Products.OrderBy(p => p.ProductId);
        string functionString = "selection";
        Product productToDisplay = GetTable.GetProduct(db, functionString);
        Console.WriteLine("Product Information:");
        Console.WriteLine("-------------------");
        Console.WriteLine($"ID: {productToDisplay.ProductId}");
        Console.ForegroundColor = ConsoleColor.DarkCyan;   
        Console.WriteLine($"Name: {productToDisplay.ProductName}");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine($"Category: {productToDisplay.Category.CategoryName}");
        Console.WriteLine($"Supplier: {productToDisplay.Supplier.CompanyName}");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"Unit Price: {productToDisplay.UnitPrice:C}");
        Console.ForegroundColor = ConsoleColor.White;
        if (productToDisplay.UnitsInStock > 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Units in Stock: {productToDisplay.UnitsInStock}");
            Console.ForegroundColor = ConsoleColor.White;
        }
        else if (productToDisplay.UnitsInStock == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Units in Stock: {productToDisplay.UnitsInStock}");
            Console.ForegroundColor = ConsoleColor.White;
        }
        else
        {
            Console.WriteLine($"Units in Stock: {productToDisplay.UnitsInStock}");
        }
        Console.WriteLine($"Units on Order: {productToDisplay.UnitsOnOrder}");
        Console.WriteLine($"Reorder Level: {productToDisplay.ReorderLevel}");
        Console.WriteLine($"Quantity per Unit: {productToDisplay.QuantityPerUnit}");
        if (productToDisplay.Discontinued == true)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Discontinued: {productToDisplay.Discontinued}");
            Console.ForegroundColor = ConsoleColor.White;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Discontinued: {productToDisplay.Discontinued}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        Console.WriteLine("Press enter to continue");
        Console.ReadLine();
    }
}
