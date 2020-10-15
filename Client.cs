using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using EventStore.Client;

namespace ClientApp
{
    public static class Client
    {
        public static EventStoreClient CreateClient()
        {
            var settings = new EventStoreClientSettings
            {
                CreateHttpMessageHandler = () =>
                    new HttpClientHandler
                    {
                        ServerCertificateCustomValidationCallback =
                            (message, certificate2, x509Chain, sslPolicyErrors) => true
                    },
                ConnectivitySettings =
                {
                    Address = new Uri("http://localhost:2113")
                },
                DefaultCredentials = new UserCredentials("admin", "changeit")
            };
            var client = new EventStoreClient(settings);
            return client;
        }

        public static async Task<StreamRevision> GetLastRevisionOnStream(this EventStoreClient client,
            string streamName)
        {
            var peekLast = client.ReadStreamAsync(Direction.Backwards, streamName, StreamPosition.End, 1);
            var streamStatus = await peekLast.ReadState;

            StreamRevision revision;
            switch (streamStatus)
            {
                case ReadState.Ok:
                    await peekLast.GetAsyncEnumerator().MoveNextAsync();
                    revision = StreamRevision.FromStreamPosition(peekLast.Current.OriginalEventNumber);
                    break;
                case ReadState.StreamNotFound:
                    revision = StreamRevision.None;
                    break;
                default:
                    throw new SwitchExpressionException();
            }

            return revision;
        }
    }
}