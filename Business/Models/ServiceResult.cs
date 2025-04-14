namespace Business.Models;

public class ServiceResult
{
    public bool Succeeded { get; set; }
    public int? StatusCode { get; set; }
    public string? Message { get; set; }
}

public class ServiceResult<T> : ServiceResult
{
    public T? Result { get; set; }
}

