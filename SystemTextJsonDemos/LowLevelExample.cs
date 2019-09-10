using System.Threading.Tasks;

namespace SystemTextJsonDemos
{
    public static class LowLevelExample
    {
        public static async Task RunAsync()
        {
            await using var stream = JsonStreamFactory.CreateGraphQlResponseStream();

            var parser = new LowLevelGraphQlResponseParser();

            var user = parser.ParseResponseJsonWithoutPipeReader(stream);
        }
    }
}
