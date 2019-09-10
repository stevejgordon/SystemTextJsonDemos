using System.IO;
using System.Text;

namespace SystemTextJsonDemos
{
    public static class JsonStreamFactory 
    {
        public static Stream CreateGraphQlResponseStream()
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

        public static Stream CreateEmptyGraphQlResponseStream()
        {
            var stream = new MemoryStream();

            using var writer = new StreamWriter(stream, new UTF8Encoding(), leaveOpen: true);

            // Sample Response Body

            //{
            //    "data": {
            //        "users": [                    
            //        ]
            //    }
            //}

            writer.Write(@"{""data"":{""users"":[]}}");
            writer.Flush();

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }
    }
}
