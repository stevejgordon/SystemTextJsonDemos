using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SystemTextJsonDemos
{
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
}