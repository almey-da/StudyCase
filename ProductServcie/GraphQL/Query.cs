using HotChocolate.AspNetCore.Authorization;
using ProductServcie.Models;

namespace ProductServcie.GraphQL
{
    public class Query
    {
        [Authorize]
        public IQueryable<Product> GetProducts([Service] StudyCaseContext context) =>
            context.Products;

        public async Task<Product> GetProductByIdAsync(
        int id,
        [Service] StudyCaseContext context)
        {
            var product = context.Products.Where(o => o.Id == id).FirstOrDefault();

            return await Task.FromResult(product);
        }

    }
}
