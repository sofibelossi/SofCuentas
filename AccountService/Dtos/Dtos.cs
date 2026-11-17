using AccountService.Models;
namespace AccountService.Dtos;

public record CreateAccountRequest(string Owner);
public record AccountResponse (int Id, string Owner, decimal Balance);
public record CreateMovementRequest (MovementType Type, decimal Amount, string? Description);
public record MovementResponse (int Id, MovementType Type, decimal Amount, string? Description, DateTime CreatedAt);
