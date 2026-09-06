using System.ComponentModel.DataAnnotations;

namespace __NAMESPACE__.Entity.Base
{
    public class SystemEntity
    {
        [Required]
        [MaxLength(64)]
        public string CreationUser { get; set; } = null!;

        [Required]
        public DateTimeOffset CreationDate { get; set; }

        [Required]
        [MaxLength(64)]
        public string UpdateUser { get; set; } = null!;

        [Required]
        public DateTimeOffset UpdateDate { get; set; }

        [Required]
        public bool IsActive { get; set; }
    }
}
