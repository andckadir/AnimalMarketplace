namespace AnimalMarketplace.Services;

public class ServiceResult<T>
{
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
    public string Token { get; set; }
    public T Data { get; set; }
    
    public static ServiceResult<T> Success(string token, T data)
        => new() { IsSuccess = true, Token = token, Data = data };
    
    public static ServiceResult<T> Failure(string error)
        => new() { IsSuccess = false, ErrorMessage = error };
}