using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MediatR;
using Swashbuckle.AspNetCore.Swagger;
using StoreSample.Commands.Settings;
using Microsoft.AspNetCore.Mvc;
using EventStore.ClientAPI;
using Microsoft.Extensions.Options;
using StoreSample.Commands.Repositories;
using StoreSample.Commands.Infrastructure.Swagger;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Filters;
using System;
using StoreSample.Commands.Services.Text;
using StoreSample.Commands.Services.Images;

namespace StoreSample.Commands
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });
            services.AddApiVersioning(options =>
            {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });
            services.AddMvc().AddControllersAsServices();

            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(Configuration)
                    .Filter.ByExcluding(e => e.Level < Serilog.Events.LogEventLevel.Warning && Matching.WithProperty<string>("RequestPath", p => p.IndexOf("swagger", StringComparison.OrdinalIgnoreCase) >= 0)(e))
                    .Filter.ByExcluding(Matching.WithProperty<string>("RequestPath", p => p.IndexOf("favicon.icon", StringComparison.OrdinalIgnoreCase) >= 0))
                    .CreateLogger();
                builder.AddSerilog(dispose: true);
            });
            services.AddSingleton(Log.Logger);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info { Title = "Store Queries API", Version = "v1" });

                options.DescribeAllParametersInCamelCase();
                options.DescribeStringEnumsInCamelCase();

                options.OperationFilter<VersionOperationFilter>("1");
                options.OperationFilter<NotMappedOperationFilter>();
                options.OperationFilter<FileUploadFilter>();
            });

            services.AddMediatR();

            services.AddOptions();
            services.Configure<EventStoreSettings>(Configuration.GetSection("EventStore"));
            services.AddSingleton(sp =>
            {
                var connectionString = sp.GetRequiredService<IOptions<EventStoreSettings>>().Value.ConnectionString;
                var connection = EventStoreConnection.Create(connectionString, ConnectionSettings.Create().EnableVerboseLogging().UseConsoleLogger());
                connection.ConnectAsync().Wait();
                return connection;
            });
            services.AddScoped(typeof(IRepository<>), typeof(EventSourcedRepository<>));
            services.AddScoped<IHtmlSanitizer, SimpleHtmlSanitizer>();
            services.AddScoped<IImageStoreService, FileSystemImageStoreService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Store Queries API V1");
            });

            app.UseMvc();

            var applicationLifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
            applicationLifetime.ApplicationStopped.Register(Log.CloseAndFlush);
        }
    }
}
