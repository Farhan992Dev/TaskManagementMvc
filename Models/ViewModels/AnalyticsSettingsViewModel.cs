using System.ComponentModel.DataAnnotations;

namespace TaskManagementMvc.Models.ViewModels
{
    /// <summary>
    /// View model for managing analytics settings in admin panel
    /// </summary>
    public class AnalyticsSettingsViewModel
    {
        [Display(Name = "پروژه ID کلریتی مایکروسافت")]
        [StringLength(50, ErrorMessage = "پروژه ID نمی‌تواند بیشتر از 50 کاراکتر باشد")]
        public string ClarityProjectId { get; set; } = string.Empty;

        [Display(Name = "فعال سازی ردیابی کلریتی")]
        public bool ClarityEnabled { get; set; } = true;

        [Display(Name = "وضعیت پیکربندی")]
        public bool IsConfigured => !string.IsNullOrEmpty(ClarityProjectId) && ClarityEnabled;
    }
}
