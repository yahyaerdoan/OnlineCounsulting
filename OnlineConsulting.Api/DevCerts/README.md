# Dev HTTPS certificate

`onlineconsulting-dev.pfx` (gitignored via `*.pfx`, not committed) replaces the normal ASP.NET Core
dev cert **only in Development**, only because its Subject Alternative Name list includes
`10.0.2.2` - the Android emulator's alias for the host machine. The stock `dotnet dev-certs https`
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
