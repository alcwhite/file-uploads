using Microsoft.AspNetCore.Mvc;
using Web.Services;
using System.Collections.Generic;
using EventsManagement.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;

namespace EventsManagement.Controllers
{
    

  [ApiController, Route("api/files")]
  public class FilesController : Controller
  {
    private readonly BlobService blob;
    public FilesController(BlobService blob) => this.blob = blob;

  [HttpPost("upload")]
    public async Task<List<EventFile>> UploadFile([FromQuery] int eventId)
    {
      var files = HttpContext.Request.Form.Files;
      var currentFiles = new List<EventFile>();
      foreach (IFormFile file in files)
      {        
        EventFile thisFile = new EventFile();
        thisFile.Type = file.ContentType;
        thisFile.Size = file.Length;
        Console.WriteLine(file.Name);
        thisFile.Id = int.Parse(file.Name);
        var newName = await blob.UploadFile(eventId, file.FileName, file);
        thisFile.Name = newName;
        currentFiles.Add(thisFile);
      }
      return currentFiles;
    }
    [HttpDelete("delete")]
    public async void DeleteFile([FromQuery] int eventId, [FromQuery] string fileName)
    {
      await blob.DeleteFile(eventId, fileName);
    }
    [HttpGet("download")]
    public async Task<FileStreamResult> DownloadFile([FromQuery] string fileName, [FromQuery] int eventId)
    {
      var thisFile = await blob.DownloadFile(eventId, fileName);

      Response.Headers["Content-Disposition"] = $"inline; filename={fileName}";
      
      return File(thisFile.Content, thisFile.ContentType);
    }


    // ignore this; it's just for the test app
    [HttpGet("search")]
    public async Task<List<EventFile>> FindBlobs([FromQuery] int eventId)
    {
      var fileList = new List<EventFile>();
      var blobList = await blob.GetBlobList(eventId);
      var count = 0;
      foreach (var blob in blobList)
      {
        EventFile thisBlob = new EventFile();
        thisBlob.Name = blob.Name;
        thisBlob.Id = count;
        thisBlob.Size = (long)blob.Properties.ContentLength;
        thisBlob.Type = blob.Properties.ContentType;
        fileList.Add(thisBlob);
        count++;
      }
      return fileList;
    }

  }
}
