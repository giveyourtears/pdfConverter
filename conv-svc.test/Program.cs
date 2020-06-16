using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace Inex.Umk.Conv.Test
{
  class Program
  {
    static int Main(string[] args)
    {
      try
      {
        Log.Info("Start test");
        IWebHost host = Init(args);
        TestAll(host);
        Log.Info("OK");
        return 0;
      }
      catch (Exception ex)
      {
        Log.Error(ex.Message);
        return 1;
      }
    }

    static void TestAll(IWebHost host)
    {
      //Test_ProcessController.TestAll(host);
      Test_ConvertController.TestAll(host);
      //Test_PreviewController.TestAll(host);
      //Test_ExtractController.TestAll(host);
    }

    static IWebHost Init(string[] args)
    {
      System.Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
      IWebHost host = Inex.Umk.Conv.Program.CreateWebHostBuilder(args).Build();
      return host;
    }

  }
}
