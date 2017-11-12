using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;

namespace StoreSample.Commands.Infrastructure.Swagger
{
    public class NotMappedOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            var notMappedProperties = context.ApiDescription
                .ParameterDescriptions.Where(p => p?.ModelMetadata?.ContainerType?.GetTypeInfo()?.GetProperty(p.ModelMetadata.PropertyName)?.GetCustomAttribute<NotMappedAttribute>() != null)
                .ToArray();

            foreach (var property in notMappedProperties)
            {
                operation.Parameters.Remove(operation.Parameters.FirstOrDefault(p => p.Name.Equals(property.Name, StringComparison.OrdinalIgnoreCase)));
            }
        }
    }

}
