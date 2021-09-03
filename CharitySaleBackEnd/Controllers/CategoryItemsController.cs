using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CharitySaleBackEnd.Models;
using CharitySaleBackEnd.Models.Context;

namespace CharitySaleBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryItemsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoryItemsController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a list of all product categories
        /// </summary>
        /// <returns></returns>
        // GET: api/CategoryItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryItem>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }
    }
}
