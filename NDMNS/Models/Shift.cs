using System.ComponentModel.DataAnnotations;

namespace NDMNS.Models
{
    public partial class Shift
    {
        [Display(Name = "Shift ID")]
        public string Id { get; set; } = "";

        [Display(Name = "User ID")]
        public string UserId { get; set; } = "";

        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Start Time")]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "End Time")]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; } = "";
    }
}
