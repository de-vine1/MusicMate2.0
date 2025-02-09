using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace MusicMateAPI.Services
{
    public class SpotifyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public SpotifyService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public async Task<string?> GetSongUrlAsync(string songId)
        {
            var token = await GetAccessTokenAsync();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Bearer",
                token
            );

            var response = await _httpClient.GetAsync(
                $"https://api.spotify.com/v1/tracks/{songId}"
            );
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            var songUrl = json["preview_url"]?.ToString();

            return songUrl;
        }

        public async Task<Stream> GetSongStreamAsync(string songUrl)
        {
            var response = await _httpClient.GetAsync(
                songUrl,
                HttpCompletionOption.ResponseHeadersRead
            );
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }

        private async Task<string> GetAccessTokenAsync()
        {
            var clientId =
                Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_ID")
                ?? throw new InvalidOperationException("SPOTIFY_CLIENT_ID is not set");
            var clientSecret =
                Environment.GetEnvironmentVariable("SPOTIFY_CLIENT_SECRET")
                ?? throw new InvalidOperationException("SPOTIFY_CLIENT_SECRET is not set");
            var authHeader = Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}")
            );

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://accounts.spotify.com/api/token"
            );
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", authHeader);
            request.Content = new FormUrlEncodedContent(
                new[] { new KeyValuePair<string, string>("grant_type", "client_credentials") }
            );

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(content);
            var accessToken =
                json["access_token"]?.ToString()
                ?? throw new InvalidOperationException("Access token is null");

            return accessToken;
        }
    }
}
