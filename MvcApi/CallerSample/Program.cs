using System.Net.Http.Headers;
using Casdoor.Client;

// usage: dotnet run --project MvcApi/CallerSample [URL of the API]
var apiUrl = args.Length > 0 ? args[0] : "http://localhost:5076/WeatherForecast";

var options = new CasdoorOptions
{
    Endpoint = "https://door.casdoor.com",
    OrganizationName = "casbin",
    ApplicationName = "app-example",
    ApplicationType = "native", // webapp, webapi or native
    ClientId = "b800a86702dd4d29ec4d",
    ClientSecret = "1219843a8db4695155699be3a67f10796f2ec1d5",
};
var client = new CasdoorClient(new HttpClient(), options);

var token = await client.RequestPasswordTokenAsync("admin", "123");
if (token.IsError || token.AccessToken is null)
{
    Console.WriteLine($"Failed to get the token: {token.Error}");
    return 1;
}
Console.WriteLine($"token: {token.AccessToken}");

var httpClient = new HttpClient();
try
{
    // without a token the API answers 401
    var anonymous = await httpClient.GetAsync(apiUrl);
    Console.WriteLine($"Without the token: {(int)anonymous.StatusCode} {anonymous.StatusCode}");

    var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
    var response = await httpClient.SendAsync(request);
    if (!response.IsSuccessStatusCode)
    {
        Console.WriteLine($"API request failed with status code: {(int)response.StatusCode} {response.StatusCode}");
        return 1;
    }

    Console.WriteLine("API Response:");
    Console.WriteLine(await response.Content.ReadAsStringAsync());
    return 0;
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"HTTP request exception: {ex.Message}. Is ApiSample running at {apiUrl}?");
    return 1;
}
