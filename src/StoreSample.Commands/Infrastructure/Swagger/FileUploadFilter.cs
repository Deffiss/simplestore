using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.AspNetCore.Http;
using System;

namespace StoreSample.Commands.Infrastructure.Swagger
{
    public class FileUploadFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var formFileParams = context.ApiDescription.ParameterDescriptions
                .Where(pd => pd.Type == typeof(IFormFile) || pd.ModelMetadata?.ContainerType == typeof(IFormFile))
                .Select(pd => pd.Name).ToArray();

            if (!formFileParams.Any()) return;

            var paramsToRemove = operation.Parameters.Where(p => formFileParams.Any(pdn => pdn.Equals(p.Name, StringComparison.OrdinalIgnoreCase))).ToArray();

            foreach (var p in paramsToRemove)
            {
                operation.Parameters.Remove(p);
            }

            operation.Consumes.Add("application/form-data");

            operation.Parameters.Add(new NonBodyParameter
            {
                Name = "file",
                In = "formData",
                Description = "File",
                Required = true,
                Type = "file"
            });
        }
    }
}
