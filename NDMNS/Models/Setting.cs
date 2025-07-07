using System.ComponentModel.DataAnnotations;

namespace NDMNS.Models
{
    public partial class Setting
    {
        [Display(Name = "Setting ID")]
        public string Id { get; set; } = "";

        [Display(Name = "Setting Name")]
        public string Name { get; set; } = "";

        [Display(Name = "Setting Code")]
        public string Code { get; set; } = "";

        [Display(Name = "Setting Value")]
        public string Value { get; set; } = "";
    }
}
