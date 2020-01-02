using Microsoft.AspNetCore.Mvc;
using Web.Services;
using System.Collections.Generic;
using EventsManagement.Core;
using System.Threading.Tasks;
using System.Linq;
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
                blob.CreateContainer(id);
                return id;
            }
    }
    

  [ApiController, Route("api/files")]
  public class FilesController : Controller
  {
    string accountName = "quickstarttest";
    private readonly BlobService blob;
    public FilesController(BlobService blob) => this.blob = blob;

  [HttpPost("upload")]
    public async Task<List<EventFile>> UploadFile([FromQuery] int eventId)
    {
      var files = HttpContext.Request.Form.Files;
      var ids = HttpContext.Request.Form.Keys;
      var allFiles = ids.Zip(files, (k, v) => new { k, v }).ToDictionary(x => x.k, x => x.v);
      var currentFiles = new List<EventFile>();
      foreach (KeyValuePair<string, IFormFile> file in allFiles)
      {        
        EventFile thisFile = new EventFile();
        thisFile.Name = file.Value.FileName;
        thisFile.Size = file.Value.Length;
        thisFile.Id = int.Parse(file.Key);
        await blob.UploadFiles(eventId, thisFile.Name, file.Value);
        thisFile.Path = $"https://{accountName}.blob.core.windows.net/event{eventId}/{thisFile.Name}";
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

  }
}
