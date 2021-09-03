using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CharitySaleBackEnd.Models;
using CharitySaleBackEnd.Models.Context;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using CharitySaleBackEnd.Services;

namespace CharitySaleBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductItemsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _hostEnv;

        public ProductItemsController(AppDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _db = context;
            _hostEnv = hostingEnvironment;
        }

        /// <summary>
        /// Get a list of all products
        /// </summary>
        /// <returns></returns>
        // GET: api/ProductItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductItem>>> GetProducts()
        {
            return await _db.Products.ToListAsync();
        }

        /// <summary>
        /// Get a product by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/ProductItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductItem>> GetProductItem(int id)
        {
            var productItem = await _db.Products.FindAsync(id);

            if (productItem == null)
            {
                return NotFound();
            }

            return productItem;
        }

        /// <summary>
        /// Change product (quantity in stock) by id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="productItem"></param>
        /// <returns>Empty response with status code</returns>
        /// PUT: api/ProductItems/5
        /// <response code="201">Successful item change</response>
        /// <response code="204">Database has not changed</response>     
        /// <response code="404">Database error</response>     
        [ProducesResponseType(201)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductItem(int id, ProductItem productItem)
        {
            // Stock count management
            if (id != productItem.Id)
            {
                return BadRequest();
            }

            bool isValidProduct = !String.IsNullOrEmpty(productItem.Name) && productItem.Price >= 0 && productItem.Count >= 0 && _db.ProductItemExists(productItem);
            if (isValidProduct)
                _db.Entry(productItem).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            if (isValidProduct)
                return StatusCode(201);

            return NoContent();
        }

        /// <summary>
        /// Checkout product list of order
        /// </summary>
        /// <param name="productItems"></param>
        /// <returns>Empty response with status code</returns>
        /// PUT: api/ProductItems/5
        /// <response code="201">Successful order - number of items in stock has been updated</response>
        /// <response code="205">Not enough goods in stock</response>     
        /// <response code="404">Database error</response>     
        [ProducesResponseType(201)]
        [ProducesResponseType(205)]
        [ProducesResponseType(404)]
        [HttpPut]
        public async Task<IActionResult> PutProductItem([FromBody] Products productItems)
        {
            // Checkout the order
            Products missingProducts = new();
            foreach (var item in productItems)
            {
                // get from db existing entity
                // if exists, then change count
                //var dbItem = _db.GetProduct(item);
                var dbItem = _db.Products.Where(dbitem => dbitem.Id == item.Id).FirstOrDefault();
                if (dbItem != null)
                {
                    // check if available needed quantity of item
                    int balance = dbItem.Count - item.Count;
                    if (balance >= 0)
                        dbItem.Count = balance;
                    else
                        missingProducts.Add(dbItem);
                }
            }

            try
            {
                if (missingProducts.Count == 0)
                    await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }

            if (missingProducts.Count == 0)
                return StatusCode(201);

            return StatusCode(205);
        }

        /// <summary>
        /// Post a new product
        /// </summary>
        /// <param name="productItem"></param>
        /// <returns>Empty response with status code</returns>
        /// POST: api/ProductItems
        /// <response code="201">Item published successfully</response>
        /// <response code="205">Incorrect incoming data</response>     
        /// <response code="404">Database error</response>     
        [ProducesResponseType(201)]
        [ProducesResponseType(205)]
        [ProducesResponseType(404)]
        [HttpPost]
        public async Task<ActionResult<ProductItem>> PostProductItem([FromBody] ProductItem productItem)
        {
            // Post new product
            Debug.WriteLine("PostProductItem: " + productItem);
            bool isValidProduct = productItem.IsValidProduct();
            if (isValidProduct)
            {
                DataFile dataFile = new DataFile(productItem.ImageFile);
                if (dataFile.FileSourceIsCorrect)
                {
                    string imagesFilePath = Path.Combine(_hostEnv.ContentRootPath, "uploads", "images");
                    productItem.PreviewImageFileName = CreateImageFile(dataFile, imagesFilePath);
                }

                try
                {
                    _db.Products.Add(productItem);
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
            }

            if (isValidProduct)
                return StatusCode(201);

            return StatusCode(205);
        }

        /// <summary>
        /// Delete product by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Empty response with status code</returns>
        // DELETE: api/ProductItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductItem(int id)
        {
            var productItem = await _db.Products.FindAsync(id);
            if (productItem == null)
            {
                return NotFound();
            }

            _db.Products.Remove(productItem);
            await _db.SaveChangesAsync();

            return NoContent();
        }


        private string CreateImageFile(DataFile dataFile, string path)
        {
            string uniqueString = System.Guid.NewGuid().ToString();
            string imgFileName = uniqueString + "." + dataFile.FileExtension;
            try { System.IO.File.WriteAllBytes(Path.Combine(path, imgFileName), dataFile.FileBinData); }
            catch (Exception) { return string.Empty; }
            return imgFileName;
        }
    }
}
