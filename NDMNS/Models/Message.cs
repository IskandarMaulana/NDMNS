using System.ComponentModel.DataAnnotations;

namespace NDMNS.Models
{
    public partial class Message
    {
        [Display(Name = "Message ID")]
        public string Id { get; set; } = "";

        [Display(Name = "Downtime ID")]
        public string DowntimeId { get; set; } = "";

        [Display(Name = "Message Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Recipient")]
        public string Recipient { get; set; } = "";

        [Display(Name = "Recipient Type")]
        public int RecipientType { get; set; }

        [Display(Name = "Message ID Reference")]
        public string MessageId { get; set; } = "";

        [Display(Name = "Message Text")]
        public string Text { get; set; } = "";

        [Display(Name = "Image")]
        public string Image { get; set; } = "";

        /// <summary>
        /// Type 1 = Downtime, 2 = Uptime, 3 Intermittent
        /// </summary>
        [Display(Name = "Message Type")]
        public int Type { get; set; }

        /// <summary>
        /// Category 1 = Alert, 2 = Response, 3 = Update
        /// </summary>
        [Display(Name = "Message Category")]
        public int Category { get; set; }

        /// <summary>
        /// Level is how many attempt to sent alert for a downtime
        /// </summary>
        [Display(Name = "Alert Level")]
        public int Level { get; set; }

        /// <summary>
        /// Status 1 = Sent, 2 = Responded
        /// </summary>
        [Display(Name = "Message Status")]
        public int Status { get; set; }

        [Display(Name = "Downtime Description")]
        public string DowntimeDescription { get; set; } = "";

        [Display(Name = "Downtime TicketNumber")]
        public string DowntimeTicketNumber { get; set; } = "";

        [Display(Name = "Recipient Name")]
        public string RecipientName { get; set; } = "";
    }
}
