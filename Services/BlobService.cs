using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

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

    public string RenameFile(string fileName, IFormFile fileInfo)
    {
      var extensionIndex = fileName.LastIndexOf('.');
      var id = fileInfo.Name;
      var noExtension = fileName.Substring(0, extensionIndex);
      var onlyExtension = fileName.Substring(extensionIndex);
      return $"{noExtension}{id}{onlyExtension}";
    }
    
    public async Task<string> UploadFile(int thisEventid, string fileName, IFormFile fileInfo)
    {
      var containerClient = await this.GetContainerClient(thisEventid);
      await containerClient.CreateIfNotExistsAsync(PublicAccessType.None);

      var blobs = containerClient.GetBlobsAsync();
      var blobNames = new List<string>();
      await foreach (var blob in blobs) blobNames.Add(blob.Name);
      var newName = blobNames.Contains(fileName) ? RenameFile(fileName, fileInfo) : fileName;
      
      BlobClient blobClient = containerClient.GetBlobClient(newName);
      using var readStream = fileInfo.OpenReadStream(); 
      await blobClient.UploadAsync(readStream);

      return newName;
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
      var containerClient = await this.GetContainerClient(id);
      var blobs = containerClient.GetBlobsAsync();
      var allBlobs = new List<BlobItem>();
      await foreach (var blob in blobs) allBlobs.Add(blob);

      return allBlobs;
    }

  }
}
