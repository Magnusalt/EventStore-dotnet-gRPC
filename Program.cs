using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;

namespace ClientApp
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var client = Client.CreateClient();
            var streamName = "a-good-stream-name-nonexixting";

            // Setup a subscription starting from the current end of the stream.
            var sub = await client.SubscribeToStreamAsync(streamName, StreamPosition.End, EventAppeared);

            // hacky way of getting last revision, do not use!
            var revision = await client.GetLastRevisionOnStream(streamName);

            var writeResult = await client.AppendToStreamAsync(streamName, revision, SampleData.GetSampleData());

            // Next revision number to be used
            revision = writeResult.NextExpectedStreamRevision;

            // A read that will fetch the 5 last items.
            await using var readStreamResult =
                client.ReadStreamAsync(Direction.Backwards, streamName, StreamPosition.End, 5);

            if (await readStreamResult.ReadState == ReadState.Ok)
                await foreach (var res in readStreamResult)
                    Console.WriteLine($"Reading event number: {res.OriginalEventNumber}");
        }

        private static Task EventAppeared(StreamSubscription subscription, ResolvedEvent resolvedEvent,
            CancellationToken token)
        {
            Console.WriteLine($"Subscription event number: {resolvedEvent.OriginalEventNumber}");
            return Task.CompletedTask;
        }
    }
}