using Inex.Umk.Conv.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
//using Inex.Monitor;
using Prometheus;

namespace Inex.Umk.Conv
{
  public class Startup
  {
    const string _serviceName = "conv";
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc()
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
        .AddControllersAsServices();
      services.AddSingleton<FileInfoPDFService>();
      //services.AddMetrics(_serviceName);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      string LicenseKey = "MTYzMTM4QDMxMzcyZTMzMmUzMGFUQjc1cHFqVzc5WGIrMXZOVEtESlJSRnJiQTM3Q3IwdnFlZnZWeWxCU0k9;MTYzMTM5QDMxMzcyZTMzMmUzMGhScktaMzUxc1Y3V3Riam1TSlRQNTIwZlJlWWR3Ui9odm5Oc01PblZaVlE9;MTYzMTQwQDMxMzcyZTMzMmUzMGZmVUsveWpCSnltQ0M0ZjBKRk5DT2RPSzdvRlZjNUxxcHdUZHNEZldiRDg9;MTYzMTQxQDMxMzcyZTMzMmUzMGtyVW9jQ0hJWnl6TjJ6WCtuZ2lJY3hKNjdBVFRHZjlKdmlqL1UxQjROQjg9;MTYzMTQyQDMxMzcyZTMzMmUzME10Szg1bGdBRlhZYVd4ZGJCWXVBZUlCaEpwdlU0bzZHSjRiK2kxZFhVdVk9;MTYzMTQzQDMxMzcyZTMzMmUzMFg3VzhXbjRxakFFZkZvYjhPQ1pxaTFjck55UWY4ZG1ibktmSWxubjM5Ykk9;MTYzMTQ0QDMxMzcyZTMzMmUzMG5OZWRZc0ZMRDdoOGpkaGtRQXErY0F3NFF2RnNlZ3JBQThFbThPQVVmcE09;MTYzMTQ1QDMxMzcyZTMzMmUzMGVTVVJNZ2tuZUVSOW51S3RDb2F2dENTWlA0Qzl5RjZtYTZtS3dJdGxTZjg9;MTYzMTQ2QDMxMzcyZTMzMmUzME9rTTlTN2R0alV1MjI5MGYyRjRyRUZNdW10cFJ2Y2c5c05mMVBkdnFYRVk9;MTYzMTQ3QDMxMzcyZTMzMmUzME10Szg1bGdBRlhZYVd4ZGJCWXVBZUlCaEpwdlU0bzZHSjRiK2kxZFhVdVk9;NT8mJyc2IWhiZH1gfWN9YmRoYmF8YGJ8ampqanNiYmlmamlmanMDHmghJiAnMj59MicyMTI2JX0kPCE4EzQ+Mjo/fTA8Pg==;MTYzMTI4QDMxMzcyZTMzMmUzMGhScktaMzUxc1Y3V3Riam1TSlRQNTIwZlJlWWR3Ui9odm5Oc01PblZaVlE9;MTYzMTI5QDMxMzcyZTMzMmUzMGZmVUsveWpCSnltQ0M0ZjBKRk5DT2RPSzdvRlZjNUxxcHdUZHNEZldiRDg9;MTYzMTMwQDMxMzcyZTMzMmUzMGtyVW9jQ0hJWnl6TjJ6WCtuZ2lJY3hKNjdBVFRHZjlKdmlqL1UxQjROQjg9;MTYzMTMxQDMxMzcyZTMzMmUzME10Szg1bGdBRlhZYVd4ZGJCWXVBZUlCaEpwdlU0bzZHSjRiK2kxZFhVdVk9;MTYzMTMyQDMxMzcyZTMzMmUzMFg3VzhXbjRxakFFZkZvYjhPQ1pxaTFjck55UWY4ZG1ibktmSWxubjM5Ykk9;MTYzMTMzQDMxMzcyZTMzMmUzMG5OZWRZc0ZMRDdoOGpkaGtRQXErY0F3NFF2RnNlZ3JBQThFbThPQVVmcE09;MTYzMTM0QDMxMzcyZTMzMmUzMGVTVVJNZ2tuZUVSOW51S3RDb2F2dENTWlA0Qzl5RjZtYTZtS3dJdGxTZjg9;MTYzMTM1QDMxMzcyZTMzMmUzME9rTTlTN2R0alV1MjI5MGYyRjRyRUZNdW10cFJ2Y2c5c05mMVBkdnFYRVk9;MTYzMTM2QDMxMzcyZTMzMmUzME10Szg1bGdBRlhZYVd4ZGJCWXVBZUlCaEpwdlU0bzZHSjRiK2kxZFhVdVk9;NT8mJyc2IWhiZH1gfWN9YmRoYmF8YGJ8ampqanNiYmlmamlmanMDHmghJiAnMj59MicyMTI2JX0kPCE4EzQ+Mjo/fTA8Pg==";
      Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(LicenseKey);
      ILogger<Startup> logger = app.ApplicationServices.GetService<ILogger<Startup>>();
      logger.LogInformation("Startup.Configure");
      //app.UseMetrics(_serviceName);
      app.UseMetricServer();
      app.UseHttpMetrics();

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
      }
      app.UseHttpsRedirection();
      app.UseMvc();
    }
  }
}
