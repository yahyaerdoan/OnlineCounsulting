var builder = DistributedApplication.CreateBuilder(args);

var mailpit = builder.AddMailPit("mailpit", httpPort: 8025)
    .WithEnvironment("MP_SMTP_AUTH_ACCEPT_ANY", "true")
    .WithEnvironment("MP_SMTP_AUTH_ALLOW_INSECURE", "true")
    .WithDataVolume("onlineconsulting-mailpit");

var api = builder.AddProject<Projects.OnlineConsulting_Api>("api").WithHttpHealthCheck("/health")
    .WithEnvironment("Email__SmtpHost", mailpit.Resource.Host)
    .WithEnvironment("Email__SmtpPort", mailpit.Resource.Port)
    .WithEnvironment("Email__UseSsl", "false")
    .WithEnvironment("Email__Username", "dev")
    .WithEnvironment("Email__Password", "dev");

var mauiWeb = builder.AddProject<Projects.OnlineConsulting_Maui_Web>("maui-web").WithHttpHealthCheck("/health").WithReference(api).WaitFor(api);

api.WithEnvironment("Tenancy__Hosting__PlatformOrigin", mauiWeb.GetEndpoint("https"));
api.WithEnvironment("Media__PublicOrigin", api.GetEndpoint("https"));
mauiWeb.WithEnvironment("Api__PublicBaseUrl", api.GetEndpoint("https"));

builder.Build().Run();
