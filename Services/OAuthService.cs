using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DotNetEnv;
using Newtonsoft.Json.Linq;

public class OAuthService
{
    private readonly HttpClient _httpClient;
    private readonly JwtService _jwtService;

    public OAuthService(HttpClient httpClient, JwtService jwtService)
    {
        _httpClient = httpClient;
        _jwtService = jwtService;
        Env.Load();
    }

    public async Task<string> GetGoogleAccessTokenAsync(string code)
    {
        var clientId = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_ID");
        var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_CLIENT_SECRET");
        var redirectUri = Environment.GetEnvironmentVariable("GOOGLE_REDIRECT_URI");

        var requestBody = new FormUrlEncodedContent(
            new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
            }
        );

        var response = await _httpClient.PostAsync(
            "https://oauth2.googleapis.com/token",
            requestBody
        );
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(responseBody);

        return json["access_token"].ToString();
    }

    public async Task<string> GetSpotifyAccessTokenAsync(string code)
    {
        var clientId = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID");
        var clientSecret = Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET");
        var redirectUri = Environment.GetEnvironmentVariable("SPOTIFY_REDIRECT_URI");

        var requestBody = new FormUrlEncodedContent(
            new[]
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
            }
        );

        var response = await _httpClient.PostAsync(
            "https://accounts.spotify.com/api/token",
            requestBody
        );
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var json = JObject.Parse(responseBody);

        return json["access_token"].ToString();
    }

    public async Task<string> HandleGoogleAccessTokenAsync(string code)
    {
        var accessToken = await GetGoogleAccessTokenAsync(code);
        // Use the access token to get user info from Google
        var userInfoResponse = await _httpClient.GetAsync(
            $"https://www.googleapis.com/oauth2/v1/userinfo?access_token={accessToken}"
        );
        userInfoResponse.EnsureSuccessStatusCode();

        var userInfo = await userInfoResponse.Content.ReadAsStringAsync();
        var userJson = JObject.Parse(userInfo);

        var userId = Guid.NewGuid(); // Replace with actual user ID retrieval logic
        var email = userJson["email"].ToString();

        return _jwtService.GenerateToken(userId, email);
    }

    public async Task<string> HandleSpotifyAccessTokenAsync(string code)
    {
        var accessToken = await GetSpotifyAccessTokenAsync(code);
        // Use the access token to get user info from Spotify
        var request = new HttpRequestMessage(HttpMethod.Get, "https://api.spotify.com/v1/me");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var userInfoResponse = await _httpClient.SendAsync(request);
        userInfoResponse.EnsureSuccessStatusCode();

        var userInfo = await userInfoResponse.Content.ReadAsStringAsync();
        var userJson = JObject.Parse(userInfo);

        var userId = Guid.NewGuid(); // Replace with actual user ID retrieval logic
        var email = userJson["email"].ToString();

        return _jwtService.GenerateToken(userId, email);
    }
}
