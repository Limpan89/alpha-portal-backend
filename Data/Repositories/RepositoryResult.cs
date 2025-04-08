namespace Data.Repositories;

public class RepositoryResult
{
    public bool Succeded { get; set; }
    public int? StatusCode { get; set; }
    public string? Message { get; set; }
}

public class RepositoryResult<T> : RepositoryResult
{
    public T? Result { get; set; }
}
