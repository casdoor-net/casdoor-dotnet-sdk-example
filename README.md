# Casdoor .NET SDK Example

[![Build](https://github.com/casdoor-net/casdoor-dotnet-sdk-example/actions/workflows/build.yml/badge.svg)](https://github.com/casdoor-net/casdoor-dotnet-sdk-example/actions/workflows/build.yml)
[![License](https://img.shields.io/github/license/casdoor-net/casdoor-dotnet-sdk-example)](https://github.com/casdoor-net/casdoor-dotnet-sdk-example/blob/master/LICENSE)
[![Discord](https://img.shields.io/discord/1022748306096537660?logo=discord&label=discord&color=5865F2)](https://discord.gg/5rPsrAzK7S)

Samples of signing in with [Casdoor](https://casdoor.ai/) in .NET, using [casdoor-dotnet-sdk](https://github.com/casdoor-net/casdoor-dotnet-sdk).

| Sample                    | Package                                                                 | Description                                                                |
|---------------------------|-------------------------------------------------------------------------|----------------------------------------------------------------------------|
| [ConsoleApp](#consoleapp) | [Casdoor.Client](https://www.nuget.org/packages/Casdoor.Client)         | A console app: gets a token with a username and password, and parses it    |
| [MvcApp](#mvcapp)         | [Casdoor.AspNetCore](https://www.nuget.org/packages/Casdoor.AspNetCore) | An ASP.NET Core MVC web app that signs users in with Casdoor (OIDC)        |
| [MvcApi](#mvcapi)         | [Casdoor.AspNetCore](https://www.nuget.org/packages/Casdoor.AspNetCore) | A web API protected by Casdoor access tokens, and a console app calling it |

![MvcApi](docs/assets/MvcApi.gif)

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download) or newer
- A Casdoor server. The samples are preconfigured for the public demo server https://door.casdoor.com, so they run as is. To use your own, see [Casdoor installation](https://casdoor.ai/docs/basic/server-installation).

```shell
git clone https://github.com/casdoor-net/casdoor-dotnet-sdk-example
cd casdoor-dotnet-sdk-example
```

## Configuration

Skip this section to try the samples with the public demo server.

In your Casdoor, create (or reuse) an organization and an application. All samples take the same options, in `Program.cs` (ConsoleApp, CallerSample) or in the `Casdoor` section of `appsettings.json` (MvcApp, ApiSample):

```json
"Casdoor": {
    "Endpoint": "https://door.casdoor.com",
    "OrganizationName": "casbin",
    "ApplicationName": "app-example",
    "ApplicationType": "webapp",
    "ClientId": "b800a86702dd4d29ec4d",
    "ClientSecret": "1219843a8db4695155699be3a67f10796f2ec1d5",
    "CallbackPath": "/callback",
    "RequireHttpsMetadata": false
}
```

| Name                 | Required | Description                                                                            |
|----------------------|----------|----------------------------------------------------------------------------------------|
| Endpoint             | Yes      | Casdoor server URL                                                                     |
| OrganizationName     | Yes      | Organization of the application                                                        |
| ApplicationName      | Yes      | Name of the application                                                                |
| ApplicationType      | Yes      | `webapp` (signs users in, OIDC), `webapi` (verifies bearer tokens) or `native`         |
| ClientId             | Yes      | Client ID of the application                                                           |
| ClientSecret         | Yes      | Client secret of the application                                                       |
| CallbackPath         | No       | Path Casdoor redirects back to after signing in, `/casdoor/signin-callback` by default |
| RequireHttpsMetadata | No       | Whether the Casdoor endpoint must be HTTPS                                             |
| Scope                | No       | Scopes to request, for example `openid profile email`                                  |

More options: [casdoor-dotnet-sdk README](https://github.com/casdoor-net/casdoor-dotnet-sdk/blob/master/README.md).

## ConsoleApp

[ConsoleApp/Program.cs](ConsoleApp/Program.cs) uses `CasdoorClient`, the API client of Casdoor, without ASP.NET Core:

1. reads the OpenID Connect configuration of Casdoor (`options.GetOpenIdConnectConfigurationAsync()`),
2. gets the tokens of a user with the username and password (`client.RequestPasswordTokenAsync()`, the application needs the **Password** grant type),
3. verifies the access token with the keys of Casdoor and reads the user from it (`client.ParseJwtToken()`).

```shell
dotnet run --project ConsoleApp
```

## MvcApp

[MvcApp](MvcApp) signs users in with Casdoor over OpenID Connect and keeps the session in a cookie:

```csharp
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCasdoor(builder.Configuration.GetSection("Casdoor"))
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
```

Every page needs a signed-in user, so opening the app redirects to the Casdoor sign-in page; the home page then shows the claims of the user. **Sign out** ends the session of the app.

```shell
dotnet run --project MvcApp
```

Open http://localhost:5000 (or https://localhost:5001). On the demo server, sign in with username `admin` and password `123`.

![mvcapp](docs/assets/mvcapp-login.png)

With your own Casdoor, add `http://localhost:5000/callback` and `https://localhost:5001/callback` (the `CallbackPath`) to the application's **Redirect URLs**.

## MvcApi

[MvcApi/ApiSample](MvcApi/ApiSample) is a web API that accepts only the access tokens issued by Casdoor. `AddCasdoor()` with `"ApplicationType": "webapi"` sets up JWT bearer authentication (the keys come from Casdoor, the audience is the client ID), and `[Authorize]` protects the controller:

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddCasdoor(builder.Configuration.GetSection("Casdoor"));
```

[MvcApi/CallerSample](MvcApi/CallerSample) gets a token from Casdoor and calls the API with `Authorization: Bearer <access token>`.

Run the API, at http://localhost:5076 (Swagger UI at http://localhost:5076/swagger):

```shell
dotnet run --project MvcApi/ApiSample
```

In another terminal, run the caller:

```shell
dotnet run --project MvcApi/CallerSample
```

```
token: eyJhbGciOiJSUzI1NiIs...
Without the token: 401 Unauthorized
API Response:
[{"date":"2026-10-08","temperatureC":-7,"temperatureF":20,"summary":"Sweltering"}, ...]
```

## Resources

- [Casdoor documentation](https://casdoor.ai/docs/overview)
- [casdoor-dotnet-sdk](https://github.com/casdoor-net/casdoor-dotnet-sdk)

## License

[Apache-2.0](LICENSE)
