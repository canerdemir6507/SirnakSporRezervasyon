using System.Text.RegularExpressions;

namespace SirnakSport.Application.Validators;

/// <summary>
/// Öğrenci numarası doğrulama kuralları.
/// Şırnak Üniversitesi öğrenci numarası formatı: 9 haneli, sadece rakam, "2" ile başlar.
/// </summary>
public static partial class StudentNumberValidator
{
    private const int RequiredLength = 9;
    private const string ValidPrefix = "2";

    /// <summary>
    /// Öğrenci numarasını doğrular. Başarısızsa hata mesajı döner.
    /// </summary>
    public static (bool IsValid, string? ErrorMessage) Validate(string? studentNumber)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            return (false, "Öğrenci numarası boş olamaz.");
        }

        var trimmed = studentNumber.Trim();

        // Sadece rakam kontrolü
        if (!DigitsOnly().IsMatch(trimmed))
        {
            return (false, "Öğrenci numarası sadece rakamlardan oluşmalıdır.");
        }

        // 9 haneli olmalı
        if (trimmed.Length != RequiredLength)
        {
            return (false, $"Öğrenci numarası {RequiredLength} haneli olmalıdır. (Girilen: {trimmed.Length} hane)");
        }

        // Prefix kontrolü (Şırnak Üniversitesi: "2" ile başlar)
        if (!trimmed.StartsWith(ValidPrefix))
        {
            return (false, $"Geçersiz öğrenci numarası formatı. Şırnak Üniversitesi öğrenci numaraları '{ValidPrefix}' ile başlamalıdır.");
        }

        return (true, null);
    }

    [GeneratedRegex(@"^\d+$")]
    private static partial Regex DigitsOnly();
}
