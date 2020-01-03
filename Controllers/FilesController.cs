using Microsoft.AspNetCore.Mvc;
using Web.Services;
using System.Collections.Generic;
using EventsManagement.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace EventsManagement.Controllers
{
    [ApiController, Route("api/events")]
    public class EventsController : Controller
    {
        private readonly BlobService blob;
        public EventsController(BlobService blob) => this.blob = blob;
        [HttpPost("{id}")]
            public int AddEvent(int id)
            {
              // basically create container when create event
                blob.CreateContainer(id);
                return id;
            }
    }
    

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
        thisFile.Name = file.FileName;
        thisFile.Size = file.Length;
        await blob.UploadFiles(eventId, thisFile.Name, file);
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
      foreach (var blob in blobList)
      {
        EventFile thisBlob = new EventFile();
        thisBlob.Name = blob.Name;
        thisBlob.Size = (long)blob.Properties.ContentLength;
        thisBlob.Type = blob.Properties.ContentType;
        fileList.Add(thisBlob);
      }
      return fileList;
    }

  }
}
