using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Users
{

    [Key] public int UserId { get; set; }
    public required string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    
    // Navigation property (1 user → many tasks)
    public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();

}
