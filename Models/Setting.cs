using System.ComponentModel.DataAnnotations;

namespace TaskManagementMvc.Models
{
    public class Setting
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Key { get; set; }

        [Required]
        [StringLength(1000)]
        public string Value { get; set; }
    }
}
