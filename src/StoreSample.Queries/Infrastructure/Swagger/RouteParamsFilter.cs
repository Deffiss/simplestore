using System;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace StoreSample.Queries.Infrastructure.Swagger
{
    public class RouteParamsFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var parameters = context.ApiDescription.ParameterDescriptions.ToArray();
            foreach (var p in parameters)
            {
                if (p.Source.DisplayName.Equals("Path")
                    && !(context.ApiDescription.RelativePath.IndexOf($"{{{p.Name}}}", StringComparison.OrdinalIgnoreCase) >= 0))
                {
                    operation.Parameters.Remove(operation.Parameters.FirstOrDefault(pr => pr.Name.Equals(p.Name, StringComparison.OrdinalIgnoreCase)));
                }
            }
        }
    }
}
