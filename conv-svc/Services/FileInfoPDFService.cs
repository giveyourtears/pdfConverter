using Inex.Umk.IO;
using Inex.Umk.Messages;
using Microsoft.Extensions.Logging;
using Syncfusion.DocIO.DLS;
using Syncfusion.DocIORenderer;
using Syncfusion.EJ2.PdfViewer;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using Syncfusion.Pdf.Parsing;
using Syncfusion.Presentation;
using Syncfusion.PresentationToPdfConverter;
using Syncfusion.XlsIO;
using Syncfusion.XlsIORenderer;
using Syncfusion.XPS;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Inex.Umk.Conv.Services
{
  public class FileInfoPDFService
  {
    ILogger<FileInfoPDFService> _logger;

    public FileInfoPDFService(ILogger<FileInfoPDFService> logger)
    {
      _logger = logger;
    }
    public byte[] Convert(Stream file, string fileName)
    {
      _logger.LogDebug($"Start convert file, check file extenstion");
      string ext = System.IO.Path.GetExtension(fileName).Trim('.');
      FileExtension.Extensions type = CheckFileExtension.GetFileExtension(ext);
      switch (type)
      {
        case FileExtension.Extensions.Doc:
        case FileExtension.Extensions.Rtf:
        case FileExtension.Extensions.Dot:
          return WordToPdf(file);
      }
      _logger.LogDebug("FileInfoPDFService.Convert....OK");
      return null;
    }

    byte[] WordToPdf(Stream file)
    {
      _logger.LogDebug($"Start convert word file");
      byte[] buff;
      using (WordDocument document = new WordDocument(file, Syncfusion.DocIO.FormatType.Automatic))
      using (DocIORenderer render = new DocIORenderer())
      using (PdfDocument pdf = render.ConvertToPDF(document))
      using (MemoryStream memoryStream = new MemoryStream())
      {
        pdf.Save(memoryStream);
        memoryStream.Position = 0;
        buff = memoryStream.ToArray();
        _logger.LogDebug("FileInfoPDFService.WordToPdf....OK");
        return buff;
      }
    }

    public byte[] ExcelToPDF(Stream file)
    {
      _logger.LogDebug($"Start convert excel file");
      byte[] buff;
      try
      {
        using (ExcelEngine excelEngine = new ExcelEngine())
        using (MemoryStream memoryStream = new MemoryStream())
        {
          IApplication application = excelEngine.Excel;
          application.DefaultVersion = ExcelVersion.Excel2013;
          IWorkbook workbook = application.Workbooks.Open(file);
          XlsIORenderer renderer = new XlsIORenderer();
          using (PdfDocument pdfDocument = renderer.ConvertToPDF(workbook))
          {
            pdfDocument.Save(memoryStream);
            memoryStream.Position = 0;
            buff = memoryStream.ToArray();
            _logger.LogDebug("FileInfoPDFService.ExcelToPDF....OK");
            return buff;
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
      }
      return null;
    }

    public byte[] PresentationToPDF(Stream file)
    {
      _logger.LogDebug($"Start convert presentation file");
      byte[] buff;
      try
      {
        using (IPresentation presentation = Presentation.Open(file))
        using (PdfDocument pdfDocument = PresentationToPdfConverter.Convert(presentation))
        using (MemoryStream memoryStream = new MemoryStream())
        {
          pdfDocument.Save(memoryStream);
          memoryStream.Position = 0;
          buff = memoryStream.ToArray();
          _logger.LogDebug("FileInfoPDFService.PresentationToPDF....OK");
          return buff;
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
      }
      return null;
    }

    public byte[] XPSToPDF(Stream file)
    {
      _logger.LogDebug($"Start convert xps file");
      byte[] buff;
      try
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          XPSToPdfConverter converter = new XPSToPdfConverter();
          using (PdfDocument document = converter.Convert(file))
          {
            document.Save(memoryStream);
            memoryStream.Position = 0;
            buff = memoryStream.ToArray();
            _logger.LogDebug("FileInfoPDFService.XPSToPDF....OK");
            return buff;
          }
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
      }
      return null;
    }
    public byte[] TiffToPDF(Stream file)
    {
      _logger.LogDebug($"Start convert tiff file");
      byte[] buff;
      try
      {
        using (MemoryStream memoryStream = new MemoryStream())
        using (PdfDocument document = new PdfDocument())
        {
          document.PageSettings.Margins.All = 0;
          PdfTiffImage tiffImage = new PdfTiffImage(file);
          int frameCount = tiffImage.FrameCount;
          for (int i = 0; i < frameCount; i++)
          {
            PdfPage page = document.Pages.Add();
            PdfGraphics graphics = page.Graphics;
            tiffImage.ActiveFrame = i;
            graphics.DrawImage(tiffImage, 0, 0, page.GetClientSize().Width, page.GetClientSize().Height);
          }
          document.Save(memoryStream);
          memoryStream.Position = 0;
          buff = memoryStream.ToArray();
          _logger.LogDebug("FileInfoPDFService.TiffToPDF....OK");
          return buff;
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
      }
      return null;
    }

    public byte[] ImageToPDF(Stream file)
    {
      _logger.LogDebug($"Start convert image files");
      byte[] buff;
      try
      {
        using (MemoryStream memoryStream = new MemoryStream())
        using (PdfDocument document = new PdfDocument())
        {
          PdfPage page = document.Pages.Add();
          PdfGraphics pdfGraphics = page.Graphics;
          PdfBitmap image = new PdfBitmap(file);
          pdfGraphics.DrawImage(image, 0, 0);
          document.Save(memoryStream);
          memoryStream.Position = 0;
          buff = memoryStream.ToArray();
          _logger.LogDebug("FileInfoPDFService.ImageToPDF....OK");
          return buff;
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex.Message);
      }
      return null;
    }


    public ExtractTextFileResponse Extract(Stream pdfStream)
    {
      _logger.LogDebug($"Extract text from file");

      pdfStream.Position = 0;
      PdfRenderer pdfRenderer = new PdfRenderer();
      pdfRenderer.Load(pdfStream);
      ExtractTextFileResponse extractResult = new ExtractTextFileResponse();

      for (int i = 0; i < pdfRenderer.PageCount; i++)
      {
        string text = pdfRenderer.ExtractText(i, out _);
        extractResult.Values.Add(new KeyValuePair<int, string>(i, text));
      }

      _logger.LogDebug("FileInfoPDFService.Extract....OK");
      return extractResult;
    }

    public PreviewFileResponse Preview(Stream stream, int pageIndex)
    {
      _logger.LogDebug($"Create first page preview from file");
      using (MemoryStream streamImage = new MemoryStream())
      using (PdfRenderer renderer = new PdfRenderer())
      {
        renderer.Load(stream);
        using (Bitmap bitmapimage = renderer.ExportAsImage(pageIndex))
        {
          bitmapimage.Save(streamImage, ImageFormat.Jpeg);
          PreviewFileResponse previewResult = new PreviewFileResponse { Image = streamImage.ToArray() };
          _logger.LogDebug($"FileInfoPDFService.Preview....OK");
          return previewResult;
        }
      }
    }
  }
}
