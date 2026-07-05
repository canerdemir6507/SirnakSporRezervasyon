using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SirnakSport.Application.DTOs.ApprovedStudent;
using SirnakSport.Application.Interfaces;

namespace SirnakSport.API.Controllers;

/// <summary>
/// Onaylı öğrenci yönetimi (Whitelist). Sadece Admin erişimi.
/// Admin, kayıt olabilecek öğrenci numaralarını bu endpoint üzerinden yönetir.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class ApprovedStudentsController : ControllerBase
{
    private readonly IApprovedStudentService _service;

    public ApprovedStudentsController(IApprovedStudentService service)
    {
        _service = service;
    }

    /// <summary>
    /// Tüm onaylı öğrencileri listeler.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ApprovedStudentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _service.GetAllAsync();
        return Ok(result.Data);
    }

    /// <summary>
    /// Öğrenci numarasına göre onaylı öğrenci sorgular.
    /// </summary>
    [HttpGet("{studentNumber}")]
    [ProducesResponseType(typeof(ApprovedStudentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByStudentNumber(string studentNumber)
    {
        var result = await _service.GetByStudentNumberAsync(studentNumber);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

        return Ok(result.Data);
    }

    /// <summary>
    /// Onaylı öğrenci listesine yeni öğrenci ekler.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApprovedStudentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateApprovedStudentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.CreateAsync(dto);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Birden fazla öğrenciyi toplu olarak onaylı listeye ekler.
    /// </summary>
    [HttpPost("bulk")]
    [ProducesResponseType(typeof(IEnumerable<ApprovedStudentDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> BulkCreate([FromBody] BulkCreateApprovedStudentDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _service.BulkCreateAsync(dto);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

        return StatusCode(result.StatusCode, result.Data);
    }

    /// <summary>
    /// Onaylı öğrenciyi pasif yapar (kayıt yapamaz hale gelir).
    /// </summary>
    [HttpPut("{id:guid}/deactivate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var result = await _service.DeactivateAsync(id);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

        return Ok(new { message = "Öğrenci başarıyla pasif yapıldı." });
    }

    /// <summary>
    /// Pasif öğrenciyi tekrar aktif yapar.
    /// </summary>
    [HttpPut("{id:guid}/activate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Activate(Guid id)
    {
        var result = await _service.ActivateAsync(id);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

        return Ok(new { message = "Öğrenci başarıyla aktif yapıldı." });
    }

    /// <summary>
    /// Onaylı öğrenciyi listeden kalıcı olarak siler.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });

        return Ok(new { message = "Öğrenci listeden kalıcı olarak silindi." });
    }
}
