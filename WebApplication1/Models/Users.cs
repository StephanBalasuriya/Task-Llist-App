using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public class Users
{

[Key]public int UserId { get; set; }
public  required string Username { get; set; } = string.Empty;
public string Email { get; set; } = string.Empty;
}
