using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using EventStore.Client;

namespace ClientApp
{
    public class SampleData
    {
        public int Count { get; set; }
        public string Name { get; set; }

        public static IEnumerable<EventData> GetSampleData()
        {
            var sampleObject = new SampleData {Count = 4, Name = "Sample1"};
            var data = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(sampleObject));

            var evt = new EventData(Uuid.NewUuid(), "event-type", data);
            return new[] {evt};
        }
    }
}