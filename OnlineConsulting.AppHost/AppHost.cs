var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.OnlineConsulting_Api>("api").WithHttpHealthCheck("/health");

var mauiWeb = builder.AddProject<Projects.OnlineConsulting_Maui_Web>("maui-web").WithHttpHealthCheck("/health").WithReference(api).WaitFor(api);

api.WithEnvironment("Auth__ClientOrigin", mauiWeb.GetEndpoint("https"));

builder.AddProject<Projects.OnlineConsulting_UserInterface>("userinterface").WithHttpHealthCheck("/health").WithReference(api).WaitFor(api);

builder.Build().Run();
