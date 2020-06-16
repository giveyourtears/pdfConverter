using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Inex.Umk.Conv.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AboutController : ControllerBase
  {
    ILogger<AboutController> _logger;
    public AboutController(
      ILogger<AboutController> logger
      )
    {
      _logger = logger;
    }

    // GET api/about
    [HttpGet]
    public ActionResult<AboutInfo> Get()
    {
      _logger.LogDebug("AboutController.Get()");
      AboutInfo ai = new AboutInfo();
      Assembly assm = Assembly.GetExecutingAssembly();
      ai.Copyright = assm.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright;
      ai.Trademark = assm.GetCustomAttribute<AssemblyTrademarkAttribute>().Trademark;
      ai.InformationalVersion = assm.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
      ai.Envs.Add("ASPNETCORE_ENVIRONMENT", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
      _logger.LogDebug("AboutController.Get()..OK");
      return Ok(ai);
    }
  }

  /// <summary>
  /// Информация о продукте
  /// </summary>
  public class AboutInfo
  {
    public string Copyright { get; set; }
    public string Trademark { get; set; }
    public string InformationalVersion { get; set; }
    /// <summary>
    /// Значения специальных переменных окружения
    /// </summary>
    public Dictionary<string, string> Envs = new Dictionary<string, string>();
  }

}