using System.ComponentModel.DataAnnotations;
using TaskManagementMvc.Models;

namespace TaskManagementMvc.Models.ViewModels
{
    public class SendToJuleViewModel
    {
        public int TaskId { get; set; }

        public TaskItem Task { get; set; }

        [Required(ErrorMessage = "لطفا آدرس Git Repository را وارد کنید.")]
        [Display(Name = "آدرس Git Repository")]
        public string RepositoryUrl { get; set; }

        [Required(ErrorMessage = "لطفا دستور خود را وارد کنید.")]
        [Display(Name = "دستور شما (Prompt)")]
        public string Prompt { get; set; }
    }
}
