namespace PeladaPay.API.Contracts;

public sealed record ApiResponse<T>(int StatusCode, string Message, T? Data);

public sealed record ApiErrorResponse(int StatusCode, string Message);

public sealed record ApiValidationErrorResponse(int StatusCode, string Message, IReadOnlyCollection<string> Errors);
