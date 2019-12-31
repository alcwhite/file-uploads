using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using EventsManagement.Core;

using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Azure.Storage.Blob;

namespace Web.Services
{
  public class BlobService
  {
    
    static string connectionString = "DefaultEndpointsProtocol=https;AccountName=quickstarttest;AccountKey=PrBYaCosCT+xVfWV3ngeZeQTqMhKzIJxxi82eeRHLJ+eV9+Hh8CrKVRN/lfzPeD7otSBahh4z0dVSeTfQwLD/g==;EndpointSuffix=core.windows.net";
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
    public async Task UploadFiles(int thisEventid, string fileName, JObject fileInfo)
    {
      var containerClient = await this.GetContainerClient(thisEventid);
      BlobClient blobClient = containerClient.GetBlobClient(fileName);

      Console.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

      using (StringReader reader = new StringReader(fileInfo.ToString()))
      {
        string readText = await reader.ReadToEndAsync();
        await blobClient.UploadAsync(readText);
      };

      
    }
    public async Task DownloadFile(int thisEventId, string fileName)
    {
      var containerClient = await this.GetContainerClient(thisEventId);
      BlobClient blobClient = containerClient.GetBlobClient(fileName);

      string downloadFilePath = "";

      Console.WriteLine("\nDownloading blob to\n\t{0}\n", downloadFilePath);

      BlobDownloadInfo download = await blobClient.DownloadAsync();

      using FileStream downloadFileStream = System.IO.File.OpenWrite(downloadFilePath);
      await download.Content.CopyToAsync(downloadFileStream);
      downloadFileStream.Close();
    }
    public async Task DeleteFile(int eventId, string fileName)
    {
      var containerClient = await this.GetContainerClient(eventId);
      var blobs = containerClient.DeleteBlobIfExistsAsync(fileName);
    }
    public async Task DeleteContainer(int id)
    {
      var containerClient = await this.GetContainerClient(id);
      await containerClient.DeleteAsync();
    }

  }
}
