using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreSample.Commands.Products
{
    public class Create : IRequest
    {
        [NotMapped]
        public Guid Id { get; set; } = Guid.NewGuid();

        [FromBody]
        public CreateProductData ProductData { get; set; }
    }

    public class Update : IRequest
    {
        [FromRoute]
        public Guid Id { get; set; }

        [FromBody]
        public UpdateProductData ProductData { get; set; }
    }

    public class CreateProductData
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid CategoryId { get; set; }

        public IDictionary<string, string> Properties { get; set; }
    }

    public class UpdateProductData
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public Guid? CategoryId { get; set; }

        public IDictionary<string, string> Properties { get; set; }
    }

    public class UploadImage : IRequest
    {
        [FromRoute]
        public Guid Id { get; set; }

        public IFormFile File { get; set; }
    }

    public class Remove : IRequest
    {
        [FromRoute]
        public Guid Id { get; set; }
    }
}
