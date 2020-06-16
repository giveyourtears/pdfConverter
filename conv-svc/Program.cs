using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using System;
using System.IO;

namespace Inex.Umk.Conv
{
  public class Program
  {
    public static void Main(string[] args)
    {
      ConfigureLogging();
      try
      {
        IWebHost host = CreateWebHostBuilder(args).Build();
        Log.Information("Start Service");
        host.Run();
        Log.Information("Stop Service");
      }
      catch (Exception ex)
      {
        Log.Fatal(ex.Message);
      }
      finally
      {
        Log.CloseAndFlush();
      }
    }

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        WebHost.CreateDefaultBuilder(args)
        .ConfigureAppConfiguration((context, config) =>
        {
          config.SetBasePath(IO.Folder.CurrentFolder);
          IHostingEnvironment env = context.HostingEnvironment;
          config.AddJsonFile(Path.Combine(IO.Folder.ConfigFolder, "appsettings.json"), optional: true, reloadOnChange: true)
                            .AddJsonFile(Path.Combine(IO.Folder.ConfigFolder, $"appsettings.{env.EnvironmentName}.json"),
                                optional: true, reloadOnChange: true);
        })
        .UseStartup<Startup>()
        .UseSerilog();


    private static void ConfigureLogging()
    {
      var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
      var configuration = new ConfigurationBuilder()
        .SetBasePath(IO.Folder.CurrentFolder)
        .AddJsonFile(Path.Combine(IO.Folder.ConfigFolder, "appsettings.json"), optional: false, reloadOnChange: true)
        .AddJsonFile(
          Path.Combine(IO.Folder.ConfigFolder, $"appsettings.{environment}.json"),
          optional: true)
        .Build();

      FileSinkOptions fso = ConfigureFileSink(configuration);

      Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .WriteTo.Debug()
        .WriteTo.Console()
        .WriteTo.File(fso.PathFormat, rollingInterval: RollingInterval.Day, buffered: true, levelSwitch: fso.LogLevel,
          outputTemplate: fso.OutputTemplate, retainedFileCountLimit: fso.RetainedFileCountLimit)
        .WriteTo.Elasticsearch(ConfigureElasticSink(configuration, environment))
        .Enrich.WithProperty("Environment", environment)
        .CreateLogger();
    }

    static FileSinkOptions ConfigureFileSink(IConfigurationRoot configuration)
    {
      LogEventLevel level = (Serilog.Events.LogEventLevel)Enum.Parse(typeof(Serilog.Events.LogEventLevel), configuration["File:LogLevel"]);
      FileSinkOptions retVal = new FileSinkOptions();
      retVal.LogLevel = new LoggingLevelSwitch(level);
      retVal.OutputTemplate = configuration["File:OutputTemplate"];
      retVal.PathFormat = configuration["File:PathFormat"];
      retVal.RetainedFileCountLimit = int.Parse(configuration["File:RetainedFileCountLimit"]);
      return retVal;
    }

    private static ElasticsearchSinkOptions ConfigureElasticSink(IConfigurationRoot configuration, string environment)
    {
      string service = configuration["ServiceName"];
      LogEventLevel level = (Serilog.Events.LogEventLevel)Enum.Parse(typeof(Serilog.Events.LogEventLevel), configuration["ElasticConfiguration:LogLevel"]);
      Uri uri = new Uri(CommonUtils.CommonReplaceEnvVars(configuration["ElasticConfiguration:Uri"], new string[] { "DEV_PG_HOST" }));
      return new ElasticsearchSinkOptions(uri)
      {
        AutoRegisterTemplate = true,
        LevelSwitch = new LoggingLevelSwitch(level),
        IndexFormat = $"{service.ToLower().Replace(".", "-")}-{environment?.ToLower().Replace(".", "-")}_{DateTime.UtcNow:yyyy-MM-dd}"
      };
    }

    /// <summary>
    /// Параметры настройки записи логов в файл
    /// </summary>
    class FileSinkOptions
    {
      /// <summary>
      /// Формат пути файлов логов
      /// </summary>
      public string PathFormat { get; set; }
      /// <summary>
      /// Формат вывода записей в лог
      /// </summary>
      public string OutputTemplate { get; set; }
      /// <summary>
      /// Макс. кол-во файлов лога
      /// </summary>
      public int RetainedFileCountLimit { get; set; }
      /// <summary>
      /// Уровень детализации
      /// </summary>
      public LoggingLevelSwitch LogLevel { get; set; }
    }

  }
}
