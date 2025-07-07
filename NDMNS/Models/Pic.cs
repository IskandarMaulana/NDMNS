using System.ComponentModel.DataAnnotations;

namespace NDMNS.Models
{
    public partial class Pic
    {
        [Display(Name = "PIC ID")]
        public string Id { get; set; } = "";

        [Display(Name = "Site ID")]
        public string SiteId { get; set; } = "";

        [Display(Name = "Employee Number")]
        public string Nrp { get; set; } = "";

        [Display(Name = "PIC Name")]
        public string Name { get; set; } = "";

        /// <summary>
        /// Role 1 = PIC Site, 2 = PIC Head Office,
        /// </summary>
        [Display(Name = "Role")]
        public int Role { get; set; }

        [Display(Name = "WhatsApp Number")]
        public string WhatsappNumber { get; set; } = "";

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; } = "";

        [Display(Name = "Site Name")]
        public string SiteName { get; set; } = "";
    }
}
