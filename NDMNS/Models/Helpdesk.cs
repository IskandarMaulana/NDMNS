using System.ComponentModel.DataAnnotations;

namespace NDMNS.Models
{
    public partial class Helpdesk
    {
        [Display(Name = "Helpdesk ID")]
        public string Id { get; set; } = "";

        [Display(Name = "ISP ID")]
        public string IspId { get; set; } = "";

        [Display(Name = "Helpdesk Name")]
        public string Name { get; set; } = "";

        [Display(Name = "ISP Name")]
        public string IspName { get; set; } = "";

        /// <summary>
        /// Role 1 = Helpdesk, 2 = Team Leader, etc.
        /// </summary>
        [Display(Name = "Role")]
        public int Role { get; set; }

        [Display(Name = "WhatsApp Number")]
        public string WhatsappNumber { get; set; } = "";

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; } = "";
    }
}
