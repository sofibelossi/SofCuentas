using AccountService.Dtos;
using AccountService.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace AccountService.Controllers;

[ApiController]
[Route("api/accounts")]
public class AccountsController(IAccountService service, IValidator<CreateAccountRequest> accountValidator, IValidator<CreateMovementRequest> movementValidator) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType<AccountResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateAccountRequest req)
    {
        var result = await accountValidator.ValidateAsync(req);
        if (!result.IsValid) return ValidationError(result);

        var created = await service.CreateAsync(req);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet]
    public async Task<ActionResult<List<AccountResponse>>> GetAll() =>
        Ok(await service.GetAllAsync());

    [HttpGet("{id:int}")]
    [ProducesResponseType<AccountResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccountResponse>> GetById(int id) =>
        Ok (await service.GetByIdAsync(id));

    [HttpPost("{id:int}/movements")]
    [ProducesResponseType<MovementResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> AddMovement (int id, CreateMovementRequest req)
    {
        var result = await movementValidator.ValidateAsync(req);
        if (!result.IsValid) return ValidationError(result);

        var movement = await service.AddMovementAsync(id, req);
        return CreatedAtAction(nameof(GetMovements), new { id }, movement);
    }

    [HttpGet("{id:int}/movements")]
    public async Task<ActionResult<List<MovementResponse>>> GetMovements(int id) =>
        Ok(await service.GetMovementsAsync(id));

    private IActionResult ValidationError(FluentValidation.Results.ValidationResult result)
    {
        var errors = result.Errors
            .GroupBy(e => e.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
        return ValidationProblem(new ValidationProblemDetails(errors));
    }
}