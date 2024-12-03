using Newtonsoft.Json;

namespace ImageSharingWithCloud.Models
{
    /*
     * Entity model for an image.
     */
    public class Image
    {
        // Primary key field for Cosmos DB (Guid assigned by app)
        [JsonProperty(PropertyName = "id")] 
        public string Id { get; set; }

        public string Caption { get; set; }

        public string Description { get; set; }

        public DateTime DateTaken { get; set; }
  
        public string UserId { get; set; }

        public string UserName { get; set; }

        // The image has been validated.
        public bool Valid { get; set; }

        // The image has been approved.
        public bool Approved { get; set; }
    }
}