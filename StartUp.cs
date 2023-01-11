
namespace ProductShop
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using ProductShop.Data;
    using ProductShop.DTOs.User;
    using ProductShop.Models;


    using AutoMapper;
    using Newtonsoft.Json;
    using ProductShop.DTOs.Product;
    using System.ComponentModel.DataAnnotations;
    using ProductShop.DTOs.Category;
    using ProductShop.DTOs.Categories_Products;
    using Microsoft.EntityFrameworkCore;
    using AutoMapper.QueryableExtensions;
    using Newtonsoft.Json.Serialization;

    public class StartUp
    {

        private static string filePath;

        public static void Main(string[] args)
        {

            Mapper.Initialize(cfg => cfg.AddProfile(typeof(ProductShopProfile)));
            ProductShopContext dbContext = new ProductShopContext();

            InitializeOutputFilePath("users-and-products.json");
            //InitializeOutputFilePath("categories-by-products.json");
            // InitializeOutputFilePath("users-sold-products.json");
            // InitializeFilePath("users.json");
            // InitializeFilePath("products.json");
            // InitializeFilePath("categories.json");
            // InitializeFilePath("categories-products.json");


            //dbContext.Database.EnsureDeleted();
            //dbContext.Database.EnsureCreated();
            //Console.WriteLine("Database was created!");

            // var inputJson = File.ReadAllText(filePath);

            // Console.WriteLine(ImportUsers(dbContext, inputJson));
            // Console.WriteLine(ImportProducts(dbContext, inputJson));
            // Console.WriteLine(ImportCategories(dbContext,inputJson));
            // Console.WriteLine(ImportCategoryProducts(dbContext,inputJson));
            // Console.WriteLine(String.Join(" ", GetProductsInRange(dbContext)));
            //Console.WriteLine(String.Join(" ", GetSoldProducts(dbContext)));

            var json = GetUsersWithProducts(dbContext);
            File.WriteAllText(filePath, json);
        }

        //import 
        //01 Import Users
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {

            var userDtos = JsonConvert.DeserializeObject<List<ImportUserDto>>(inputJson);

            ICollection<User> users = new List<User>();

            foreach (var uDto in userDtos)
            {
                if (!IsValid(uDto)) continue;

                User user = Mapper.Map<User>(uDto);
                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";

        }
        //02 Import Products
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var productDtos = JsonConvert.DeserializeObject<List<ImportProductDto>>(inputJson);
            ICollection<Product> products = new List<Product>();

            foreach (var pDto in productDtos)
            {
                if (!IsValid(pDto)) continue;

                Product product = Mapper.Map<Product>(pDto);
                products.Add(product);

            }
            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";

        }

        //03 Import Categories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            //JsonSerializerSettings settings = new JsonSerializerSettings()
            //{                
            //    NullValueHandling = NullValueHandling.Ignore,

            //};

            var categoriesDtos = JsonConvert.DeserializeObject<List<ImportCategoryDto>>(inputJson);
            ICollection<Category> categories = new List<Category>();

            foreach (var cDto in categoriesDtos)
            {
                if (!IsValid(cDto)) continue;

                Category category = Mapper.Map<Category>(cDto);
                categories.Add(category);
            }
            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";

        }

        //04 Import Category and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoriesProductsDtos = JsonConvert.DeserializeObject<List<ImportCategoryProductDto>>(inputJson);
            ICollection<CategoryProduct> validCP = new List<CategoryProduct>();

            foreach (var cpDto in categoriesProductsDtos)
            {
                if (!IsValid(cpDto)) continue;

                if (!context.Categories.Any(c => c.Id == cpDto.CategoryId) ||
                    !context.Products.Any(p => p.Id == cpDto.ProductId)) continue;

                CategoryProduct categoryProduct = Mapper.Map<CategoryProduct>(cpDto);
                validCP.Add(categoryProduct);

            }

            context.CategoryProducts.AddRange(validCP);
            context.SaveChanges();

            return $"Successfully imported {validCP.Count}";

        }

        //export 
        //05 Export Products in Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                    .Where(p => p.Price >= 500 && p.Price <= 1000)
                    .OrderBy(p => p.Price)
                    .ProjectTo<ExportProductsInRange>()
                    .ToArray();

            var json = JsonConvert.SerializeObject(products, Formatting.Indented);
            return json;
        }

        //06 Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                   .Where(u => u.ProductsSold.Any(p => p.BuyerId.HasValue))
                   .OrderBy(u => u.LastName)
                   .ThenBy(u => u.FirstName)
                   .ProjectTo<ExportUsersWithSoldProductsDto>()
                   .ToArray();


            var json = JsonConvert.SerializeObject(users, Formatting.Indented);
            return json;

        }

        //07 Export Categories by Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Include(c => c.CategoryProducts)
                    .Select(c => new
                    {
                        category = c.Name,
                        productsCount = c.CategoryProducts.Count(),
                        averagePrice = c.CategoryProducts.Average(c => c.Product.Price).ToString("F2"),
                        totalRevenue = c.CategoryProducts.Sum(c => c.Product.Price).ToString("F2")

                    })
                    .OrderByDescending(p => p.productsCount)
                    .ToArray();

            var json = JsonConvert.SerializeObject(categories, Formatting.Indented);
            return json;

        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            //var users = context.Users
            //    .Where(u => u.ProductsSold.Any(ps => ps.BuyerId != null))
            //    .Include(u => u.ProductsSold)
            //    .ToList()
            //    .Select(u => new
            //    {
            //        u.FirstName,
            //        u.LastName,
            //        u.Age,
            //        soldProducts = new
            //        {
            //            count = u.ProductsSold
            //            .Where(p => p.BuyerId != null)
            //            .Count(),
            //            products = u.ProductsSold
            //            .Where(p => p.BuyerId != null)
            //            .Select(p => new
            //            {
            //                p.Name,
            //                p.Price
            //            })
            //            .ToList()
            //        }

            //    })
            //    .OrderByDescending(u => u.soldProducts.count)
            //    .ToList();


            var users = context.Users
                .Where(x => x.ProductsSold.Any(x => x.BuyerId.HasValue))
                .OrderByDescending(x => x.ProductsSold.Count)
                .ProjectTo<ExportUserFullInfo>()
                .ToList();


            var withUserCount = new
            {
                usersCount = users.Count,
                users,

            };




            var json = JsonConvert.SerializeObject(withUserCount, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });
            return json;

            

        }


        //helpers methods
        private static void InitializeFilePath(string fileName)
        {
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../Datasets/", fileName);

        }

        private static void InitializeOutputFilePath(string fileName)
        {
            filePath = Path.Combine(Directory.GetCurrentDirectory(), "../../../Results/", fileName);
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();
            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult);

            return isValid;
        }
    }
}
//DefaultContractResolver defaultContractResolver = new DefaultContractResolver()
//{
//    NamingStrategy = new KebabCaseNamingStrategy(),

//};

//JsonSerializerSettings settings = new JsonSerializerSettings()
//{
//    ContractResolver = defaultContractResolver,
//    NullValueHandling = NullValueHandling.Ignore,
//    Formatting = Formatting.Indented,

//};