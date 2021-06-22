using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RVTR.Media.Context;
using RVTR.Media.Context.Repositories;
using RVTR.Media.Domain.Interfaces;
using Swashbuckle.AspNetCore.SwaggerGen;
using zipkin4net.Middleware;

namespace RVTR.Media.Service
{
  /// <summary>
  ///
  /// </summary>
  public class Startup
  {
    /// <summary>
    ///
    /// </summary>
    private readonly IConfiguration _configuration;

    /// <summary>
    ///
    /// </summary>
    /// <param name="configuration"></param>
    public Startup(IConfiguration configuration)
    {
      _configuration = configuration;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="services"></param>
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddApiVersioning(options =>
      {
        options.ReportApiVersions = true;
      });

      services.AddControllers();
      services.AddCors(cors =>
      {
        cors.AddPolicy("Public", policy =>
        {
          policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
        });
      });

      services.AddDbContext<MediaContext>(options =>
      {
        options.UseNpgsql(_configuration.GetConnectionString("pgsql"));
      }, ServiceLifetime.Transient);

      services.AddScoped<ClientZipkinMiddleware>();
      services.AddScoped<IUnitOfWork, UnitOfWork>();
      services.AddSwaggerGen();
      services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ClientSwaggerOptions>();
      services.AddControllers().AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
      );
      services.AddVersionedApiExplorer(options =>
      {
        options.GroupNameFormat = "VV";
        options.SubstituteApiVersionInUrl = true;
      });
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="descriptionProvider"></param>
    /// <param name="applicationBuilder"></param>
    /// <param name="hostEnvironment"></param>
    /// <param name="loggerFactory"></param>
    public void Configure(IApiVersionDescriptionProvider descriptionProvider, IApplicationBuilder applicationBuilder, IWebHostEnvironment hostEnvironment, ILoggerFactory loggerFactory)
    {
      loggerFactory.AddFile("Logs/ts-{Date}.txt");

      if (hostEnvironment.IsDevelopment())
      {
        applicationBuilder.UseDeveloperExceptionPage();
      }

      applicationBuilder.UseZipkin();
      applicationBuilder.UseTracing("mediaapi.rest");
      applicationBuilder.UseRouting();
      applicationBuilder.UseSwagger(options =>
      {
        options.RouteTemplate = "rest/media/{documentName}/swagger.json";
      });
      applicationBuilder.UseSwaggerUI(options =>
      {
        options.RoutePrefix = "rest/media";

        foreach (var description in descriptionProvider.ApiVersionDescriptions)
        {
          options.SwaggerEndpoint($"/rest/media/{description.GroupName}/swagger.json", description.GroupName);
        }
      });

      applicationBuilder.UseCors();
      applicationBuilder.UseAuthorization();
      applicationBuilder.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
