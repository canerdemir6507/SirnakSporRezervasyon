using AutoMapper;
using SirnakSport.Application.Common;
using SirnakSport.Application.DTOs.ApprovedStudent;
using SirnakSport.Application.Interfaces;
using SirnakSport.Application.Validators;
using SirnakSport.Domain.Entities;
using SirnakSport.Domain.Interfaces;

namespace SirnakSport.Application.Services;

/// <summary>
/// Onaylı öğrenci yönetimi servisi.
/// Admin tarafından whitelist'e öğrenci ekleme/çıkarma işlemleri.
/// </summary>
public class ApprovedStudentService : IApprovedStudentService
{
    private readonly IApprovedStudentRepository _repository;
    private readonly IMapper _mapper;

    public ApprovedStudentService(IApprovedStudentRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ServiceResult<IEnumerable<ApprovedStudentDto>>> GetAllAsync()
    {
        var students = await _repository.GetActiveStudentsAsync();
        var dtos = _mapper.Map<IEnumerable<ApprovedStudentDto>>(students);
        return ServiceResult<IEnumerable<ApprovedStudentDto>>.Success(dtos);
    }

    public async Task<ServiceResult<ApprovedStudentDto>> GetByStudentNumberAsync(string studentNumber)
    {
        var student = await _repository.GetByStudentNumberAsync(studentNumber.Trim());
        if (student == null)
            return ServiceResult<ApprovedStudentDto>.Failure("Onaylı öğrenci bulunamadı.", 404);

        var dto = _mapper.Map<ApprovedStudentDto>(student);
        return ServiceResult<ApprovedStudentDto>.Success(dto);
    }

    public async Task<ServiceResult<ApprovedStudentDto>> CreateAsync(CreateApprovedStudentDto dto)
    {
        // Öğrenci numarası format kontrolü
        var (isValid, errorMessage) = StudentNumberValidator.Validate(dto.StudentNumber);
        if (!isValid)
            return ServiceResult<ApprovedStudentDto>.Failure(errorMessage!);

        var trimmedNumber = dto.StudentNumber.Trim();

        // Aynı numara daha önce eklenmiş mi?
        if (await _repository.IsStudentApprovedAsync(trimmedNumber))
            return ServiceResult<ApprovedStudentDto>.Failure("Bu öğrenci numarası zaten onaylı listesinde mevcut.", 409);

        var student = new ApprovedStudent
        {
            Id = Guid.NewGuid(),
            StudentNumber = trimmedNumber,
            FullName = dto.FullName.Trim(),
            Department = dto.Department?.Trim() ?? string.Empty,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(student);

        var resultDto = _mapper.Map<ApprovedStudentDto>(student);
        return ServiceResult<ApprovedStudentDto>.Success(resultDto, 201);
    }

    public async Task<ServiceResult<IEnumerable<ApprovedStudentDto>>> BulkCreateAsync(BulkCreateApprovedStudentDto dto)
    {
        if (dto.Students == null || dto.Students.Count == 0)
            return ServiceResult<IEnumerable<ApprovedStudentDto>>.Failure("Öğrenci listesi boş olamaz.");

        var createdStudents = new List<ApprovedStudent>();
        var errors = new List<string>();

        foreach (var (studentDto, index) in dto.Students.Select((s, i) => (s, i)))
        {
            // Format kontrolü
            var (isValid, errorMessage) = StudentNumberValidator.Validate(studentDto.StudentNumber);
            if (!isValid)
            {
                errors.Add($"Satır {index + 1} ({studentDto.StudentNumber}): {errorMessage}");
                continue;
            }

            var trimmedNumber = studentDto.StudentNumber.Trim();

            // Duplicate kontrolü
            if (await _repository.IsStudentApprovedAsync(trimmedNumber))
            {
                errors.Add($"Satır {index + 1} ({trimmedNumber}): Bu öğrenci numarası zaten mevcut.");
                continue;
            }

            // Aynı batch içinde duplicate kontrolü
            if (createdStudents.Any(s => s.StudentNumber == trimmedNumber))
            {
                errors.Add($"Satır {index + 1} ({trimmedNumber}): Listede tekrar eden öğrenci numarası.");
                continue;
            }

            createdStudents.Add(new ApprovedStudent
            {
                Id = Guid.NewGuid(),
                StudentNumber = trimmedNumber,
                FullName = studentDto.FullName.Trim(),
                Department = studentDto.Department?.Trim() ?? string.Empty,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (createdStudents.Count == 0)
            return ServiceResult<IEnumerable<ApprovedStudentDto>>.Failure(
                $"Hiçbir öğrenci eklenemedi. Hatalar:\n{string.Join("\n", errors)}");

        foreach (var student in createdStudents)
            await _repository.AddAsync(student);

        var resultDtos = _mapper.Map<IEnumerable<ApprovedStudentDto>>(createdStudents);

        if (errors.Count > 0)
        {
            // Kısmi başarı — bazı kayıtlar eklendi, bazıları hatalı
            return ServiceResult<IEnumerable<ApprovedStudentDto>>.Success(resultDtos, 201);
        }

        return ServiceResult<IEnumerable<ApprovedStudentDto>>.Success(resultDtos, 201);
    }

    public async Task<ServiceResult> DeactivateAsync(Guid id)
    {
        var student = await _repository.GetByIdAsync(id);
        if (student == null)
            return ServiceResult.Failure("Onaylı öğrenci bulunamadı.", 404);

        student.IsActive = false;
        student.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(student);

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> ActivateAsync(Guid id)
    {
        var student = await _repository.GetByIdAsync(id);
        if (student == null)
            return ServiceResult.Failure("Onaylı öğrenci bulunamadı.", 404);

        student.IsActive = true;
        student.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(student);

        return ServiceResult.Success();
    }

    public async Task<ServiceResult> DeleteAsync(Guid id)
    {
        var student = await _repository.GetByIdAsync(id);
        if (student == null)
            return ServiceResult.Failure("Onaylı öğrenci bulunamadı.", 404);

        await _repository.DeleteAsync(student);
        return ServiceResult.Success();
    }
}
