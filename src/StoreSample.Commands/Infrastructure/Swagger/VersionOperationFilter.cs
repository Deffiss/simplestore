using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;

namespace StoreSample.Commands.Infrastructure.Swagger
{
    public class VersionOperationFilter : IOperationFilter
    {
        private readonly string _defaultVersion;

        public VersionOperationFilter(string defaultVersion)
        {
            _defaultVersion = defaultVersion;
        }

        public void Apply(Operation operation, OperationFilterContext context)
        {
            var versionParam = (NonBodyParameter)operation.Parameters.FirstOrDefault(p => p.Name == "version");
            versionParam.Default = _defaultVersion;
            versionParam.Description = $"API version (default is '{_defaultVersion}')";
        }
    }
}
