﻿using System.ComponentModel.DataAnnotations;

namespace SurveySystem.Models;

public class UserRegistrationModel
{
    [Key]
    [Required(ErrorMessage = "Username is required")]
    [MaxLength(50,ErrorMessage = "Username cannot be more than 50 characters long")]
    public string Username { get; set; }
    
    [Key]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [Required(ErrorMessage = "Email is required")]
    public string Email { get; set; }
    
    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; }
    
    [Required(ErrorMessage = "Password confirmation is required")]
    [Compare("Password", ErrorMessage = "Password and Confirmed Password must match")]
    public string PasswordConfirm { get; set; }
}