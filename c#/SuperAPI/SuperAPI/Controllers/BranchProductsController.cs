﻿using Microsoft.AspNetCore.Mvc;
using Super.Core;
using Super.Core.Models;
using Super.Core.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SuperAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchProductsController : ControllerBase
    {
        private readonly IBranchProductService _IbranchProduct;

        public BranchProductsController(IBranchProductService IbranchProduct)
        {
            _IbranchProduct = IbranchProduct;
        }
        // GET: api/<BranchProductsController>
        [HttpGet]
        public IEnumerable<BranchProduct> GetAllBranchProducts()
        {
            return _IbranchProduct.GetAllBranchProducts();
        }

        // GET api/<BranchProductsController>/5
        [HttpGet("{Id}")]
        public BranchProduct GetBranchProductById(int Id)
        {
            return _IbranchProduct.GetBranchProductById(Id);
        }

        // POST api/<BranchProductsController>
        [HttpPost]
        public void AddBranchProduct([FromBody] BranchProduct branchProduct)
        {
            _IbranchProduct.AddBranchProduct(branchProduct);
        }

        // PUT api/<BranchProductsController>/5
        [HttpPut("{Id}")]
        public void UpdateBranchProduct(int Id, [FromBody] BranchProduct branchProduct)
        {
            _IbranchProduct.UpdateBranchProduct(Id, branchProduct);
        }

        // DELETE api/<BranchProductsController>/5
        [HttpDelete("{Id}")]
        public void DeleteBranchProduct(int Id)
        {
            _IbranchProduct.DeleteBranchProduct(Id);
        }
    }
}
