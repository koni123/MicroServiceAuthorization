using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Hosting;
using Shared.Functions.ServiceTester.Configuration;
using Shared.Functions.ServiceTester.Extensions;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsApplicationBuilder(ServiceTesterConstants.Service3);
builder.Build().Run();
