namespace NDMNS.Models
{
    public class WhatsAppClient
    {
        public string QrCode { get; set; } = string.Empty;
        public bool IsReady { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
