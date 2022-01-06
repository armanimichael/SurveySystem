using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace SurveySystem.Models;

public class RefreshToken
{
    [Key]
    public string Token { get; set; } = null!;
    
    public DateTime ExpiryDate { get; set; }
    public string UserId { get; set; } = null!;
    
    [ForeignKey(nameof(UserId))]
    public IdentityUser User { get; set; }
}