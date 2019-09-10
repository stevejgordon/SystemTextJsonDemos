using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Text.Json;
using System.Threading.Tasks;

namespace SystemTextJsonDemos
{
    public class LowLevelGraphQlResponseParser
    {
        public async Task<UserDataModel> ParseResponseJsonAsync(Stream stream, StreamPipeReaderOptions options)
        {
            var pipeReader = PipeReader.Create(stream, options);

            var state = new JsonReaderState();

            while (true)
            {
                var result = await pipeReader.ReadAsync(); // read from the pipe

                var buffer = result.Buffer;

                var (userDataModel, arrayIsEmpty) = ParseJson(buffer, result.IsCompleted, ref state); // read complete items from the current buffer

                if (result.IsCompleted || userDataModel != null || arrayIsEmpty)
                {
                    pipeReader.Complete(); // mark the PipeReader as complete
                    return userDataModel;
                }

                pipeReader.AdvanceTo(buffer.Start, buffer.End); // only mark as examined until we have enough buffered data to read what we need
            }
        }

        public UserDataModel ParseResponseJsonWithoutPipeReader(Stream stream)
        {
            var length = (int)stream.Length;
            var buffer = ArrayPool<byte>.Shared.Rent(length);

            var bufferSpan = buffer.AsSpan();

            stream.Read(bufferSpan);

            try
            {   
                return ParseJson(bufferSpan.Slice(0, length)).UserData;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        private static (UserDataModel UserData, bool arrayIsEmpty) ParseJson(in ReadOnlySequence<byte> dataUtf8, bool isFinalBlock, ref JsonReaderState state)
        {
            var jsonReader = new Utf8JsonReader(dataUtf8, isFinalBlock, state);

            return ParseCore(jsonReader);
        }

        private static (UserDataModel UserData, bool arrayIsEmpty) ParseJson(in ReadOnlySpan<byte> dataUtf8)
        {
            var state = new JsonReaderState();

            var jsonReader = new Utf8JsonReader(dataUtf8, true, state);

            return ParseCore(jsonReader);
        }

        private static (UserDataModel UserData, bool arrayIsEmpty) ParseCore(Utf8JsonReader jsonReader)
        {
            var foundEndArray = false;
            var arrayStarted = false;
            var userFound = false;
            var userPropertyCounter = 0;

            string firstName = null;
            string lastName = null;
            DateTime createdDate = default;

            while (jsonReader.Read() && !foundEndArray)
            {
                JsonTokenType tokenType = jsonReader.TokenType;

                switch (tokenType)
                {
                    case JsonTokenType.StartArray:
                        arrayStarted = true;
                        break;

                    case JsonTokenType.StartObject: // we have a user to parse
                        if (arrayStarted)
                        {
                            userFound = true;
                        }
                        break;

                    case JsonTokenType.PropertyName:
                        if (userFound)
                        {
                            userPropertyCounter++;
                        }
                        break;

                    case JsonTokenType.String:
                        if (userPropertyCounter == 2)
                        {
                            firstName = jsonReader.GetString();
                        }
                        if (userPropertyCounter == 3)
                        {
                            lastName = jsonReader.GetString();
                        }
                        if (userPropertyCounter == 4)
                        {
                            createdDate = jsonReader.GetDateTime();
                        }
                        break;

                    case JsonTokenType.EndArray:
                        foundEndArray = true;
                        break;
                }
            }

            if (foundEndArray && !userFound)
            {
                return (null, true);
            }

            if (userFound)
            {
                var model = new UserDataModel
                {
                    FirstName = firstName,
                    LastName = lastName,
                    CreatedOn = createdDate
                };

                return (model, false);
            }

            return (null, false);
        }
    }
}
