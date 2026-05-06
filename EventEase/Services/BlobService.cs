using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace EventEase.Services
{
    public class BlobService
    {
        private readonly string _connectionString;

        public BlobService(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("BlobConnection");
        }

        public async Task<string> UploadFileAsync(IFormFile file, string containerName)
        {
            var containerClient = new BlobContainerClient(_connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);

            var blobClient = containerClient.GetBlobClient(fileName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return blobClient.Uri.ToString();
        }
    }
}