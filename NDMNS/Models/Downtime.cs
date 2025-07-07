using System.ComponentModel.DataAnnotations;

namespace NDMNS.Models
{
    public partial class Downtime
    {
        [Display(Name = "Downtime ID")]
        public string Id { get; set; } = "";

        [Display(Name = "Network ID")]
        public string NetworkId { get; set; } = "";

        [Display(Name = "Network Name")]
        public string NetworkName { get; set; } = "";

        [Display(Name = "Site ID")]
        public string SiteId { get; set; } = "";

        [Display(Name = "Site Name")]
        public string SiteName { get; set; } = "";

        [Display(Name = "ISP ID")]
        public string IspId { get; set; } = "";

        [Display(Name = "ISP Name")]
        public string IspName { get; set; } = "";

        [Display(Name = "Ticket Number")]
        public string TicketNumber { get; set; } = "";

        [Display(Name = "Description")]
        public string Description { get; set; } = "";

        [Display(Name = "Date")]
        public DateTime Date { get; set; }

        [Display(Name = "Start Time")]
        public DateTime Start { get; set; }

        [Display(Name = "End Time")]
        public DateTime? End { get; set; }

        /// <summary>
        /// Category 1 = INTERNAL, 2 = ISP
        /// </summary>
        [Display(Name = "Category")]
        public int Category { get; set; }

        /// <summary>
        /// Sub Category 1 = Cabling; 1 = Electricity; 2 = Full Traffic; 3 = Genset / Power Backup; 4 = Link Disable;
        /// 5 = Maintenance Internal; 6 = No Data Package; 7 = Radio Issue; 8 = Tower Issue; 9 = FO Cut; 10 = Signal / Radio Degraded;
        /// 11 = BTS Issue; PLN Issue; 12 = Maintenance Perangkat ISP; 13 = Router / Modem Issue; 14 = CP Issue
        /// </summary>
        [Display(Name = "Sub Category")]
        public int Subcategory { get; set; }

        [Display(Name = "Action Taken")]
        public string? Action { get; set; } = "";

        /// <summary>
        /// Status 1 = Ongoing, 2 = Resolved
        /// </summary>
        [Display(Name = "Status")]
        public int Status { get; set; }
    }
}
