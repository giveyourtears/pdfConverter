using Inex.Umk.Conv.Test.Properties;
using Inex.Umk.IO;
using Inex.Umk.Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using Xunit;

namespace Inex.Umk.Conv.Test
{
  class Test_PreviewController
  {
    IWebHost _host;
    Inex.Umk.Conv.Controllers.FilePdfController _ctl;

    public Test_PreviewController(IWebHost host)
    {
      _host = host;
      _ctl = _host.Services.GetService<Inex.Umk.Conv.Controllers.FilePdfController>();
    }
    public static void TestAll(IWebHost host)
    {
      Test_PreviewController t = new Test_PreviewController(host);
      t.Test_PreviewPdfTrue();
      //t.Test_PreviewPdfFalse();
      //t.Test_PreviewPdfFalsePage();
    }

    void Test_PreviewPdfTrue()
    {
      File.WriteAllBytes(Path.Combine(Folder.TempFolder, "Sample.pdf"), Resources.Sample);
      FileInfo physicalFile = new FileInfo(Path.Combine(Folder.TempFolder, "Sample.pdf"));
      IFormFile formFile = physicalFile.AsMockIFormFile();
      using (Stream stream = formFile.OpenReadStream()) 
      {
        ActionResult<PreviewResponse> res = _ctl.PreviewPdf(stream, 0);
        Assert.NotNull(res);
        Assert.IsType<ActionResult<PreviewResponse>>(res);
      }
      File.Delete(Path.Combine(Folder.TempFolder, "Sample.pdf"));
    }
    void Test_PreviewPdfNullFalse()
    {
      IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy image")), 0, 0, "Data", "image.docx");
      using (Stream stream = file.OpenReadStream())
      {
        ActionResult<PreviewResponse> res = _ctl.PreviewPdf(stream, 0);
        Assert.NotNull(res);
        Assert.IsType<ActionResult<PreviewResponse>>(res);
      }
    }
    void Test_PreviewPdfFalsePage()
    {
      File.WriteAllBytes(Path.Combine(Folder.TempFolder, "Sample.pdf"), Resources.Sample);
      FileInfo physicalFile = new FileInfo(Path.Combine(Folder.TempFolder, "Sample.pdf"));
      IFormFile formFile = physicalFile.AsMockIFormFile();
      using (Stream stream = formFile.OpenReadStream())
      {
        ActionResult<PreviewResponse> res = _ctl.PreviewPdf(stream, -2);
        File.Delete(Path.Combine(Folder.TempFolder, "Sample.pdf"));
        Assert.NotNull(res);
        Assert.IsType<ActionResult<PreviewResponse>>(res);
      }
    }
  }
}
