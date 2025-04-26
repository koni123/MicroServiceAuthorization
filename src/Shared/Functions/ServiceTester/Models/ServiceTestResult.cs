using System.Net;

namespace Shared.Functions.ServiceTester.Models;

/// <summary>
/// Represents the result of a service test.
/// </summary>

public class ServiceTestResult
{
    public required string CallingService { get; set; }

    public required List<ServiceTestResultItem> Results { get; set; }
}

public class ServiceTestResultItem
{
    /// <summary>
    /// Name of the service.
    /// </summary>
    public string ServiceName { get; }

    /// <summary>
    /// Value indicating whether the test was successful.
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// HTTP status code returned by the service, if any.
    /// </summary>
    public HttpStatusCode? StatusCode { get; }

    /// <summary>
    /// Response content returned by the service, if any.
    /// </summary>
    public string? ResponseContent { get; }

    /// <summary>
    /// Error message, if any.
    /// </summary>
    public string? ErrorMessage { get; }

    private ServiceTestResultItem(
        string serviceName,
        bool success,
        HttpStatusCode? statusCode = null,
        string? responseContent = null,
        string? errorMessage = null)
    {
        ServiceName = serviceName;
        Success = success;
        StatusCode = statusCode;
        ResponseContent = responseContent;
        ErrorMessage = errorMessage;
    }

    /// <summary>
    /// Creates a successful service test result.
    /// </summary>
    /// <param name="serviceName">The name of the service.</param>
    /// <param name="statusCode">The HTTP status code returned by the service.</param>
    /// <param name="responseContent">The response content returned by the service.</param>
    /// <returns>A new instance of the <see cref="ServiceTestResult"/> class representing a successful test.</returns>
    public static ServiceTestResultItem SuccessResult(string serviceName, HttpStatusCode statusCode, string responseContent) =>
        new(serviceName, true, statusCode, responseContent);

    /// <summary>
    /// Creates an error service test result.
    /// </summary>
    /// <param name="serviceName">The name of the service.</param>
    /// <param name="statusCode">The HTTP status code returned by the service.</param>
    /// <param name="errorMessage">The error message.</param>
    /// <returns>A new instance of the <see cref="ServiceTestResult"/> class representing an error test.</returns>
    public static ServiceTestResultItem ErrorResult(string serviceName, HttpStatusCode statusCode, string errorMessage) =>
        new(serviceName, false, statusCode, errorMessage: errorMessage);

    /// <summary>
    /// Creates an exception service test result.
    /// </summary>
    /// <param name="serviceName">The name of the service.</param>
    /// <param name="exceptionMessage">The exception message.</param>
    /// <returns>A new instance of the <see cref="ServiceTestResult"/> class representing an exception test.</returns>
    public static ServiceTestResultItem ExceptionResult(string serviceName, string exceptionMessage) =>
        new(serviceName, false, errorMessage: exceptionMessage);
}