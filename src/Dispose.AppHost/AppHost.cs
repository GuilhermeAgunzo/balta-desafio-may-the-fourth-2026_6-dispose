var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.Dispose_API>("dispose-api")
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.Dispose_Web>("dispose-web")
    .WithExternalHttpEndpoints()
    .WaitFor(api);

builder.Build().Run();
