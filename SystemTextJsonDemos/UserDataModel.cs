using System;
using System.Text.Json.Serialization;

namespace SystemTextJsonDemos
{
    public class UserDataModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [JsonPropertyName("createdTimeStamp")] 
        public DateTime CreatedOn { get; set; }
    }
}