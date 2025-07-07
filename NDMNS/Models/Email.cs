using System.ComponentModel.DataAnnotations;

namespace NDMNS.Models
{
    public partial class Email
    {
        [Display(Name = "Email ID")]
        public string Id { get; set; } = "";

        [Display(Name = "Downtime ID")]
        public string DowntimeId { get; set; } = "";

        /// <summary>
        /// Type 1 = Downtime, 2 = Uptime, 3 = Intermittent
        /// </summary>
        [Display(Name = "Email Type")]
        public int Type { get; set; }

        /// <summary>
        /// Status 1 = Sent, 2 = Failed
        /// </summary>
        [Display(Name = "Email Status")]
        public int Status { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; } = "";

        [Display(Name = "To Recipients")]
        public string To { get; set; } = "";

        [Display(Name = "CC Recipients")]
        public string Cc { get; set; } = "";

        [Display(Name = "Email Body")]
        public string Body { get; set; } = "";

        [Display(Name = "Image")]
        public string Image { get; set; } = "";

        [Display(Name = "Email Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Downtime Description")]
        public string DowntimeDescription { get; set; } = "";

        [Display(Name = "Downtime TicketNumber")]
        public string DowntimeTicketNumber { get; set; } = "";

        public List<DetailEmailPic> DetailEmailPics { get; set; } = [];
        public List<DetailEmailHelpdesk> DetailEmailHelpdesks { get; set; } = [];
    }

    public class DetailEmailPic
    {
        [Display(Name = "Detail ID")]
        public string Id { get; set; } = "";

        [Display(Name = "Email ID")]
        public string EmailId { get; set; } = "";

        [Display(Name = "PIC ID")]
        public string PicId { get; set; } = "";

        /// <summary>
        /// Type of Email Recipient 1 = To, 2 = CC
        /// </summary>
        [Display(Name = "Recipient Type")]
        public int Type { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; } = "";

        [Display(Name = "PIC Name")]
        public string PicName { get; set; } = "";
    }

    public class DetailEmailHelpdesk
    {
        [Display(Name = "Detail ID")]
        public string Id { get; set; } = "";

        [Display(Name = "Email ID")]
        public string EmailId { get; set; } = "";

        [Display(Name = "Helpdesk ID")]
        public string HelpdeskId { get; set; } = "";

        /// <summary>
        /// Type of Email Recipient 1 = To, 2 = CC
        /// </summary>
        [Display(Name = "Recipient Type")]
        public int Type { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; } = "";

        [Display(Name = "Helpdesk Name")]
        public string HelpdeskName { get; set; } = "";
    }
}
