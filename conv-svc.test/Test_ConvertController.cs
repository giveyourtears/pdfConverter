using Inex.Umk.Conv.Test.Properties;
using Inex.Umk.IO;
using Inex.Umk.Messages;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing;
using System.IO;
using System.Text;
using Xunit;

namespace Inex.Umk.Conv.Test
{
  class Test_ConvertController
  {
    IWebHost _host;
    Inex.Umk.Conv.Controllers.FilePdfController _ctl;

    public Test_ConvertController(IWebHost host)
    {
      _host = host;
      _ctl = _host.Services.GetService<Inex.Umk.Conv.Controllers.FilePdfController>();
    }
    public static void TestAll(IWebHost host)
    {
      Test_ConvertController t = new Test_ConvertController(host);
      t.Test_ConvertFileRtf();
      t.Test_ConvertFilePdf();
      t.Test_ConvertFileDocxTrue();
      t.Test_ConvertFileWrongExtensionFalse();
      t.Test_NullFileFalse();
    }

    void Test_ConvertFilePdf()
    {
      File.WriteAllBytes(Path.Combine(Folder.TempFolder, "Sample.pdf"), Resources.Sample);
      FileInfo physicalFile = new FileInfo(Path.Combine(Folder.TempFolder, "Sample.pdf"));
      IFormFile formFile = physicalFile.AsMockIFormFile();
      using (Stream stream = formFile.OpenReadStream())
      {
        ActionResult<ConvertResponse> res = _ctl.Convert(stream, formFile.FileName);
        Assert.NotNull(res);
        Assert.IsType<ActionResult<ConvertResponse>>(res);
      }
      File.Delete(Path.Combine(Folder.TempFolder, "Sample.pdf"));
    }
    void Test_ConvertFileRtf()
    {
      byte[] bytes = Encoding.ASCII.GetBytes(Resources.one);
      File.WriteAllBytes(Path.Combine(Folder.TempFolder, "testrtf.rtf"), bytes);
      FileInfo physicalFile = new FileInfo(Path.Combine(Folder.TempFolder, "testrtf.rtf"));
      IFormFile formFile = physicalFile.AsMockIFormFile();
      using (Stream stream = formFile.OpenReadStream()) 
      {
        ActionResult<ConvertResponse> res = _ctl.Convert(stream, formFile.FileName);
        Assert.NotNull(res);
        Assert.IsType<ActionResult<ConvertResponse>>(res);
      }
      File.Delete(Path.Combine(Folder.TempFolder, "testrtf.txt"));
    }
    void Test_ConvertFileDocxTrue()
    {
      File.WriteAllBytes(Path.Combine(Folder.TempFolder, "sss.docx"), Resources.sss);
      FileInfo physicalFile = new FileInfo(Path.Combine(Folder.TempFolder, "sss.docx"));
      IFormFile formFile = physicalFile.AsMockIFormFile();
      using (Stream stream = formFile.OpenReadStream())
      {
        ActionResult<ConvertResponse> res = _ctl.Convert(stream, formFile.FileName);
        Assert.NotNull(res);
        Assert.IsType<ActionResult<ConvertResponse>>(res);
      }
      File.Delete(Path.Combine(Folder.TempFolder, "sss.docx"));
    }
    void Test_ConvertFileWrongExtensionFalse()
    {
      File.WriteAllBytes(Path.Combine(Folder.TempFolder, "sss.abv"), Resources.sss);
      FileInfo physicalFile = new FileInfo(Path.Combine(Folder.TempFolder, "sss.abv"));
      IFormFile formFile = physicalFile.AsMockIFormFile();
      using (Stream stream = formFile.OpenReadStream())
      {
        ActionResult<ConvertResponse> res = _ctl.Convert(stream, formFile.FileName);
        Assert.NotNull(res);
        Assert.IsType<ActionResult<ConvertResponse>>(res);
      }
      File.Delete(Path.Combine(Folder.TempFolder, "sss.abv"));
    }
    void Test_NullFileFalse()
    {
      IFormFile file = new FormFile(new MemoryStream(Encoding.UTF8.GetBytes("dummy image")), 0, 0, "Data", "image.docx");
      using (Stream stream = file.OpenReadStream())
      {
        ActionResult<ConvertResponse> res = _ctl.Convert(stream, file.FileName);
        Assert.NotNull(res);
        Assert.NotNull(res.Value);
        Assert.IsType<ActionResult<ConvertResponse>>(res);
      }
    }
  }
}