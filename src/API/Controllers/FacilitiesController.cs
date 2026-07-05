using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SirnakSport.Application.DTOs.Facility;
using SirnakSport.Application.Interfaces;

namespace SirnakSport.API.Controllers;

/// <summary>
/// Spor tesisleri yönetimi.
/// GET: Herkese açık, CUD: Sadece Admin.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class FacilitiesController : ControllerBase
{
    private readonly IFacilityService _facilityService;

    public FacilitiesController(IFacilityService facilityService)
    {
        _facilityService = facilityService;
    }

    /// <summary>
    /// Tüm aktif tesisleri listeler.
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<FacilityDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _facilityService.GetAllAsync();
        return Ok(result.Data);
    }

    /// <summary>
    /// Belirli bir tesisin detayını getirir.
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(FacilityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _facilityService.GetByIdAsync(id);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    /// <summary>
    /// Yeni tesis ekler. (Sadece Admin)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(FacilityDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] CreateFacilityDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _facilityService.CreateAsync(dto);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Tesis bilgilerini günceller. (Sadece Admin)
    /// </summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(FacilityDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFacilityDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _facilityService.UpdateAsync(id, dto);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    /// <summary>
    /// Tesisi pasif duruma getirir (soft delete). (Sadece Admin)
    /// </summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _facilityService.DeleteAsync(id);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

        return Ok(new { message = "Tesis başarıyla silindi." });
    }
}
