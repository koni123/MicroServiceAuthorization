using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Shared.Functions.ServiceTester.Configuration;
using Shared.Functions.ServiceTester.Extensions;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsApplicationBuilder(ServiceTesterConstants.Service2);
builder.Build().Run();