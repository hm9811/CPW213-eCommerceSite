using eCommerceSite.Data;
using eCommerceSite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eCommerceSite.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductContext _context;

        public ProductController(ProductContext context)
        {
             _context = context;
        }

        /// <summary>
        /// Displays a view that lists a page of products
        /// </summary>
        public async Task<IActionResult> Index(int? id)
        {
            int pageNum = id ?? 1;
            const int PageSize = 3;
            ViewData["CurrentPage"] = pageNum;

            int numProducts = await ProductDb.GetTotalProductsAsync(_context);
            
            int totalPages = (int)Math.Ceiling((double)numProducts / PageSize);

            ViewData["MaxPage"] = totalPages;

            // Get all products from database
            //List<Product> products = _context.Products.ToList();
            List<Product> products =
                await ProductDb.GetProductsAsync(_context, PageSize, pageNum);

            // Send list of products to view to be displayed
            return View(products);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product p)
        {
            if (ModelState.IsValid)
            {
                await ProductDb.AddProductAsync(_context, p);

                TempData["Message"] = $"{p.Title} was added successfully";

                // redirect back to catalog page
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            //Get product with corresponding id
            Product p =
                await ProductDb.GetProductIdAsync(_context, id);
            // Product p2 = await _context
            //                    .Products
            //                    .Where(prod => prod.ProductId == id)
            //                     .SingleAsync();

            //pass product to view
            return View(p);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Product p)
        {
            if (ModelState.IsValid)
            {
                _context.Entry(p).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                ViewData["Message"] = "Product updated successfully";
            }

            return View(p);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            Product p = await ProductDb.GetProductIdAsync(_context, id);
            return View(p);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Product p = await ProductDb.GetProductIdAsync(_context, id);

            _context.Entry(p).State = EntityState.Deleted;

            await _context.SaveChangesAsync();

            TempData["Message"] = $"{p.Title} was deleted";

            return RedirectToAction("Index");
        }
    }
}
