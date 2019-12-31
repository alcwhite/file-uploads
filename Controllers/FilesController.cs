using Microsoft.AspNetCore.Mvc;
using Web.Services;
using System;
using System.Collections.Generic;
using EventsManagement.Core;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

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
      // empty
      var files = HttpContext.Request.Form.Files;
      Console.WriteLine(files.Count);
      var currentFiles = new List<EventFile>();
      foreach (IFormFile file in files)
      {        
        EventFile thisFile = new EventFile();
        thisFile.Name = file.FileName;
        thisFile.Size = 100;
        thisFile.Id = 0;
        await blob.UploadFiles(eventId, thisFile.Name, file);
        thisFile.Path = $"https://{accountName}.blob.core.windows.net/event{eventId}/{thisFile.Name}";
        currentFiles.Add(thisFile);
      }
      return currentFiles;
    }
    [HttpDelete("search")]
    public async void DeleteFile([FromQuery] int eventId, string name)
    {
      await blob.DeleteFile(eventId, name);
    }

  }
}
