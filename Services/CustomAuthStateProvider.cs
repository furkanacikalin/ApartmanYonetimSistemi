using Microsoft.AspNetCore.Components.Authorization;
using Blazored.LocalStorage;
using System.Security.Claims;
using ApartmanYonetimSistemi.Models;

namespace ApartmanYonetimSistemi.Services;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public CustomAuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var userSession = await _localStorage.GetItemAsync<UserSession>("UserSession");
            if (userSession == null) return new AuthenticationState(_anonymous);

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userSession.Id.ToString()),
                new Claim(ClaimTypes.Name, userSession.Username),
                new Claim(ClaimTypes.Role, userSession.Role)
            }, "CustomAuth"));

            return new AuthenticationState(claimsPrincipal);
        }
        catch
        {
            return new AuthenticationState(_anonymous);
        }
    }

    public async Task UpdateAuthenticationState(UserSession? userSession)
    {
        ClaimsPrincipal claimsPrincipal;

        if (userSession != null)
        {
            await _localStorage.SetItemAsync("UserSession", userSession);
            claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userSession.Id.ToString()),
                new Claim(ClaimTypes.Name, userSession.Username),
                new Claim(ClaimTypes.Role, userSession.Role)
            }, "CustomAuth"));
        }
        else
        {
            await _localStorage.RemoveItemAsync("UserSession");
            claimsPrincipal = _anonymous;
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(claimsPrincipal)));
    }
}