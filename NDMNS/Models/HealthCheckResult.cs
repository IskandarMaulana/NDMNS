namespace NDMNS.Models
{
    public class HealthCheckResult
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public decimal ResponseTime { get; set; }
    }
}
