using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Inex.Umk.Messages;
using System.IO;
using System.Text;
using Xunit;
using Inex.Umk.IO;
using Inex.Umk.Conv.Test.Properties;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Inex.Umk.Conv.Test
{
  class Test_ExtractController
  {
    IWebHost _host;
    Inex.Umk.Conv.Controllers.FilePdfController _ctl;

    public Test_ExtractController(IWebHost host)
    {
      _host = host;
      _ctl = _host.Services.GetService<Inex.Umk.Conv.Controllers.FilePdfController>();
    }
    public static void TestAll(IWebHost host)
    {
      Test_ExtractController t = new Test_ExtractController(host);
      t.Test_ExtractPdfTrue();
      //t.Test_ExtractPdfNullFalse();
    }

    void Test_ExtractPdfTrue()
    {
      File.WriteAllBytes(Path.Combine(Folder.TempFolder, "Sample.pdf"), Resources.Sample);
      FileInfo physicalFile = new FileInfo(Path.Combine(Folder.TempFolder, "Sample.pdf"));
      IFormFile formFile = physicalFile.AsMockIFormFile();
      using (Stream stream = formFile.OpenReadStream())
      {
        ActionResult<ExtractTextResponse> res = _ctl.ExtractText(stream);
        Assert.NotNull(res);
        Assert.IsType<ActionResult<ExtractTextResponse>>(res);
      }
      File.Delete(Path.Combine(Folder.TempFolder, "Sample.pdf"));
    }
    void Test_ExtractPdfNullFalse()
    {
      IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy image")), 0, 0, "Data", "image.docx");
      using (Stream stream = file.OpenReadStream())
      {
        ActionResult<ExtractTextResponse> res = _ctl.ExtractText(stream);
        Assert.NotNull(res);
        Assert.IsType<ActionResult<ExtractTextResponse>>(res);
        Assert.IsType<ActionResult<ExtractTextResponse>>(res);
      }
    }
  }
}
