var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.OnlineConsulting_Api>("api");

builder.AddProject<Projects.OnlineConsulting_UserInterface>("userinterface").WithReference(api).WaitFor(api);

builder.AddProject<Projects.OnlineConsulting_Maui_Web>("maui-web").WithReference(api).WaitFor(api);

builder.Build().Run();
