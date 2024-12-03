using Azure;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ImageSharingWithCloud.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.EntityFrameworkCore;

namespace ImageSharingWithCloud.DAL
{
    public class ImageStorage : IImageStorage
    {
        private readonly ILogger<ImageStorage> _logger;

        private readonly CosmosClient _imageDbClient;

        private readonly string _imageDatabase;

        private readonly Container _imageDbContainer;

        private readonly BlobContainerClient _blobContainerClient;


        public ImageStorage(IConfiguration configuration,
                            CosmosClient imageDbClient,
                            ILogger<ImageStorage> logger)
        {
            this._logger = logger;

            /*
             * Use Cosmos DB client to store metadata for images.
             */
            _imageDbClient = imageDbClient;

            _imageDatabase = configuration[StorageConfig.ImageDbDatabase];
            var imageContainer = configuration[StorageConfig.ImageDbContainer];
            logger.LogInformation("ImageDb (Cosmos DB) is being accessed here: " + imageDbClient.Endpoint);
            logger.LogDebug("ImageDb using database {0} and container {1}",
                _imageDatabase, imageContainer);
            var imageDbDatabase = imageDbClient.GetDatabase(_imageDatabase);
            this._imageDbContainer = imageDbDatabase.GetContainer(imageContainer);

            /*
             * Use Blob storage client to store images in the cloud.
             */
            var imageStorageUriFromConfig = configuration[StorageConfig.ImageStorageUri];
            if (imageStorageUriFromConfig == null)
            {
                throw new ArgumentNullException("Missing Blob service URI in configuration: " + StorageConfig.ImageStorageUri);
            }
            var imageStorageUri = new Uri(imageStorageUriFromConfig);

            // TODO (done) get the shared key credential for accessing blob storage.
            StorageSharedKeyCredential credential = new StorageSharedKeyCredential(
                configuration[StorageConfig.ImageStorageAccountName],
                configuration[StorageConfig.ImageStorageAccessKey]);


            var blobServiceClient = new BlobServiceClient(imageStorageUri, credential, null);

            var storageContainer = configuration[StorageConfig.ImageStorageContainer];
            if (storageContainer == null)
            {
                throw new KeyNotFoundException("Missing Blob container name in configuration: " + StorageConfig.ImageStorageContainer);
            }
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(storageContainer);
            logger.LogInformation("ImageStorage (Blob storage) being accessed here: " + _blobContainerClient.Uri);
        }

        /**
         * Use this to generate the singleton Cosmos DB client that is injected into all instances of ImageStorage.
         */
        public static CosmosClient GetImageDbClient(IWebHostEnvironment environment, IConfiguration configuration)
        {
            string imageDbUri = configuration[StorageConfig.ImageDbUri];
            if (imageDbUri == null)
            {
                throw new ArgumentNullException("Missing configuration: " + StorageConfig.ImageDbUri);
            }
            string imageDbAccessKey = configuration[StorageConfig.ImageDbAccessKey];

            CosmosClientOptions cosmosClientOptions = null;
            //if (environment.IsDevelopment())
            //{
            //    cosmosClientOptions = new CosmosClientOptions()
            //    {
            //        HttpClientFactory = () =>
            //        {
            //            HttpMessageHandler httpMessageHandler = new HttpClientHandler()
            //            {
            //                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            //            };

            //            return new HttpClient(httpMessageHandler);
            //        },
            //        ConnectionMode = ConnectionMode.Gateway
            //    };
            //}

            CosmosClient imageDbClient = new CosmosClient(imageDbUri, imageDbAccessKey, cosmosClientOptions);
            // builder.Services.AddSingleton<CosmosClient>(imageDbClient);
            return imageDbClient;
        }

        /*
         * 
         */
        public async Task InitImageStorage()
        {

            _logger.LogInformation("Initializing image storage (Cosmos DB)....");
            await _imageDbClient.CreateDatabaseIfNotExistsAsync(_imageDatabase);
            _logger.LogInformation("....initialization completed.");
        }

        /*
         * Save image metadata in the database.
         */
        public async Task<string> SaveImageInfoAsync(Image image)
        {
            image.Id = Guid.NewGuid().ToString();
            _logger.LogDebug(image.Id);
            // TODO (done)
            Image item = await _imageDbContainer.CreateItemAsync<Image>(
                item: image,
                partitionKey: new PartitionKey(image.UserId));
            String itemId = image.Id;
            
            return itemId;

        }

        public async Task<Image> GetImageInfoAsync(string userId, string imageId)
        {
            return await _imageDbContainer.ReadItemAsync<Image>(imageId, new PartitionKey(userId));
        }

        public async Task<IList<Image>> GetAllImagesInfoAsync()
        {
            var results = new List<Image>();
            var iterator = _imageDbContainer.GetItemLinqQueryable<Image>()
                                           .Where(im => im.Valid && im.Approved)
                                           .ToFeedIterator();
            // Iterate over the paged query result.
            while (iterator.HasMoreResults)
            {
                var images = await iterator.ReadNextAsync();
                // Iterate over a page in the query result.
                foreach (var image in images)
                {
                    results.Add(image);
                }
            }
            return results;
        }

        public async Task<IList<Image>> GetImageInfoByUserAsync(ApplicationUser user)
        {
            var results = new List<Image>();
            var query = _imageDbContainer.GetItemLinqQueryable<Image>()
                                        .WithPartitionKey<Image>(user.Id)
                                        .Where(im => im.Valid && im.Approved);
            // TODO (done) complete this

            var response = await query.ToListAsync();
            results.AddRange(response);

            return results;
        }

        public async Task UpdateImageInfoAsync(Image image)
        {
            await _imageDbContainer.ReplaceItemAsync<Image>(image, image.Id, new PartitionKey(image.UserId));
        }

        /*
         * Remove both image files and their metadata records in the database.
         */
        public async Task RemoveImagesAsync(ApplicationUser user)
        {
            var query = _imageDbContainer.GetItemLinqQueryable<Image>().WithPartitionKey<Image>(user.Id);
            var iterator = query.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var images = await iterator.ReadNextAsync();
                foreach (Image image in images)
                {
                    await RemoveImageAsync(image);
                }
            }
            /*
             * Not available?
             * await imageDbContainer.DeleteAllItemsByPartitionKeyStreamAsync(new PartitionKey(image.UserId))
             */
        }

        public async Task RemoveImageAsync(Image image)
        {
            try
            {
                await _imageDbContainer.DeleteItemAsync<Image>(image.Id, new PartitionKey(image.UserId));
                await RemoveImageFileAsync(image);
            }
            catch (RequestFailedException e)
            {
                _logger.LogError("Exception while removing blob image: {trace}", e.StackTrace);
            }
        }


        /**
         * The name of a blob containing a saved image (imageId is key for metadata record).
         */
        private static string BlobName(string userId, string imageId)
        {
            return "image-" + imageId + ".jpg";
        }

        private string BlobUri(string userId, string imageId)
        {
            return _blobContainerClient.Uri + "/" + BlobName(userId, imageId);
        }

        public async Task SaveImageFileAsync(IFormFile imageFile, string userId, string imageId)
        {
            _logger.LogInformation("Saving image with id {imageId} to blob storage", imageId);

            var headers = new BlobHttpHeaders()
            {
                ContentType = "image/jpeg"
            };

            /*
             * TODO upload data to blob storage
             * 
             * Tip: You need to reset the stream position to the beginning before uploading:
             * See https://stackoverflow.com/a/47611795.
             */
            
            var stream = imageFile.OpenReadStream();
            stream.Seek(0, SeekOrigin.Begin);
            
            BlobClient blobClient = _blobContainerClient.GetBlobClient(BlobName(userId, imageId));
            
            await blobClient.UploadAsync(stream, httpHeaders: headers);
            _logger.LogInformation("Image {imageId} uploaded to blob storage!", imageId);


        }

        private async Task RemoveImageFileAsync(Image image)
        {
            BlobClient blobClient = _blobContainerClient.GetBlobClient(BlobName(image.UserId, image.Id));
            _logger.LogInformation("Deleting image blob at URI {uri}", blobClient.Uri);
            await blobClient.DeleteAsync();
        }

        public string ImageUri(string userId, string imageId)
        {
            return BlobUri(userId, imageId);
        }

    }
}
