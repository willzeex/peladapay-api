namespace PeladaPay.API.Contracts;

public sealed record ApiResponse<T>(int StatusCode, string Message, T? Data);
