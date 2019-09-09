using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SystemTextJsonDemos
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // MID-LEVEL - JsonDocument

            await using var stream = CreateGraphQlResponseStream();

            var parser = new GraphQlResponseParser();

            var user = await parser.ParseResponseJson(stream);
        }

        private static Stream CreateGraphQlResponseStream()
        {
            var stream = new MemoryStream();

            using var writer = new StreamWriter(stream, new UTF8Encoding(), leaveOpen: true);

            // Sample Response Body

            //{
            //    "data": {
            //        "users": [
            //        {
            //            "userId": "d1f225df-dc7e-46a3-9c13-4fddb82715c0",
            //            "firstName": "Jane",
            //            "lastName": "Doe",
            //            "createdTimeStamp": "2019-04-28T15:58:25.347+00:00",
            //            "modifiedTimeStamp": "2019-06-15T10:28:25.347+00:00"
            //        }
            //        ]
            //    }
            //}

            writer.Write(@"{""data"":{""users"":[{""userId"":""d1f225df-dc7e-46a3-9c13-4fddb82715c0"",""firstName"":""Jane"",""lastName"":""Doe"",""createdTimeStamp"":""2019-04-28T15:58:25.347+00:00"",""modifiedTimeStamp"":""2019-06-15T10:28:25.347+00:00""}]}}");
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }

    public class GraphQlResponseParser
    {
        public async Task<UserDataModel> ParseResponseJson(Stream stream)
        {
            using var document = await JsonDocument.ParseAsync(stream);

            var jsonElement = document.RootElement.GetProperty("data").GetProperty("users");

            if (jsonElement.ValueKind != JsonValueKind.Array) 
                return null;
            
            var user = jsonElement.EnumerateArray().FirstOrDefault();

            if (user.ValueKind == JsonValueKind.Undefined) 
                return null;

            var data = new UserDataModel();

            if (user.TryGetProperty("firstName", out var firstNameValue))
            {
                data.FirstName = firstNameValue.GetString();
            }

            if (user.TryGetProperty("lastName", out var cultureValue))
            {
                data.LastName = cultureValue.GetString();
            }

            if (user.TryGetProperty("createdTimeStamp", out var createdOnValue) && createdOnValue.ValueKind != JsonValueKind.Null && createdOnValue.TryGetDateTime(out var createdOn))
            {
                data.CreatedOn = createdOn;
            }

            return data;
        }
    }

    public class UserDataModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedOn { get; set; }
    }

}
