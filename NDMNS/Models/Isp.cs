using System.ComponentModel.DataAnnotations;

namespace NDMNS.Models
{
    public partial class Isp
    {
        [Display(Name = "ISP ID")]
        public string Id { get; set; } = "";

        [Display(Name = "ISP Name")]
        public string Name { get; set; } = "";

        [Display(Name = "WhatsApp Group")]
        public string WhatsappGroup { get; set; } = "";

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; } = "";

        [Display(Name = "WhatsApp Group Name")]
        public string WhatsappGroupName { get; set; } = "";
    }
}
