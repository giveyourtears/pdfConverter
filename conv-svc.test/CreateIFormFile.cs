using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Inex.Umk.Conv.Test
{
  public static class Test
  {
    public static IFormFile AsMockIFormFile(this FileInfo physicalFile)
    {
      var formFile = new Mock<IFormFile>();
      FileStream fs = physicalFile.OpenRead();
      int bytesCount = (int)fs.Length;
      byte[] bytes = new byte[bytesCount];
      int bytesRead = fs.Read(bytes, 0, bytesCount);
      Stream stream = new MemoryStream(bytes);
      var writer = new StreamWriter(stream);
      var fileName = physicalFile.Name;
      formFile.Setup(_ => _.FileName).Returns(fileName);
      formFile.Setup(_ => _.Length).Returns(stream.Length);
      formFile.Setup(m => m.OpenReadStream()).Returns(stream);
      formFile.Setup(m => m.ContentDisposition).Returns(string.Format("inline; filename={0}", fileName));
      formFile.Verify();
      return formFile.Object;
    }
  }
}
