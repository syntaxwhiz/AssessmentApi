using System.ComponentModel.DataAnnotations;

namespace AssessmentApiProject.Models
{
    public class User
    {
        [Required(ErrorMessage = "Name is required.")]
        public string? Name { get; set; }
        
        [Required(ErrorMessage = "Address is required.")]
        public string? Address { get; set; }
    }
}
