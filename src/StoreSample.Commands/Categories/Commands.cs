using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreSample.Commands.Categories
{
    public class Create : IRequest
    {
        [NotMapped]
        public Guid Id { get; set; } = Guid.NewGuid();

        [FromBody]
        public CategoryData CategoryData { get; set; }
    }

    public class Update : IRequest
    {
        [FromRoute]
        public Guid Id { get; set; }

        [FromBody]
        public CategoryData CategoryData { get; set; }
    }

    public class CategoryData
    {
        public string Name { get; set; }

        public IDictionary<string, string> Properties { get; set; }
    }
}
