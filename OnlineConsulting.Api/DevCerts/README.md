# Dev HTTPS certificate

`onlineconsulting-dev.pfx` (gitignored via `*.pfx`, not committed) is served **only in Development**
and **only to connections made by IP address** (no TLS SNI hostname), i.e. the Android emulator
calling `https://10.0.2.2:7012`, because its Subject Alternative Name list includes `10.0.2.2` - the
emulator's alias for the host machine. Connections made by name (`https://localhost`: browsers,
Aspire health checks, maui-web) keep getting the normal ASP.NET Core dev cert, so they stay trusted
as long as `dotnet dev-certs https --trust` has been run once on the machine. Selection lives in
`Configurations/Extensions/DevelopmentCertificates.cs`.

This file is self-signed and deliberately **not** trusted on Windows; only the Android app trusts it
(for `10.0.2.2` only). Serving it to every client made Windows reject HTTPS to the API. The stock `dotnet dev-certs https`
certificate only covers `localhost`/`127.0.0.1`, so a native Android WebView loading an image or
iframe directly against `https://10.0.2.2:7012/...` fails TLS hostname verification (the app's own
HttpClient doesn't hit this because it bypasses cert validation in Debug builds).

If this file is missing (e.g. a fresh clone), `Program.cs` falls back to the normal dev cert
automatically - nothing breaks, you just won't be able to load API media from the Android emulator
until you regenerate this file.

Regenerate with OpenSSL (password below must match `Program.cs`'s `ConfigureKestrel` call):

```sh
openssl req -x509 -newkey rsa:2048 -keyout dev.key -out dev.crt -days 3650 -nodes -subj "/CN=OnlineConsulting Dev" \
  -addext "subjectAltName=DNS:localhost,DNS:*.dev.localhost,DNS:*.dev.internal,IP:127.0.0.1,IP:::1,IP:10.0.2.2" \
  -addext "extendedKeyUsage=serverAuth"
openssl pkcs12 -export -out onlineconsulting-dev.pfx -inkey dev.key -in dev.crt -passout pass:devcert123
openssl x509 -in dev.crt -outform DER -out ../../OnlineConsulting.Maui/Platforms/Android/Resources/raw/dev_cert.der
```

The last line also refreshes the Android app's bundled public cert (`network_security_config.xml`
trusts it for the `10.0.2.2` domain only) - re-run it if you regenerate this cert.
