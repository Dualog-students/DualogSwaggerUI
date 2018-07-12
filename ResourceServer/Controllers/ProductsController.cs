using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net;

namespace SwaggerUI.ResourceServer.Controllers
{
    /// <summary>
    /// Provides access points for the product resources
    /// </summary>
    [Route("products")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public class ProductsController : Controller
    {
        /// <summary>
        /// Gets all products.
        /// </summary>
        /// <returns>A list of products</returns>
        /// <response code="200">Success</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Authorize("readAccess")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Product[]))]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]

        public IEnumerable<Product> GetAll()
        {
            yield return new Product
            {
                Id = 1,
                SerialNo = "ABC123",
                Status = ProductStatus.InStock
            };
            yield return new Product
            {
                Id = 2,
                SerialNo = "DEF456",
                Status = ProductStatus.ComingSoon
            };
            yield return new Product
            {
                Id = 3,
                SerialNo = "GHI789",
                Status = ProductStatus.InStock
            };
        }

        /// <summary>
        /// Gets a product by a product id.
        /// </summary>
        /// <param name="id">The product id.</param>
        /// <returns>A single product.</returns>
        /// <response code="200">Success</response>
        /// <response code="404">Resource not found</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("{id}")]
        [Authorize("readAccess")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Product))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public Product GetById(int id)
        {
            return new Product
            {
                Id = 1,
                SerialNo = "ABC123",
                Status = ProductStatus.InStock
            };

        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="product">The item to create.</param>
        /// /// <remarks>
        /// Sample request:
        ///
        ///     POST /Product
        ///     {
        ///        "Id": 1,
        ///        "SerialNo": "Item1"
        ///     }
        ///
        /// </remarks>
        /// <returns>A newly created Product</returns>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>   
        [HttpPost]
        [Authorize("writeAccess")]
        [ProducesResponseType((int)HttpStatusCode.Created, Type = typeof(Product))]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public Product Post([FromBody]Product product)
        {
            return product;
        }


        /// <summary>
        /// Deletes a product by its product id.
        /// </summary>
        /// <param name="id">The id of the product to delete.</param>
        /// /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /Product
        ///     {
        ///        "Id": 1
        ///     }
        /// </remarks>
        /// <response code="201">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>  

        [HttpDelete("{id}")]
        [Authorize("writeAccess")]
        public void Delete(int id)
        {
        }
    }


    public class Product
    {
        public int Id { get; internal set; }
        public string SerialNo { get; set; }
        public ProductStatus Status { get; set; }
    }

    public enum ProductStatus
    {
        InStock, ComingSoon
    }
}
