using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementMvc.Models
{
    public class TaskItem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان تسک الزامی است")]
        [Display(Name = "عنوان")]
        [StringLength(200, ErrorMessage = "عنوان نمی‌تواند بیشتر از 200 کاراکتر باشد")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "توضیحات")]
        [StringLength(1000, ErrorMessage = "توضیحات نمی‌تواند بیشتر از 1000 کاراکتر باشد")]
        public string? Description { get; set; }

        [Display(Name = "اولویت")]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        [Display(Name = "ساعت تکمیل‌شده")]
        [Range(0, double.MaxValue, ErrorMessage = "ساعت‌های تکمیل‌شده باید عدد مثبت باشد")]
        public double CompletedEstimateHours { get; set; }

        [Display(Name = "تخمین اولیه (ساعت)")]
        [Range(0, double.MaxValue, ErrorMessage = "تخمین اولیه باید عدد مثبت باشد")]
        public double? OriginalEstimateHours { get; set; }

        [Display(Name = "تاریخ شروع")]
        [DataType(DataType.Date)]
        public DateTime? StartAt { get; set; }

        [Display(Name = "تاریخ پایان")]
        [DataType(DataType.Date)]
        public DateTime? EndAt { get; set; }

        [Display(Name = "تاریخ ایجاد")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        [Display(Name = "تاریخ به‌روزرسانی")]
        public DateTime? UpdatedAt { get; set; }

        [Display(Name = "آرشیو شده")]
        public bool IsArchived { get; set; } = false;

        [Display(Name = "تاریخ آرشیو")]
        public DateTime? ArchivedAt { get; set; }

        [Display(Name = "آرشیو شده توسط")]
        public string? ArchivedBy { get; set; }

        // Foreign Keys
        [Display(Name = "انجام‌دهنده")]
        public int? PerformerId { get; set; }

        [Required]
        [Display(Name = "ایجاد شده توسط")]
        public int CreatedById { get; set; }

        [Display(Name = "به‌روزرسانی شده توسط")]
        public int? UpdatedById { get; set; }

        [Display(Name = "پروژه")]
        public int? ProjectId { get; set; }

        [Display(Name = "ستون بورد")]
        public int? BoardColumnId { get; set; }

        // Navigation Properties
        [ForeignKey("PerformerId")]
        public virtual ApplicationUser? Performer { get; set; }

        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; } = null!;

        [ForeignKey("UpdatedById")]
        public virtual ApplicationUser? UpdatedBy { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project? Project { get; set; }

        [ForeignKey("BoardColumnId")]
        public virtual BoardColumn? BoardColumn { get; set; }

        public virtual ICollection<TaskAttachment> Attachments { get; set; } = new List<TaskAttachment>();
        public virtual ICollection<TaskHistory> HistoryEntries { get; set; } = new List<TaskHistory>();
    }

    public enum TaskPriority
    {
        [Display(Name = "کم")]
        Low = 1,
        [Display(Name = "متوسط")]
        Medium = 2,
        [Display(Name = "زیاد")]
        High = 3,
        [Display(Name = "بحرانی")]
        Critical = 4
    }
}
