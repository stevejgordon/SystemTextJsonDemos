using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System.IO;
using System.IO.Pipelines;
using System.Text.Json;
using System.Threading.Tasks;

namespace SystemTextJsonDemos
{
    class Program
    {
        static async Task Main(string[] args)
        {
            _ = BenchmarkRunner.Run<SerialiseBenchmarks>();

            //await MidLevelExample.Run();

            //await LowLevelExample.RunAsync();
        }
    }

    [MemoryDiagnoser]
    public class SerialiseBenchmarks
    {
        private GraphQlResponseParser _midLevelParser;
        private LowLevelGraphQlResponseParser _lowLevelParser;
        private StreamPipeReaderOptions _streamPipeReaderOptions;
        private Stream _stream;
        private Stream _emptyStream;

        [GlobalSetup]
        public void Setup()
        {
            _midLevelParser = new GraphQlResponseParser();
            _lowLevelParser = new LowLevelGraphQlResponseParser();
            _stream = JsonStreamFactory.CreateGraphQlResponseStream();
            _emptyStream = JsonStreamFactory.CreateEmptyGraphQlResponseStream();

            // needed for benchmarking to avoid stream closure
            // passed in as argument since under normal use we won't need to allocate this and can allow the stream to close
            _streamPipeReaderOptions = new StreamPipeReaderOptions(leaveOpen: true);
        }

        [Benchmark]
        public async Task HighLevel()
        {
            _stream.Position = 0;
            await JsonSerializer.DeserializeAsync<GraphQlModel>(_stream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        [Benchmark]
        public async Task HighLevelEmpty()
        {
            _emptyStream.Position = 0;
            await JsonSerializer.DeserializeAsync<GraphQlModel>(_emptyStream, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        [Benchmark]
        public async Task MidLevel()
        {
            _stream.Position = 0;
            await _midLevelParser.ParseResponseJsonAsync(_stream);
        }

        [Benchmark]
        public async Task MidLevelEmpty()
        {
            _emptyStream.Position = 0;
            await _midLevelParser.ParseResponseJsonAsync(_emptyStream);
        }

        [Benchmark]
        public async Task LowLevel()
        {
            _stream.Position = 0;
            await _lowLevelParser.ParseResponseJsonAsync(_stream, _streamPipeReaderOptions);
        }
        
        [Benchmark]
        public async Task LowLevelEmpty()
        {
            _emptyStream.Position = 0;
            await _lowLevelParser.ParseResponseJsonAsync(_emptyStream, _streamPipeReaderOptions);
        }

        [Benchmark]
        public void LowLevelWithoutPipe()
        {
            _stream.Position = 0;
            _lowLevelParser.ParseResponseJsonWithoutPipeReader(_stream);
        }

        [Benchmark]
        public void LowLevelEmptyWithoutPipe()
        {
            _emptyStream.Position = 0;
            _lowLevelParser.ParseResponseJsonWithoutPipeReader(_emptyStream);
        }
    }
}
