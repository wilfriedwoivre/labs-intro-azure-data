using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;
using Microsoft.Azure.Storage.Blob;
using System.Linq;
using Soat.Masterclass.Labs.Models.Storage;

namespace Soat.Masterclass.Labs.Repositories
{
    public class StorageBlobHelper : IStorageBlobHelper
    {
        private readonly CloudBlobClient blobClient;
        private string containerName;
        public StorageBlobHelper(string accountName, string accountKey, string container)
        {
            var storageAccount = new CloudStorageAccount(new StorageCredentials(accountName, accountKey), true);
            blobClient = storageAccount.CreateCloudBlobClient();
            this.containerName = container;

            CreateContainerIfNotExistsAsync().Wait();
        }

        public async Task CreateBlobAsync(string fileName, string content)
        {
            var container = blobClient.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(fileName);

            if (await blob.ExistsAsync())
            {
                throw new Exception("File already exists");
            }

            await blob.UploadTextAsync(content);
        }

        public async Task UpdateBlobAsync(string fileName, string content)
        {
            var container = blobClient.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(fileName);
            await blob.UploadTextAsync(content);
        }

        public async Task DeleteBlobAsync(string fileName)
        {
            var container = blobClient.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(fileName);
            await blob.DeleteIfExistsAsync();
        }

        public async Task<BlobItem> GetBlobContentAsync(string fileName)
        {
            var container = blobClient.GetContainerReference(containerName);
            var blob = container.GetBlockBlobReference(fileName);
            if (!(await blob.ExistsAsync()))
            {
                throw new Exception("File doesn't exists");
            }

            return new BlobItem()
            {
                Name = blob.Name,
                LastModification = blob.Properties.LastModified.Value.DateTime,
                Size = blob.Properties.Length,
                Content = await blob.DownloadTextAsync()
            };
        }

        public async Task<IEnumerable<BlobItem>> ListBlobAsync()
        {
            var container = blobClient.GetContainerReference(containerName);

            var results = new List<BlobItem>();

            BlobContinuationToken token = null;
            do
            {
                var segment = await container.ListBlobsSegmentedAsync(token);
                results.AddRange(segment.Results.Where(b => b is CloudBlockBlob).Select(b =>
                {
                    var blob = b as CloudBlockBlob;

                    return new BlobItem()
                    {
                        Name = blob.Name,
                        LastModification = blob.Properties.LastModified.Value.DateTime,
                        Size = blob.Properties.Length
                    };

                }));

                token = segment.ContinuationToken;

            } while (token != null);
            return results;
        }

        private async Task CreateContainerIfNotExistsAsync()
        {
            var container = blobClient.GetContainerReference(containerName);
            await container.CreateIfNotExistsAsync();
        }
    }
}