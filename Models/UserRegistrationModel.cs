using System.ComponentModel.DataAnnotations;

namespace SurveySystem.Models;

public class UserRegistrationModel
{
    [Required(ErrorMessage = "Username is required")]
    [MaxLength(50,ErrorMessage = "Username cannot be more than {0} chars long")]
    public string Username { get; set; }
    
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Password confirmation is required")]
    public string PasswordConfirm { get; set; }
}