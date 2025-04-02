using System.ComponentModel.DataAnnotations;

namespace backend.Data
{
    public class User
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Name must be 2-50 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be 8-100 characters")]
        public string Password { get; set; } = string.Empty;

        public DateTime LastLogin { get; set; }
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Role is required")]
        [RegularExpression("^(Admin|Teacher|Student)$", ErrorMessage = "Invalid role")]
        public string Role { get; set; } = "Student"; // Default secure role
    }
}