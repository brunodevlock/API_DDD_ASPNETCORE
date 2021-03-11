using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;


[Route("products")]
public class ProductController : ControllerBase 
{
    [HttpGet]
    [Route("")]
    public async Task<ActionResult<List<Product>>> Get(
        [FromServices]DataContext context
    )
    {
        var products = await context
        .Products
        .Include(x => x.Category)
        .AsNoTracking()
        .ToListAsync();
        
        return Ok(products);
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id, 
    [FromServices]DataContext context)
    {
        var product = await context
        .Products.Include(x => x.Category)
        .AsNoTracking()
        .FirstOrDefaultAsync(x => x.Id == id);
        
        return Ok(product);
    }


    //products/categories/1
    [HttpGet]
    [Route("categories/{id:int}")]
    public async Task<ActionResult<Product>> GetByCategory(
    [FromServices]DataContext context)
    {
        var products = await context
        .Products
        .Include(x => x.Category)
        .AsNoTracking()
        .Where(x => x.CategoryId == id)
        .ToListAsync();
        
        return Ok(products);
    }


    [HttpPost]
    [Route("")]
    public async Task<ActionResult<List<Product>>> Post(
        [FromBody]Product model, 
        [FromServices]DataContext context)
    {
        if(!ModelState.IsValid)
        {
            context.Products.Add(model);
            await context.SaveChangesAsync();
            return Ok(model);
        }
        else
        {

            return BadRequest(ModelState);
        }


    }

}
