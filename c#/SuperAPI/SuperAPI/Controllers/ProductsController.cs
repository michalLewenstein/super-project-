﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Super.Core;
using Super.Core.DTOs;
using Super.Core.Models;
using Super.Core.Service;
using Super.Service;
using SuperAPI.Models;
using System.Data;
using static System.Net.Mime.MediaTypeNames;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SuperAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy = "User")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductsController(IProductService productService, IMapper mapper)
        {
            _productService = productService;
            _mapper = mapper;

        }
        // GET: api/<ProductsController>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult GetAllProducts()
        {
            var productDtos = _mapper.Map<List<ProductDto>>(_productService.GetAllProducts());

            foreach (var productDto in productDtos)
            {
                if (productDto.ImageUrl != null)
                {
                    var path = Path.Combine(Environment.CurrentDirectory, "images/", productDto.ImageUrl);
                    if (System.IO.File.Exists(path))
                    {
                        byte[] bytes = System.IO.File.ReadAllBytes(path);
                        string imageBase64 = Convert.ToBase64String(bytes);
                        productDto.ImageUrl = string.Format("data:image/jpeg;base64,{0}", imageBase64);
                    }
                    else
                    {
                        // אם הקובץ לא קיים, הגדר את ImageUrl ל-null או תמונה ברירת מחדל
                        productDto.ImageUrl = null; // או תמונה ברירת מחדל
                    }
                }
            }

            return Ok(productDtos);
        }


        // GET api/<ProductsController>/5
        [HttpGet("{Id}")]
        [AllowAnonymous]
        public ActionResult GetProductById(int Id)
        {
            var p = _mapper.Map<ProductDto>(_productService.GetProductById(Id));
            if (p.ImageUrl != null)
            {
                var path = Path.Combine(Environment.CurrentDirectory, "images/", p.ImageUrl);
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                string imageBae64 = Convert.ToBase64String(bytes);
                p.ImageUrl = string.Format("data:image/jpeg;base64,{0}", imageBae64);
                return Ok(p);

            }
            p.ImageUrl = null;
            return Ok(p);


        }

        // POST api/<ProductsController>
        //[Authorize(Policy = "Admin")]
        //[HttpPost]
        //public void Post([FromBody] ProductPostModel product)
        //{
        //    _productService.AddProduct(_mapper.Map<Product>(product));

        //}
        [AllowAnonymous]
        [HttpPost]
        [Consumes("multipart/form-data")]
        public IActionResult Post([FromForm] ProductPostModel product)
        {
            if (product.ImageUrl == null)
            {
                return BadRequest("You must enter an image.");
            }

            try
            {
                var myPath = Path.Combine(Environment.CurrentDirectory, "images", product.ImageUrl.FileName);
                using (FileStream fs = new FileStream(myPath, FileMode.Create))
                {
                    product.ImageUrl.CopyTo(fs);
                }

                var p = _mapper.Map<Product>(product);
                p.ImageUrl = product.ImageUrl.FileName;
                _productService.AddProduct(p);

                return Ok("Product added successfully.");
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
        // PUT api/<ProductsController>/5
        //[Authorize(Policy = "Admin")]
        [HttpPut("{Id}")]
        [Consumes("multipart/form-data")]
        public IActionResult Put(int Id, [FromForm] ProductPostModel product)
        {
            var existingProduct = _productService.GetProductById(Id);
            if (existingProduct == null)
            {
                return BadRequest("Product not found");
            }

            if (product.ImageUrl != null && product.ImageUrl.Length > 0) // אם נשלחה תמונה
            {
                try
                {
                    // מחיקת תמונה קודמת (אם קיימת)
                    if (!string.IsNullOrEmpty(existingProduct.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(Environment.CurrentDirectory, "images", existingProduct.ImageUrl);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    // שמירת תמונה חדשה
                    var newImagePath = Path.Combine(Environment.CurrentDirectory, "images", product.ImageUrl.FileName);
                    using (FileStream fs = new FileStream(newImagePath, FileMode.Create))
                    {
                        product.ImageUrl.CopyTo(fs);
                    }

                    // עדכון המוצר
                    var updatedProduct = _mapper.Map<Product>(product);
                    updatedProduct.ImageUrl = product.ImageUrl.FileName;
                    _productService.UpdateProduct(Id, updatedProduct);
                }
                catch (Exception ex)
                {
                    return BadRequest($"Error processing image: {ex.Message}");
                }
            }
            else // אם לא נשלחה תמונה
            {
                _productService.UpdateProduct(Id, _mapper.Map<Product>(product));
            }

            return Ok();
        }


        // DELETE api/<ProductsController>/5
        //[Authorize(Policy = "Admin")]
        [AllowAnonymous]
        [HttpDelete("{Id}")]
        public void Delete(int Id)
        {
            _productService.DeleteProduct(Id);
        }
    }
}

