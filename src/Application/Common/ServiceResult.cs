namespace SirnakSport.Application.Common;

/// <summary>
/// Service katmanından dönen sonuç modeli — başarı/hata durumunu ve veriyi taşır.
/// </summary>
public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
    public T? Data { get; set; }
    public string? ErrorMessage { get; set; }
    public int StatusCode { get; set; }

    public static ServiceResult<T> Success(T data, int statusCode = 200)
    {
        return new ServiceResult<T>
        {
            IsSuccess = true,
            Data = data,
            StatusCode = statusCode
        };
    }

    public static ServiceResult<T> Failure(string errorMessage, int statusCode = 400)
    {
        return new ServiceResult<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            StatusCode = statusCode
        };
    }
}

/// <summary>
/// Veri döndürmeyen operasyonlar için ServiceResult.
/// </summary>
public class ServiceResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public int StatusCode { get; set; }

    public static ServiceResult Success(int statusCode = 200)
    {
        return new ServiceResult
        {
            IsSuccess = true,
            StatusCode = statusCode
        };
    }

    public static ServiceResult Failure(string errorMessage, int statusCode = 400)
    {
        return new ServiceResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            StatusCode = statusCode
        };
    }
}
