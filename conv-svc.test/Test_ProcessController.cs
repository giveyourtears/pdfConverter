using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using Inex.Umk.Messages;
using Microsoft.AspNetCore.Http;
using System.IO;
using Inex.Umk.Conv.Test.Properties;
using Inex.Umk.IO;
using Xunit;
using Microsoft.AspNetCore.Http.Internal;
using System.Text;

namespace Inex.Umk.Conv.Test
{
  class Test_ProcessController
  {
    IWebHost _host;
    Inex.Umk.Conv.Controllers.FilePdfController _ctl;

    public Test_ProcessController(IWebHost host)
    {
      _host = host;
      _ctl = _host.Services.GetService<Inex.Umk.Conv.Controllers.FilePdfController>();
    }
    public static void TestAll(IWebHost host)
    {
      Test_ProcessController t = new Test_ProcessController(host);
      t.Test_ProcessFileTrue();
      t.Test_WrongExtensionFileFalse();
      t.Test_NullFileFalse();
    }
    void Test_ProcessFileTrue()
    {
      File.WriteAllBytes(Path.Combine(Folder.TempFolder, "Sample.pdf"), Resources.Sample);
      FileInfo physicalFile = new FileInfo(Path.Combine(Folder.TempFolder, "Sample.pdf"));
      IFormFile formFile = physicalFile.AsMockIFormFile();
      ActionResult<ProcessResponse> res = _ctl.Process(formFile);
      Assert.NotNull(res);
      Assert.NotNull(res.Value);
      Assert.IsType<ActionResult<ProcessResponse>>(res);
      Assert.NotNull(((ActionResult<ProcessResponse>)res.Value).Value);
      File.Delete(Path.Combine(Folder.TempFolder, "Sample.pdf"));
    }
    void Test_WrongExtensionFileFalse()
    {
      File.WriteAllBytes(Path.Combine(Folder.TempFolder, "bad.jpg"), Resources.Sample);
      FileInfo physicalFile = new FileInfo(Path.Combine(Folder.TempFolder, "bad.jpg"));
      IFormFile formFile = physicalFile.AsMockIFormFile();
      ActionResult<ProcessResponse> res = _ctl.Process(formFile);
      Assert.NotNull(res);
      File.Delete(Path.Combine(Folder.TempFolder, "bad.jpg"));
    }
    void Test_NullFileFalse()
    {
      IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy image")), 0, 0, "Data", "image.png");
      ActionResult<ProcessResponse> res = _ctl.Process(file);
      Assert.NotNull(res);
    }
  }
}