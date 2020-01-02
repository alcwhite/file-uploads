using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace Web.Services
{
  public class BlobService
  {
    
    static string connectionString = "DefaultEndpointsProtocol=https;AccountName=quickstarttest;AccountKey=PrBYaCosCT+xVfWV3ngeZeQTqMhKzIJxxi82eeRHLJ+eV9+Hh8CrKVRN/lfzPeD7otSBahh4z0dVSeTfQwLD/g==;EndpointSuffix=core.windows.net";
    static string accountName = "quickstarttest";
    public BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
  

    
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

      Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

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

  }
}
