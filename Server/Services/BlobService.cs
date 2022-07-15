using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace FilesInBlazor.Server.Services
{
    public interface IBlobService
    {
        /// <summary>
        /// Managed container name
        /// </summary>
        string Container { get; }

        /// <summary>
        /// Delete if exists blob in container
        /// </summary>
        /// <param name="fileName">named file in container</param>
        /// <returns>async task</returns>
        Task DeleteFile(string fileName);

        /// <summary>
        /// verify if exists blob in container
        /// </summary>
        /// <param name="fileName">named file in container</param>
        /// <returns>async task bool, true : exists, false : not exists</returns>
        Task<bool> ExistsFile(string fileName);

        /// <summary>
        /// Get blob in container
        /// </summary>
        /// <param name="fileName">named file in container</param>
        /// <returns>async task</returns>
        Task<BlobItem?> GetBlobItemOrDefault(string fileName);

        /// <summary>
        /// Upload blob in container
        /// </summary>
        /// <param name="fileName">named file in container</param>
        /// <param name="path">Local path</param>
        /// <returns>async task</returns>
        Task UploadFile(string fileName, string path);

        /// <summary>
        /// Update if exists blob in container
        /// </summary>
        /// <param name="fileName">named file in container</param>
        /// <param name="path">Local path</param>
        /// <returns>async task</returns>
        Task UpdateFile(string fileName, string path);

        /// <summary>
        /// Get all blobs of container
        /// </summary>
        /// <returns>enumerable blob items</returns>
        Task<IEnumerable<BlobItem>> GetBlobItems();
    }

    /// <summary>
    /// manage container Files in blob
    /// </summary>
    internal class BlobService : IBlobService
    {
        private const string Container = "files";

        string IBlobService.Container => Container;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public BlobService(IConfiguration configuration, ILogger logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task DeleteFile(string fileName)
        {
            _logger.LogInformation(nameof(DeleteFile));

            var blobClient = GetBlobClient(fileName);

            if (! await blobClient.ExistsAsync())
            {
                return;
            }

            await blobClient.DeleteAsync();
        }

        public async Task<bool> ExistsFile(string fileName)
        {
            _logger.LogInformation(nameof(ExistsFile));

            return await GetBlobClient(fileName).ExistsAsync();
        }

        public async Task UpdateFile(string fileName, string path)
        {
            _logger.LogInformation(nameof(UpdateFile));

            var blobClient = GetBlobClient(fileName);

            if (! await blobClient.ExistsAsync())
            {
                return;
            }

            await blobClient.UploadAsync(path, true);
        }

        public async Task<IEnumerable<BlobItem>> GetBlobItems()
        {
            _logger.LogInformation(nameof(GetBlobItems));

            List<BlobItem> blobItems = new();

            IAsyncEnumerator<BlobItem>? enumeratorAsyncBlob = null;

            try
            {
                enumeratorAsyncBlob = ContainerClient.GetBlobsAsync().GetAsyncEnumerator();

                while (await enumeratorAsyncBlob.MoveNextAsync())
                {
                    blobItems.Add(enumeratorAsyncBlob.Current);
                }
            }
            finally
            {
                if (enumeratorAsyncBlob is not null)
                    await enumeratorAsyncBlob.DisposeAsync();
            }

            return blobItems;
        }

        public async Task<BlobItem?> GetBlobItemOrDefault(string fileName)
        {
            _logger.LogInformation(nameof(GetBlobItemOrDefault));

            var containerClient = ContainerClient;

            BlobItem? blobItem = (await GetBlobItems()).FirstOrDefault(p => p.Name.Equals(fileName));

            return blobItem;
        }

        public async Task UploadFile(string fileName, string path)
        {
            _logger.LogInformation(nameof(UploadFile));

            var blobClient = GetBlobClient(fileName);

            await blobClient.UploadAsync(path, false);
        }

        #region Private members

        /// <summary>
        /// On get, generates a new instance based on the connection of '<see cref="_configuration"/>'
        /// </summary>
        private BlobContainerClient ContainerClient => new BlobServiceClient(_configuration.GetConnectionString("blob")).GetBlobContainerClient(Container);

        /// <summary>
        /// Generates a new instance based on the connection of '<see cref="_configuration"/>' and a new blob client
        /// </summary>
        /// <param name="fileName">named file blob</param>
        /// <returns>new <see cref="BlobClient"/></returns>
        private BlobClient GetBlobClient(string fileName) => ContainerClient.GetBlobClient(fileName);

        #endregion
    }
}
