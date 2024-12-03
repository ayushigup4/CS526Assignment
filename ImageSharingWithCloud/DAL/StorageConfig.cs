using Microsoft.Data.SqlClient;

namespace ImageSharingWithCloud.DAL
{
    public class StorageConfig
    {
        /*
         * Keys for user database metadata.
        */
        public const string ApplicationDbConnString = "Data:ApplicationDb:ConnectionString";

        public const string ApplicationDbUser = "Credentials:ApplicationDb:User";

        public const string ApplicationDbPassword = "Credentials:ApplicationDb:Password";

        public const string ApplicationDbDatabase = "Data:ApplicationDb:Database";

        /*
         * Keys for image database (Cosmos DB) metadata.
         */
        public const string ImageDbUri = "Data:ImageDb:Uri";

        public const string ImageDbAccountName = "Data:ImageDb:AccountName";

        public const string ImageDbAccessKey = "Credentials:ImageDb:AccessKey";

        public const string ImageDbDatabase = "Data:ImageDb:Database";

        public const string ImageDbContainer = "Data:ImageDb:Container";

        /*
         * Keys for image storage (Blob) metadata.
         */
        public const string ImageStorageUri = "Data:ImageStorage:Uri";

        public const string ImageStorageAccountName = "Data:ImageStorage:AccountName";

        public const string ImageStorageAccessKey = "Credentials:ImageStorage:AccessKey";

        public const string ImageStorageContainer = "Data:ImageStorage:Container";

        /*
         * Keys for log storage (Table) metadata.
         */
        public const string LogEntryDbUri = "Data:LogEntryDb:Uri";

        public const string LogEntryDbAccountName = "Data:LogEntryDb:AccountName";

        public const string LogEntryDbAccessKey = "Credentials:LogEntryDb:AccessKey";

        public const string LogEntryDbTable = "Data:LogEntryDb:Table";

        /*
         * URI for key vault with credentials for databases.
         */
        public const string KeyVaultUri = "Data:KeyVault:Uri";


        /*
         * Add database and user credentials to database connection string.
         */
        public static string GetDatabaseConnectionString(string connectionString, string database, string user=null, string password=null)
        {
            var connStringBuilder = new SqlConnectionStringBuilder(connectionString);
            connStringBuilder.InitialCatalog = database;
            if (user != null && password != null)
            {
                connStringBuilder.UserID = user;
                connStringBuilder.Password = password;
            }
            return connStringBuilder.ConnectionString;
        }

    }
}
