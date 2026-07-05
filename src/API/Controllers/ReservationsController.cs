using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SirnakSport.Application.DTOs.Reservation;
using SirnakSport.Application.Interfaces;

namespace SirnakSport.API.Controllers;

/// <summary>
/// Rezervasyon yönetimi.
/// Tüm endpointler kimlik doğrulama gerektirir.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReservationsController : ControllerBase
{
    private readonly IReservationService _reservationService;

    public ReservationsController(IReservationService reservationService)
    {
        _reservationService = reservationService;
    }

    /// <summary>
    /// Tüm rezervasyonları listeler. (Sadece Admin)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<ReservationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _reservationService.GetAllAsync();
        return Ok(result.Data);
    }

    /// <summary>
    /// Giriş yapan kullanıcının kendi rezervasyonlarını listeler.
    /// </summary>
    [HttpGet("my")]
    [ProducesResponseType(typeof(IEnumerable<ReservationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyReservations()
    {
        var userId = GetCurrentUserId();
        var result = await _reservationService.GetByUserAsync(userId);
        return Ok(result.Data);
    }

    /// <summary>
    /// Belirli bir tesis ve tarih için boş saatleri sorgular.
    /// </summary>
    [HttpGet("available")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<AvailableSlotDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAvailableSlots(
        [FromQuery] Guid facilityId,
        [FromQuery] string date)
    {
        var result = await _reservationService.GetAvailableSlotsAsync(facilityId, date);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    /// <summary>
    /// Yeni rezervasyon oluşturur.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ReservationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateReservationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetCurrentUserId();
        var result = await _reservationService.CreateAsync(userId, dto);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Rezervasyonu iptal eder.
    /// Kullanıcı sadece kendi rezervasyonunu, Admin tüm rezervasyonları iptal edebilir.
    /// </summary>
    [HttpPut("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var userId = GetCurrentUserId();
        var isAdmin = User.IsInRole("Admin");
        var result = await _reservationService.CancelAsync(id, userId, isAdmin);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

        return Ok(new { message = "Rezervasyon başarıyla iptal edildi." });
    }

    /// <summary>
    /// Rezervasyonu onaylar. (Sadece Admin)
    /// </summary>
    [HttpPut("{id:guid}/approve")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Approve(Guid id)
    {
        var result = await _reservationService.ApproveAsync(id);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

        return Ok(new { message = "Rezervasyon başarıyla onaylandı." });
    }

    /// <summary>
    /// JWT claim'inden kullanıcı ID'sini çıkarır.
    /// </summary>
    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Geçersiz kullanıcı kimliği.");
        }
        return userId;
    }
}
