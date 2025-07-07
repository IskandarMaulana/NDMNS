using System.ComponentModel.DataAnnotations;

namespace NDMNS.Models
{
    public partial class User
    {
        [Display(Name = "User ID")]
        public string Id { get; set; } = "";

        [Display(Name = "Full Name")]
        public string Name { get; set; } = "";

        [Display(Name = "User Code")]
        public string Code { get; set; } = "";

        [Display(Name = "NRP")]
        public string Nrp { get; set; } = "";

        [Display(Name = "Password")]
        public string Password { get; set; } = "";

        [Display(Name = "Role")]
        public string Role { get; set; } = "";

        [Display(Name = "Email Address")]
        public string Email { get; set; } = "";

        [Display(Name = "WhatsApp Number")]
        public string WhatsApp { get; set; } = "";

        /// <summary>
        /// WhatsApp Client Status 1 = Disconnected, 2 Connected
        /// </summary>
        [Display(Name = "WhatsApp Client Status")]
        public int WhatsAppClient { get; set; }

        /// <summary>
        /// User Status 1 = Not Active, 2 Active
        /// </summary>
        [Display(Name = "User Status")]
        public int Status { get; set; }
    }

    public class LoginRequest
    {
        [Display(Name = "NRP")]
        public string Nrp { get; set; } = "";

        [Display(Name = "Password")]
        public string Password { get; set; } = "";
    }
}
