using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebStoreApi.Data;
using WebStoreApi.Models;

namespace WebStoreApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        // initial the database context
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<Products> GetProducts()
        {
            // LINQ With condition
            // หาสินค้าที่ราคาสูงสุด 2 ชิ้น
            // 
            // var maxPriceProducts = _context.Products.Where(x => x.ProductId != 0) // ค้นหาสินค้าทั้งหมด
            //     .OrderByDescending(p => p.UnitPrice) // จัดเรียงจากราคาสูงสุดไปต่ำสุด
            //     .Take(2).ToList(); // หยิบสินค้า 2 ชิ้น

            // LINQ With Join 2 tables
            // หาสินค้าที่มีราคาสูงสุด-ต่ำสุด 3 ชิ้น และหมวดหมู่ของสินค้า
            var allProduct = (
                from category in _context.Categories // ค้นหาหมวดหมู่ทั้งหมด
                join product in _context.Products  // ค้นหาสินค้าทั้งหมด
                on category.CategoryId equals product.CategoryId // ทำการเชื่อมข้อมูลระหว่าง 2 ตาราง
                where category.CategoryStatus == 1 // ค้นหาหมวดหมู่ที่มีสถานะเป็น 1
                orderby product.UnitPrice descending // จัดเรียงจากราคาสูงสุดไปต่ำสุด
                select new { 
                    product.ProductId,
                    product.ProductName, 
                    product.UnitPrice, 
                    product.UnitInStock,
                    product.ProductPicture,
                    product.CreatedDate,
                    product.ModifiedDate,
                    category.CategoryName,
                    category.CategoryStatus
                }
            ).ToList();





            return Ok(allProduct);
        }

       // Get : api/Product/id
       [HttpGet("{id}")]
         public ActionResult<Products> GetProductById(int id)
         {
              // Linq with join 2 tables
                // ค้นหาสินค้าตามรหัสสินค้า
                var products = (
                    from category in _context.Categories // ค้นหาหมวดหมู่ทั้งหมด
                    join product in _context.Products  // ค้นหาสินค้าทั้งหมด
                    on category.CategoryId equals product.CategoryId // ทำการเชื่อมข้อมูลระหว่าง 2 ตาราง
                    where product.ProductId == id // ค้นหาสินค้าตามรหัสสินค้า
                    select new { 
                        product.ProductId,
                        product.ProductName, 
                        product.UnitPrice, 
                        product.UnitInStock,
                        product.ProductPicture,
                        product.CreatedDate,
                        product.ModifiedDate,
                        category.CategoryName,
                        category.CategoryStatus
                    }
                ).FirstOrDefault();
              return Ok(products);
         }

        // Post : api/Product
        [HttpPost]
        public ActionResult<Products> PostProduct(Products product)
        {
            // บันทึกข้อมูลสินค้า
            _context.Products.Add(product);
            _context.SaveChanges();
            return Ok(product);
        }

        // Put : api/Product/id
        [HttpPut("{id}")]
        public ActionResult<Products> PutProduct(int id, Products product)
        {
            // แก้ไขข้อมูลสินค้า
            var products = _context.Products.Find(id);
            products.ProductName = product.ProductName;
            products.UnitPrice = product.UnitPrice;
            products.UnitInStock = product.UnitInStock;
            products.ProductPicture = product.ProductPicture;
            products.CategoryId = product.CategoryId;
            products.CreatedDate = product.CreatedDate;
            products.ModifiedDate = product.ModifiedDate;
            _context.SaveChanges();
            return Ok(products);
        }

        // Delete : api/Product/id
        [HttpDelete("{id}")]
        public ActionResult<Products> DeleteProduct(int id)
        {
            // ลบข้อมูลสินค้า
            var products = _context.Products.Find(id);
            _context.Products.Remove(products);
            _context.SaveChanges();
            return Ok(products);
        }
    }
}