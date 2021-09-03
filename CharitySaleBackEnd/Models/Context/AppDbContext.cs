/// ==========================================
///  Title:     Charity-Sale-BackEnd
///  Author:    Jevgeni Kostenko
///  Date:      29.08.2021
/// ==========================================
/// 
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CharitySaleBackEnd.Models.Context
{
    public class AppDbContext : DbContext
    {
        public DbSet<CategoryItem> Categories { get; set; }
        public DbSet<ProductItem> Products { get; set; }

        public AppDbContext(DbContextOptions options) : base(options) { }

        public bool SaveProductsToDatabase(IEnumerable<ProductItem> productList)
        {
            bool successDbUpdate = false;
            try
            {
                foreach (var item in productList)
                {
                    // check ProductItem - is valid item? If NOT then ignore item
                    if (!item.IsValidProduct())
                        continue;
                    successDbUpdate = true;
                    int productCategoryId = GetCategoryIdByName(item.CategoryName);

                    // if has category in database, then use existing
                    if (productCategoryId != -1 && !String.IsNullOrEmpty(item.CategoryName))
                    {
                        // use existing category ID
                        item.CategoryId = productCategoryId;
                    }
                    else if (productCategoryId == -1 && !String.IsNullOrEmpty(item.CategoryName))
                    {
                        // if NOT EMPTY category name, but has`t in database
                        // create new CategoryItem
                        var newCategory = Categories.Add(new CategoryItem() { Name = item.CategoryName }).Entity;
                        SaveChanges();
                        item.CategoryId = newCategory.Id;
                    }
                    Products.Add(item);
                }
            }
            catch (Exception)
            {
                return false;
            }
            return successDbUpdate;
        }

        public bool ProductItemExists(ProductItem product)
        {
            return Products.Any(e => e.Id == product.Id);
        }


        public ProductItem GetProduct(ProductItem product)
        {
            return Products.Where(dbitem => dbitem.Id == product.Id).First();
        }

        private int GetCategoryIdByName(string categoryName)
        {
            var catId = Categories.Where(cat => cat.Name == categoryName)?.FirstOrDefault();
            return catId != null ? catId.Id : -1;
        }
    }
}
