using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using UserGroupManager.Web.Models;

namespace UserGroupManager.Web.Services
{
    public class AuthenticationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthenticationService(HttpClient httpClient, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> LoginAsync(LoginDTO loginDTO)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDTO);

            if(!response.IsSuccessStatusCode)
            {
                return false;
            }

            var authResponse = await response.Content.ReadFromJsonAsync<AuthResponseDTO>();

            if (authResponse?.AuthToken == null)
            {
                return false;
            }

            await _localStorage.SetItemAsync("authToken", authResponse.AuthToken);
            //_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResponse.AuthToken);

            ((CustomAuthStateProvider)_authenticationStateProvider).NotifyUserAuthentication(authResponse.AuthToken);

            return true;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            _httpClient.DefaultRequestHeaders.Authorization = null;

            ((CustomAuthStateProvider)_authenticationStateProvider).NotifyUserLogout();
        }
    }
}
