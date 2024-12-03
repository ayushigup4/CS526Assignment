using Azure;
using Azure.Data.Tables;
using ImageSharingWithCloud.Models;
using ImageSharingWithCloud.Models.ViewModels;

namespace ImageSharingWithCloud.DAL
{
    public class LogContext : ILogContext
    {
        private TableClient _tableClient;

        private ILogger<LogContext> logger;

        public LogContext(IConfiguration configuration, ILogger<LogContext> logger)
        {
            this.logger = logger;

            Uri logTableServiceUri = null;
            string logTableName = null;
            /*
             * TODO (done) Get the table service URI and table name.
             */
            
            logTableServiceUri= new Uri(configuration[StorageConfig.LogEntryDbUri]);
            logTableName= configuration[StorageConfig.LogEntryDbTable];
            
            logger.LogInformation("Looking up Storage URI... ");
            
            logger.LogInformation("Using Table Storage URI: " + logTableServiceUri);
            logger.LogInformation("Using Table: " + logTableName);
            
            // Access key will have been loaded from Secrets (Development) or Key Vault (Production)
            TableSharedKeyCredential credential = new TableSharedKeyCredential(
                configuration[StorageConfig.LogEntryDbAccountName],
                configuration[StorageConfig.LogEntryDbAccessKey]);

            logger.LogInformation("Initializing table client....");
            // TODO (done) Set the table client for interacting with the table service (see TableClient constructors)
            
            _tableClient = new TableClient(logTableServiceUri, logTableName, credential);
            
            

            logger.LogInformation("....table client URI = " + _tableClient.Uri);
        }


        public async Task AddLogEntryAsync(string userId, string userName, ImageView image)
        {
            var entry = new LogEntry(userId, image.Id)
            {
                Username = userName,
                Caption = image.Caption,
                ImageId = image.Id,
                Uri = image.Uri
            };

            logger.LogDebug("Adding log entry for image: {0}", image.Id);

            Response response = null;
            // TODO (done) add a log entry for this image view
            
            response = await _tableClient.AddEntityAsync(entry);
            logger.LogInformation("Log entry successfully added for image: {0} by user: {1}", image.Id, userName);


            if (response.IsError)
            {
                logger.LogError("Failed to add log entry, HTTP response {response}", response.Status);
            } 
            else
            {
                logger.LogDebug("Added log entry with HTTP response {response}", response.Status);
            }

        }

        public AsyncPageable<LogEntry> Logs(bool todayOnly = false)
{
            if (todayOnly)
            {
                // TODO (done) just return logs for today 
                return _tableClient.QueryAsync<LogEntry>(logEntry => logEntry.EntryDate.Date == DateTime.UtcNow.Date);
            }
            else
            {
                return _tableClient.QueryAsync<LogEntry>(logEntry => true);
            }
}

    }
}