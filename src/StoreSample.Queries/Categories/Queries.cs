using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;

namespace StoreSample.Queries.Categories
{
    public class GetAll : IRequest<Category[]>
    {
    }

    public class GetById : IRequest<Category>
    {
        [FromRoute]
        public Guid Id { get; set; }
    }
}
