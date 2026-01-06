namespace WebApplication1.Dtos;

using System.ComponentModel.DataAnnotations;
using WebApplication1.Dtos;

public record UserDto(
    int UserId,
    string Username,
    string PasswordHash,
    string Email
);

// DTO for creating a user 
public record CreateUserDto(
    [Required, MinLength(1)] string Username,
    string PasswordHash,
    [EmailAddress] string Email
);

public record UpdateUserDto(
    string? Username,
    string? PasswordHash,
    [EmailAddress] string? Email
);