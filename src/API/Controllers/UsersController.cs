using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SirnakSport.Application.DTOs.User;
using SirnakSport.Application.Interfaces;

namespace SirnakSport.API.Controllers;

/// <summary>
/// Kullanıcı yönetimi. (Sadece Admin)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Tüm kullanıcıları listeler.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _userService.GetAllAsync();
        return Ok(result.Data);
    }

    /// <summary>
    /// Belirli bir kullanıcının detayını getirir.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _userService.GetByIdAsync(id);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

        return Ok(result.Data);
    }
}
