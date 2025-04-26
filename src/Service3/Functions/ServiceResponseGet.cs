using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Shared.Common.Auth.Settings;
using Shared.Functions.ServiceTester;
using Shared.Functions.ServiceTester.Configuration;
using Shared.Functions.ServiceTester.Models;

namespace Service3.Functions;

public class ServiceResponseGet
{
    private readonly IServiceTesterFunction _serviceTesterFunction;
    private readonly string _requiredRoleName;

    public ServiceResponseGet(IServiceTesterFunction serviceTesterFunction, ClientRoleSettings clientRoleSettings)
    {
        _serviceTesterFunction = serviceTesterFunction;
        _requiredRoleName = clientRoleSettings.RoleName;
    }


    [Function(nameof(ServiceResponseGet))]
    public IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = ServiceTesterConstants.ServiceTesterResponseRoute)]
        HttpRequest req)
    {
        if (_serviceTesterFunction.AuthorizeRequest(req, _requiredRoleName) == false)
        {
            return new UnauthorizedObjectResult(new ServiceApiResponse { Response = "You can NOT access the service" });
        }

        return new OkObjectResult(new ServiceApiResponse { Response = "You can access the service" });
    }
}