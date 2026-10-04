# OnlineCounsulting

## Local Setup

Secrets never go in `appsettings.json`; the API reads them from user-secrets in development and from environment variables elsewhere.

The JWT signing key is required (at least 64 bytes); the API won't start without it:

```powershell
$key = [Convert]::ToBase64String([Security.Cryptography.RandomNumberGenerator]::GetBytes(64))
dotnet user-secrets set "TokenOptions:SecurityKey" $key --project OnlineConsulting.Api
```

Outside development set `TokenOptions__SecurityKey` as an environment variable or in the secret store.

