using Microsoft.AspNetCore.Mvc;
using Web.Services;
using System.Collections.Generic;
using EventsManagement.Core;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;

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
    public async Task<File> UploadFile([FromQuery] int eventId, [FromQuery] int fileId)
    {
        var file = HttpContext.Request.Body;
        // convert type Stream to be usable
        File thisFile = new File();
        thisFile.Name = file["name"].ToString();
        thisFile.Size = int.Parse(file["size"].ToString());
        thisFile.Id = fileId;
        await blob.UploadFiles(eventId, file["name"].ToString(), file);
        thisFile.Path = $"https://{accountName}.blob.core.windows.net/event{eventId}/{thisFile.Name}";
        
      return thisFile;
    }
    [HttpDelete("search")]
    public async void DeleteFile([FromQuery] int eventId, string name)
    {
      await blob.DeleteFile(eventId, name);
    }

  }
}
