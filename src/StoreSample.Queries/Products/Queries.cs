using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;

namespace StoreSample.Queries.Products
{
    public class GetById : IRequest<Product>
    {
        [FromRoute]
        public Guid Id { get; set; }
    }

    public class GetAll : IRequest<(Product[] Products, long TotalCount)>
    {
        [FromRoute]
        public Guid? CategoryId { get; set; }

        [FromQuery]
        public long Limit { get; set; } = 50;

        [FromQuery]
        public long Offset { get; set; }
    }
}
