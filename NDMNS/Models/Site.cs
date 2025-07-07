using System.ComponentModel.DataAnnotations;

namespace NDMNS.Models
{
    public partial class Site
    {
        [Display(Name = "Site ID")]
        public string Id { get; set; } = "";

        [Display(Name = "Site Name")]
        public string Name { get; set; } = "";

        [Display(Name = "WhatsApp Group")]
        public string WhatsappGroup { get; set; } = "";

        [Display(Name = "WhatsApp Group Name")]
        public string WhatsappGroupName { get; set; } = "";

        [Display(Name = "Location")]
        public string Location { get; set; } = ""!;
    }
}
