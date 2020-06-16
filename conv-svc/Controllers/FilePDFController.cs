using Inex.Umk.Conv.Services;
using Inex.Umk.IO;
using Inex.Umk.Messages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Inex.Umk.Conv.Controllers
{
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class FilePdfController : ControllerBase
  {
    FileInfoPDFService _fileInfoPdf;
    ILogger<FilePdfController> _logger;
    public FilePdfController(FileInfoPDFService fileInfoPdf, ILogger<FilePdfController> logger)
    {
      _fileInfoPdf = fileInfoPdf;
      _logger = logger;
    }

    [HttpPost]
    public ActionResult<ProcessResponse> Process(IFormFile file)
    {
      _logger.LogDebug($"Process response");
      ProcessResponse response = new ProcessResponse();
      string fileExtension = Path.GetExtension(file.FileName).Trim('.');
      try
      {
        using (Stream stream = file.OpenReadStream())
        {
          if (file.Length == 0)
            throw new Exception("Файл не может быть пуст");
          if (CheckFileExtension.GetFileExtension(fileExtension) == FileExtension.Extensions.Unknown)
            throw new Exception("Неподдерживаемый тип файла");
          if (CheckFileExtension.GetFileExtension(fileExtension) == FileExtension.Extensions.Pdf)
          {
            ExtractTextFileResponse extractResult = _fileInfoPdf.Extract(stream);
            PreviewFileResponse previewResult = _fileInfoPdf.Preview(stream, 0);
            ProcessFileResponse result = new ProcessFileResponse { Image = previewResult.Image, TextValues = extractResult.Values };
            response.Values.Add(result);
            return response;
          }
          else
          {
            byte[] resultPdf = _fileInfoPdf.Convert(stream, file.FileName);
            using (Stream pdfStream = new MemoryStream(resultPdf))
            {
              ExtractTextFileResponse extractResult = _fileInfoPdf.Extract(pdfStream);
              PreviewFileResponse previewResult = _fileInfoPdf.Preview(pdfStream, 0);
              ProcessFileResponse result = new ProcessFileResponse { Name = file.FileName, Pdf = resultPdf, TextValues = extractResult.Values, Image = previewResult.Image };
              response.Values.Add(result);
            }
          }
        }
        _logger.LogDebug("PdfController.Process() OK");
        return Ok(response);
      }
      catch (Exception e)
      {
        _logger.LogError(e.Message);
        return BadRequest($"Ошибка обработки файла '{file.FileName}'. Фаил неизвестного формата или файл поврежден");
      }
    }

    [HttpPost]
    [Route("convert")]
    public ActionResult<ConvertResponse> Convert(Stream stream, string fileName)
    {
      try
      {
        string fileExtension = Path.GetExtension(fileName).Trim('.');
        if (stream.Length == 0)
          throw new Exception("Поток не может быть пуст");
        if (CheckFileExtension.GetFileExtension(fileExtension) == FileExtension.Extensions.Unknown)
          throw new Exception("Неподдерживаемый тип файла");
        if (CheckFileExtension.GetFileExtension(fileExtension) == FileExtension.Extensions.Pdf)
          throw new Exception("Выберите другой файл, конвертация pdf в pdf не нужна");
        _logger.LogDebug($"Start convert");
        ConvertResponse responseConvert = new ConvertResponse();
        byte[] result = _fileInfoPdf.Convert(stream, fileName);
        ConvertFileResponse convertResult = new ConvertFileResponse { Name = fileName, Pdf = result };
        responseConvert.Values.Add(convertResult);
        _logger.LogDebug("PdfController.Convert() OK");
        return Ok(responseConvert);
      }
      catch (Exception e)
      {
        _logger.LogError(e.Message);
        return BadRequest(e.Message);
      }
    }

    [HttpPost]
    [Route("extract")]
    public ActionResult<ExtractTextResponse> ExtractText(Stream stream)
    {
      try
      {
        if (stream.Length == 0)
          throw new Exception("Поток не может быть пуст");
        _logger.LogDebug($"Start extract text");
        ExtractTextResponse responseExtract = new ExtractTextResponse();
        ExtractTextFileResponse extractResult = _fileInfoPdf.Extract(stream);
        responseExtract.Values.Add(extractResult);
        _logger.LogDebug("PdfController.ExtractText() OK");
        return Ok(responseExtract);
      }
      catch (Exception e)
      {
        _logger.LogError(e.Message);
        return BadRequest(e.Message);
      }
    }

    [HttpPost]
    [Route("preview")]
    public ActionResult<PreviewResponse> PreviewPdf(Stream stream, int pageIndex)
    {
      try
      {
        if (stream.Length == 0)
          throw new Exception("Поток не может быть пуст");
        if (pageIndex < 0)
          throw new Exception("Номер страницы не может быть меньше 0");
        _logger.LogDebug($"Start create preview");
        PreviewResponse responsePreview = new PreviewResponse();
        PreviewFileResponse previewResult = _fileInfoPdf.Preview(stream, pageIndex);
        responsePreview.Values.Add(previewResult);
        _logger.LogDebug("PdfController.PreviewPdf() OK");
        return Ok(responsePreview);
      }
      catch (Exception e)
      {
        _logger.LogError(e.Message);
        return BadRequest(e.Message);
      }
    }
  }
}
