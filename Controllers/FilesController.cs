using Microsoft.AspNetCore.Mvc;
using Web.Services;
using System;
using System.Collections.Generic;
using EventsManagement.Core;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace EventsManagement.Controllers
{
    [ApiController, Route("api/events")]
    public class EventsController : Controller
    {
        string accountName = "quickstarttest";
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
      var files = Request.Form;
      Console.WriteLine(files.Count);
      var fileIds = files.Keys.ToList();
      Console.WriteLine(fileIds.Count);
      var currentFiles = new List<EventFile>();
      fileIds.ForEach(async fileId =>
      {
        var id = int.Parse(fileId);
        files.TryGetValue(fileId, out StringValues file);          
        EventFile thisFile = new EventFile();
        var fileObject = JObject.Parse(file);
        thisFile.Name = fileObject["name"].ToString();
        thisFile.Size = int.Parse(fileObject["size"].ToString());
        thisFile.Id = id;
        await blob.UploadFiles(eventId, fileObject["name"].ToString(), fileObject);
        thisFile.Path = $"https://{accountName}.blob.core.windows.net/event{eventId}/{thisFile.Name}";
        currentFiles.Add(thisFile);
      });
      return currentFiles;
    }
    [HttpDelete("search")]
    public async void DeleteFile([FromQuery] int eventId, string name)
    {
      await blob.DeleteFile(eventId, name);
    }

  }
}
