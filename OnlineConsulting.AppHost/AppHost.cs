var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.OnlineConsulting_Api>("api");

var mauiWeb = builder.AddProject<Projects.OnlineConsulting_Maui_Web>("maui-web").WithReference(api).WaitFor(api);

// Confirm-email/reset-password links are built server-side from this - must resolve to
// maui-web's actual (Aspire-assigned, so not hardcodable) origin, not a placeholder.
api.WithEnvironment("Auth__ClientOrigin", mauiWeb.GetEndpoint("https"));

builder.AddProject<Projects.OnlineConsulting_UserInterface>("userinterface").WithReference(api).WaitFor(api);

builder.Build().Run();
