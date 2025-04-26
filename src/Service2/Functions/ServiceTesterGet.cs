using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Shared.Common.Auth.Settings;
using Shared.Functions.ServiceTester;
using Shared.Functions.ServiceTester.Configuration;
using Shared.Functions.ServiceTester.Models;

namespace Service2.Functions;

public class ServiceTesterGet
{
    private readonly IServiceTesterFunction _serviceTesterFunction;
    private readonly string _requiredRoleName;

    public ServiceTesterGet(IServiceTesterFunction serviceTesterFunction, ClientRoleSettings clientRoleSettings)
    {
        _serviceTesterFunction = serviceTesterFunction;
        _requiredRoleName = clientRoleSettings.RoleName;
    }

    [Function(nameof(ServiceTesterGet))]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ServiceTesterConstants.ServiceTesterGetRoute)]
        HttpRequest req,
        CancellationToken cancellationToken)
    {
        if (_serviceTesterFunction.AuthorizeRequest(req, _requiredRoleName) == false)
        {
            return new UnauthorizedObjectResult(new ServiceApiResponse { Response = "You can NOT access the service" });
        }
        
        var result = await _serviceTesterFunction.CallAllServicesAsync(ServiceTesterConstants.Service2, cancellationToken);
        return new OkObjectResult(result);
    }
}