using AccountService.Data;
using AccountService.Dtos;
using AccountService.Errors;
using AccountService.Models;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Services;

public interface IAccountService
{
    Task<AccountResponse> CreateAsync(CreateAccountRequest req);
    Task<List<AccountResponse>> GetAllAsync();
    Task<AccountResponse> GetByIdAsync(int id);
    Task<MovementResponse> AddMovementAsync(int accountId, CreateMovementRequest req);
    Task<List<MovementResponse>> GetMovementsAsync(int accountId);
}

public class AccountServiceImpl(AccountsDbContext db) : IAccountService
{
    public async Task<AccountResponse> CreateAsync(CreateAccountRequest req)
    {
        var account = new Account { Owner = req.Owner };
        db.Accounts.Add(account);
        await db.SaveChangesAsync();
        return ToResponse(account);
    }

    public async Task<List<AccountResponse>> GetAllAsync() =>
        await db.Accounts.AsNoTracking()
            .Select(a => new AccountResponse(a.Id, a.Owner, a.Balance))
            .ToListAsync();

    public async Task<AccountResponse> GetByIdAsync(int id) =>
        ToResponse(await FindAsync(id));

    public async Task<MovementResponse> AddMovementAsync(int accountId, CreateMovementRequest req)
    {
        var account = await FindAsync(accountId);

        if (req.Type == MovementType.Withdrawal && account.Balance < req.Amount)
            throw new InsufficientFundsException(
                $"Saldo disponible {account.Balance}, monto solicitado {req.Amount}.");

        account.Balance += req.Type == MovementType.Deposit ? req.Amount : -req.Amount;

        var movement = new Movement
        {
            AccountId = accountId,
            Type = req.Type,
            Amount = req.Amount,
            Description = req.Description
        };
        db.Movements.Add(movement);

        // Un solo SaveChanges = una transacción: se guarda el movimiento y el nuevo saldo, o nada.
        // RowVersion detecta si otro request modificó la cuenta en paralelo (409).
        await db.SaveChangesAsync();

        return new MovementResponse(movement.Id, movement.Type, movement.Amount,
            movement.Description, movement.CreatedAt);
    }

    public async Task<List<MovementResponse>> GetMovementsAsync(int accountId)
    {
        await FindAsync(accountId);
        return await db.Movements.AsNoTracking()
            .Where(m => m.AccountId == accountId)
            .OrderByDescending(m => m.CreatedAt)
            .Select(m => new MovementResponse(m.Id, m.Type, m.Amount, m.Description, m.CreatedAt))
            .ToListAsync();
    }

    private async Task<Account> FindAsync(int id) =>
        await db.Accounts.FirstOrDefaultAsync(a => a.Id == id)
        ?? throw new NotFoundException($"La cuenta {id} no existe.");

    private static AccountResponse ToResponse(Account a) => new(a.Id, a.Owner, a.Balance);
}