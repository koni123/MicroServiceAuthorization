using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shared.Common.Auth.Services;
using Shared.Common.Auth.Util;
using Shared.Functions.ServiceTester.Configuration;
using Shared.Functions.ServiceTester.Models;

namespace Shared.Functions.ServiceTester;

public interface IServiceTesterFunction
{
    /// <summary>
    /// Convenience method to call all services asynchronously.
    /// </summary>
    /// <param name="callingService">Name of the calling service.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of service test results.</returns>
    Task<ServiceTestResult> CallAllServicesAsync(string callingService, CancellationToken cancellationToken);

    bool AuthorizeRequest(HttpRequest req, string requiredRoleName);
}

/// <summary>
/// Implementation of the <see cref="IServiceTesterFunction"/> interface.
/// </summary>
public class ServiceTesterFunction : IServiceTesterFunction
{
    private readonly IAuthHttpService _service1Service;
    private readonly IAuthHttpService _service2Service;
    private readonly IAuthHttpService _service3Service;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceTesterFunction"/> class.
    /// </summary>
    /// <param name="service1Service">The service for the first service.</param>
    /// <param name="service2Service">The service for the second service.</param>
    /// <param name="service3Service">The service for the third service.</param>
    public ServiceTesterFunction(
        [FromKeyedServices(ServiceTesterConstants.Service1)]
        IAuthHttpService service1Service,
        [FromKeyedServices(ServiceTesterConstants.Service2)]
        IAuthHttpService service2Service,
        [FromKeyedServices(ServiceTesterConstants.Service3)]
        IAuthHttpService service3Service
    )
    {
        _service1Service = service1Service;
        _service2Service = service2Service;
        _service3Service = service3Service;
    }

    /// <inheritdoc/>
    public async Task<ServiceTestResult> CallAllServicesAsync(string callingService, CancellationToken cancellationToken)
    {
        var serviceCallTasks = new List<Task<ServiceTestResultItem>>
        {
            CallServiceAsync(ServiceTesterConstants.Service1, _service1Service, cancellationToken),
            CallServiceAsync(ServiceTesterConstants.Service2, _service2Service, cancellationToken),
            CallServiceAsync(ServiceTesterConstants.Service3, _service3Service, cancellationToken)
        };

        var serviceTestResult = new ServiceTestResult
        {
            CallingService = callingService,
            Results = []
        };

        while (serviceCallTasks.Count > 0)
        {
            var completedTask = await Task.WhenAny(serviceCallTasks);
            serviceTestResult.Results.Add(await completedTask);
            serviceCallTasks.Remove(completedTask);
        }
        
        serviceTestResult.Results = serviceTestResult.Results.OrderBy(r => r.ServiceName).ToList();

        return serviceTestResult;
    }

    public bool AuthorizeRequest(HttpRequest req, string requiredRoleName)
    {
        var claims = ClaimsPrincipalParser.ParseRoles(req);
        return claims.Claims.Any(c => c.Value == requiredRoleName);
    }

    /// <summary>
    /// Calls a specific service asynchronously.
    /// </summary>
    /// <param name="serviceName">The name of the service.</param>
    /// <param name="authHttpService">The HTTP service to use for the call.</param>
    /// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the service test result.</returns>
    private static async Task<ServiceTestResultItem> CallServiceAsync(
        string serviceName,
        IAuthHttpService authHttpService,
        CancellationToken cancellationToken)
    {
        try
        {
            using var message = new HttpRequestMessage(HttpMethod.Get, $"api/{ServiceTesterConstants.ServiceTesterResponseRoute}");
            using var response = await authHttpService.SendAsync(message, cancellationToken);
            if (response.IsSuccessStatusCode)
            {
                return ServiceTestResultItem.SuccessResult(
                    serviceName,
                    response.StatusCode,
                    await response.Content.ReadAsStringAsync(cancellationToken));
            }

            return ServiceTestResultItem.ErrorResult(
                serviceName,
                response.StatusCode,
                await response.Content.ReadAsStringAsync(cancellationToken));
        }
        catch (Exception e)
        {
            return ServiceTestResultItem.ExceptionResult(
                serviceName,
                e.Message);
        }
    }
}