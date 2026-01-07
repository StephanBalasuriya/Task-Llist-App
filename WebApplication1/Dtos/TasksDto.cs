namespace WebApplication1.Dtos;

using System.ComponentModel.DataAnnotations;
using WebApplication1.Dtos;

public enum TaskStatusDto
{
    ToDo,
    InProgress,
    Completed
}

// Transport shape for creating/updating tasks
public record TaskUpsertDto(
    string Title,
    string? Description,
    TaskStatusDto Status,
    DateTime? DueDate
    // [Required] int? UserId
);

// Transport shape for returning task details
public record TaskDetailsDto(
    int TaskId,
    string Title,
    string? Description,
    TaskStatusDto Status,
    DateTime? CreatedAt,
    DateTime? DueDate,
    DateTime? UpdatedAt
    
);


