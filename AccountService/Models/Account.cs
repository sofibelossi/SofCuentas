namespace AccountService.Models;

public class Account
{
    public int Id { get; set; }
    public string Owner { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public byte[] RowVersion { get; set; } = [];
    public List<Movement> Movements { get; set; } = [];
}
public enum MovementType
{
    Deposit = 1,
    Withdrawal = 2
}

public class Movement
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public MovementType Type { get; set; }
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}
