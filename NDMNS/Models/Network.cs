using System.ComponentModel.DataAnnotations;

namespace NDMNS.Models
{
    public partial class Network
    {
        [Display(Name = "Network ID")]
        public string Id { get; set; } = "";

        [Display(Name = "Network Name")]
        public string Name { get; set; } = "";

        [Display(Name = "IP Address")]
        public string Ip { get; set; } = "";

        [Display(Name = "Latency (ms)")]
        public decimal Latency { get; set; }

        [Display(Name = "Network Status")]
        public int Status { get; set; }

        [Display(Name = "Last Update")]
        public DateTime LastUpdate { get; set; }

        [Display(Name = "Site ID")]
        public string SiteId { get; set; } = "";

        [Display(Name = "Site Name")]
        public string SiteName { get; set; } = "";

        [Display(Name = "Site Location")]
        public string SiteLocation { get; set; } = "";

        [Display(Name = "ISP ID")]
        public string IspId { get; set; } = "";

        [Display(Name = "ISP Name")]
        public string IspName { get; set; } = "";

        [Display(Name = "CID")]
        public string Cid { get; set; } = "";
    }
}
