namespace WebApplication1.Dtos;

using System.ComponentModel.DataAnnotations;
using WebApplication1.Dtos;

public record UserDto(
    int UserId,
    string Username,
    string Email
);

// DTO for creating a user 
public record CreateUserDto(
    [Required, MinLength(1)] string Username,
    [EmailAddress] string Email
);

public record UpdateUserDto(
    string? Username,
    [EmailAddress] string? Email
);