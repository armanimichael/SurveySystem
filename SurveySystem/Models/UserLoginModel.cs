﻿using System.ComponentModel.DataAnnotations;

namespace SurveySystem.Models;

public class UserLoginModel
{
    [Required(ErrorMessage = "Username is required")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = null!;
}