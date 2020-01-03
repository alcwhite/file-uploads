using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using EventsManagement.Core;
using System.Collections.Generic;

using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Web.Services
{
  public class BlobService
  {
    
    private readonly BlobServiceClient blobServiceClient;

    public BlobService(BlobServiceClient blobServiceClient)
    {
      this.blobServiceClient = blobServiceClient;
    }
    
    public async Task<BlobContainerClient> GetContainerClient(int id)
    {
      string containerName = "event" + id.ToString();
      BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
      return containerClient;
    }
    
    public async Task CreateContainer(int id)
    {
      string containerName = "event" + id.ToString();

      BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(containerName);
    }
    public async Task UploadFiles(int thisEventid, string fileName, IFormFile fileInfo)
    {
      var containerClient = await this.GetContainerClient(thisEventid);
      BlobClient blobClient = containerClient.GetBlobClient(fileName);

      var readStream = fileInfo.OpenReadStream(); 
      await blobClient.UploadAsync(readStream);
      readStream.Close();
    }
    public async Task<BlobDownloadInfo> DownloadFile(int thisEventId, string fileName)
    {
      var containerClient = await this.GetContainerClient(thisEventId);
      BlobClient blobClient = containerClient.GetBlobClient(fileName);
      BlobDownloadInfo download = await blobClient.DownloadAsync();
      return download;
    }
    public async Task DeleteFile(int eventId, string fileName)
    {
      var containerClient = await this.GetContainerClient(eventId);
      await containerClient.DeleteBlobIfExistsAsync(fileName);
    }
    public async Task DeleteContainer(int id)
    {
      var containerClient = await this.GetContainerClient(id);
      await containerClient.DeleteAsync();
    }

    // ignore this; it's just for the test app
    public async Task<List<BlobItem>> GetBlobList(int id)
    {
      var allBlobContainers = this.blobServiceClient.GetBlobContainersAsync();
      var allContainers = new List<string>();
      await foreach (var container in allBlobContainers)
      {
        allContainers.Add(container.Name);
      }
      var containerClientExists = allContainers.Contains($"event{id}");
      var blobList = new List<BlobItem>();
      if (containerClientExists)
      {
        var containerClient = await this.GetContainerClient(id);
        var blobs = containerClient.GetBlobsAsync();
        await foreach (var blob in blobs)
        {
          blobList.Add(blob);
        }
      }
      return blobList;
    }

  }
}
