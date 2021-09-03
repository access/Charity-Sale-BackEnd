using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CharitySaleBackEnd.Models;
using CharitySaleBackEnd.Models.Context;
using Newtonsoft.Json;
using CharitySaleBackEnd.Services;

namespace CharitySaleBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BulkUploadController : ControllerBase
    {
        private readonly AppDbContext _db;

        public BulkUploadController(AppDbContext context)
        {
            _db = context;
        }

        /// <summary>
        /// Allows uploading products data from configuration files such as JSON, XML, into a database.
        /// </summary>
        /// <remarks>
        ///     POST: api/BulkUpload
        ///     <para>
        ///         "headers": { 'Content-Type': 'application/x-www-form-urlencoded' }
        ///     </para>
        ///     <para>
        ///         "bulkdata": "data:application/json;base64,Ww0KICB7DQogICAg...Q0KXQ=="
        ///     </para>
        /// </remarks>
        /// <param name="bulkdata"></param>
        /// <returns>Empty response with status code</returns>
        /// <response code="201">Successful file upload code</response>
        /// <response code="204">Input data incorrect</response>     
        /// <response code="404">Database error</response>     
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ProductItem>> PostProductItem([FromForm] string bulkdata)
        {
            DataFile dataFile = new(bulkdata);
            bool dbUpdated = false;
            Products result = new Products();
            if (dataFile.FileSourceIsCorrect)
            {
                switch (dataFile.FileExtension.ToUpper())
                {
                    case "JSON":
                        try { result = JsonConvert.DeserializeObject<Products>(dataFile.FileContent); }
                        catch (Exception) { }
                        dbUpdated = _db.SaveProductsToDatabase(result);
                        break;
                    case "XML":
                        Serializer serializer = new Serializer();
                        try { result = serializer.Deserialize<Products>(dataFile.FileContent); }
                        catch (Exception) { }
                        dbUpdated = _db.SaveProductsToDatabase(result);
                        break;
                    default:
                        break;
                }
            }

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return NotFound();
            }
            if (dbUpdated)
                return StatusCode(201);
            return NoContent();
        }
    }
}
