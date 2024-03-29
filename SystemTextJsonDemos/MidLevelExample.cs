﻿using System.Threading.Tasks;

namespace SystemTextJsonDemos
{
    public static class MidLevelExample
    {
        public static async Task RunAsync()
        {
            await using var stream = JsonStreamFactory.CreateGraphQlResponseStream();

            var parser = new GraphQlResponseParser();

            var user = await parser.ParseResponseJsonAsync(stream);
        }
    }
}
