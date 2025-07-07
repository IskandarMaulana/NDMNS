namespace NDMNS.Models
{
    public class DtoResponse<T>
    {
        public int status { get; set; }

        public String? message { get; set; }

        public T? data { get; set; }

        public string? type { get; set; }

        public string? title { get; set; }

        public Dictionary<string, string[]>? errors { get; set; }

        public string? traceId { get; set; }
    }
}
