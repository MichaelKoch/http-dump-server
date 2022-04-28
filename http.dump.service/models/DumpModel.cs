using Microsoft.Extensions.Primitives;

namespace http.dump.service.models
{
    public class DumpModel
    {
        public DateTime DateTime
        {
            get; set;
        }
        public string? Method
        {
            get; set;
        }
        public string? Url
        {
            get; set;
        }
        public string? ContentType
        {
            get; set;
        }
        public long? ContentLength
        {
            get; set;
        }
        public string? Query
        {
            get; set;
        }
        public object? Body
        {
            get; set;
        }
        public IDictionary<string, string[]>? Header { get; set; }
    }
}
