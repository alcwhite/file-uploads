using Microsoft.AspNetCore.Mvc;
using Web.Services;
using System.Collections.Generic;
using EventsManagement.Core;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Diagnostics;

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

  [HttpPost("")]
    public async Task<List<File>> UploadFile([FromBody] string files)
    {
      var f = JObject.Parse(files);
      var allInfo = f.ToObject<Dictionary<string, string>>();
      allInfo.ToDictionary(kv => kv.Key);
      allInfo.TryGetValue("eventInfo", out string eventIdString);
      var eventId = int.Parse(eventIdString);
      var convertedFiles = new List<File>();
      var allFiles = allInfo.Where(kv => kv.Key.Contains("File"));
      foreach (var kv in allFiles)
      {
        var fileId = kv.Key.Substring(5);
        var file = JObject.Parse(kv.Value);
        File thisFile = new File();
        thisFile.Name = file["name"].ToString();
        thisFile.Size = int.Parse(file["size"].ToString());
        thisFile.Id = int.Parse(fileId);
        await blob.UploadFiles(eventId, file["name"].ToString(), file.ToString());
        thisFile.Path = $"https://{accountName}.blob.core.windows.net/event{eventId}/{thisFile.Name}";
        convertedFiles.Add(thisFile);
      };
      return convertedFiles;
    }
    [HttpDelete("search")]
    public async void DeleteFile([FromQuery] int eventId, string name)
    {
      await blob.DeleteFile(eventId, name);
    }

  }
}
